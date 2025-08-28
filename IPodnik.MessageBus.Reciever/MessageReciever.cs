using IPodnik.Infrastructure.DTO.Server;
using MassTransit;
using System;
using System.Threading.Tasks;

public class MessageReceiver : IConsumer<AgentJobDto>
{
    private readonly IBusControl busControl;
    private readonly string queueName = "taskQueue";

    public MessageReceiver(string hostName, int port)
    {
        busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host(new Uri($"rabbitmq://{hostName}:{port}"), h =>
            {
                h.Username("PocUser");
                h.Password("PocPassword");
            });
        });
    }

    public async Task Consume(ConsumeContext<AgentJobDto> context)
    {
        var message = context.Message;
        Console.WriteLine($"Prijata zprava: {message}");

        await Task.Delay(1000);
    }

    public async Task StartListeningAsync()
    {
        busControl.ConnectReceiveEndpoint(queueName, e =>
        {
            e.Consumer(() => new MessageReceiver("10.0.128.4", 5672)); 
        });

        await busControl.StartAsync();
        Console.ReadLine();
    }

    public async Task StopAsync()
    {
        await busControl.StopAsync();
    }
}
