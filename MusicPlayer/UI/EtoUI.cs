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
using System.IO;
using System.Net;

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
        /// Ditionary conating UI elements.
        /// </summary>
        private Dictionary<UIElements, Control> _uiElements;

        /// <summary>
        /// The delay to use when a user types in the filter field.
        /// </summary>
        private UITimer _filterDelay;

        /// <summary>
        /// The last used filter text.
        /// </summary>
        private string _filterText;

        /// <summary>
        /// The current song to display.
        /// </summary>
        private string _currentSong;

        /// <summary>
        /// The duration of the current song.
        /// </summary>
        private TimeSpan _currentSongDuration;

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

        #region Actions

        /// <summary>
        /// Returns to the home page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void HomeButton_Click(object sender, EventArgs e)
        {
            Render(ViewType.Home);
        }

        /// <summary>
        /// Go to the server page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ServerButton_Click(object sender, EventArgs e)
        {
            Render(ViewType.Server);
        }

        /// <summary>
        /// Returns to the playing page.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void AudioButton_Click(object sender, EventArgs e)
        {
            Render(ViewType.Playing);
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
                Render(ViewType.Playing);
                _player.Play(temp.FirstOrDefault());
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
                    Render(ViewType.Playing);
                    _player.Play(temp.FirstOrDefault());
                }
            }
        }

        #endregion

        #region Interface

        /// <summary>
        /// Sets the song duration on the UI.
        /// </summary>
        /// <param name="duration">The duration.</param>
        public void SetSongDuration(TimeSpan duration)
        {
            _currentSongDuration = duration;
            if(_uiElements.ContainsKey(UIElements.Slider) && _uiElements[UIElements.Slider] != null)
            {
                ((Slider)_uiElements[UIElements.Slider]).MaxValue = (int)duration.TotalMilliseconds;
                ((Slider)_uiElements[UIElements.Slider]).Value = 0;
            }
        }

        public void SetSongs(List<Song> songs)
        {
            
        }

        /// <summary>
        /// Sets the current active song.
        /// </summary>
        /// <param name="song">The song to set.</param>
        public void SetSong(Song song)
        {
            if (!song.SourceIsDb)
            {
                song = _player.GetDetailsFromDbOrFile(song);
            }

            _currentSong = song.Band + " - " + song.Title;
            if (_uiElements.ContainsKey(UIElements.CurrentSong) && _uiElements[UIElements.CurrentSong] != null)
            {
                ((Label)_uiElements[UIElements.CurrentSong]).Text = _currentSong;
            }
        }

        /// <summary>
        /// Sets the songs position.
        /// </summary>
        /// <param name="currentTime">The current song time.</param>
        public void SetSongPosition(TimeSpan currentTime)
        {
            if (_uiElements.ContainsKey(UIElements.Slider) && _uiElements[UIElements.Slider] != null)
            {
                var slider = ((Slider)_uiElements[UIElements.Slider]);
                var trackbar = (System.Windows.Forms.TrackBar)slider.ControlObject;
                trackbar.Invoke((System.Windows.Forms.MethodInvoker)(delegate () 
                {
                    if (slider.MaxValue < currentTime.TotalMilliseconds)
                    {
                        SetSongDuration(_currentSongDuration);
                    }

                    slider.Value = (int)currentTime.TotalMilliseconds;
                }));
            }
        }

        public void SetNotification(string message)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region MainUI

        /// <summary>
        /// Add all the main controls to the form.
        /// </summary>
        private void AddControls()
        {
            _uiElements = new Dictionary<UIElements, Control>();
            this.Closing += EtoUI_Closing;
            ////this.Size = new Eto.Drawing.Size(1600, 700);
            this.Title = "MusicPlayer";
            this.WindowStyle = Eto.Forms.WindowStyle.Default;
            var formHandler = (System.Windows.Forms.Form)this.ControlObject;
            formHandler.Size = new System.Drawing.Size(900, 450);
            formHandler.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericSerif, (float)10);
            formHandler.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            //formHandler.AutoScaleDimensions = new System.Drawing.SizeF((float)0.2, (float)0.2);
            Render(ViewType.Home);
        }

        /// <summary>
        /// Creates the toolbar.
        /// </summary>
        /// <param name="mainLayout">The main table layout.</param>
        private void CreateToolBar(TableLayout mainLayout)
        {
            _uiElements[UIElements.HomeButton] = CreateToolBarbutton("Return to home", Resource.GetImage("Home-96.png"), HomeButton_Click);
            _uiElements[UIElements.ServerButton] = CreateToolBarbutton("Host server", Resource.GetImage("Satellite Sending Signal-96.png"), ServerButton_Click, _player != null && _player.Hosting);
            _uiElements[UIElements.AudioButton] = CreateToolBarbutton("Currently Playing", Resource.GetImage("Speaker-96.png"), AudioButton_Click, _player != null && _player.IsPlaying());
            _uiElements[UIElements.Notification] = new Label
            {
                Text = string.Empty
            };

            mainLayout.Rows.Add(new TableRow
            {
                Cells =
                {
                    new TableCell
                    {
                        Control = new TableLayout
                        {
                            Spacing = new Eto.Drawing.Size(5, 5),
                            Padding = new Padding(0, 0, 0, 2),
                            Rows =
                            {
                                new TableRow
                                {
                                    Cells =
                                    {
                                        new TableCell
                                        {
                                            Control = _uiElements[UIElements.HomeButton],
                                            ScaleWidth = false
                                        },
                                        new TableCell
                                        {
                                            Control = _uiElements[UIElements.AudioButton],
                                            ScaleWidth = false
                                        },
                                        new TableCell
                                        {
                                            Control = _uiElements[UIElements.ServerButton],
                                            ScaleWidth = false
                                        },
                                        new TableCell
                                        {
                                            Control =  _uiElements[UIElements.Notification],
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
        /// Creates a toolbar button.
        /// </summary>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="image">The image.</param>
        /// <param name="handler">The handler.</param>
        /// <param name="visible">A boolean indicating whether the button should be visible.</param>
        /// <returns>The button.</returns>
        private Button CreateToolBarbutton(string toolTip, Bitmap image, EventHandler<EventArgs> handler, bool visible = true, int width = 22)
        {
            var button = new Button
            {
                Image = new Bitmap(image, 22, 22),
                Width = width,
                BackgroundColor = ColorPallete.Colors[ColorPallete.Color.Primary2],
                ToolTip = toolTip,
                Visible = visible
            };

            button.Click += handler;
            var nativeButton = (System.Windows.Forms.Button)button.ControlObject;
            nativeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            nativeButton.FlatAppearance.BorderSize = 0;
            return button;
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
            mainActions.Cells.Add(new TableCell { ScaleWidth = true });
            mainActions.Cells.Add(CreateActionCell(OpenFromFile_Click, "Open files", Resource.GetImage("Audio File-96.png")));
            mainActions.Cells.Add(new TableCell { ScaleWidth = true });
            mainActions.Cells.Add(new TableCell { ScaleWidth = false });
            mainActions.Cells.Add(new TableCell { ScaleWidth = true });
            mainActions.Cells.Add(CreateActionCell(OpenFromFolder_Click, "Open files in folder", Resource.GetImage("Open Folder-96.png")));
            mainActions.Cells.Add(new TableCell { ScaleWidth = true });
            mainActions.Cells.Add(new TableCell { ScaleWidth = false });
            mainActions.Cells.Add(new TableCell { ScaleWidth = true });
            mainActions.Cells.Add(CreateActionCell(OpenFromFile_Click, "Open", Resource.GetImage("Copy Filled-100.png")));
            mainActions.Cells.Add(new TableCell { ScaleWidth = true });
            actionsLayout.Rows.Add(null);
            actionsLayout.Rows.Add(mainActions);
            actionsLayout.Rows.Add(null);
            TableRow mainActions2 = new TableRow();
            mainActions2.Cells.Add(new TableCell { ScaleWidth = true });
            mainActions2.Cells.Add(new TableCell { ScaleWidth = false });
            mainActions2.Cells.Add(new TableCell { ScaleWidth = true });
            mainActions2.Cells.Add(CreateActionCell(ServerButton_Click, "Host stream", Resource.GetImage("Satellite Sending Signal-96.png")));
            mainActions2.Cells.Add(new TableCell { ScaleWidth = true });
            mainActions2.Cells.Add(new TableCell { ScaleWidth = false });
            mainActions2.Cells.Add(new TableCell { ScaleWidth = true });
            mainActions2.Cells.Add(CreateActionCell(OpenFromFile_Click, "Connect to stream", Resource.GetImage("GPS Searching-96.png")));
            mainActions2.Cells.Add(new TableCell { ScaleWidth = true });
            mainActions2.Cells.Add(new TableCell { ScaleWidth = false });
            mainActions2.Cells.Add(new TableCell { ScaleWidth = true });
            actionsLayout.Rows.Add(mainActions2);
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
            openFromFile.ToolTip = toolTip;
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

            openFromFile.Image = new Bitmap(image, 50, 50);
            openFromFile.MinimumSize = new Eto.Drawing.Size(100, 100);

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
            _uiElements[UIElements.PlayPauseButton] = CreateToolBarbutton("Play or pause the music", Resource.GetImage("Pause-96.png"), PlayPauseButton_Click, true, 56);
            _uiElements[UIElements.NextButton] = CreateToolBarbutton("Skip to the next song", Resource.GetImage("End-96.png"), NextButton_Click, true, 56);
            _uiElements[UIElements.Slider] = new Slider
            {
                Width = 150,
                Cursor = Cursors.VerticalSplit,
                Height = 17,
                MinValue = 0
            };

            var nativeSlider = (System.Windows.Forms.TrackBar)_uiElements[UIElements.Slider].ControlObject;
            nativeSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            nativeSlider.Scroll += NativeSlider_Scroll;
            _uiElements[UIElements.CurrentSong] = new Label
            {
                TextColor = ColorPallete.Colors[ColorPallete.Color.Primary4],
                BackgroundColor = ColorPallete.Colors[ColorPallete.Color.Primary2],
                Text = _currentSong,
                TextAlignment = Eto.Forms.TextAlignment.Center
            };

            var uilabel = (Label)_uiElements[UIElements.CurrentSong];
            uilabel.Font = new Font(uilabel.Font.Family, 14, Eto.Drawing.FontStyle.Bold);

            // Create the action row.
            TableLayout contentLayout = new TableLayout();
            TableRow controlRow = new TableRow
            {
                Cells =
                {
                    new TableCell
                    {
                        ScaleWidth = true,
                        Control = new TableLayout
                        {
                            Spacing = new Eto.Drawing.Size(5, 5),
                            Rows =
                            {
                                new TableRow
                                {
                                    Cells =
                                    {
                                        new TableCell
                                        {
                                            ScaleWidth = false,
                                            Control = _uiElements[UIElements.PlayPauseButton]
                                        },
                                        new TableCell
                                        {
                                            ScaleWidth = false,
                                            Control = _uiElements[UIElements.NextButton]
                                        },
                                        new TableCell
                                        {
                                            ScaleWidth = true,
                                            Control = _uiElements[UIElements.CurrentSong]
                                        },
                                        new TableCell
                                        {
                                            ScaleWidth = false,
                                            Control = _uiElements[UIElements.Slider]
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            contentLayout.Rows.Add(controlRow);

            // Create the list view.
            var songTable = new DynamicLayout
            {
                BackgroundColor = ColorPallete.Colors[ColorPallete.Color.Primary3]
            };

            RenderPartialSongList(songTable, string.Empty);
            _uiElements[UIElements.MusicList] = new Scrollable
            {
                Border = BorderType.None,
                Content = songTable
            };

            var nativeScrollable = (System.Windows.Forms.ScrollableControl)_uiElements[UIElements.MusicList].ControlObject;
            ((Scrollable)_uiElements[UIElements.MusicList]).UpdateScrollSizes();

            var filterbox = new TextBox
            {
                ToolTip = "Search",
                Width = 142,
                ShowBorder = false,
                PlaceholderText = "Filter",
                Height = 15,
                BackgroundColor = ColorPallete.Colors[ColorPallete.Color.Primary3],
                TextColor = Colors.White
            };

            filterbox.TextChanged += Filterbox_TextChanged;
            var nativeFilterBox = (System.Windows.Forms.TextBox)filterbox.ControlObject;
            nativeFilterBox.Padding = new System.Windows.Forms.Padding(5);

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
                                                Padding = new Padding(2, 2),
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
                                            Control = _uiElements[UIElements.MusicList],
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
        }

        /// <summary>
        /// Pauses or plays the music.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void PlayPauseButton_Click(object sender, EventArgs e)
        {
            if (_player != null)
            {
                _player.PausePlay(null, null);
                string resource = _player.IsPlaying() ? "Pause-96.png" : "Play-96.png";
                ((Button)_uiElements[UIElements.PlayPauseButton]).Image = new Bitmap(Resource.GetImage(resource), 25, 25);
            }
        }

        /// <summary>
        /// Skips the song.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void NextButton_Click(object sender, EventArgs e)
        {
            if (_player != null)
            {
                _player.NextRandomSong();
            }
        }

        /// <summary>
        /// The scroll evnt for the slider, moves the song position.
        /// </summary>
        /// <param name="sender">The trackbar.</param>
        /// <param name="e">The evet arguments.</param>
        private void NativeSlider_Scroll(object sender, EventArgs e)
        {
            System.Windows.Forms.TrackBar temp = (System.Windows.Forms.TrackBar)sender;
            _player.MoveToTime(new TimeSpan(0, 0, 0, 0, temp.Value));
        }

        /// <summary>
        /// Renders the results when the user stops typing.
        /// </summary>
        /// <param name="sender">The timer.</param>
        /// <param name="e">The parameters.</param>
        private void FilterDelay_Elapsed(object sender, EventArgs e)
        {
            RenderPartialSongList((DynamicLayout)((Scrollable)_uiElements[UIElements.MusicList]).Content, _filterText);
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

                var songListResult = songList.ToList();
                int row = 0;
                for (; row < songList.Count(); row++)
                {
                    var song = songListResult[row];
                    if (!song.SourceIsDb)
                    {
                        song = _player.GetDetailsFromDbOrFile(song);
                    }

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
               CreateSongLabel(song, song != null ? song.Title : "Test", "Title"),
               CreateSongLabel(song, song != null ? song.Band : "Test", "Band"),
               CreateSongLabel(song, song != null ? song.Gengre : "Test", "Gengre"),
               CreateSongLabel(song, song != null ? song.DateAdded.ToString() : "Test", "Date Added"),
               CreateSongLabel(song, song != null ? song.DateCreated.ToString() : "Test", "Date Created")
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
                TextColor = Colors.White,
                Visible = song == null ? false : true,
                 VerticalAlignment = Eto.Forms.VerticalAlignment.Center,
                Width = 100
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

        #region Server

        /// <summary>
        /// Shows the server settings page.
        /// </summary>
        private void ShowServerSettings()
        {
            var contentCell = GetContent();
            _uiElements[UIElements.IPPort] = new TextBox
            {
                Text = "8963",
                ToolTip = "The port the server will be hosted on",
                PlaceholderText = "Please enter a port (TCP)",
                BackgroundColor = ColorPallete.Colors[ColorPallete.Color.Primary1],
                TextColor = Colors.White
            };

            var nativePort = (System.Windows.Forms.TextBox)_uiElements[UIElements.IPPort].ControlObject;
            nativePort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            nativePort.Font = new System.Drawing.Font(nativePort.Font.FontFamily, 14);
            nativePort.Margin = new System.Windows.Forms.Padding(5);

            var serverStatusButton = new Button
            {
                Text = _player != null && _player.Hosting ? "Disconnect" : "Stream",
                BackgroundColor = ColorPallete.Colors[ColorPallete.Color.Primary2],
                TextColor = Colors.White,
                Font = new Font(SystemFont.TitleBar, 8),
            };

            serverStatusButton.Click += ServerStatusButton_Click;
            var nativebutton = (System.Windows.Forms.Button)serverStatusButton.ControlObject;
            nativebutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            nativebutton.FlatAppearance.BorderSize = 0;
            _uiElements[UIElements.ServerButton] = serverStatusButton;

            TableLayout content = new TableLayout
            {
                Rows =
                {
                    new TableRow
                    {
                        Cells =
                        {
                            null,
                            new TableCell
                            {
                                ScaleWidth = false,
                                Control = new Label
                                {
                                    Text = "Host a stream",
                                    TextAlignment = Eto.Forms.TextAlignment.Center,
                                    Font = new Font(SystemFont.TitleBar, 12),
                                    TextColor = ColorPallete.Colors[ColorPallete.Color.Primary1],
                                    ToolTip = "Remember to forward the port below to your local ip adres when hosting a stream over the internet"
                                }
                            },
                            null
                        }
                    },
                    null,
                    new TableRow
                    {
                        Cells =
                        {
                            null,
                            new TableCell
                            {
                                Control = new TableLayout
                                {
                                    Rows =
                                    {
                                        new TableRow
                                        {
                                            Cells =
                                            {
                                                null, 
                                                new TableCell
                                                {
                                                    Control = _uiElements[UIElements.IPPort]
                                                },
                                                null
                                            }
                                        }
                                    }
                                }
                            },
                            null
                        }
                    },
                    null,
                    new TableRow
                    {
                        Cells =
                        {
                            null,
                            new TableCell
                            {
                                Control = new TableLayout
                                {
                                    Rows =
                                    {
                                        new TableRow
                                        {
                                            Cells =
                                            {
                                                null,
                                                new TableCell
                                                {
                                                    Control = _uiElements[UIElements.ServerButton]
                                                },
                                                null
                                            }
                                        }
                                    }
                                }
                            },
                            null
                        }
                    },
                    null
                }
            };

            contentCell.Control = content;
        }

        /// <summary>
        /// Connects of disconnects a server.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The event arguments.</param>
        private void ServerStatusButton_Click(object sender, EventArgs e)
        {
            EnsurePlayer();
            if (!_player.Hosting)
            {
                string port = ((TextBox)_uiElements[UIElements.IPPort]).Text;
                int parsedPort = 0;
                if (int.TryParse(port, out parsedPort))
                {
                    _player.StartAudioServer(IPAddress.Loopback, parsedPort);
                    Render(ViewType.Home);
                }
                else
                {
                    MessageBox.Show("The entered port was not a number", MessageBoxType.Warning);
                }
            }
            else
            {
                _player.DisconnectFromAudioServer();
                Render(ViewType.Home);
            }
        }

        #endregion

        #region Common

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
        /// Ensures that a musicplayer exists.
        /// </summary>
        /// <param name="reset">Dispose and create a new player.</param>
        private void EnsurePlayer(bool reset = false)
        {
            if (reset && _player != null)
            {
                _player.Dispose();
            }

            if (_player == null)
            {
                _player = new Player(this);
            }
        }

        /// <summary>
        /// Renders the main layout (refresh).
        /// Eto does not support content changes.
        /// </summary>
        private void Render(ViewType type)
        {
            this.SuspendLayout();
            _uiElements[UIElements.Slider] = null;
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
                    _uiElements[UIElements.AudioButton].Visible = true;
                    ShowPlayingContent();
                    break;
                case ViewType.Server:
                    ShowServerSettings();
                    break;
                case ViewType.Home:
                default:
                    CreateMainActions();
                    break;
            }

            Task.Run(delegate ()
            {
                if (this.Content != null)
                {
                    var nativeTable = (System.Windows.Forms.Form)this.ControlObject;
                    nativeTable.Invoke((System.Windows.Forms.MethodInvoker)(delegate ()
                    {
                        this.Content = _mainLayout;
                        this.ResumeLayout();
                        this.Width = this.Width == 901 ? 900 : 901;
                    }));
                }
                else
                {
                    this.Content = _mainLayout;
                    this.ResumeLayout();
                }
            });
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

        #endregion
    }
}
