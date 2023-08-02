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
channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);

// kiểm tra loại tin nhắn muốn gửi
// với args là mảng các phần tử truyền vào
//          // ex my_program.exe param1 param2 param3 => agrs =[param1 param2 param3]
            // => args.Length = 3
var severity = (args.Length > 0) ? args[0] : "info";

// Khởi tạo giá trị cho message nếu args có nhiều ít nhất 2 phần tử thì message sẽ có giá trị
            //  là một chuổi gộp các ký tự trong mảng tham số đàu vào trừ phần tử đàu tiên.
// ngược lại nếu args chỉ có một phần tử thì nó mang giá trị mặc định.
var message = (args.Length > 1)
              ? string.Join(" ", args.Skip(1).ToArray()) //skip(1) tức là bỏ qua phần tử đàu tiên.
              : "Hello World!";
// Summary code line 20 and 25: tham số đầu vào gồm một mảng 2 phần tử phần tử đầu tiên là phân loại
        // các message phần tử thứ hai là Message là phép nối chuổi các tham số kể từ tham số thứ 2 trở đi.

var body = Encoding.UTF8.GetBytes(message);
channel.BasicPublish(exchange: "direct_logs",
                     routingKey: severity,
                     basicProperties: null,
                     body: body);
Console.WriteLine($" [x] Sent '{severity}':'{message}'");

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();