using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusicPlayer.Models;

namespace MusicPlayer
{
    public partial class Progress : Form
    {
        public Progress(List<Song> source, string destination)
        {
            InitializeComponent();
            Thread th = new Thread(new ThreadStart(() => Copy(source, destination)));
            th.Start();
        }

        private void Copy(List<Song> source, string destination){
            for (int i = 0; i < source.Count; i++)
            {
                string path = source[i].Location;
                //Console.WriteLine(i + "   " + path);
                label1.Invoke((MethodInvoker)(() => label1.Text = "Copying: " + path));
                progressBar1.Invoke((MethodInvoker)(()=> progressBar1.Value = (int)(((i + 1) / (double) source.Count) * 100)));
                progressBar1.Invoke((MethodInvoker)(() => progressBar1.Update()));
                try
                {
                    File.Copy(path, destination + "\\" + Path.GetFileName(path), true);
                }
                catch (Exception e) 
                { 
                    MessageBox.Show(e.Message); 
                }
            }

            this.Invoke((MethodInvoker)(()=>this.Dispose()));
        }
    }
}
