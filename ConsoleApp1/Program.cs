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


        static void Main(string[] args)
        {
            IPAddress ipEscucha = IPAddress.Any; //indicamos que escuche por cualquier tarjeta de red local 
            //IPAddress ipEscucha = IPAddress.Parse("0.0.0.0"); //o podemos indicarle la IP de la tarjeta de red local 
            int puertoEscucha = 8000; //puerto por el cual escucharemos datos             
            puntoLocal = new IPEndPoint(ipEscucha, puertoEscucha);
            new Thread(Escuchador).Start();
            Console.ReadLine(); //esperar a que el usuario escriba algo y de enter 
            corriendo = false; //finalizar el servidor 
        }

        private static void Escuchador()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(puntoLocal);
            Console.WriteLine("escuchando...");
            //declarar buffer para recibir los datos y le damos un tamaño máximo de datos recibidos por cada mensaje 
            byte[] buffer = new byte[1024];
            //definir objeto para obtener la IP y Puerto de quien nos envía los datos 
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
        private static void Enviar()
        {
            string datosAEnviar = "aqui poner datos a enviar";
            string ipDestino = "190.144.144.144"; //poner ip destino
            int puertoDestino = 8002;
            byte[] datosEnBytes = Encoding.Default.GetBytes(datosAEnviar);
            EndPoint ipPuertoRemoto = new IPEndPoint(IPAddress.Parse(ipDestino), puertoDestino);
            socket.SendTo(datosEnBytes, ipPuertoRemoto);
            //si ya tienes un EndPoint como por ejemplo el de quien  
            //te ha enviado datos, entonces puedes usar ese en el método SendTo 
        }
    }
}
