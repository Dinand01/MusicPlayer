using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using MusicPlayer.Models;
using MusicPlayer.Extensions;
using MusicPlayer.Controller;
using System.Net;

namespace MusicPlayer
{
    /// <summary>
    /// Class describing the GUI and main actions of the program
    /// </summary>
    public class GUI
    {
        #region Variables

        /// <summary>
        /// The instance of the music player controller.
        /// </summary>
        private Player _player;

        /// <summary>
        /// The Main panel.
        /// </summary>
        private Form _form;

        /// <summary>
        /// The table layout.
        /// </summary>
        private TableLayoutPanel _table;

        /// <summary>
        /// The left column.
        /// </summary>
        private FlowLayoutPanel _layout;

        /// <summary>
        /// The list of songs.
        /// </summary>
        private ListViewExtended _view;

        /// <summary>
        /// The previous selected item key in the list view.
        /// </summary>
        private string previouskey;

        /// <summary>
        /// A lock.
        /// </summary>
        private object m_lock = new object();

        /// <summary>
        /// The song position bar.
        /// </summary>
        private TrackBar songPos;

        /// <summary>
        /// The network client.
        /// </summary>
        private NetworkClient _networkClient = null;

        #endregion

        public GUI(Form form1)
        {
            previouskey = "";

            _player = new Player(this);
            this._form = form1;
            _table = new TableLayoutPanel();
            _table.Parent = _form;
            _table.Dock = DockStyle.Fill;

            var menu = new MenuStrip();
            _table.SetColumnSpan(menu, 2);
            _table.Controls.Add(menu, 0, 0);

            menu.Dock = DockStyle.Top;

            // first index is horizontal, second vertical
            var menuItems = new ToolStripMenuItem[1, 5];
            menuItems[0, 0] = new ToolStripMenuItem("Actions");
            menuItems[0, 0].Name = "Actions";
            menu.Items.Add(menuItems[0, 0]);

            menuItems[0, 1] = new ToolStripMenuItem("Open");
            menuItems[0, 1].Name = "Open";
            menuItems[0, 1].Click += new EventHandler(FileMenuOpen);
            menuItems[0, 0].DropDownItems.Add(menuItems[0, 1]);

            menuItems[0, 2] = new ToolStripMenuItem("Open all in folder");
            menuItems[0, 2].Name = "Open";
            menuItems[0, 2].Click += new EventHandler(FolderMenuOpen);
            menuItems[0, 0].DropDownItems.Add(menuItems[0, 2]);

            menuItems[0, 3] = new ToolStripMenuItem("Copy random songs");
            menuItems[0, 3].Name = "Copy";
            menuItems[0, 3].Click += new EventHandler(CopyRandoms);
            menuItems[0, 0].DropDownItems.Add(menuItems[0, 3]);

            //menuItems[0, 4] = new ToolStripMenuItem("Search");
            //menuItems[0, 4].Name = "Search";
            //menuItems[0, 4].Click += new EventHandler(OpenQueryForm);
            //menuItems[0, 0].DropDownItems.Add(menuItems[0, 4]);

            RichTextBox message = new RichTextBox();
            message.Dock = DockStyle.Top;
            message.Height = 20;
            message.ReadOnly = true;
            message.Name = "Notification";
            message.Enabled = false;
            _table.Controls.Add(message, 1, 1);

            // First col = flow layout
            _layout = new FlowLayoutPanel();

            _layout.Dock = DockStyle.Left;
            _table.Controls.Add(_layout, 0, 1);
            _table.SetRowSpan(_layout, 2);
            _view = new ListViewExtended();
            _view.View = View.Details;
            var col = new ColumnHeader();
            col.Text = "Title";
            col.Width = 250;
            col.AutoResize(ColumnHeaderAutoResizeStyle.None);
            _view.Columns.Add(col);

            col = new ColumnHeader();
            col.Text = "Band";
            col.Width = 150;
            col.AutoResize(ColumnHeaderAutoResizeStyle.None);
            _view.Columns.Add(col);

            col = new ColumnHeader();
            col.Text = "Gengre";
            col.Width = 100;
            col.AutoResize(ColumnHeaderAutoResizeStyle.None);
            _view.Columns.Add(col);

            _view.Columns.Add("Date Created");
            _view.Columns.Add("Date Added");
            _view.ItemActivate += new EventHandler(ViewClick);
            _view.Scroll += new ScrollEventHandler(HandleScrollEvent);
            _view.Dock = DockStyle.Fill;
            _view.HideSelection = false;

            _table.Controls.Add(_view, 1, 2);

            Button play = new Button();
            play.Text = "Play/Pause";
            play.Name = "play";
            play.Click += new EventHandler(_player.PausePlay);
            _layout.Controls.Add(play);

            Button next = new Button();
            next.Text = "Next";
            next.Name = "next";
            next.MouseClick += new MouseEventHandler(_player.OnWaveOutStop);
            _layout.Controls.Add(next);

            songPos = new TrackBar();
            songPos.TickStyle = TickStyle.None;
            songPos.Width = _layout.Width - 20;
            songPos.Scroll += new EventHandler(_player.MoveSongPosition);
            _layout.Controls.Add(songPos);

            Label networkl = new Label();
            networkl.Text = "Network";
            networkl.Font = new Font(FontFamily.GenericSansSerif, 12.0F, FontStyle.Bold);
            _layout.Controls.Add(networkl);

            TextBox ipaddressb = new TextBox();
            ipaddressb.Name = "IPAddress";
            ipaddressb.Text = "127.0.0.1";
            ipaddressb.Width = 100;
            _layout.Controls.Add(ipaddressb);

            TextBox portb = new TextBox();
            portb.Name = "Port";
            portb.Text = "" + 8963;
            portb.Width = 30;
            _layout.Controls.Add(portb);

            CheckBox cbox = new CheckBox();
            cbox.Text = "Host";
            cbox.Name = "Host";
            cbox.Checked = false;
            cbox.CheckedChanged += Cbox_CheckedChanged;
            _layout.Controls.Add(cbox);

            Button connect = new Button();
            connect.Text = "Connect";
            connect.Name = "Connect";
            connect.MouseClick += new MouseEventHandler(Connect_Click);
            _layout.Controls.Add(connect);

            TextBox hostmessage = new TextBox();
            hostmessage.Visible = false;
            hostmessage.Name = "HostMessage";
            hostmessage.TextChanged += Hostmessage_TextChanged;
            _layout.Controls.Add(hostmessage);
        }

        #region Interface

        public void Open(string[] args)
        {
            List<Song> temp = _player.LoadAll(args, null);

            AddSongsToListView(temp, _view);

            _view.Columns[0].Width = -1;
            _view.Columns[1].Width = -1;
        }


        /// <summary>
        /// Sets the activePath of the player as active in the view
        /// </summary>
        public void SetActive(Player player)
        {
            if (player.ActiveSong != null)
            {
                string key = player.ActiveSong.Location;
                _view.Invoke((MethodInvoker)(delegate ()
                {
                    if (_view.Items.ContainsKey(key))
                    {
                        if (previouskey != "")
                        {
                            _view.Items[previouskey].BackColor = Color.White;
                        }

                        previouskey = key;
                        _view.EnsureVisible(_view.Items[key].Index);
                        HandleScrollEvent(null, null);
                        _view.Items[key].BackColor = Color.LightBlue;
                        SetNotification(player.ActiveSong.Band + " - " + player.ActiveSong.Title);
                    }
                }));
            }
        }

        /// <summary>
        /// Sets the notification.
        /// </summary>
        /// <param name="text">The text to set.</param>
        public void SetNotification(string text)
        {
            var messageCtrl = _table.Controls.Find("Notification", true).First();
            messageCtrl.Invoke((MethodInvoker)(delegate ()
            {
                messageCtrl.Text = text;
            }));
        }

        /// <summary>
        /// Adds songs to the list view.
        /// </summary>
        /// <param name="songs">The songs.</param>
        public void AddSongsToListView(List<Song> songs)
        {
            AddSongsToListView(songs, this._view, false);
        }

        /// <summary>
        /// Adds songs to the list view 
        /// </summary>
        /// <param name="songs"></param>
        /// <param name="view"></param>
        private void AddSongsToListView(List<Song> songs, ListView view, bool setActive = true)
        {
            ListViewItem[] items = new ListViewItem[songs.Count];
            for (int i = 0; i < songs.Count; i++)
            {
                ListViewItem item = new ListViewItem(songs[i].Title);
                item.Name = songs[i].Location;
                item.SubItems.AddRange(songs[i].ToSubItemArray());
                items[i] = item;
            }

            view.Invoke((MethodInvoker)(() => view.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None)));
            view.Invoke((MethodInvoker)(() => view.Items.AddRange(items)));
            if (setActive)
            {
                SetActive(_player);
            }
        }

        /// <summary>
        /// updates the trackbar with a range of the total number of seconds in the song
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void redrawTrackbar(int min, int max)
        {
            songPos.Invoke((MethodInvoker)(delegate()
            {
                songPos.SetRange(min, max);
                songPos.Update();
            }));
        }

        /// <summary>
        /// Sets the gui pos of the trackbar
        /// </summary>
        /// <param name="pos">pos in seconds</param>
        public void SetTrackbarPos(int pos)
        {
            try
            {
                songPos.Invoke((MethodInvoker)(() => songPos.Value = pos));
            }
            catch { }
        }

        #endregion

        #region GUIActions

        /// <summary>
        /// Handles the click event of teh connect button.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The arguments.</param>
        private void Connect_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            string ipadress = _layout.Controls.Find("IPAddress", false).FirstOrDefault().Text;
            string port = _layout.Controls.Find("Port", false).FirstOrDefault().Text;
            bool host = ((CheckBox)_layout.Controls.Find("Host", false).FirstOrDefault()).Checked;
            //bool multicast = ((CheckBox)layout.Controls.Find("Multicast", false).FirstOrDefault()).Checked;

            if (_networkClient != null)
            {
                _networkClient.Dispose();
                _networkClient = null;
            }

            IPAddress parsedAddress = IPAddress.None;
            int parsedPort = 0;
            if (IPAddress.TryParse(ipadress, out parsedAddress) && int.TryParse(port, out parsedPort))
            {

                if (button.Text == "Connect")
                {
                    if (!string.IsNullOrEmpty(ipadress) && !string.IsNullOrEmpty(port))
                    {
                        if (host)
                        {
                            _player.StartAudioServer(parsedAddress, parsedPort);
                        }
                        else
                        {

                            _player.Dispose();
                            _player = null;
                            _networkClient = new NetworkClient(this, parsedAddress, parsedPort);
                            //player.ListenToAudioServer(ipadress, port, multicast);

                        }

                        button.Text = "Disconnect";
                    }
                }
                else
                {

                    if (_player == null)
                    {
                        _player = new Player(this);
                    }
                    else
                    {
                        _player.DisconnectFromAudioServer();
                    }

                    button.Text = "Connect";
                }
            }
        }

        /// <summary>
        /// Opens the file menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileMenuOpen(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "MP3 (.mp3)|*.mp3|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = true;
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                List<Song> temp = _player.LoadAll(openFileDialog1.FileNames, null);

                AddSongsToListView(temp, _view);
                _view.Columns[0].Width = -1;
                _view.Columns[1].Width = -1;
            }
        }

        /// <summary>
        /// Loads all the songs in a folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FolderMenuOpen(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a folder that contains music files";
                dialog.ShowNewFolderButton = false;
                dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string folder = dialog.SelectedPath;
                    List<Song> temp = _player.LoadAll(Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories), null);
                    AddSongsToListView(temp, _view);

                    _view.Columns[0].Width = -1;
                    _view.Columns[1].Width = -1;
                }
            }

        }

        /// <summary>
        /// Menu to copy random songs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyRandoms(object sender, EventArgs e)
        {
            string destination = "";
            string source = "";
            int number = 0;

            CopyRandomSongs querry = new CopyRandomSongs();

            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            if (querry.ShowDialog() == DialogResult.OK)
            {
                number = int.Parse(querry.textBox1.Text);
                destination = querry.destination;
                source = querry.source;
                querry.Dispose();

                var allFiles = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);
                List<Song> tempLoadedFiles = _player.LoadAll(allFiles, number);

                AddSongsToListView(tempLoadedFiles, _view);


                _view.Columns[0].Width = -1;
                _view.Columns[1].Width = -1;
                _player.CopyRandomSongs(destination);
            }
        }

        /// <summary>
        /// Opens the querry menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenQueryForm(object sender, EventArgs e)
        {
            OpenSpecial form = new OpenSpecial();

            if (form.ShowDialog() == DialogResult.OK)
            {
                // TODO
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// retrieves the details of the songs for all visible items and updates the view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleScrollEvent(object sender, ScrollEventArgs e)
        {
            ListViewItem lvi1 = _view.GetItemAt(_view.ClientRectangle.X + 6, _view.ClientRectangle.Y + 6);
            ListViewItem lvi2 = _view.GetItemAt(_view.ClientRectangle.X + 6, _view.ClientRectangle.Bottom - 6);
            int startidx = _view.Items.IndexOf(lvi1);
            int endidx = _view.Items.IndexOf(lvi2);
            if (endidx == 0)
                endidx = _view.Items.Count;

            bool viewneedsupdate = false;

            Task.Run(delegate()
            {
                lock (m_lock)
                {
                    for (int i = startidx; i <= endidx; i++)
                    {
                        if (i != -1)
                        {
                            Song song = null;
                            _view.Invoke((MethodInvoker)(() => song = _player.GetSongAtLocation(_view.Items[i].Name)));
                            if (song != null && (song.Gengre == null || song.Album == null))
                            {
                                var tempSong = _player.LoadDetails(song);
                                if (tempSong != null)
                                {
                                    _view.Invoke((MethodInvoker)(() => _view.Items[i].SubItems.Clear()));
                                    _view.Invoke((MethodInvoker)(() => _view.Items[i].Name = tempSong.Location));
                                    _view.Invoke((MethodInvoker)(() => _view.Items[i].Text = tempSong.Title));
                                    _view.Invoke((MethodInvoker)(() => _view.Items[i].SubItems.AddRange(tempSong.ToSubItemArray())));
                                    viewneedsupdate = true;
                                }
                            }
                        }
                    }

                    if (viewneedsupdate)
                    {
                        _view.Invoke((MethodInvoker)(() => _view.Update()));
                    }
                }
            });
        }

        /// <summary>
        /// Toggle visibility of message box (sends messages to clients).
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Cbox_CheckedChanged(object sender, EventArgs e)
        {
            var cbox = (CheckBox)sender;
            var ctrl = _layout.Controls.Find("HostMessage", false).First();
            if (cbox.Checked)
            {
                ctrl.Visible = true;
            }
            else
            {
                ctrl.Visible = false;
            }
        }

        /// <summary>
        /// Changes the song to the item in the view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewClick(object sender, EventArgs e)
        {
            _player.Play(_view.FocusedItem.Name);
        }

        /// <summary>
        /// Transmits the text to the clients when hosting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hostmessage_TextChanged(object sender, EventArgs e)
        {
            if (_player != null)
            {
                _player.NotifyClients(((TextBox)sender).Text);
            }
        }

        #endregion

        public void Dispose()
        {
            if (_networkClient != null)
            {
                _networkClient.Dispose();
            }

            if (_player != null)
            {
                _player.Dispose();
            }
        }
    }
}
