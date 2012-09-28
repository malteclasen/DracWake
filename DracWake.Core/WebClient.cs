﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DracWake.Core
{
    public class WebClient : IWebClient, IDisposable
    {
        static WebClient()
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

        private CookieAwareWebClient _client = new CookieAwareWebClient();

        public async Task<string> Get(Uri uri)
        {
            SetHeaders();
            var raw = await _client.DownloadDataTaskAsync(uri);
            return Encoding.UTF8.GetString(raw);
        }

        private void SetHeaders()
        {
            _client.Headers[HttpRequestHeader.Accept] = "text/html, application/xhtml+xml, */*";
            _client.Headers[HttpRequestHeader.AcceptLanguage] = "en-US,en;q=0.8,de-DE;q=0.5,de;q=0.3";
            _client.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            _client.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
            _client.Headers[HttpRequestHeader.CacheControl] = "no-cache";
            _client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
        }

        public async Task<string> Post(Uri uri, byte[] data)
        {
            SetHeaders();
            var raw = await _client.UploadDataTaskAsync(uri, "POST", data);
            return Encoding.UTF8.GetString(raw);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
