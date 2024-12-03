namespace WebLab1
{
    public class ChatMessage
    {
        public int senderId;
        public int receiverId;
        public string message;

        public ChatMessage(int senderId, int receiverId, string message)
        {
            this.senderId = senderId;
            this.receiverId = receiverId;
            this.message = message;
        }
    }
}
