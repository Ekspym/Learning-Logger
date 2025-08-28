

var receiver = new MessageReceiver("10.0.128.4", 5672);
await receiver.StartListeningAsync();