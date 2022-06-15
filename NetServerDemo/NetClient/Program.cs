using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NetClient;

namespace NetServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "蓝创精英团队 NetServer 客户端";
            NetClient.NetClient client = null;
            bool isTrue = true;
            while (isTrue)
            {
                Console.WriteLine("请输入本地需要连接的ip:端口号:");
                var read = Console.ReadLine();
                if (read.IndexOf(":") > -1)
                {
                    var data = read.Split(':');
                    var ip = data[0];
                    var port = int.Parse(data[1]);
                    client = new NetClient.NetClient(ip, port);
                    client.Start();
                    break;
                }
                else
                {
                    continue;
                }
            }
            string message = string.Empty;
            while ((message = Console.ReadLine()) != "exit")
            {
                client.Send(message);
            }
            Console.WriteLine("客户端启动，测试中!!");
            Console.ReadLine();
        }
    }
}
