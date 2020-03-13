namespace Test.Contracts
{

    public class ClientMessage
    {

        public string ConnectionId { get; set; }
 
        public string Group { get; set; }

        public object Payload { get; set; }
    }
}