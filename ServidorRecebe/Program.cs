using System;
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
            int qtdbytes;
            String filename;

            socketreceber.Bind(endereco);

            // 1. Recebe o nome do arquivo
            qtdbytes = socketreceber.ReceiveFrom(data, ref endereco);
            filename = Encoding.UTF8.GetString(data, 0, qtdbytes);
            Console.WriteLine("Arquivo recebido: " + filename);

            FileStream outfile; // output file
            outfile = new System.IO.FileStream(filename,
                                               System.IO.FileMode.Create,
                                               System.IO.FileAccess.Write);

            int chaveCriptografia = 5;
            int fatorCripto = 18 + chaveCriptografia; // 23


            qtdbytes = socketreceber.ReceiveFrom(data, ref endereco);                         // 2. Recebe os dados do arquivo criptografados

            String conteudoCriptografado = Encoding.UTF8.GetString(data, 0, qtdbytes);

            for (int i = 0; i < conteudoCriptografado.Length; ++i)
            {

                char caractereCriptografado = conteudoCriptografado[i];                        // Pega o caractere já montado corretamente da string
                char caractereDescriptografado = (char)(caractereCriptografado - fatorCripto); // Descriptografa o char
                outfile.WriteByte((byte)caractereDescriptografado);                            // Converte o char para byte e grava no arquivo
            }

            outfile.Close();
            socketreceber.Close();

            Console.WriteLine("Arquivo salvo e descriptografado com sucesso!");
        }
    }
}