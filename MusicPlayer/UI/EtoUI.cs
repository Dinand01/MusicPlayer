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
using System.Windows.Threading;

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
        private Player _player, _copyFiles;

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
        /// The copy files directories.
        /// </summary>
        private string _sourceDir, _destDir;

        /// <summary>
        /// The duration of the current song.
        /// </summary>
        private TimeSpan _currentSongDuration;

        /// <summary>
        /// The network client.
        /// </summary>
        private NetworkClient _networkClient;

        /// <summary>
        /// The dispatcher for the ui thread.
        /// </summary>
        private Dispatcher _uiDispatcher = Dispatcher.CurrentDispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="EtoUI" /> class.
        /// </summary>
        public EtoUI() : base(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EtoUI" /> class.
        /// </summary>
        /// <param name="args">The arguments (files).</param>
        public EtoUI(string[] args)
        {
            AddControls();
            _filterDelay = new UITimer();
            _filterDelay.Interval = 0.5;
            _filterDelay.Elapsed += FilterDelay_Elapsed;
            if(args != null && args.Length > 0)
            {
                try
                {
                    EnsurePlayer();
                    var temp = _player.LoadAll(args);
                    _player.Play(temp.FirstOrDefault());
                    if (_player.IsPlaying())
                    {
                        Render(ViewType.Home);
                    }
                }
                catch
                {
                    MessageBox.Show("Invalid file", MessageBoxType.Warning);
                }
            }
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
        /// Returns to the client view.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The event arguments.</param>
        private void ClientButton_Click(object sender, EventArgs e)
        {
            Render(ViewType.Client);
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
        /// Opens the copy window.
        /// </summary>
        /// <param name="sender"><The button./param>
        /// <param name="e">The evnt args.</param>
        private void OpenCopy_Click(object sender, EventArgs e)
        {
            Render(ViewType.Copy);
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
                RemoveNetworkClient();
                List<Song> temp = _player.LoadAll(openFileDialog1.Filenames.ToArray());
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
                    RemoveNetworkClient();
                    EnsurePlayer();
                    string folder = dialog.Directory;
                    List<Song> temp = _player.LoadAll(Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories));
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
            _uiDispatcher.Invoke(delegate ()
            {
                if (_uiElements.ContainsKey(UIElements.Slider) && _uiElements[UIElements.Slider] != null)
                {
                    ((Slider)_uiElements[UIElements.Slider]).MaxValue = (int)duration.TotalMilliseconds;
                    ((Slider)_uiElements[UIElements.Slider]).Value = 0;
                }
            });
        }

        /// <summary>
        /// Sets the progress of the copy thread.
        /// </summary>
        /// <param name="value">The current value.</param>
        /// <param name="total">The maximum value.</param>
        public void SetCopyProgress(int value, int total)
        {
            _uiDispatcher.Invoke(delegate ()
            {
                if (_uiElements.ContainsKey(UIElements.CopyProgress) && _uiElements[UIElements.CopyProgress] != null)
                {
                    ((ProgressBar)_uiElements[UIElements.CopyProgress]).MaxValue = total;
                    ((ProgressBar)_uiElements[UIElements.CopyProgress]).Value = value;
                }

                if(value == total || !_copyFiles.IsCopying())
                {
                    _destDir = _sourceDir = string.Empty;
                    if (_copyFiles != null)
                    {
                        _copyFiles.Dispose();
                        _copyFiles = null;
                    }
                }
            });
        }

        /// <summary>
        /// Sets the available songs.
        /// </summary>
        /// <param name="songs">The songs to set.</param>
        public void SetSongs(List<Song> songs)
        {
            try
            {
                Render(ViewType.Playing, false);
            }
            catch (Exception e)
            {

            }
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
            _uiDispatcher.Invoke(delegate ()
            {
                if (_uiElements.ContainsKey(UIElements.CurrentSong) && _uiElements[UIElements.CurrentSong] != null)
                {
                    ((Label)_uiElements[UIElements.CurrentSong]).Text = _currentSong;
                }
            });
        }

        /// <summary>
        /// Sets the songs position.
        /// </summary>
        /// <param name="currentTime">The current song time.</param>
        public void SetSongPosition(TimeSpan currentTime)
        {
            _uiDispatcher.Invoke(delegate ()
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

                        if (currentTime.TotalMilliseconds < slider.MaxValue)
                        {
                            slider.Value = (int)currentTime.TotalMilliseconds;
                        }
                    }));
                }
            });
        }

        /// <summary>
        /// Sets the volume.
        /// </summary>
        /// <param name="value">The value in percent.</param>
        public void SetVolume(int value)
        {
            _uiDispatcher.Invoke(delegate ()
            {
                if (_uiElements.ContainsKey(UIElements.Volume) && _uiElements[UIElements.Volume] != null)
                {
                    var slider = ((Slider)_uiElements[UIElements.Volume]);
                    slider.Value = value;
                }
            });
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
            this.Icon = new Icon(1, Resource.GetImage("Music-96.png"));
            this.Title = "Musicplayer";
            this.WindowStyle = WindowStyle.Default;
            var formHandler = (System.Windows.Forms.Form)this.ControlObject;
            formHandler.Size = new System.Drawing.Size(900, 450);
            formHandler.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericSerif, (float)10);
            formHandler.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
            _uiElements[UIElements.ClientButton] = CreateToolBarbutton("Connect to a stream", Resource.GetImage("GPS Searching-96.png"), ClientButton_Click, _networkClient != null);
            _uiElements[UIElements.AudioButton] = CreateToolBarbutton("Currently Playing", Resource.GetImage("Speaker-96.png"), AudioButton_Click, (_player != null && _player.IsPlaying()) || _networkClient != null);
            _uiElements[UIElements.CopyButton] = CreateToolBarbutton("Currently copying", Resource.GetImage("Copy Filled-100.png"), OpenCopy_Click, _copyFiles != null && _copyFiles.IsCopying());
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
                            Spacing = new Eto.Drawing.Size(0, 5),
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
                                            Control = _uiElements[UIElements.ClientButton],
                                            ScaleWidth = false
                                        },
                                        new TableCell
                                        {
                                            Control = _uiElements[UIElements.CopyButton],
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
        /// <param name="width">The width of the button.</param>
        /// <param name="enabled">A boolean indicating whether the button was enabled.</param>
        /// <returns>The button.</returns>
        private Button CreateToolBarbutton(string toolTip, Bitmap image, EventHandler<EventArgs> handler, bool visible = true, int width = 22, bool enabled = true)
        {
            var button = new Button
            {
                Image = new Bitmap(image, 22, 22),
                Width = width,
                BackgroundColor = ColorPallete.Colors[ColorPallete.Color.Primary2],
                ToolTip = toolTip,
                Visible = visible,
                Enabled = enabled
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
            mainActions.Cells.Add(CreateActionCell(OpenCopy_Click, "Copy random files", Resource.GetImage("Copy Filled-100.png")));
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
            mainActions2.Cells.Add(CreateActionCell(ClientButton_Click, "Connect to stream", Resource.GetImage("GPS Searching-96.png")));
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
            _uiElements[UIElements.PlayPauseButton] = CreateToolBarbutton("Play or pause the music", Resource.GetImage("Pause-96.png"), PlayPauseButton_Click, _networkClient == null, 60);
            _uiElements[UIElements.NextButton] = CreateToolBarbutton("Skip to the next song", Resource.GetImage("End-96.png"), NextButton_Click, _networkClient == null, 60);
            _uiElements[UIElements.Slider] = new Slider
            {
                Width = 150,
                Cursor = Cursors.VerticalSplit,
                Height = 17,
                MinValue = 0,
                ToolTip = "Song position",
                Enabled = _networkClient == null
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
            var songTable = new TableLayout
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

            _uiElements[UIElements.Volume] = new Slider
            {
                Width = 65,
                Cursor = Cursors.VerticalSplit,
                Height = 13,
                MinValue = 0,
                MaxValue = 100,
                ToolTip = "Volume",
                Value = Player.GetVolume()
            };

            var nativeVolume = (System.Windows.Forms.TrackBar)_uiElements[UIElements.Volume].ControlObject;
            nativeVolume.TickStyle = System.Windows.Forms.TickStyle.None;
            nativeVolume.Scroll += NativeVolume_Scroll;

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
                                                            new TableCell
                                                            {
                                                                Control = _uiElements[UIElements.Volume]
                                                            },
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
        /// Event handler for the volume change.
        /// </summary>
        /// <param name="sender">The forms trackbar.</param>
        /// <param name="e">The event arguments.</param>
        private void NativeVolume_Scroll(object sender, EventArgs e)
        {
            int volume = ((System.Windows.Forms.TrackBar)sender).Value;
            if(_player != null)
            {
                _player.SetVolume(volume);
            }

            if(_networkClient != null)
            {
                _networkClient.SetVolume(volume);
            }
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
            RenderPartialSongList((TableLayout)((Scrollable)_uiElements[UIElements.MusicList]).Content, _filterText);
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
        private void RenderPartialSongList(TableLayout songTable, string searchTerm)
        {
            if ((_player != null && _player.SongList != null) || _networkClient != null && _networkClient.ReceivedSongs != null)
            {
                songTable.SuspendLayout();
                IEnumerable<Song> songList = _player == null ? _networkClient.ReceivedSongs : _player.SongList;
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    songList = _player.SongList.Where(s => s.Title.ToLower().Contains(searchTerm.ToLower()) || (s.Band != null && s.Band.ToLower().Contains(searchTerm.ToLower())));
                }

                songList = songList.OrderBy(s => s.Band).ThenBy(s => s.Title).Take(100).ToList();
                if (songTable.Rows == null || songTable.Rows.Count < 100)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        songTable.Rows.Add(CreateSongRow(null));
                    }
                }

                var songListResult = songList.ToList();
                int row = 0;
                for (; row < songListResult.Count; row++)
                {
                    var song = songListResult[row];
                    if (!song.SourceIsDb)
                    {
                     song = _player.GetDetailsFromDbOrFile(song);
                    }

                    var rowLabels = songTable.Rows[row].Cells.Select(c => c.Control).OfType<Label>().ToList();
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
                }

                if(row < 100)
                {
                    songTable.Rows.Skip(row).OfType<TableRow>().SelectMany(r => r.Cells.Select(c => c.Control)).ToList()
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
        private TableCell[] CreateSongRow(Song song)
        {
            return new TableCell[]
            {
               new TableCell(CreateSongLabel(song, song != null ? song.Title : "Test", "Title")),
               new TableCell(CreateSongLabel(song, song != null ? song.Band : "Test", "Band")),
               new TableCell(CreateSongLabel(song, song != null ? song.Gengre : "Test", "Gengre")),
               new TableCell(CreateSongLabel(song, song != null ? song.DateAdded.ToString() : "Test", "Date Added")),
               new TableCell(CreateSongLabel(song, song != null ? song.DateCreated.ToString() : "Test", "Date Created"))
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
            _uiElements[UIElements.ServerStatusButton] = serverStatusButton;

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
                                                    Control = _uiElements[UIElements.ServerStatusButton]
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

        #region Client

        /// <summary>
        /// Shows the client settings page.
        /// </summary>
        private void ShowClientSettings()
        {
            var contentCell = GetContent();
            _uiElements[UIElements.IPPort] = CreateTextBox("8963", "The port the client will connect to", "Please enter a port (TCP)", ColorPallete.Colors[ColorPallete.Color.Primary1]);
            _uiElements[UIElements.IPAddress] = CreateTextBox("127.0.0.1", "The ip address the client will connect to", "Please enter an ip address", ColorPallete.Colors[ColorPallete.Color.Primary1]);

            var clientStatusButton = new Button
            {
                Text = _networkClient != null ? "Disconnect" : "Connect",
                BackgroundColor = ColorPallete.Colors[ColorPallete.Color.Primary2],
                TextColor = Colors.White,
                Font = new Font(SystemFont.TitleBar, 8),
            };

            clientStatusButton.Click += ClientStatusButton_Click;
            var nativebutton = (System.Windows.Forms.Button)clientStatusButton.ControlObject;
            nativebutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            nativebutton.FlatAppearance.BorderSize = 0;
            _uiElements[UIElements.ClientStatusButton] = clientStatusButton;

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
                                    Text = "Connect to a stream",
                                    TextAlignment = Eto.Forms.TextAlignment.Center,
                                    Font = new Font(SystemFont.TitleBar, 12),
                                    TextColor = ColorPallete.Colors[ColorPallete.Color.Primary1]
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
                                    Spacing = new Size(10, 10),
                                    Rows =
                                    {
                                        new TableRow
                                        {
                                            Cells =
                                            {
                                                null,
                                                new TableCell
                                                {
                                                    Control = _uiElements[UIElements.IPAddress]
                                                },
                                                null
                                            }
                                        },
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
                                                    Control = _uiElements[UIElements.ClientStatusButton]
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
        /// Attempt to connect to a server.
        /// </summary>
        /// <param name="sender">The button</param>
        /// <param name="e">The event arguments.</param>
        private void ClientStatusButton_Click(object sender, EventArgs e)
        {
            if (_networkClient == null)
            {
                string ipAddress = ((TextBox)_uiElements[UIElements.IPAddress]).Text;
                string port = ((TextBox)_uiElements[UIElements.IPPort]).Text;
                int portParsed = 0;
                IPAddress ipParsed = IPAddress.Any;
                if (int.TryParse(port, out portParsed) && IPAddress.TryParse(ipAddress, out ipParsed))
                {
                    if (_player != null)
                    {
                        _player.Dispose();
                    }

                    try
                    {
                        _networkClient = new NetworkClient(this, ipParsed, portParsed);
                        Render(ViewType.Playing);
                    }
                    catch
                    {
                        _networkClient = null;
                        MessageBox.Show("Could not connect to the specified server", MessageBoxType.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("The entered ip address or port is not valid");
                }
            }
            else
            {
                RemoveNetworkClient();
                Render(ViewType.Home);
            }
        }

        #endregion

        #region Copy

        /// <summary>
        /// Creates the copy content window.
        /// </summary>
        private void CreateCopyContent()
        {
            var contentCell = GetContent();
            var progressBar = new ProgressBar
            {
                MaxValue = 100,
                MinValue = 0,
                Value = 0,
                Visible = _copyFiles != null,
                Width = 450,
                Height = 5
            };

            _uiElements[UIElements.CopyProgress] = progressBar;
            _uiElements[UIElements.CopyAmount] = CreateTextBox("500", "Enter the amount of songs to copy", "Enter a number", ColorPallete.Colors[ColorPallete.Color.Primary1]);

            TableLayout layout = new TableLayout
            {
                Rows =
                {
                    null,
                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell
                            {
                                Control = new TableLayout
                                {
                                    Visible = _copyFiles == null || !_copyFiles.IsCopying(),
                                    Rows =
                                    {
                                        new TableRow
                                        {
                                            Cells =
                                            {
                                                null,
                                                new TableCell
                                                {
                                                    Control = _uiElements[UIElements.CopyAmount]
                                                },
                                                null
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    null,
                    new TableRow
                    {
                        Cells =
                        {
                            new TableCell
                            {
                                Control = new TableLayout
                                {
                                    Visible = _copyFiles == null || !_copyFiles.IsCopying(),
                                    Rows =
                                    {
                                        new TableRow
                                        {
                                            Cells =
                                            {
                                                null,
                                                CreateActionCell(CopySourceFolder_Click, "The source to copy files from", Resource.GetImage("Open Folder-96.png")),
                                                null,
                                                CreateActionCell(CopyDestFolder_Click, "The destination folder", Resource.GetImage("Open Folder-96.png")),
                                                null
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
                                Control = new TableLayout
                                {
                                    Visible = _copyFiles != null && _copyFiles.IsCopying(),
                                    Rows =
                                    {
                                        new TableRow
                                        {
                                            Cells =
                                            {
                                                null, 
                                                new TableCell
                                                {
                                                    Control = _uiElements[UIElements.CopyProgress]
                                                },
                                                null
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    null
                }
            };

            contentCell.Control = layout;
        }

        /// <summary>
        /// Selects the soure folder (recursive).
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void CopySourceFolder_Click(object sender, EventArgs e)
        {
            using (SelectFolderDialog dialog = new SelectFolderDialog())
            {
                dialog.Title = "Select a folder that contains music files";
                dialog.Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                if (dialog.ShowDialog(this) == DialogResult.Ok)
                {
                    _sourceDir = dialog.Directory;
                    CopyFiles();
                }
            }
        }

        /// <summary>
        /// Selects the destination folder (recursive).
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void CopyDestFolder_Click(object sender, EventArgs e)
        {
            using (SelectFolderDialog dialog = new SelectFolderDialog())
            {
                dialog.Title = "Select destination folder";
                dialog.Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                if (dialog.ShowDialog(this) == DialogResult.Ok)
                {
                    _destDir = dialog.Directory;
                    CopyFiles();
                }
            }
        }

        /// <summary>
        /// Ensures that a musicplayer exists.
        /// </summary>
        private void CopyFiles()
        {
            if (!string.IsNullOrEmpty(_sourceDir) && !string.IsNullOrEmpty(_destDir))
            {
                if (_copyFiles != null)
                {
                    _copyFiles.Dispose();
                }

                _copyFiles = new Player(this);
                _copyFiles.LoadAll(Directory.GetFiles(_sourceDir, "*.*", SearchOption.AllDirectories));
                int amount = 0;
                if (int.TryParse(((TextBox)_uiElements[UIElements.CopyAmount]).Text, out amount))
                {
                    _copyFiles.CopyRandomSongs(_destDir, amount);
                    Render(ViewType.Copy);
                }
                else
                {
                    MessageBox.Show("The amount of song is not a number", MessageBoxType.Warning);
                }
            }

        }

        #endregion

        #region Common

        /// <summary>
        /// Creates a textbox.
        /// </summary>
        /// <param name="value">The initial value.</param>
        /// <param name="toolTip">The tooltip.</param>
        /// <param name="placeHolder">The placeholder.</param>
        /// <param name="background">The backgroundColor.</param>
        /// <returns>A textbox.</returns>
        private TextBox CreateTextBox(string value, string toolTip, string placeHolder, Color background)
        {
            var result = new TextBox
            {
                Text = value,
                ToolTip = toolTip,
                PlaceholderText = placeHolder,
                BackgroundColor = background,
                TextColor = Colors.White
            };

            var nativeTextBox = (System.Windows.Forms.TextBox)result.ControlObject;
            nativeTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            nativeTextBox.Font = new System.Drawing.Font(nativeTextBox.Font.FontFamily, 14);
            nativeTextBox.Margin = new System.Windows.Forms.Padding(5);
            return result;
        }

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
        /// <param name="type">The view to render.</param>
        /// <param name="async">Render the view assynchronously.</param>
        private void Render(ViewType type, bool async = true)
        {
            if (async)
            {
                Task.Run(delegate ()
                {
                    RenderSync(type);
                });
            }
            else
            {
                RenderSync(type);
            }
        }

        /// <summary>
        /// Renders a view synchronously.
        /// </summary>
        /// <param name="type">The view.</param>
        private void RenderSync(ViewType type)
        {
            var nativeForm = (System.Windows.Forms.Form)this.ControlObject;
            _uiDispatcher.Invoke(delegate ()
            {
                _uiElements[UIElements.Slider] = null;
                _uiElements[UIElements.CopyProgress] = null;
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
                    case ViewType.Client:
                        ShowClientSettings();
                        break;
                    case ViewType.Copy:
                        CreateCopyContent();
                        break;
                    case ViewType.Home:
                    default:
                        CreateMainActions();
                        break;
                }

                this.Content = _mainLayout;
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
        /// Removes the network client.
        /// </summary>
        private void RemoveNetworkClient()
        {
            if(_networkClient != null)
            {
                _networkClient.Dispose();
                _networkClient = null;
            }
        }

        /// <summary>
        /// Disposes off all assets.
        /// </summary>
        public new void Dispose()
        {
            RemoveNetworkClient();

            if (_player != null)
            {
                _player.Dispose();
            }

            if(_copyFiles != null)
            {
                _copyFiles.Dispose();
            }
        }

        #endregion
    }
}
