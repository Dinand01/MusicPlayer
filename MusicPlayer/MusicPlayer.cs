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

namespace MusicPlayer
{

    public partial class MusicPlayer : Form
    {
        GUI Gui;

        /// <summary>
        /// Register the default connection factory and create the connection string
        /// Initialize the form
        /// </summary>
        /// <param name="args"></param>
        public MusicPlayer(string[] args)
        {
            InitializeComponent();

            string directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            directory = directory.Replace("file:\\", "");
            var dataSource = "Data Source=" + directory + "\\MusicPlayer.DAL.DbContext.sdf;Persist Security Info=False;";

            System.Data.Entity.Database.DefaultConnectionFactory =
            new System.Data.Entity.Infrastructure.SqlCeConnectionFactory(
            "System.Data.SqlServerCe.4.0",
            directory,
            dataSource);
            
            //player = new Player();
            Gui = new GUI(this);
            if (args.Length != 0)
                Gui.Open(args);

            var test = dataSource;
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
