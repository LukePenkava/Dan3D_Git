using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

[XmlRoot(ElementName = "Items")]
public class XmlItemsPool
{
   [XmlElement(ElementName = "Item")]
   public XmlPoolItem[] itemList { get; set; }
}

public class XmlPoolItem {
    
    [XmlElement("ItemName")]
    public string ItemName { get; set; }

    [XmlElement("MaxAmount")]
    public string MaxAmount { get; set; }
   
}
