
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

var exchangeName = "DirectExchange";
channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true);

Console.Write("Direct Exchange : Write a message or press [enter] to exit :");
var message = Console.ReadLine();

Console.Write("Write a messageType(1 or 2) or press [enter] to exit :");
var messageType = Console.ReadLine();

while (!string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(messageType))
{
    var body = Encoding.UTF8.GetBytes(message);

    //prevent message lost when rabbitMq server dies
    //1)make massage durable by passing properties parameter in BasicPublish method
    //2)make queue durable in QueueDeclare method
    var properties = channel.CreateBasicProperties();
    properties.Persistent = true;

    channel.BasicPublish(exchange: exchangeName,
                         routingKey: messageType,
                         basicProperties: properties,
                         body: body);
    Console.WriteLine($"Sent : {message} of type {messageType}");


    Console.Write("Direct Exchange : Write a message or press [enter] to exit :");
    message = Console.ReadLine();

    Console.Write("Write a messageType(1,2) or press [enter] to exit :");
    messageType = Console.ReadLine();
}
