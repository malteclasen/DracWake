using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace DracWake.Core
{
	public class Controller
	{
		private readonly IWebClient _webClient;
		private readonly Uri _baseAddress;

		public string Username { get; set; }
		public string Password { get; set; }

		public Controller(IWebClient webClient, Uri baseAddress)
		{
			_webClient = webClient;
			_baseAddress = baseAddress;
			Username = "root";
			Password = "calvin";
		}

		private async Task Login()
		{
			var loginPage = await _webClient.Get(new Uri(_baseAddress, "/cgi-bin/webcgi/login"));
			var indexPage = await _webClient.Post(new Uri(_baseAddress, "/cgi-bin/webcgi/login"), Encoding.UTF8.GetBytes("user="+Username+"&password="+Password));

			if (!indexPage.Contains("main_en.xsl"))
				throw new Exception("login failed, expected redirect to index page");
		}

		private async Task Logout()
		{
			await _webClient.Get(new Uri(_baseAddress, "/cgi-bin/webcgi/logout"));
		}

		private async Task<T> ExecuteLoggedIn<T>(Func<Task<T>> action)
		{
			try
			{
				await Login();
				var result = await action();
				await Logout();
				return result;
			}
			catch
			{
				Logout().RunSynchronously();
				throw;
			}
		}

		private async Task<PowerState> PowerOnCommand()
		{
			var powerPage = await _webClient.Post(new Uri(_baseAddress, "/cgi-bin/webcgi/power?cat=C00&tab=T02&id=P01"), Encoding.UTF8.GetBytes("caller=&pageCode=&pageId=&pageName=&action=1"));
			return ParsePowerState(powerPage);
		}

		private PowerState ParsePowerState(string xmlPage)
		{
			var doc = XDocument.Parse(xmlPage);
			var property = doc.Descendants("property").Where(n => n.Attributes().Any(a => a.Name == "name" && a.Value == "PowerStatus"));
			var rawState = property.First().Descendants("value").First().FirstNode.ToString();
			switch (rawState)
			{
				case "ON": return PowerState.On;
				case "OFF": return PowerState.Off;
				default: throw new Exception("unknown power state " + rawState);
			}
		}

		private async Task<PowerState> GetPowerStateCommand()
		{
			var powerPage = await _webClient.Get(new Uri(_baseAddress, "/cgi-bin/webcgi/power?cat=C00&tab=T02&id=P01"));
			return ParsePowerState(powerPage);
		}

		public async Task<PowerState> PowerOnAsync()
		{
			return await ExecuteLoggedIn<PowerState>(PowerOnCommand);
		}

		public PowerState PowerOn()
		{
			return PowerOnAsync().Result;
		}

		public async Task<PowerState> GetPowerStateAsync()
		{
			return await ExecuteLoggedIn<PowerState>(GetPowerStateCommand);
		}

		public PowerState GetPowerState()
		{
			return GetPowerStateAsync().Result;
		}
	}
}
