namespace MusicPlayer.Core.Communication.StreamingMessages
{
    public class SeekSongMessage : StreamingMessage
    {
        public double Position { get; set; }
    }
}
