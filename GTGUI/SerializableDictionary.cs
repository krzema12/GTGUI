
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

// http://weblogs.asp.net/pwelter34/444961

[XmlRoot("dictionary")]
public class SerializableDictionary<TKey, TValue>
	: Dictionary<TKey, TValue>, IXmlSerializable
{
	#region IXmlSerializable Members
	public System.Xml.Schema.XmlSchema GetSchema()
	{
		return null;
	}
	public void ReadXml(System.Xml.XmlReader reader)
	{
		XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
		bool wasEmpty = reader.IsEmptyElement;
		reader.Read();
		if (wasEmpty)
			return;
		while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
		{
			reader.ReadStartElement("item");
			reader.ReadStartElement("key");
			TKey key = (TKey)keySerializer.Deserialize(reader);
			reader.ReadEndElement();
			reader.ReadStartElement("value");
			var nodeType = Type.GetType("GTGUI.ViewModels.Blocks." + reader.Name);
			XmlSerializer valueSerializer = new XmlSerializer(nodeType);
			TValue value = (TValue)valueSerializer.Deserialize(reader);
			reader.ReadEndElement();
			this.Add(key, value);
			reader.ReadEndElement();
			reader.MoveToContent();
		}
		reader.ReadEndElement();
	}
	public void WriteXml(System.Xml.XmlWriter writer)
	{
		XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
		foreach (TKey key in this.Keys)
		{
			writer.WriteStartElement("item");
			writer.WriteStartElement("key");
			keySerializer.Serialize(writer, key);
			writer.WriteEndElement();
			writer.WriteStartElement("value");
			TValue value = this[key];
			XmlSerializer valueSerializer = new XmlSerializer(value.GetType());
			valueSerializer.Serialize(writer, value);
			writer.WriteEndElement();
			writer.WriteEndElement();
		}
	}
	#endregion
}
