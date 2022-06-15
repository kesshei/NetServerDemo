using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "蓝创精英团队 NetServer 服务端";
            Console.WriteLine("请输入本地需要监听的端口号:");
            var read = Console.ReadLine();
            NetServer server = new NetServer(read);
            server.Start();
            Console.WriteLine("服务启动，测试中!!");
            Console.ReadLine();
        }
    }
}
