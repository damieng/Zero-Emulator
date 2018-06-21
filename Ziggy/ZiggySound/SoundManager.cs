using System;
using System.Reflection;
using System.Resources;
using System.Threading;
using Microsoft.DirectX.DirectSound;

namespace ZeroSound
{
    public unsafe class SoundManager : IDisposable
    {
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
            string dllName = args.Name.Contains(",") ? args.Name.Substring(0, args.Name.IndexOf(',')) : args.Name.Replace(".dll", "");

            dllName = dllName.Replace(".", "_");
            if (dllName.EndsWith("_resources")) return null;

            ResourceManager rm = new ResourceManager(typeof(SoundManager).Namespace + ".Properties.Resources", Assembly.GetExecutingAssembly());

            return Assembly.Load((byte[])rm.GetObject(dllName));
        }

        private const int SAMPLE_SIZE = 882;
        private const int BUFFER_COUNT = 4;
        public bool soundEnabled = true;
        private Device _device;
        private SecondaryBuffer _soundBuffer;
        private Notify _notify;
        public bool isPlaying = false;
        private readonly int _bufferSize;
        private readonly int _bufferCount;

        private readonly System.Collections.Queue _fillQueue;
        private readonly System.Collections.Queue _playQueue;
        private uint lastSample;

        private Thread _waveFillThread;
        private readonly AutoResetEvent _fillEvent = new AutoResetEvent(true);
        private bool _isFinished;
        private bool disposed;

        public SoundManager(IntPtr handle, short bitsPerSample, short channels, int samplesPerSecond) {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            _fillQueue = new System.Collections.Queue(BUFFER_COUNT);
            _playQueue = new System.Collections.Queue(BUFFER_COUNT);
            _bufferSize = SAMPLE_SIZE * 2 * 2;
            for (int i = 0; i < BUFFER_COUNT; i++)
                _fillQueue.Enqueue(new byte[_bufferSize]);

            _bufferCount = BUFFER_COUNT;

            _device = new Device();
            _device.SetCooperativeLevel(handle, CooperativeLevel.Priority);

            WaveFormat wf = new WaveFormat
            {
                FormatTag = WaveFormatTag.Pcm,
                SamplesPerSecond = samplesPerSecond,
                BitsPerSample = bitsPerSample,
                Channels = channels
            };
            wf.BlockAlign = (short)(wf.Channels * (wf.BitsPerSample / 8));
            wf.AverageBytesPerSecond = wf.SamplesPerSecond * wf.BlockAlign;

            // Create a buffer
            BufferDescription bufferDesc = new BufferDescription(wf)
            {
                BufferBytes = _bufferSize * _bufferCount,
                ControlPositionNotify = true,
                GlobalFocus = true,
                ControlVolume = true,
                ControlEffects = false
            };
            _soundBuffer = new SecondaryBuffer(bufferDesc, _device);

            _notify = new Notify(_soundBuffer);
            BufferPositionNotify[] posNotify = new BufferPositionNotify[_bufferCount];
            for (int i = 0; i < posNotify.Length; i++) {
                posNotify[i] = new BufferPositionNotify
                {
                    Offset = i * _bufferSize,
                    EventNotifyHandle = _fillEvent.SafeWaitHandle.DangerousGetHandle()
                };
            }
            _notify.SetNotificationPositions(posNotify);

            _waveFillThread = new Thread(waveFillThreadProc)
            {
                IsBackground = true,
                Name = "Wave fill thread",
                Priority = ThreadPriority.Highest
            };
            _waveFillThread.Start();
        }

        ~SoundManager() {
            Dispose(false);
        }

        public void Shutdown() {
            Dispose();
        }

        public void Reset() {
        }

        public void Play() {
            // _soundBuffer.Play(0, BufferPlayFlags.Looping);
        }

        public void PlayBuffer(ref short[] samples) {
        }

        public void Stop() {
            // _soundBuffer.Stop();
        }

        public bool FinishedPlaying() {
            lock (_fillQueue.SyncRoot)
                if (_fillQueue.Count < 1)
                    return false;
            return true;
        }

        public void SetVolume(float t) {
            if (t <= 0.0f)
                _soundBuffer.Volume = -10000;
            else if (t >= 1.0f)
                _soundBuffer.Volume = 0;
            else
                _soundBuffer.Volume = (int)(-2000.0f * Math.Log10(1.0f / t));
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposed) {
                if (disposing) {
                    if (_waveFillThread != null) {
                        try {
                            _isFinished = true;
                            if (_soundBuffer != null)
                                if (_soundBuffer.Status.Playing)
                                    _soundBuffer.Stop();
                            _fillEvent.Set();

                            _waveFillThread.Join();

                            if (_soundBuffer != null)
                                _soundBuffer.Dispose();
                            if (_notify != null)
                                _notify.Dispose();

                            if (_device != null)
                                _device.Dispose();
                        }
                        catch (ThreadAbortException e) {
                            Console.WriteLine("Sound thread exception " + e.Message);
                        }
                        finally {
                            _waveFillThread = null;
                            _soundBuffer = null;
                            _notify = null;
                            _device = null;
                        }
                    }
                }
                // No unmanaged resources to release otherwise they'd go here.
            }
            disposed = true;
        }
        private void waveFillThreadProc() {
            int lastWrittenBuffer = -1;
            byte[] sampleData = new byte[_bufferSize];
            fixed (byte* lpSampleData = sampleData) {
                try {
                    _soundBuffer.Play(0, BufferPlayFlags.Looping);
                    while (!_isFinished) {
                        _fillEvent.WaitOne();

                        for (int i = (lastWrittenBuffer + 1) % _bufferCount; i != (_soundBuffer.PlayPosition / _bufferSize); i = ++i % _bufferCount) {
                            OnBufferFill((IntPtr)lpSampleData, sampleData.Length);
                            _soundBuffer.Write(_bufferSize * i, sampleData, LockFlag.None);
                            lastWrittenBuffer = i;
                        }
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine("Sound thread exception " + ex.Message);
                    //LogAgent.Error(ex);
                }
            }
        }

        protected void OnBufferFill(IntPtr buffer, int length) {
            byte[] buf = null;
            lock (_playQueue.SyncRoot)
                if (_playQueue.Count > 0)
                    buf = _playQueue.Dequeue() as byte[];
            if (buf != null) {
                uint* dst = (uint*)buffer;
                fixed (byte* srcb = buf) {
                    uint* src = (uint*)srcb;
                    for (int i = 0; i < length / 4; i++)
                        dst[i] = src[i];
                    lastSample = dst[length / 4 - 1];
                }
                lock (_fillQueue.SyncRoot)
                    _fillQueue.Enqueue(buf);
            }
            else {
                uint* dst = (uint*)buffer;
                for (int i = 0; i < length / 4; i++)
                    dst[i] = lastSample;
            }
        }

        public byte[] LockBuffer() {
            byte[] sndbuf = null;
            lock (_fillQueue.SyncRoot)
                if (_fillQueue.Count > 0)
                    sndbuf = _fillQueue.Dequeue() as byte[];
            return sndbuf;
        }

        public void UnlockBuffer(byte[] sndbuf) {
            lock (_playQueue.SyncRoot)
                _playQueue.Enqueue(sndbuf);
        }
    }
}