using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory
{
    //https://www.cloudamqp.com/ login with github
    Uri = new Uri("amqps://iyyhrngk:P8wSTuukgFTjd1NzJru3qtHIEuhhbnaF@armadillo.rmq.cloudamqp.com/iyyhrngk")

};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "topic_logs", type: ExchangeType.Topic);
// run with path topic else everything is default
var routingKey = (args.Length > 0) ? args[0] : "anonymous.info";
var message = (args.Length > 1)
              ? string.Join(" ", args.Skip(1).ToArray())
              : "Hello World!";
var body = Encoding.UTF8.GetBytes(message);
channel.BasicPublish(exchange: "topic_logs",
                     routingKey: routingKey,
                     basicProperties: null,
                     body: body);
Console.WriteLine($" [x] Sent '{routingKey}':'{message}'");