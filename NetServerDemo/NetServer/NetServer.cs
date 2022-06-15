using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetServer
{
    public class NetServer
    {
        Socket ServerSocket;
        int port = 0;
        string LocalAddress;
        public NetServer(int port)
        {
            this.port = port;
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            LocalAddress = GetLocalAddress().ToString();
            ServerSocket.Bind(new IPEndPoint(GetLocalAddress(), port));
        }
        public NetServer(string port)
        {
            int Port = 0;
            int.TryParse(port, out Port);
            this.port = Port;
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var b = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            LocalAddress = GetLocalAddress().ToString();
            ServerSocket.Bind(new IPEndPoint(GetLocalAddress(), this.port));
        }
        public IPAddress GetLocalAddress()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            int len = interfaces.Length;

            for (int i = 0; i < len; i++)
            {
                NetworkInterface ni = interfaces[i];
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    if (ni.Name == "本地连接")
                    {
                        IPInterfaceProperties property = ni.GetIPProperties();
                        foreach (UnicastIPAddressInformation ip in
                            property.UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                return  ip.Address;
                            }
                        }
                    }
                }

            }
            return IPAddress.Loopback; ;
        }
        public void Start()
        {
            ServerSocket.Listen(50);
            //负责监听客户端的线程:创建一个监听线程  
            Thread threadwatch = new Thread(watchconnecting);
            //将窗体线程设置为与后台同步，随着主线程结束而结束  
            threadwatch.IsBackground = true;
            //启动线程     
            threadwatch.Start();
            Console.WriteLine(LocalAddress + ":" + port);
            Console.WriteLine("开启监听。。。");
            Console.WriteLine("请勿输入任何数据，会停止服务!!!");
        }
        //监听客户端发来的请求  
        public void watchconnecting()
        {
            Socket connection = null;
            //持续不断监听客户端发来的请求     
            while (true)
            {
                try
                {
                    connection = ServerSocket.Accept();
                }
                catch (Exception ex)
                {
                    //提示套接字监听异常     
                    Console.WriteLine(ex.Message);
                    break;
                }
                try
                {
                    //获取客户端的IP和端口号  
                    IPAddress clientIP = (connection.RemoteEndPoint as IPEndPoint).Address;
                    int clientPort = (connection.RemoteEndPoint as IPEndPoint).Port;

                    //客户端网络结点号  
                    string remoteEndPoint = connection.RemoteEndPoint.ToString();
                    //显示与客户端连接情况
                    Console.WriteLine("成功与" + remoteEndPoint + "客户端建立连接！");

                    //IPEndPoint netpoint = new IPEndPoint(clientIP,clientPort); 
                    IPEndPoint netpoint = connection.RemoteEndPoint as IPEndPoint;

                    //创建一个通信线程      
                    ParameterizedThreadStart pts = new ParameterizedThreadStart(recv);
                    Thread thread = new Thread(pts);
                    //设置为后台线程，随着主线程退出而退出 
                    thread.IsBackground = true;
                    //启动线程     
                    thread.Start(connection);
                }
                catch (Exception)
                { }
            }
        }
        /// <summary>
        /// 接收客户端发来的信息，客户端套接字对象
        /// </summary>
        /// <param name="socketclientpara"></param>    
        public void recv(object socketclientpara)
        {
            Socket socketServer = socketclientpara as Socket;
            try
            {
                while (true)
                {
                    //创建一个内存缓冲区，其大小为1024*1024字节  即1M     
                    byte[] arrServerRecMsg = new byte[1024 * 1024];
                    //将接收到的信息存入到内存缓冲区，并返回其字节数组的长度    
                    try
                    {
                        int length = socketServer.Receive(arrServerRecMsg);
                        //Console.WriteLine(length);
                        //接受多少，发送多少
                        if (length > 0)
                        {
                            byte[] b = new byte[length];
                            Array.Copy(arrServerRecMsg, b, b.Length);

                            var ret = socketServer.Send(b);
                            //将发送的字符串信息附加到文本框txtMsg上        
                            Console.WriteLine("客户端:" + socketServer.RemoteEndPoint + "发送客户端状态:" + ret + "返回结果:" + Encoding.UTF8.GetString(b) + ",time:" + DateTime.Now.ToString());
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        //提示套接字监听异常  
                        Console.WriteLine("客户端" + socketServer.RemoteEndPoint + "已经中断连接" + "\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n");
                        //关闭之前accept出来的和客户端进行通信的套接字 
                        socketServer.Close();
                        break;
                    }
                }
                //提示套接字监听异常  
                Console.WriteLine("客户端" + socketServer.RemoteEndPoint + "已经中断连接");
                //关闭之前accept出来的和客户端进行通信的套接字 
                socketServer.Close();
            }
            catch (Exception)
            {

            }
        }
    }
}
