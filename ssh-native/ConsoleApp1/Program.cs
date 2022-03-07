using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skully.SshClient;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Log("Creating client", ConsoleColor.Yellow);
            var client = new Client();
            Log("Logging In", ConsoleColor.Green);
            client.Connect(new SshConnectionInformation(){IpAddress = "debian.local", Password = "3e4r5t6y#E$R%T^Y", UserName = "gpp"});
            client.Send();
            Console.Write("Press Any Key To Continue...");
            Console.ReadKey();
        }

        private static void Log(string message, ConsoleColor color = ConsoleColor.White)
        {
            var currentColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = currentColor;
        }
    }
}
