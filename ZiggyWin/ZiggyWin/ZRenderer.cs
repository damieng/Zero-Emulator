using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using ZeroWin.Properties;

namespace ZeroWin
{

    public partial class ZRenderer : UserControl
    {
        [DllImport("gdi32.dll")]
        private static extern bool StretchBlt(
                                    IntPtr hdcDest,
                                   int nXOriginDest,
                                   int nYOriginDest,
                                   int nWidthDest,
                                   int nHeightDest,
                                   IntPtr hdcSrc,
                                   int nXOriginSrc,
                                   int nYOriginSrc,
                                   int nWidthSrc,
                                   int nHeightSrc,
                                   int dwRop);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("User32.dll")]
        public static extern int GetWindowDC(int hWnd);

        public Form1 ziggyWin;

        private bool useDirectX = true;
        private Thread renderThread;
        private bool isSuspended;
        private bool isRendering;
        private double lastTime;
        private double frameTime;
        private double totalFrameTime;
        private int frameCount;
        public int averageFPS;

        public Point LEDIndicatorPosition { get; set; } = new Point(0, 0);
        public bool EnableVsync { get; set; }
        public bool PixelSmoothing { get; set; } = true;
        public bool DoRender { get; set; }
        public bool DirectXReady { get; private set; }

        public bool EnableDirectX {
            get => useDirectX;
            set {
                useDirectX = value;
                if (useDirectX)
                    DoubleBuffered = false;
                else {
                    DoubleBuffered = true;
                    DirectXReady = false;
                }
            }
        }

        public bool ShowScanlines { get; set; }
        public bool EnableFullScreen { get; set; }
        public bool EmulationIsPaused { get; set; }

        private Device dxDevice;
        private PresentParameters currentParams;

        private readonly DisplaySprite displaySprite = new DisplaySprite();
        private readonly InterlaceSprite scanlineSprite = new InterlaceSprite();

        private Rectangle screenRect;
        private Bitmap gdiDisplay;

        private int displayWidth = 256 + 48 + 48;
        private int displayHeight = 192 + 48 + 56;

        private Vector3 spritePos = Vector3.Empty;

        //int screenWidth = 256 + 48 + 48;
        public int ScreenWidth { get; set; }

        // int screenHeight = 192 + 48 + 56;
        public int ScreenHeight { get; set; }

        public ZRenderer() {
        }

        private void DestroyDX() {
            scanlineSprite.Destroy();
            displaySprite.Destroy();

            gdiDisplay?.Dispose();
            dxDevice?.Dispose();
        }

        public void Shutdown() {
            DestroyDX();
            DoRender = false;
            renderThread.Join();
        }

        public void SetSpeccyScreenSize(int width, int height) {
            ScreenWidth = width;
            ScreenHeight = height;
            screenRect = new Rectangle(0, 0, ScreenWidth, ScreenHeight);
            gdiDisplay = new Bitmap(ScreenWidth, ScreenHeight, PixelFormat.Format32bppRgb);

        }

        public void SetSize(int width, int height) {
            if (dxDevice != null) {
                displaySprite.Destroy();
                scanlineSprite.Destroy();
                dxDevice.Dispose();
            }

            displayHeight = height;
            displayWidth = width;

            ClientSize = new Size(width, height);
            if (EnableDirectX && !InitDirectX(width, height)) {
                DirectXReady = false;
            }
        }

        public bool InitDirectX(int width, int height, bool is16bit = false) {
            isSuspended = true;
            DestroyDX();

            displayWidth = width;
            displayHeight = height;

            AdapterInformation adapterInfo = Manager.Adapters.Default;

            ziggyWin.logger.Log("Setting up render parameters...");
            currentParams = new PresentParameters
            {
                BackBufferCount = 2,
                BackBufferWidth = width,
                BackBufferHeight = height,
                SwapEffect = SwapEffect.Discard,
                PresentFlag = PresentFlag.None,
                PresentationInterval = (EnableVsync ? PresentInterval.One : PresentInterval.Immediate),
                Windowed = !EnableFullScreen
            };

            Format currentFormat = Manager.Adapters[0].CurrentDisplayMode.Format;
            bool formatCheck = Manager.CheckDeviceType(0,
                               DeviceType.Hardware,
                               currentFormat,
                               currentFormat,
                               false);

            if (!formatCheck)
                MessageBox.Show("Invalid format", "dx error", MessageBoxButtons.OK);

            if (EnableFullScreen) {
                currentParams.DeviceWindow = Parent;
                currentParams.BackBufferFormat = currentFormat;//(is16bit ? Format.R5G6B5 : Format.X8B8G8R8);
            }
            else {
                currentParams.DeviceWindow = this;
                currentParams.BackBufferFormat = adapterInfo.CurrentDisplayMode.Format;
            }

            try {
                ziggyWin.logger.Log("Initializing directX device...");
                dxDevice = new Device(0, DeviceType.Hardware, this, CreateFlags.HardwareVertexProcessing, currentParams);
            }
            catch (DirectXException dx) {
                MessageBox.Show(dx.ErrorString, "DX error", MessageBoxButtons.OK);
                try {
                    dxDevice = new Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, currentParams);
                }
                catch (DirectXException dx2) {
                    MessageBox.Show(dx2.ErrorString, "DX error", MessageBoxButtons.OK);
                    DirectXReady = false;
                    return false;
                }
            }

            SetSpeccyScreenSize(ziggyWin.zx.GetTotalScreenWidth(), ziggyWin.zx.GetTotalScreenHeight());

            float scaleX = (displayWidth / (float)(ScreenWidth));
            float scaleY = (displayHeight / (float)(ScreenHeight));

            //Maintain 4:3 aspect ration when full screen
            if (EnableFullScreen && ziggyWin.config.MaintainAspectRatioInFullScreen) {
                if (displayHeight < displayWidth) {
                    float aspectXScale = 0.75f; // (displayHeight * 4.0f) / (displayWidth * 3.0f);
                    scaleX = (scaleX * aspectXScale);
                    int newWidth = (int)(displayWidth * aspectXScale);
                    spritePos = new Vector3((((displayWidth - newWidth)) / (scaleX * 2.0f)), 0, 0);
                }
                else //Not tested!!!
                {
                    float aspectYScale = 1.33f;// (displayWidth * 3.0f) / (displayHeight * 4.0f);
                    scaleY = (scaleY * aspectYScale);
                }
            }
            else
                spritePos = Vector3.Empty;

            if (scaleX < 1.0f)
                scaleX = 1.0f;

            if (scaleY < 1.0f)
                scaleY = 1.0f;

            Matrix scaling = Matrix.Scaling(scaleX, scaleY, 1.0f);
            Console.WriteLine("scaleX " + scaleX + "     scaleY " + scaleY);
            Console.WriteLine("pos " + spritePos);
            Texture displayTexture = new Texture(dxDevice, ScreenWidth, ScreenHeight, 1, Usage.None, currentParams.BackBufferFormat, Pool.Managed);

            displaySprite.Init(dxDevice, displayTexture, new Rectangle((int)spritePos.X, (int)spritePos.Y, ScreenWidth, ScreenHeight), scaling);

            using (MemoryStream stream = new MemoryStream()) {
                Resources.scanlines2.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                Texture interlaceTexture = Texture.FromStream(dxDevice, stream, Usage.None, Pool.Managed);

                //Why 1.5f? Because it works very well. 
                //Trying to use displayHeight/texture_width (which would seem logical) leads to strange banding on the screen.
                scanlineSprite.Init(dxDevice, interlaceTexture, new Rectangle((int)spritePos.X, (int)spritePos.Y, ScreenWidth, ScreenHeight), scaling, 1.0f, displayHeight / 1.5f);
            }

            Font systemfont = new Font(SystemFonts.MessageBoxFont.FontFamily, 10f, FontStyle.Regular);
            isSuspended = false;
            lastTime = PrecisionTimer.TimeInMilliseconds();
            DirectXReady = true;
            return true;
        }

        public ZRenderer(Form1 zw, int width, int height) {
            InitializeComponent();
            SetStyle(ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);

            ziggyWin = zw;
            displayHeight = height;
            displayWidth = width;

            ClientSize = new Size(width, height);
            if (!InitDirectX(width, height)) {
                DirectXReady = false;
                dxDevice = null;
            }
            Start();
        }

        protected override void OnPaintBackground(PaintEventArgs e) {
        }

        private void Start() {
            renderThread = new Thread(RenderDX)
            {
                Name = "Render Thread",
                Priority = ThreadPriority.Lowest
            };
            DoRender = true;
            isRendering = false;
            isSuspended = false;
            renderThread.Start();
            PrecisionTimer.TimeInSeconds();
        }

        public void Suspend() {
            if (isSuspended)
                return;
            DoRender = false;
            renderThread.Join();
            isRendering = false;
            isSuspended = true;
        }

        public void Resume() {
            if (!isSuspended)
                return;
            Start();
        }

        private void RenderDDSurface() {
            lock (ziggyWin.zx) {
                displaySprite.CopySurface(screenRect, ziggyWin.zx.ScreenBuffer);
            }

            if (EnableFullScreen)
                dxDevice.Clear(ClearFlags.Target, Color.Black, 1.0f, 0);

            dxDevice.BeginScene();
            dxDevice.RenderState.Lighting = false;

            displaySprite.Render(dxDevice, (PixelSmoothing ? TextureFilter.Linear : TextureFilter.None), TextureAddress.Border);

            if (ShowScanlines)
                scanlineSprite.Render(dxDevice, TextureFilter.Linear, TextureAddress.Wrap);

            dxDevice.EndScene();
            dxDevice.CheckCooperativeLevel(out var coopLevel);
            ResultCode deviceState = (ResultCode)coopLevel;
            switch (deviceState) {
                case ResultCode.DeviceLost:
                    Thread.Sleep(1);
                    return;
                case ResultCode.DeviceNotReset:
                    SetSize(Width, Height);
                    return;
            }

            try {
                dxDevice.Present();
            }
            catch (DeviceLostException) {
                Thread.Sleep(1);
            }
        }

        public void RenderDX() {
            while (DoRender) {
                if (ziggyWin.zx.needsPaint && !isRendering) {
                    lock (ziggyWin.zx.lockThis) {
                        ziggyWin.zx.needsPaint = false;
                        isRendering = true;
                        Invalidate();
                    }
                    frameTime = PrecisionTimer.TimeInMilliseconds() - lastTime;
                    frameCount++;
                    totalFrameTime += frameTime;

                    if (totalFrameTime > 1000.0f) {
                        averageFPS = (int)(1000 * frameCount / totalFrameTime);
                        frameCount = 0;
                        totalFrameTime = 0;
                    }
                    lastTime = PrecisionTimer.TimeInMilliseconds();
                }
                Thread.Sleep(1);
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            if (isSuspended)
                return;
            if (useDirectX && DirectXReady)
                RenderDDSurface();
            else {
                BitmapData bmpData = gdiDisplay.LockBits(screenRect, ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

                //Copy the data from the byte array into BitmapData.Scan0
                lock (ziggyWin.zx) {
                    Marshal.Copy(ziggyWin.zx.ScreenBuffer, 0, bmpData.Scan0, (ScreenWidth) * (ScreenHeight));
                }
                //Unlock the pixels
                gdiDisplay.UnlockBits(bmpData);

                IntPtr hdc = e.Graphics.GetHdc();
                IntPtr hbmp = gdiDisplay.GetHbitmap();
                IntPtr memdc = CreateCompatibleDC(hdc);

                SelectObject(memdc, hbmp);
                StretchBlt(hdc, 0, 0, Width, Height, memdc, 0, 0, ScreenWidth, ScreenHeight, 0xCC0020);

                e.Graphics.ReleaseHdc(hdc);

                DeleteObject(hbmp);
                DeleteDC(memdc);
            }
            isRendering = false;
        }

        public Bitmap GetScreen() {
            Bitmap saveBmp;
            {
                BitmapData bmpData = gdiDisplay.LockBits(screenRect, ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

                //Copy the data from the byte array into BitmapData.Scan0
                lock (ziggyWin.zx) {
                    Marshal.Copy(ziggyWin.zx.ScreenBuffer, 0, bmpData.Scan0, (ScreenWidth) * (ScreenHeight));
                }
                //Unlock the pixels
                gdiDisplay.UnlockBits(bmpData);
                Graphics g1 = CreateGraphics();

                saveBmp = new Bitmap(Width, Height, g1);
                Graphics g2 = Graphics.FromImage(saveBmp);
                IntPtr hdc = g2.GetHdc();
                IntPtr hbmp = gdiDisplay.GetHbitmap();
                IntPtr memdc = CreateCompatibleDC(hdc);

                SelectObject(memdc, hbmp);
                StretchBlt(hdc, 0, 0, Width, Height, memdc, 0, 0, ScreenWidth, ScreenHeight, 0xCC0020);

                g2.ReleaseHdc(hdc);

                DeleteObject(hbmp);
                DeleteDC(memdc);
            }
            return saveBmp;
        }
    }
}