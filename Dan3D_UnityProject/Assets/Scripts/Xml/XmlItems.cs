using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

[XmlRoot(ElementName = "items")]
public class XmlItems
{
   [XmlElement(ElementName = "item")]
   public XmlItem[] itemList { get; set; }
}

public class XmlItem {

    [XmlArray("ItemTags")]
    [XmlArrayItem("Tag")]
    public string[] Tags { get; set; }

    [XmlElement("name")]
    public string Name { get; set; }

    [XmlArray("UseFunctions")]
    [XmlArrayItem("Function")]
    public XmlUseFunction[] UseFunctions { get; set; }

    [XmlElement("title")]
    public string Title { get; set; }

    [XmlElement("description")]
    public string Description { get; set; }

    [XmlElement("amount")]
    public string Amount { get; set; }
}

public class XmlUseFunction {

    [XmlText]
    public string FunctionName { get; set; }
  
    [XmlAttribute]
    public string Value { get; set; }
}