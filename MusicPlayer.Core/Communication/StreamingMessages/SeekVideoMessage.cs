namespace MusicPlayer.Core.Communication.StreamingMessages
{
    public class SeekVideoMessage : StreamingMessage
    {
        public double Position { get; set; }
    }
}
