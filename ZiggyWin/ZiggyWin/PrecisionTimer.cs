using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace ZeroWin
{
    internal class PrecisionTimer
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private long startTime, stopTime;
        private static long freq;
        private static bool freqIsInitialized;

        public double DurationInSeconds { get; private set; }
        public double DurationInMilliseconds => DurationInSeconds * 1000;

        // Constructor
        public PrecisionTimer() {
            startTime = 0;
            stopTime = 0;
            DurationInSeconds = 0;
            if (QueryPerformanceFrequency(out freq) == false) {
                // high-performance counter not supported
                throw new Win32Exception();
            }
            freqIsInitialized = true;
        }

        // Start the timer
        public void Start() {
            QueryPerformanceCounter(out startTime);
            Thread.Sleep(0);
        }

        // Stop the timer
        public void Stop() {
            Thread.Sleep(0);
            QueryPerformanceCounter(out stopTime);
            DurationInSeconds = (stopTime - startTime) / (double)freq; //save the difference
            Thread.Sleep(0);
        }

        // Returns the current time
        public static double TimeInSeconds() {
            if (!freqIsInitialized) {
                if (QueryPerformanceFrequency(out freq) == false) {
                    // high-performance counter not supported
                    throw new Win32Exception();
                }
                freqIsInitialized = true;
            }

            Thread.Sleep(0);
            QueryPerformanceCounter(out var currentTime);
            return (currentTime / (double)freq); //save the difference
        }

        public static double TimeInMilliseconds() {
            if (!freqIsInitialized) {
                if (QueryPerformanceFrequency(out freq) == false) {
                    // high-performance counter not supported
                    throw new Win32Exception();
                }
                freqIsInitialized = true;
            }

            Thread.Sleep(0);
            QueryPerformanceCounter(out var currentTime);
            return ((double)currentTime * 1000) / freq; //save the difference
        }
    }
}