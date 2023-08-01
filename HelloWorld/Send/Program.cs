using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory {
    Uri = new Uri("amqps://iyyhrngk:P8wSTuukgFTjd1NzJru3qtHIEuhhbnaF@armadillo.rmq.cloudamqp.com/iyyhrngk")

};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "hello", // tên hàng đợi
                     durable: false, //tính bền bỉ khi restart
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

const string message = "Hello Phuoc!";
var body = Encoding.UTF8.GetBytes(message); // chuyển message về utf8

// gửi thông báo đi
channel.BasicPublish(exchange: string.Empty,
                     routingKey: "hello",   // điều hướng đến tên hàng đợi
                     basicProperties: null,
                     body: body); // message đã được mã hóa.

Console.WriteLine($" [x] Sent {message}");

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
