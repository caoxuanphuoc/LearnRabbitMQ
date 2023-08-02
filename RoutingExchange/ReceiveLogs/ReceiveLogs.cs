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

// Kiểm tra tính bắt buộc của tham số đầu vào.
if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: {0} [info] [warning] [error]",
                            Environment.GetCommandLineArgs()[0]);
    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
    Environment.ExitCode = 1;
    return;
}

// nếu có nhiều hơn một phần từ là các loại tin nhắn [info, error, warning]
// với mỗi phần tử sẽ có một routingKey khác nhau
// VỀ MẶT QUEUE:
//          + Queue hiện tại đang giải quyết trương hợp một Queue có thể tiếp nhận nhiều loại
//          + với trường hợp có 2 Queue song song và nhiệm vụ của nó khác nhau thì nên lập thêm một Queue mới
//                                                          đảm bảo về tính bất đồng bộ và nghiệp vụ bài toán
foreach (var severity in args)
{
    channel.QueueBind(queue: queueName,
                      exchange: "direct_logs",
                      routingKey: severity);
}

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

