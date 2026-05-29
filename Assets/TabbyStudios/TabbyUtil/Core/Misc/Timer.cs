using System.Diagnostics;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;

namespace TabbyStudios
{
    public class Timer
    {
        private static Stopwatch timer;
        

        public static void Start()
        {
            timer = new();
            timer.Start();
        }

        public static double Stop(string message = "")
        {
            Assert.IsTrue(timer.IsRunning);
            timer.Stop();
            if(!message.IsNullOrEmpty())
                Debug.Log($"{message} {timer.Elapsed.TotalMilliseconds}");
            return timer.Elapsed.TotalMilliseconds;
        }

        public static void LogStop()
        {
            Debug.Log(Stop());
        }
        
        public static void SafeLogStop()
        {
            if(timer is { IsRunning: true })
                Debug.Log(Stop());
        }
    }
}