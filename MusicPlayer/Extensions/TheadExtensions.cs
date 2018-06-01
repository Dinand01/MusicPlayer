using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicPlayer.Extensions
{
    /// <summary>
    /// Extensions for threads.
    /// </summary>
    public class ThreadExtensions
    {
        /// <summary>
        /// Catches all exceptions thrown by Thread.Sleep().
        /// </summary>
        /// <param name="time">The time in miliseconds.</param>
        public static void SaveSleep(int time)
        {
            try
            {
                Thread.Sleep(time);
            }
            catch { }
        }
    }
}
