using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;


namespace ConsoleApp1
{
    class Program
    {
        private static Socket socket = null;
        private static bool corriendo = false;
        private static IPEndPoint puntoLocal = null;

        UdpClient cliente = null;
        String ServerIP = "";
        int ListeningPort = 0;
        IPEndPoint ep = null;

        void Main(string[] args)
        {
            Console.Write("¿Cual sera su rol? \n");
            Console.Write("Servidor (S) o Cliente (C):  ");
            string consoleInput = Console.ReadLine();
            switch(consoleInput)
            {
                case ("S"):
                    CrearServer();
                    break;
                case ("C"):
                    CrearCliente();
                    break;
            }        
        }
        private void CrearServer()
        {
            IPAddress ipEscucha = IPAddress.Any;
            Console.Write("Puerto por el cual comunicarse:  ");
            ListeningPort = Console.Read();           
            puntoLocal = new IPEndPoint(ipEscucha, ListeningPort);
            new Thread(Escuchador).Start();
        }

        private static void Escuchador()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(puntoLocal);
            byte[] buffer = new byte[1024];
            EndPoint ipRemota = new IPEndPoint(IPAddress.Any, 0);
            corriendo = true;

            while (corriendo)
            {
                if (socket.Available == 0) //consultamos si hay datos disponibles que no hemos leido 
                {
                    Thread.Sleep(200); //esperamos 200 milisegundos para volver a preguntar 
                    continue; //esta sentencia hace que el programa regrese al ciclo while(corriendo) 
                }

                int contadorLeido = socket.ReceiveFrom(buffer, ref ipRemota);
                string datosRecibidos = Encoding.Default.GetString(buffer, 0, contadorLeido);
                Console.WriteLine("Recibí: " + datosRecibidos);
            }
        }
        void Mensajear()
        {
            while (corriendo)
            {
                string mensajeXenviar = Console.ReadLine();
                if(mensajeXenviar != "")
                {
                    byte[] datosEnBytes = Encoding.Default.GetBytes(mensajeXenviar);
                    cliente.Send(datosEnBytes, datosEnBytes.Count());
                }
                var receivedData = cliente.Receive(ref ep);
                Console.Write(ep.ToString()+":   ");
                Console.Read();
            }
        }
        void CrearCliente()
        {
            Console.Write("Ingrese IP del servidor:  ");
            ServerIP = Console.ReadLine();
            Console.Write("Puerto por el cual comunicarse:  ");
            ListeningPort = Console.Read();

            cliente = new UdpClient();
            ep = new IPEndPoint(IPAddress.Parse(ServerIP), ListeningPort); // endpoint where server is listening
            cliente.Connect(ep);
            corriendo = true;
            new Thread(Mensajear).Start();
        }
    }
}
