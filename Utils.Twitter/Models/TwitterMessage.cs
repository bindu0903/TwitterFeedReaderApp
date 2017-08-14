namespace Utils.Twitter.Models
{
    public class TwitterMessage
    {
        public long ID;
        public long SenderID;
        public long SenderScreenName;
        public long RecipientID;
        public long RecipientScreenName;
        public string Text;
        public string CreatedAt;
        public TwitterUser Sender;
        public TwitterUser Recipient;
         
    }
}
