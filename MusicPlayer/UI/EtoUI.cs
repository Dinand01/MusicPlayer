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
using System.IO;

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
        /// The scrollable containg the songs.
        /// </summary>
        private Scrollable _songCollection;

        /// <summary>
        /// The delay to use when a user types in the filter field.
        /// </summary>
        private UITimer _filterDelay;

        /// <summary>
        /// The last used filter text.
        /// </summary>
        private string _filterText;

        /// <summary>
        /// Initializes a new instance of the <see cref="EtoUI" /> class.
        /// </summary>
        public EtoUI()
        {
            AddControls();
            _filterDelay = new UITimer();
            _filterDelay.Interval = 0.5;
            _filterDelay.Elapsed += FilterDelay_Elapsed;
        }

        #endregion

        /// <summary>
        /// Add all the main controls to the form.
        /// </summary>
        private void AddControls()
        {
            this.Closing += EtoUI_Closing;
            this.Size = new Eto.Drawing.Size(1600, 700);
            this.Title = "MusicPlayer";
            this.WindowStyle = Eto.Forms.WindowStyle.Default;
            var formHandler = (System.Windows.Forms.Form)this.ControlObject;
            formHandler.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            formHandler.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericSerif, (float)18);
            //formHandler.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            //formHandler.AutoScaleDimensions = new System.Drawing.SizeF((float)2.25, (float)2.25);
            RenderMain(ViewType.Home);
        }

        #region Actions

        /// <summary>
        /// Returns to the home page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The evnt arguments.</param>
        private void HomeButton_Click(object sender, EventArgs e)
        {
            RenderMain(ViewType.Home);
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
                RenderMain(ViewType.Playing);
            }
        }

        /// <summary>
        /// Opens all the files in a folder (recursive).
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OpenFromFolder_Click(object sender, EventArgs e)
        {
            using (SelectFolderDialog dialog = new SelectFolderDialog())
            {
                dialog.Title = "Select a folder that contains music files";
                dialog.Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                if (dialog.ShowDialog(this) == DialogResult.Ok)
                {
                    EnsurePlayer();
                    string folder = dialog.Directory;
                    List<Song> temp = _player.LoadAll(Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories), null);
                    RenderMain(ViewType.Playing);
                }
            }
        }

        #endregion

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

        #region MainUI

        /// <summary>
        /// Creates the toolbar.
        /// </summary>
        /// <param name="mainLayout">The main table layout.</param>
        private void CreateToolBar(TableLayout mainLayout)
        {
            var homeButton = new Button
            {
                Image = new Bitmap(Resource.GetImage("Home-96.png"), 45, 45),
                Width = 45,
                BackgroundColor = ColorPallete.Colors[ColorPallete.Color.Primary3],
                ToolTip = "Return to home"
            };

            homeButton.Click += HomeButton_Click;
            mainLayout.Rows.Add(new TableRow
            {
                Cells =
                {
                    new TableCell
                    {
                        Control = new TableLayout
                        {
                            Rows =
                            {
                                new TableRow
                                {
                                    Cells = {
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
                                            },
                                            ScaleWidth = true
                                        },
                                        null
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Adds the main action rows to the table layout.
        /// </summary>
        /// <param name="mainLayout">The main table layout.</param>
        private void CreateMainActions()
        {
            var contentCell = GetContent();
            TableLayout actionsLayout = new TableLayout();
            TableRow mainActions = new TableRow();
            mainActions.ScaleHeight = false;
            mainActions.Cells.Add(null);
            mainActions.Cells.Add(CreateActionCell(OpenFromFile_Click, "Open files", Resource.GetImage("Audio File-96.png")));
            mainActions.Cells.Add(null);
            mainActions.Cells.Add(CreateActionCell(OpenFromFolder_Click, "Open files in folder", Resource.GetImage("Open Folder-96.png")));
            mainActions.Cells.Add(null);
            mainActions.Cells.Add(CreateActionCell(OpenFromFile_Click, "Open", Resource.GetImage("Copy Filled-100.png")));
            mainActions.Cells.Add(null);
            actionsLayout.Rows.Add(null);
            actionsLayout.Rows.Add(mainActions);
            actionsLayout.Rows.Add(null);
            contentCell.Control = actionsLayout;
        }

        /// <summary>
        /// Method for creating an action table cell.
        /// </summary>
        /// <param name="handler">The associated event handler.</param>
        /// <param name="image">The action button image.</param>
        /// <param name="toolTip">The tooltip.</param>
        /// <returns>The table cell.</returns>
        private TableCell CreateActionCell(EventHandler<EventArgs> handler, string toolTip, Bitmap image)
        {
            Button openFromFile = new Button();
            openFromFile.ToolTip = "Open";
            openFromFile.ImagePosition = ButtonImagePosition.Above;
            openFromFile.BackgroundColor = ColorPallete.Colors[ColorPallete.Color.Primary0];
            openFromFile.MouseEnter += (sender, e) =>
            {
                ((Button)sender).BackgroundColor = ColorPallete.Colors[ColorPallete.Color.Primary3];
            };

            openFromFile.MouseLeave += (sender, e) =>
            {
                ((Button)sender).BackgroundColor = ColorPallete.Colors[ColorPallete.Color.Primary0];
            };

            openFromFile.Image = new Bitmap(image, 94, 94);
            openFromFile.MinimumSize = new Eto.Drawing.Size(200, 200);

            openFromFile.Click += handler;
            var systemButton = ((System.Windows.Forms.Button)openFromFile.ControlObject);
            systemButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            systemButton.FlatAppearance.BorderSize = 0;

            return new TableCell(openFromFile, false);
        }

        #endregion

        #region PlaylistInformation

        /// <summary>
        /// Method to create the playing content pane.
        /// </summary>
        private void ShowPlayingContent()
        {
            var contentCell = GetContent();

            // Create the action row.
            TableLayout contentLayout = new TableLayout();
            TableRow controlRow = new TableRow
            {
                Cells =
                {
                    new TableCell
                    {
                        Control = new Button
                        {
                            Text = "Play",
                            Width = 50
                        }
                    },
                    new TableCell
                    {

                    }
                }
            };

            contentLayout.Rows.Add(controlRow);

            // Create the list view.
            var songTable = new DynamicLayout
            {
                BackgroundColor = ColorPallete.Colors[ColorPallete.Color.Primary2],
            };

            RenderPartialSongList(songTable, string.Empty);

            _songCollection = new Scrollable();
            var nativeScrollable = (System.Windows.Forms.ScrollableControl)_songCollection.ControlObject;
            _songCollection.Content = songTable;

            var filterbox = new TextBox
            {
                ToolTip = "Search",
                Width = 200,
                ShowBorder = false,
                PlaceholderText = "Filter"
            };

            filterbox.TextChanged += Filterbox_TextChanged;

            TableRow contentRow = new TableRow
            {
                Cells =
                {
                    new TableCell
                    {
                        ScaleWidth = true,
                        Control = new TableLayout
                        {
                            Rows =
                            {
                                new TableRow
                                {
                                    Cells =
                                    {
                                        new TableCell
                                        {
                                            ScaleWidth = true,
                                            Control = new TableLayout
                                            {
                                                Padding = new Padding(5, 5),
                                                Rows =
                                                {
                                                    new TableRow
                                                    {
                                                        Cells =
                                                        {
                                                            null,
                                                            new TableCell
                                                            {
                                                                Control = filterbox
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                },
                                new TableRow
                                {
                                    Cells =
                                    {
                                        new TableCell
                                        {
                                            Control = _songCollection,
                                            ScaleWidth = true,
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            contentLayout.Rows.Add(contentRow);
            contentCell.Control = contentLayout;
            this.Content = _mainLayout;
        }

        /// <summary>
        /// Renders the results when the user stops typing.
        /// </summary>
        /// <param name="sender">The timer.</param>
        /// <param name="e">The parameters.</param>
        private void FilterDelay_Elapsed(object sender, EventArgs e)
        {
            RenderPartialSongList((DynamicLayout)_songCollection.Content, _filterText);
            _filterDelay.Stop();
        }

        /// <summary>
        /// Filters the shown songs.
        /// </summary>
        /// <param name="sender">The textbox.</param>
        /// <param name="e">The event arguments.</param>
        private void Filterbox_TextChanged(object sender, EventArgs e)
        {
            _filterText = ((TextBox)sender).Text;
            _filterDelay.Stop();
            _filterDelay.Start();
        }

        /// <summary>
        /// Renders a partial piece of the song table.
        /// </summary>
        /// <param name="songTable">The container to render songs in.</param>
        /// <param name="searchTerm">The searhc term of the collection.</param>
        private void RenderPartialSongList(DynamicLayout songTable, string searchTerm)
        {
            if (_player != null && _player.SongList != null)
            {
                songTable.SuspendLayout();
                IEnumerable<Song> songList = _player.SongList;
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    songList = _player.SongList.Where(s => s.Title.ToLower().Contains(searchTerm.ToLower()) || s.Band != null && s.Band.ToLower().Contains(searchTerm.ToLower()));
                }

                songList = songList.OrderBy(s => s.Band).ThenBy(s => s.Title).Take(100).ToList();


                if (songTable.Rows == null || songTable.Rows.Count < 100)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        songTable.AddRow(CreateSongRow(null));
                    }
                }

                int row = 0;
                foreach (var song in songList)
                {
                    var rowLabels = songTable.Rows[row].OfType<DynamicControl>().Select(r => r.Control).OfType<Label>().ToList();
                    foreach (Label l in rowLabels)
                    {
                        l.Visible = true;
                        l.DataContext = song;
                    }

                    rowLabels[0].Text = song.Title;
                    rowLabels[1].Text = song.Band;
                    rowLabels[2].Text = song.Gengre;
                    rowLabels[3].Text = song.DateAdded.ToString();
                    rowLabels[4].Text = song.DateCreated.ToString();
                    row++;
                }

                if(row < 100)
                {
                    songTable.Rows.Skip(row).OfType<DynamicRow>().SelectMany(r => r.OfType<DynamicControl>().Select(c => c.Control)).ToList()
                        .ForEach(c => c.Visible = false);
                }

                songTable.ResumeLayout();
            }
        }

        /// <summary>
        /// Creates a row for a song.
        /// </summary>
        /// <param name="song">The song.</param>
        /// <returns>The row.</returns>
        private Control[] CreateSongRow(Song song)
        {
            return new Control[]
            {
               CreateSongLabel(song, song != null ? song.Title : string.Empty, "Title"),
               CreateSongLabel(song, song != null ? song.Band : string.Empty, "Band"),
               CreateSongLabel(song, song != null ? song.Gengre : string.Empty, "Gengre"),
               CreateSongLabel(song, song != null ? song.DateAdded.ToString() : string.Empty, "Date Added"),
               CreateSongLabel(song, song != null ? song.DateCreated.ToString() : string.Empty, "Date Created")
            };
        }

        /// <summary>
        /// Creates a song label.
        /// </summary>
        /// <param name="song">The song.</param>
        /// <param name="value">The value.</param>
        /// <param name="tooltip">The tooltip.</param>
        /// <returns>The label.</returns>
        private Label CreateSongLabel(Song song, string value, string tooltip)
        {
            Label l = new Label
            {
                Text = value,
                ToolTip = tooltip,
                Cursor = new Cursor(CursorType.Pointer),
                DataContext = song,
                Visible = song == null ? false : true
            };

            l.MouseDoubleClick += L_MouseDoubleClick;
            return l;
        }

        /// <summary>
        /// Handles a new song selection.
        /// </summary>
        /// <param name="sender">The label.</param>
        /// <param name="e">The evnt arguments.</param>
        private void L_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Label label = (Label)sender;
            Song song = label.DataContext as Song;
            if(song != null)
            {
                _player.Load(song);
            }
        }

        #endregion

        /// <summary>
        /// Clears the content row, only navigation remains.
        /// </summary>
        /// <returns>The content cell.</returns>
        private TableCell GetContent()
        {
            if(_mainLayout.Rows.Count == 1)
            { 
                _mainLayout.Rows.Add(new TableRow
                {
                    Cells =
                    {
                        new TableCell
                        {
                            ScaleWidth = true
                        }
                    }
                });
            }

            return _mainLayout.Rows[1].Cells[0];
        }

        /// <summary>
        /// Renders the main layout (refresh).
        /// Eto does not support content changes.
        /// </summary>
        private void RenderMain(ViewType type)
        {
            _mainLayout = new TableLayout
            {
                Spacing = new Eto.Drawing.Size(5, 5),
                BackgroundColor = new Color((float)0.027, (float)0.043, (float)0.067, 1)
            };

            // Create the top row
            CreateToolBar(_mainLayout);

            switch (type)
            {
                case ViewType.Playing:
                    ShowPlayingContent();
                    break;
                case ViewType.Home:
                default:
                    CreateMainActions();
                    break;
            }

            this.Content = _mainLayout;
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
        public new void Dispose()
        {
            if (_player != null)
            {
                _player.Dispose();
            }
        }
    }
}
