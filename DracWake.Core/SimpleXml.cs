using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DracWake.Core
{
	public class SimpleXmlAttributes : System.Dynamic.DynamicObject
	{
		private XElement _element;

		public SimpleXmlAttributes(XElement element)
		{
			_element = element;
		}

		public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
		{
			var attribute = _element.Attribute(binder.Name);
			if (attribute == null)
				result = null;
			else
				result = attribute.Value;
			return result != null;
		}

		public override string ToString()
		{
			return _element.ToString();
		}
	}
	
	public class SimpleXmlElement : System.Dynamic.DynamicObject
	{
		private XElement _element;

		public SimpleXmlElement(XElement element)
		{
			_element = element;
		}

		public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
		{
			switch (binder.Name)
			{
				case "Attributes":
					result = new SimpleXmlAttributes(_element);
					break;
				case "Value":
					result = _element.Value;
					break;
				default:
					var element = _element.Element(binder.Name);
					if (element == null)
						result = null;
					else
						result = new SimpleXmlElement(element);
					break;
			}

			return result != null;
		}

		public override bool TryInvokeMember(System.Dynamic.InvokeMemberBinder binder, object[] args, out object result)
		{
			var parser = new Regex("(?<element>.*)With(?<attribute>.*)");
			var match = parser.Match(binder.Name);
			var elementName = match.Groups["element"].Captures.Cast<Capture>().First().Value;
			var attributeName = match.Groups["attribute"].Captures.Cast<Capture>().First().Value;
			var attributeValue = (string)args[0];

			var element = _element.Elements(elementName).Where(e =>
			{
				var attribute = e.Attributes().First(a => string.Compare(a.Name.LocalName, attributeName, StringComparison.InvariantCultureIgnoreCase) == 0);
				return attribute.Value == attributeValue;
			}).FirstOrDefault();
			if (element == null)
				result = null;
			else
				result = new SimpleXmlElement(element);

			return result != null;
		}

		public override string ToString()
		{			
			return _element.ToString();
		}	
	}

	public class SimpleXmlDocument : System.Dynamic.DynamicObject
	{
		private XDocument _document;

		public static dynamic Parse(string document)
		{
			return new SimpleXmlDocument(XDocument.Parse(document));
		}

		public SimpleXmlDocument(XDocument document)
		{
			_document = document;
		}

		public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
		{
			var element = _document.Root.Element(binder.Name);
			if (element == null)
				result = null;
			else
				result = new SimpleXmlElement(element);
			return result != null;
		}
	
		public override string ToString()
		{
			return _document.ToString();
		}
	}
}
