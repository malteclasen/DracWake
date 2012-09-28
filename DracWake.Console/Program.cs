using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DracWake.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var webClient = new Core.WebClient())
            {
                var controller = new Core.Controller(webClient, new Uri("https://192.168.1.24"));
                //controller.PowerOn();
                System.Console.WriteLine("PowerStatus: " + controller.GetPowerState());
            }
        }
    }
}
