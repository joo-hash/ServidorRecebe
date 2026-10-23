using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ServidorRecebe
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket socketreceber = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
            EndPoint endereco = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9060);
            byte[] data = new byte[1024];
            int qtdbytes, qtd;


            String filename;

            socketreceber.Bind(endereco);
            qtdbytes = socketreceber.ReceiveFrom(data, ref endereco);
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, qtdbytes));
            filename = Encoding.ASCII.GetString(data, 0, qtdbytes);

            qtdbytes = socketreceber.ReceiveFrom(data, ref endereco);
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, qtdbytes));
            qtd = int.Parse(Encoding.ASCII.GetString(data, 0, qtdbytes));

            FileStream outfile;

            outfile = new System.IO.FileStream(filename, System.IO.FileMode.Create, System.IO.FileAccess.Write);

            for (int i = 0; i < qtd; i++)
            {
                qtdbytes = socketreceber.ReceiveFrom(data, ref endereco);
                Console.WriteLine(Encoding.ASCII.GetString(data, 0, qtdbytes));

                int checksum = 0; //
                for (int j = 0; j < qtdbytes; ++j)
                {
                    checksum += data[j];
                }
                byte[] bufferChecksum = new byte[20];//
                int qtdChecksum = socketreceber.ReceiveFrom(bufferChecksum, ref endereco);

                int checksumRecebido = int.Parse(Encoding.ASCII.GetString(bufferChecksum, 0, qtdChecksum));

                Console.WriteLine("Checksum calculado: " + checksum);
                Console.WriteLine("Checksum recebido: " + checksumRecebido);

                if (checksum != checksumRecebido)
                {
                    Console.WriteLine("ERRO: pacote corrompido!");
                    outfile.Close();
                    socketreceber.Close();
                    return;
                }

                for (int j = 0; j < qtdbytes; ++j)
                {
                    outfile.WriteByte(data[j]);
                }


            }
            outfile.Close();
            socketreceber.Close();
            Console.ReadKey();
        }
    }
}
