namespace EventSourcing.Entities
{
    public class EncryptedEvent
    {
        public long Location;
        public byte[] Content;
        public byte[] InitializationVector;

        public EncryptedEvent(long location, byte[] content, byte[] initializationVector)
        {
            Location = location;
            Content = content;
            InitializationVector = initializationVector;
        }
    }
}
