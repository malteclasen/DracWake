using CommandLine;
using CommandLine.Text;
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
			var options = new Options();
			if (CommandLineParser.Default.ParseArguments(args, options))
			{
				var webClient = new Core.WebClient();
				var controller = new Core.Controller(webClient, new Uri("https://" + options.Host));
				switch (options.Command)
				{
					case Command.GetPowerState:
						System.Console.WriteLine("PowerStatus: " + controller.GetPowerState());
						break;
					case Command.PowerOn:
						controller.PowerOn();
						break;
				}
			}
        }
    }
}
