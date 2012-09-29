using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DracWake.Console
{
	class Options : CommandLineOptionsBase
	{
		[Option("h", "host", Required = true, HelpText = "host name or IP of the DRAC5 kvm server")]
		public string Host { get; set; }

		[Option("c", "command", DefaultValue = Command.GetPowerState, HelpText = "execute this command")]
		public Command Command { get; set; }

		[Option("u", "user", DefaultValue="root", HelpText = "user name")]
		public string User { get; set; }

		[Option("p", "password", DefaultValue = "calvin", HelpText = "password")]
		public string Password { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
		}
	}
}
