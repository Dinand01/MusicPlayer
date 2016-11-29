using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusicPlayer.Models;

namespace MusicPlayer
{
    internal partial class OpenSpecial : Form
    {
        public Song result;
        public OpenSpecial()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set the dilaogresult to ok here
        /// parse parameters here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Submit_Click(object sender, EventArgs e)
        {
            result = new Song();

            if (this.addedOnChecked.Checked)
            {
                result.DateAdded = this.addedOn.Value;
            }

            if(!string.IsNullOrEmpty(this.search.Text))
            {
                result.SearchTerm = this.search.Text;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
