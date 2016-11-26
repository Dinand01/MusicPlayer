using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;


namespace MusicPlayer.UI
{
    /// <summary>
    /// The main form build with Eto.
    /// </summary>
    internal class EtoUI : Form, IDisposable
    {
        public EtoUI()
        {
            AddControls();
        }

        private void AddControls()
        {
            this.ClientSize = new Size(800, 350);
            this.Title = "MusicPlayer";
            var layout = new TableLayout();
        }

        public void Dispose()
        {
        }
    }
}
