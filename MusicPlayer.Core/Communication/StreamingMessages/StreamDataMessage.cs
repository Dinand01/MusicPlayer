namespace MusicPlayer.Core.Communication.StreamingMessages
{
    public class StreamDataMessage : StreamingMessage
    {
        public long ContentLength { get; set; }

        public byte[] Content { get; set; }
    }
}
