
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

var queueName = "QueueConnectedToDefaultExchange";
channel.QueueDeclare(queue: queueName,
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);


Console.Write("Default Exchange : Write a message or press [enter] to exit :");
var message = Console.ReadLine();
while (!string.IsNullOrEmpty(message))
{
    var body = Encoding.UTF8.GetBytes(message);

    //prevent message lost when rabbitMq server dies
    //1)make massage durable by passing properties parameter in BasicPublish method
    //2)make queue durable in QueueDeclare method
    var properties = channel.CreateBasicProperties();
    properties.Persistent = true;

    //routingKey must be equal to queue name when using default exchange
    channel.BasicPublish(exchange: string.Empty,
                         routingKey: queueName,
                         basicProperties: properties,
                         body: body);
    Console.WriteLine($"Sent : {message}");


    Console.Write("Default Exchange : Write a message or press [enter] to exit :");
    message = Console.ReadLine();
}
