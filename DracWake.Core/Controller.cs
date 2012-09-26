using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DracWake.Core
{
    public enum PowerState
    {
        On,
        Off
    }

    public class Controller
    {
        private readonly IWebClient _webClient;
        private readonly Uri _baseAddress;

        static Controller()
        {
            InstallSslValidator();
        }

        private static void InstallSslValidator()
        {
            var defaultValidator = System.Net.ServicePointManager.ServerCertificateValidationCallback;
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                (request, certificate, chain, sslPolicyErrors) => 
                    certificate.Subject.Contains("O=DO_NOT_TRUST, OU=Created by http://www.fiddler2.com") 
                    || (certificate.Subject == "CN=DRAC5 default certificate, OU=Remote Access Group, O=Dell Inc., L=Round Rock, S=Texas, C=US") 
                    || (defaultValidator != null && defaultValidator(request, certificate, chain, sslPolicyErrors));
        }

        public Controller(IWebClient webClient, Uri baseAddress)
        {
            _webClient = webClient;
            _baseAddress = baseAddress;
        }

        public async Task<PowerState> PowerOnAsync()
        {
            try
            {
                var loginPage = await _webClient.Get(new Uri(_baseAddress, "/cgi-bin/webcgi/login"));
                var mainPage = await _webClient.Post(new Uri(_baseAddress, "/cgi-bin/webcgi/login"), Encoding.UTF8.GetBytes("user=root&password=calvin"));

                if (!mainPage.Contains("main_en.xsl"))
                    throw new Exception("login failed, expected redirect to main page");

                var powerPage = await _webClient.Post(new Uri(_baseAddress, "/cgi-bin/webcgi/power?cat=C00&tab=T02&id=P01"), Encoding.UTF8.GetBytes("caller=&pageCode=&pageId=&pageName=&action=1"));

                if (!powerPage.Contains("power_en.xsl"))
                    throw new Exception("power command failed, expected status on power page");

                await _webClient.Get(new Uri(_baseAddress, "/cgi-bin/webcgi/logout"));
                return PowerState.On;
            }
            catch
            {
                _webClient.Get(new Uri(_baseAddress, "/cgi-bin/webcgi/logout")).RunSynchronously();
                throw;
            }
        }

        public PowerState PowerOn()
        {
            var task = PowerOnAsync();
            return task.Result;
            
        }
    }
}
