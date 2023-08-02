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

channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);

// declare a server-named queue
var queueName = channel.QueueDeclare().QueueName;

// hàng đợi này chỉ nhận các message được đánh dấu là info 
// đối với trường hợp nhận type mess theo tham số đầu vào xin tham khảo ../ReceiveLogs/ReceiveLogs.cs
channel.QueueBind(queue: queueName,
                      exchange: "direct_logs",
                      routingKey: "info");
// uncomment if you want receive từ mess có type là error
/*channel.QueueBind(queue: queueName,
                      exchange: "direct_logs",
                      routingKey: "error");*/

Console.WriteLine(" [*] Waiting for messages.");

// phần xử lý của consumer
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

