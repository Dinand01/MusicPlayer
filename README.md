# MusicPlayer
A .NET music player capable of streaming music to multiple clients over TCP.

Available functionality:

    Audio playback for all files supported by the system
    Stream music over TCP
    Copy a specified amount of random audio files to a specified directory
    Database for meta data (auto created)
    
Install & Usage: 
Unpack and copy exe to folder where your user account has write permissions (64 bit only).
Supports: Windows 10 (and others ?) with .NET framework 4.5

To host an audio server firewall and port forward configuration is required.

Dependancies on other libraries:

    All dependancies are stored as embedded resources, they are loaded when first required.

    Creates Database using entity framework and SQL server compact, configuration file is not used.
    Uses NAudio for playback (media foundation reader).
    Taglib is used to retrieve meta data from audio files, this data is then stored in the database for better performance.
    Installer for SQL server compact is served when it is not yet installed.
    
