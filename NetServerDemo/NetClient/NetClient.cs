using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetClient
{
    /// <summary>
    /// socket客户端
    /// </summary>
    public class NetClient
    {
        /// <summary>
        /// 客户端socket
        /// </summary>
        Socket _ClientSocket = null;
        /// <summary>
        /// IP地址
        /// </summary>
        public string ipAddress = string.Empty;
        /// <summary>
        /// IP端口
        /// </summary>
        public int Port;
        /// <summary>
        /// 接收线程
        /// </summary>
        private Thread threadReceive;
        /// <summary>
        /// 是否运行
        /// </summary>
        private bool IsRun = false;
        /// <summary>
        /// 远端地址
        /// </summary>
        private string remoteEndPoint;
        /// <summary>
        /// 本端地址
        /// </summary>
        private string LocalEndPoint;
        /// <summary>
        /// 客户端
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="Port"></param>
        public NetClient(string ip, int Port)
        {
            _ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.ipAddress = ip;
            this.Port = Port;
        }

        public void Start()
        {
            _ClientSocket.Connect(new IPEndPoint(IPAddress.Parse(this.ipAddress), this.Port));//通过IP和端口号来定位一个所要连接的服务器端
                                                                                              //客户端网络结点号  
            remoteEndPoint = _ClientSocket.RemoteEndPoint.ToString();
            LocalEndPoint = _ClientSocket.LocalEndPoint.ToString();
            Console.WriteLine($"远端地址:{remoteEndPoint} 本端地址：{LocalEndPoint}");
            threadReceive = new Thread(Receive);
            threadReceive.IsBackground = true;
            threadReceive.Start();
            IsRun = true;
        }
        /// <summary>
        /// 消息的接收
        /// </summary>
        private void Receive()
        {
            byte[] TempData = new byte[1024 * 10];
            while (IsRun)
            {
                //传递一个byte数组，用于接收数据。length表示接收了多少字节的数据
                int length = _ClientSocket.Receive(TempData);
                if (length == 0)
                {
                    IsRun = false;
                    Console.WriteLine("服务器断开链接");
                    break;
                }
                else
                {
                    string message = Encoding.UTF8.GetString(TempData, 0, length);//只将接收到的数据进行转化
                    Console.WriteLine($"远端地址:{remoteEndPoint} 本端地址：{LocalEndPoint} 获取到的数据:{message}");
                }
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Send(string data)
        {
            return _ClientSocket.Send(Encoding.UTF8.GetBytes(data));
        }
        public void Close()
        {
            IsRun = false;
            if (threadReceive != null)
            {
                try
                {
                    threadReceive.Interrupt();
                    Thread.Sleep(200);
                }
                catch (Exception)
                { }
                threadReceive = null;
            }
        }

    }
}
