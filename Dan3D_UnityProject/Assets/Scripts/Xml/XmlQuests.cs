using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

[XmlRoot(ElementName = "Quests")]
public class XmlQuests
{
   [XmlElement(ElementName = "Quest")]
   public XmlQuest[] questsList { get; set; }
}

public class XmlQuest {
    
    [XmlElement("name")]
    public string name { get; set; }  

    [XmlElement("title")]
    public string title { get; set; }

    [XmlElement("description")]
    public string description { get; set; }

    [XmlArray("Tasks")]
    [XmlArrayItem("Task")]
    public XmlTask[] tasks { get; set; }
}

public class XmlTask {

    [XmlElement("TaskIndex")]
    public string index { get; set; }

    [XmlElement("TaskName")]
    public string name { get; set; }

    [XmlElement("Description")]
    public string description { get; set; }

    [XmlElement("NarratorProgress")]
    public string narratorProgress { get; set; }

    [XmlElement("NarratorCompleted")]
    public string narratorCompleted { get; set; }

    [XmlElement("CompletedValue")]
    public string completedValue { get; set; }

    [XmlArray("TaskValues")]
    [XmlArrayItem("TaskValue")]
    public XmlTaskValue[] taskValues { get; set; }    
}

public class XmlTaskValue {
    [XmlText]
    public string Key { get; set; }
  
    [XmlAttribute]
    public string Value { get; set; }
}
