using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DracWake.Core.Tests
{
	[TestFixture]
	public class SimpleXmlTests
	{
		[Test]
		public void AccessTextNode()
		{
			var code = @"<?xml version=""1.0"" encoding=""UTF-8""?><?xml-stylesheet type=""text/xsl"" href=""/cgi/locale/power_en.xsl"" media=""screen""?><drac>
<privilege racPrivilege=""511"" login=""1"" cfg=""1"" cfguser=""1"" clearlog=""1"" servercontrol=""1"" console=""1"" vmedia=""1"" testalert=""1"" debug=""1"" />
<object name=""system"">
<property name=""PowerStatus"" type=""string""><value>OFF</value></property>
</object>
</drac>";
			dynamic simpleXml = new SimpleXmlDocument(XDocument.Parse(code));
			Assert.That(simpleXml.@object, Is.Not.Null);
			Assert.That(simpleXml.@object.propertyWithName("PowerStatus").value.Value, Is.EqualTo("OFF"));
		}

		[Test]
		public void AccessAttributeValue()
		{
			var code = @"<?xml version=""1.0"" encoding=""UTF-8""?><?xml-stylesheet type=""text/xsl"" href=""/cgi/locale/power_en.xsl"" media=""screen""?><drac>
<privilege racPrivilege=""511"" login=""1"" cfg=""1"" cfguser=""1"" clearlog=""1"" servercontrol=""1"" console=""1"" vmedia=""1"" testalert=""1"" debug=""1"" />
<object name=""system"">
<property name=""PowerStatus"" type=""string""><value>OFF</value></property>
</object>
</drac>";
			dynamic simpleXml = new SimpleXmlDocument(XDocument.Parse(code));
			Assert.That(simpleXml.privilege, Is.Not.Null);
			Assert.That(simpleXml.privilege.Attributes.racPrivilege, Is.EqualTo("511"));
		}

	}
}
