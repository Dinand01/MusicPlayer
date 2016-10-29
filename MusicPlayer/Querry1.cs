using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicPlayer
{
    public partial class Querry1 : Form
    {
        public string destination = "", source = "";
        public Querry1()
        {

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a folder that contains music files";
                dialog.ShowNewFolderButton = false;
                //dialog.SelectedPath = Environment.SpecialFolder.MyMusic.;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    source = dialog.SelectedPath;
                }
            }
            if (destination != "" && source != "" && textBox1.Text != "") {
                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select destination folder";
                dialog.ShowNewFolderButton = false;
                //dialog.SelectedPath = Environment.SpecialFolder.MyMusic.;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    destination = dialog.SelectedPath;
                }
                if (destination != "" && source != "" && textBox1.Text != "")
                {
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                }
            }
        }
    }
}
