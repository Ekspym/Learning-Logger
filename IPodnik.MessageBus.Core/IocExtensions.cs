using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageBus.Core
{
    public static class IocExtensions
    {
        public static void InstallSender(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqSettings = configuration.GetSection("RabbitMQ");
            string name = rabbitMqSettings["Username"];
            string password = rabbitMqSettings["Password"];
            string hostname = rabbitMqSettings["Hostname"];
            int port = int.Parse(rabbitMqSettings["Port"]);

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri($"rabbitmq://{hostname}:{port}"), h =>
                    {
                        h.Username(name);
                        h.Password(password);
                    });
                });
            });
        }
        public static void InstallConsumer<T>(this IServiceCollection services, IConfiguration configuration, string queueName) where T : class, IConsumer
        {
            var rabbitMqSettings = configuration.GetSection("RabbitMQ");
            string name = rabbitMqSettings["Username"];
            string password = rabbitMqSettings["Password"];
            string hostname = rabbitMqSettings["Hostname"];
            int port = int.Parse(rabbitMqSettings["Port"]);

            services.AddMassTransit(x =>
            {
                x.AddConsumer<T>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri($"rabbitmq://{hostname}:{port}"), h =>
                    {
                        h.Username(name);
                        h.Password(password);
                    });

                    cfg.ReceiveEndpoint(queueName, e =>
                    {
                        e.Consumer<T>(context);
                    });
                });
            });
        }
        public static void InstallQueueWriter(this IServiceCollection services)
        {
            services.AddScoped<IQueueWriter, QueueWriter>();
        }


    }
}

