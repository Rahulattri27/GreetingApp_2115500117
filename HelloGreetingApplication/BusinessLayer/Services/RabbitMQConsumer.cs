using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ModelLayer.DTO;
using Microsoft.Extensions.Options;
using RepositoryLayer.Interface;
//Added for Scoped Service Resolution
using Microsoft.Extensions.DependencyInjection;  

namespace BusinessLayer.Services
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQConfig _config;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQConsumer(ILogger<RabbitMQConsumer> logger, IServiceProvider serviceProvider, IOptions<RabbitMQConfig> config)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _config = config.Value;
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _config.HostName,
                UserName = _config.UserName,
                Password = _config.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare("UserRegistrationQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare("GreetingQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            _logger.LogInformation("RabbitMQ Consumer initialized successfully.");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var userConsumer = new EventingBasicConsumer(_channel);
            userConsumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation($"Received User Registration Event: {message}");

                using (var scope = _serviceProvider.CreateScope()) //Create a Scoped Dependency
                {
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    ProcessUserRegistrationEvent(message, emailService);
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            var greetingConsumer = new EventingBasicConsumer(_channel);
            greetingConsumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation($"Received Greeting Event: {message}");

                ProcessGreetingEvent(message);

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: "UserRegistrationQueue", autoAck: false, consumer: userConsumer);
            _channel.BasicConsume(queue: "GreetingQueue", autoAck: false, consumer: greetingConsumer);

            return Task.CompletedTask;
        }

        private void ProcessUserRegistrationEvent(string message, IEmailService emailService)
        {
            try
            {
                var userEvent = JsonSerializer.Deserialize<UserRegistrationEvent>(message);
                _logger.LogInformation($"Processing User Registration: {userEvent.FullName} ({userEvent.Email})");
                emailService.SendEmail(userEvent.Email, "User Registered", message); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing User Registration Event: {ex.Message}");
            }
        }

        private void ProcessGreetingEvent(string message)
        {
            try
            {
                var greetingEvent = JsonSerializer.Deserialize<GreetingEvent>(message);
                _logger.LogInformation($"Processing Greeting Event: {greetingEvent.Message} for UserId {greetingEvent.UserId}");
                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing Greeting Event: {ex.Message}");
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }

    public class UserRegistrationEvent
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime RegisteredAt { get; set; }
    }

    public class GreetingEvent
    {
        public int GreetingId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
