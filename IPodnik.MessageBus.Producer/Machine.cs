namespace IPodnik.MessageBus.Sender
{
    public class Machine
    {
        public string Id { get; }
        public string RoutingKey { get; }

        public Machine(string id, string routingKey)
        {
            Id = id;
            RoutingKey = routingKey;
        }
    }
}
