using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

var exchangeName = "DirectExchange";
channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true);

//new message will not be received by this consumer until this consumer is busy,they will be sent to other free consumers.
//ATTENETION : If no free consumer exists,messages will reamin in queue and may cause fill up.
//channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

var queueName = "Queue2ConnectedToDirectExchange";
channel.QueueDeclare(queue: queueName,
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);
//var queueName = channel.QueueDeclare().QueueName;//QueueDeclare without any parameter creates a non durable,auto delete queue with a random name
channel.QueueBind(queue: queueName,
                  exchange: exchangeName,
                  routingKey: "2");

Console.WriteLine("Direct Exchange - Receiver 2 - Waiting for messages....");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Receiver 2 - Received : {message}");

    //simulate long running task
    Thread.Sleep(message.Length * 1000);

    Console.WriteLine($"Receiver 2 - Finished : {message}");
    Console.WriteLine("Direct Exchange - Receiver 2 - Waiting for messages....");

    //prevent message lost when a receiver dies :
    //1)calling BasicAck method at the end of receiver process.
    //2)pass autoAck=false in BasicConsume method
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
};
channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();