using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory
{
    //https://www.cloudamqp.com/ login with github
    Uri = new Uri("amqps://iyyhrngk:P8wSTuukgFTjd1NzJru3qtHIEuhhbnaF@armadillo.rmq.cloudamqp.com/iyyhrngk")

};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "topic_logs", type: ExchangeType.Topic);
// declare a server-named queue
var queueName = channel.QueueDeclare().QueueName;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: {0} [binding_key...]",
                            Environment.GetCommandLineArgs()[0]);
    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
    Environment.ExitCode = 1;
    return;
}
// Định nghĩa các topic bằng các param khi chạy chương trình.
foreach (var bindingKey in args)
{
    channel.QueueBind(queue: queueName,
                      exchange: "topic_logs",
                      routingKey: bindingKey);
}

Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = ea.RoutingKey;
    Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
};
channel.BasicConsume(queue: queueName,
                     autoAck: true,
                     consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();