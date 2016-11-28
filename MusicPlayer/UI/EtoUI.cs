using Eto.Drawing;
using Eto.Forms;
using MusicPlayer.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using MusicPlayer.Models;
using System.Resources;
using System.Collections.ObjectModel;
using System.Windows;

namespace MusicPlayer.UI
{
    /// <summary>
    /// The main form build with Eto.
    /// </summary>
    internal class EtoUI : Form, IUI, IDisposable
    {
        #region Variables

        /// <summary>
        /// The main music player logic.
        /// </summary>
        private Player _player;

        /// <summary>
        /// The main table layout.
        /// </summary>
        private TableLayout _mainLayout;



        /// <summary>
        /// Initializes a new instance of the <see cref="EtoUI" /> class.
        /// </summary>
        public EtoUI()
        {
            AddControls();
        }

        #endregion

        private void AddControls()
        {
            this.Closing += EtoUI_Closing;
            this.ClientSize = new Eto.Drawing.Size(800, 350);
            this.Title = "MusicPlayer";
            this.WindowStyle = Eto.Forms.WindowStyle.Default;
            _mainLayout = new TableLayout
            {
                Spacing = new Eto.Drawing.Size(5, 5),
                BackgroundColor = new Color((float)0.027, (float)0.043, (float)0.067, 1)
            };

            var homeButton = new Button
            {
                Image = new Bitmap(Resource.GetImage("Home-96.png"), 25, 25),
                //Width = 30,
                ToolTip = "Return to home"
            };

            //var homeSystemButton = ((System.Windows.Controls.Button)homeButton.ControlObject);
            //homeSystemButton.BorderThickness = new Thickness(0);

            homeButton.Click += HomeButton_Click;
            _mainLayout.Rows.Add(new TableRow
            {
                Cells = 
                {
                    new TableCell
                    {
                        Control = homeButton,
                        ScaleWidth = false
                    },
                    new TableCell
                    {
                        Control = new Label
                        {
                            ID = Models.Controls.NotificationTop.ToString(),
                            Text = string.Empty
                        }
                    },
                    null
                }
            });

            // Add the default actions
            AddMainActions(_mainLayout);

            this.Content = _mainLayout;
        }

        private void HomeButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the files selected in a file dialog.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OpenFromFile_Click(object sender, EventArgs e)
        {
            EnsurePlayer();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filters.Add(new FileDialogFilter("Audio", new string[] { "mp3", "flac", "wma" }));

            openFileDialog1.MultiSelect = true;
            DialogResult result = openFileDialog1.ShowDialog(_mainLayout);
            if (result == DialogResult.Ok)
            {
                List<Song> temp = _player.LoadAll(openFileDialog1.Filenames.ToArray(), null);
            }
        }

        /// <summary>
        /// Ensures that a musicplayer exists.
        /// </summary>
        /// <param name="reset">Dispose and create a new player.</param>
        private void EnsurePlayer(bool reset = false)
        {
            if(reset && _player != null)
            {
                _player.Dispose();
            }

            if(_player == null)
            {
                _player = new Player(this);
            }
        }

        public void SetSongDuration(TimeSpan duration)
        {
            ////throw new NotImplementedException();
        }

        public void SetSongs(List<Song> songs)
        {
            ////throw new NotImplementedException();
        }

        public void SetSong(Song song)
        {
            ////throw new NotImplementedException();
        }

        public void SetSongPosition(TimeSpan currentTime)
        {
            ////throw new NotImplementedException();
        }

        public void SetNotification(string message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds the main action rows to the table layout.
        /// </summary>
        /// <param name="mainLayout">The main table layout.</param>
        private void AddMainActions(TableLayout mainLayout)
        {
            TableRow mainActions = new TableRow();
            mainActions.ScaleHeight = false;
            mainActions.Cells.Add(null);
            mainActions.Cells.Add(CreateActionCell());
            mainActions.Cells.Add(null);
            mainActions.Cells.Add(CreateActionCell());
            mainActions.Cells.Add(null);
            mainActions.Cells.Add(CreateActionCell());
            mainActions.Cells.Add(null);
            mainLayout.Rows.Add(null);
            mainLayout.Rows.Add(mainActions);
            mainLayout.Rows.Add(null);
        }

        /// <summary>
        /// Method for creating an action table cell.
        /// </summary>
        /// <returns>The table cell.</returns>
        private TableCell CreateActionCell()
        {
            Button openFromFile = new Button();
            openFromFile.ToolTip = "Open";
            openFromFile.ImagePosition = ButtonImagePosition.Above;
            openFromFile.BackgroundColor = ColorPallete.Colors[ColorPallete.Color.Primary0];
            var image = Resource.GetImage("Audio File-96.png");
            //openFromFile.MouseEnter += (sender, e) =>
            //{
            //    ((Button)sender).BackgroundColor = ColorPallete.Colors[ColorPallete.Color.Primary0];
            //};

            //openFromFile.MouseLeave += (sender, e) =>
            //{
            //    ((Button)sender).BackgroundColor = ColorPallete.Colors[ColorPallete.Color.Primary3];
            //};

            //openFromFile.Font = new Font(SystemFont.Bold, 16, FontDecoration.Underline);
            //openFromFile.TextColor = new Color(255, 255, 255);
            openFromFile.Image = new Bitmap(image, 94, 94);
            openFromFile.MinimumSize = new Eto.Drawing.Size(100, 100);
            
            openFromFile.Click += OpenFromFile_Click;
            //var systemButton = ((System.Windows.Controls.Button)openFromFile.ControlObject);
            //systemButton.BorderThickness = new Thickness(0);
            //systemButton.Foreground = ColorPallete.Colors[ColorPallete.Color.Primary3].ToBrush();
            //systemButton.OverridesDefaultStyle = true;
            //systemButton.ApplyTemplate();

            //systemButton.MouseMove += (sender, e) =>
            //{
            //    ((System.Windows.Controls.Button)sender).Background = ColorPallete.Colors[ColorPallete.Color.Primary0].ToBrush();
            //};

            //systemButton.MouseLeave += (sender, e) =>
            //{
            //    systemButton.Background = ColorPallete.Colors[ColorPallete.Color.Primary3].ToBrush();
            //};

            return new TableCell(openFromFile, false);
        }

        /// <summary>
        /// Handles the close event.
        /// </summary>
        private void EtoUI_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Dispose();
        }

        /// <summary>
        /// Disposes off all assets.
        /// </summary>
        public void Dispose()
        {
            if (_player != null)
            {
                _player.Dispose();
            }
        }
    }
}
