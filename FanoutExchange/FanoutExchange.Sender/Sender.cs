
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

var exchangeName = "FanoutExchange";
channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout, durable: true);

Console.Write("Fanout Exchange : Write a message or press [enter] to exit :");
var message = Console.ReadLine();
while (!string.IsNullOrEmpty(message))
{
    var body = Encoding.UTF8.GetBytes(message);

    //prevent message lost when rabbitMq server dies
    //1)make massage durable by passing properties parameter in BasicPublish method
    //2)make queue durable in QueueDeclare method
    var properties = channel.CreateBasicProperties();
    properties.Persistent = true;

    channel.BasicPublish(exchange: exchangeName,
                         routingKey: "",
                         basicProperties: properties,
                         body: body);
    Console.WriteLine($"Sent : {message}");


    Console.Write("Fanout Exchange : Write a message or press [enter] to exit :");
    message = Console.ReadLine();
}
