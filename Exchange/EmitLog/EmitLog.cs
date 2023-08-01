using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory
{
    //https://www.cloudamqp.com/ login with github
    Uri = new Uri("amqps://iyyhrngk:P8wSTuukgFTjd1NzJru3qtHIEuhhbnaF@armadillo.rmq.cloudamqp.com/iyyhrngk")

};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

//tạo Exchange
channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

var message = GetMessage(args);
var body = Encoding.UTF8.GetBytes(message);

// phát hành Exchange logs
channel.BasicPublish(exchange: "logs",
                     routingKey: string.Empty,
                     basicProperties: null,
                     body: body);
Console.WriteLine($" [x] Sent {message}");

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

static string GetMessage(string[] args)
{
    return ((args.Length > 0) ? string.Join(" ", args) : "info: Hello World!");
}