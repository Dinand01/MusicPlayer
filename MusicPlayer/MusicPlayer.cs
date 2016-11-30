using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusicPlayer.Extensions;
using System.Data.Entity.Infrastructure;
using MusicPlayer.UI;
using Eto.Forms;

namespace MusicPlayer
{
    /// <summary>
    /// Class containing the main form. 
    /// TODO: replace GUI with web based UI using cefsharp.
    /// </summary>
    public partial class MusicPlayer : System.Windows.Forms.Form
    {
        //private GUI Gui;
        private EtoUI Gui;

        /// <summary>
        /// Register the default connection factory and create the connection string
        /// Initialize the form
        /// </summary>
        /// <param name="args"></param>
        public MusicPlayer(string[] args)
        {
            InitializeComponent();
            //this.FormBorderStyle = FormBorderStyle.None;

            // Set config for the entity framework


            // Populate window
            //Gui = new GUI(this);
            //if (args.Length != 0)
            //{
            //    Gui.Open(args);
            //}

            //Gui = new GUI(this);
            Gui = new EtoUI();
            var native = Gui.ToNative(true);
            native.Location = new Point(0, 0);
            Controls.Add(native);
        }

        /// <summary>
        /// Dispose of the GUI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Gui.Dispose();
        }
    }
}
