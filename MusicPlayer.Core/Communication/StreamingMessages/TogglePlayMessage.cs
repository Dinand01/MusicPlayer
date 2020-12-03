namespace MusicPlayer.Core.Communication.StreamingMessages
{
    public class TogglePlayMessage : StreamingMessage
    {
        public bool Playing { get; set; }
    }
}
