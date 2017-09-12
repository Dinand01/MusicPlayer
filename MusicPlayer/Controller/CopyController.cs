using MusicPlayer.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// The copy controller.
    /// </summary>
    internal class CopyController : ICopy
    {
        /// <summary>
        /// The current copy completion.
        /// </summary>
        private double _percentage = 0;

        /// <summary>
        /// The randommizer.
        /// </summary>
        private Random _rand = new Random((int)DateTime.Now.ToFileTimeUtc());

        /// <summary>
        /// The progress changed event.
        /// </summary>
        public event ProgressChanged ProgressChanged;

        /// <summary>
        /// Copies random song from the sourceLoc to the destination loc.
        /// </summary>
        /// <param name="sourceLoc">The source location.</param>
        /// <param name="destinationLoc">The destination location.</param>
        /// <param name="amount">The amount of songs to copy.</param>
        public void CopyRandomSongs(string sourceLoc, string destinationLoc, int amount)
        {
            var directory = new DirectoryInfo(sourceLoc);
            var masks = new[] { "*.mp3", "*.wav", "*.flac" };
            var sourceList = masks.SelectMany(m => directory.EnumerateFiles(m, SearchOption.AllDirectories)).ToList();
            List<FileInfo> toCopy = new List<FileInfo>();

            if (sourceList.Count < amount)
            {
                amount = sourceList.Count;
            }

            while (toCopy.Count < amount)
            {
                toCopy.Add(TakeRandom(sourceList, toCopy));
            }
            

            Task.Run(() => Copy(toCopy, destinationLoc));
        }

        /// <summary>
        /// Takes a random song, excludes the alreadychosensongs.
        /// </summary>
        /// <param name="source">The source pool.</param>
        /// <param name="alreadyChosenSongs">The chosen songs.</param>
        /// <returns>A random song.</returns>
        private FileInfo TakeRandom(List<FileInfo> source, List<FileInfo> alreadyChosenSongs)
        {
            List<FileInfo> pool = source.Except(alreadyChosenSongs).ToList();
            FileInfo randLoc = pool[_rand.Next(0, pool.Count - 1)];
            return randLoc;
        }

        /// <summary>
        /// Copies the source files to the destination.
        /// </summary>
        /// <param name="source">The files to copy.</param>
        /// <param name="destination">The destination.</param>
        private void Copy(List<FileInfo> source, string destination)
        {
            long bytesTransfered = 0;
            long totalBytes = source.Aggregate<FileInfo, long>(0, (res, info) => res += info.Length);
            for (int i = 0; i < source.Count; i++)
            {
                try
                {
                    File.Copy(source[i].FullName, destination + "\\" + Path.GetFileName(source[i].FullName), true);
                    bytesTransfered += source[i].Length;
                    ProgressChanged?.Invoke(GetProgress(bytesTransfered, totalBytes));
                }
                catch 
                {
                }
            }
        }

        /// <summary>
        /// Gets the progress.
        /// </summary>
        /// <param name="complete">The amount of bytes that are transfered.</param>
        /// <param name="total">The total amount of bytes to transfer.</param>
        /// <returns>The percentage.</returns>
        private double GetProgress(long complete, long total)
        {
            return complete / (double)total * 100;
        }
    }
}
