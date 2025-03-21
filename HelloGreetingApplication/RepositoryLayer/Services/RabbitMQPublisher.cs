using System;
using System.Text;
using RabbitMQ.Client;
using Microsoft.Extensions.Options;
using System.Text.Json;
using ModelLayer.DTO;

namespace RepositoryLayer.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQConfig _config;
        public RabbitMQPublisher(IOptions<RabbitMQConfig> config)
        {
            _config = config.Value;
        }

        public void PublishMessage<T>(string queueName, T message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _config.HostName,
                UserName = _config.UserName,
                Password = _config.Password
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var jsonMessage = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: properties,
                                 body: body);
        }
    }
}


