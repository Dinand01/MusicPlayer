using System;
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
    public partial class Form1 : Form
    {
        GUI Gui;
        public Form1(string[] args)
        {

            InitializeComponent();
            //player = new Player();
            Gui = new GUI(this);
            if (args.Length != 0)
                Gui.Open(args);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Gui.Dispose();
        }
    }
}
