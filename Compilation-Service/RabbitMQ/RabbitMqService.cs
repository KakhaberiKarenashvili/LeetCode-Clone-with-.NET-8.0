using System.Text;
using Compilation_Service.Dto.Request;
using Compilation_Service.Dto.Response;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Compilation_Service.RabbitMQ;

public class RabbitMqService
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<RabbitMqService> _logger;
    private  IConnection _connection;
    private  IModel _channel;

    public RabbitMqService(ILogger<RabbitMqService> logger)
    {
        _logger = logger;
        
        _connectionFactory = new ConnectionFactory()
        {
            HostName = "rabbitmq",
            Port = 5672,
            UserName = "user", 
            Password = "password"
        };
        _connection = _connectionFactory.CreateConnection();

        _channel = _connection.CreateModel();
        
        _channel.QueueDeclare(queue: "requestQueue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        
        _channel.QueueDeclare(queue: "resultQueue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }
    
    public void SendResult(SubmissionResultResponseDto result)
    {
        if (_channel == null)
        {
            throw new InvalidOperationException("Channel is not initialized.");
        }
        
        var json = JsonSerializer.Serialize(result);
        var body = Encoding.UTF8.GetBytes(json);
        _channel.BasicPublish(exchange: "", routingKey: "resultQueue", basicProperties: null, body: body);
    }
    
    public void ReceiveRequest(Action<SubmissionRequestDto> processRequest)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation("Received request in queue");
            var request = JsonConvert.DeserializeObject<SubmissionRequestDto>(message);
            processRequest(request);
        };
        _channel.BasicConsume(queue: "requestQueue", autoAck: true, consumer: consumer);
    }
    
    public void Close()
    {
        _channel.Close();
        _connection.Close();
    }
}