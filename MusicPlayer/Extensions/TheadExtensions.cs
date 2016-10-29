using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicPlayer.Extensions
{
    public class ThreadExtensions
    {
        public static void SaveSleep(int time)
        {
            try
            {
                Thread.Sleep(time);
            }
            catch
            {
            }
        }
    }
}
