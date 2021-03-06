﻿using CefSharp;
using MusicPlayer;
using MusicPlayerWeb.CefComponents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MusicPlayerWeb
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The instance of the musicplayer interface.
        /// </summary>
        private MusicPlayerGate _musicPlayer;

        /// <summary>
        /// The dispatcher for the current thread;
        /// </summary>
        private Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // TODO: Build usefull window controls into CEF, get rid of default windows controls. 
            ////this.AllowsTransparency = true;
            ////this.WindowStyle = WindowStyle.None;
            ////this.BorderThickness = new Thickness(0);
            _musicPlayer = new MusicPlayerGate(this.Browser, this);
            this.Browser.JavascriptObjectRepository.Register("MusicPlayer", _musicPlayer, isAsync: true);
            this.Browser.JavascriptObjectRepository.ObjectBoundInJavascript += (sender, e) =>
            {
                Logger.LogInfo($"C# object was registered in javascript variable: {e.ObjectName}");
            };

            this.Browser.DisplayHandler = new DisplayHandler(this, _dispatcher);
            this.KeyDown += MainWindow_KeyDown;

            this.Browser.Loaded += (sender, e) =>
            {
                string[] args = Environment.GetCommandLineArgs();
                if (args?.Length > 1)
                {
                    FileAttributes attr = File.GetAttributes(args[1]);
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        _musicPlayer.LoadFolder(args[1]);
                    }
                    else
                    {
                        _musicPlayer.OpenFiles(args.Skip(1).Take(args.Length - 1).ToArray());
                    }
                }
            };
        }

        /// <summary>
        /// Handle keydown events for the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F5:
                    this.Browser.Reload(true);
                    break;
                case Key.F12:
                    this.Browser.ShowDevTools();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Handle the click for the show dev tools.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_ShowDev_Click(object sender, RoutedEventArgs e)
        {
            this.Browser.ShowDevTools();
        }

        /// <summary>
        /// Dispose of the music player UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Cef.Shutdown();
            _musicPlayer.Dispose();
            _musicPlayer = null;
        }

        /// <summary>
        /// Try to initialize cefsharp when it is not initialized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (_musicPlayer != null && this.WindowState == WindowState.Maximized && !this.Browser.IsInitialized)
            {
                Cef.Initialize();
            }
        }

        /// <summary>
        /// Play or Pause the music.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayPause_Click(object sender, EventArgs e)
        {
            _musicPlayer.TogglePlay();
        }

        /// <summary>
        /// Skip to the next song.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Next_Click(object sender, EventArgs e)
        {
            _musicPlayer.NextSong();
        }
    }
}
