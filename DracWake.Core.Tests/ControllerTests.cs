using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Ploeh.AutoFixture;

namespace DracWake.Core.Tests
{
    [TestFixture]
    public class ControllerTests
    {
        private class MockWebClient : Core.IWebClient
        {
            public async Task<string> Get(Uri uri)
            {
                if (uri.PathAndQuery == "/cgi-bin/webcgi/login")
                    return @"<?xml version=""1.0"" encoding=""UTF-8""?><?xml-stylesheet type=""text/xsl"" href=""/cgi/locale/login_en.xsl"" media=""screen""?><drac>
<CARD_IP>192.168.1.24</CARD_IP><OEM>0</OEM><MSG>0x0</MSG><LANG>en</LANG>
<LOGIN><RESP><RC>0x0</RC><SID>0</SID><STATE>0x00000000</STATE><STATENAME></STATENAME></RESP></LOGIN><SCLEnabled>0</SCLEnabled>
</drac>
";
                return null;
            }

            public async Task<string> Post(Uri uri, byte[] data)
            { 
                var decodedData = Encoding.UTF8.GetString(data);
                if (uri.PathAndQuery == "/cgi-bin/webcgi/login" && decodedData == "user=root&password=calvin")
                    return @"<?xml version=""1.0"" encoding=""UTF-8""?><?xml-stylesheet type=""text/xsl"" href=""/cgi/locale/index_en.xsl"" media=""screen""?><drac>
<privilege racPrivilege=""511"" login=""1"" cfg=""1"" cfguser=""1"" clearlog=""1"" servercontrol=""1"" console=""1"" vmedia=""1"" testalert=""1"" debug=""1"" />
<IpAddress>192.168.1.24</IpAddress><HttpsPortNumber>443</HttpsPortNumber><SCLEnabled>0</SCLEnabled>
</drac>
";
                if (uri.PathAndQuery == "/cgi-bin/webcgi/power?cat=C00&tab=T02&id=P01" && decodedData.Contains("action=1"))
                    return @"<?xml version=""1.0"" encoding=""UTF-8""?><?xml-stylesheet type=""text/xsl"" href=""/cgi/locale/power_en.xsl"" media=""screen""?><drac>
<privilege racPrivilege=""511"" login=""1"" cfg=""1"" cfguser=""1"" clearlog=""1"" servercontrol=""1"" console=""1"" vmedia=""1"" testalert=""1"" debug=""1"" />
<object name=""system"">
<property name=""PowerStatus"" type=""string""><value>ON</value></property>
</object>
</drac>";
                return null;
            }
        }

        [Test]
        public void PowerOn()
        {
            var fixture = new Fixture().Customize(new Ploeh.AutoFixture.AutoMoq.AutoMoqCustomization());
            fixture.Register<IWebClient>(() => new MockWebClient());
            var controller = fixture.CreateAnonymous<Core.Controller>();

            controller.Invoking(c => c.PowerOn()).ShouldNotThrow();
        }
    }
}
