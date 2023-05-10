using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

[XmlRoot(ElementName = "Dialogues")]
public class XmlDialogues
{
   [XmlElement(ElementName = "DialogueObject")]
   public XmlDialogue[] DialogueList { get; set; }
}

public class XmlDialogue {
    
    [XmlElement("Index")]
    public string Index { get; set; }

    [XmlElement("DialogueProgress")]
    public string DialogueProgress { get; set; }

    [XmlArray("RequiredProgress")]
    [XmlArrayItem("UnlockValue")]
    public string[] UnlockValues { get; set; }

    [XmlElement("Type")]
    public string Type { get; set; }

    [XmlArray("Dialogue")]
    [XmlArrayItem("DialogueLine")]
    public XmlDialogueLine[] LinesArray { get; set; }
}

public class XmlDialogueLine {

    [XmlElement("Index")]
    public string Index { get; set; }

    [XmlElement("Character")]
    public string Character { get; set; }

    [XmlElement("Line")]
    public string Line { get; set; }

    [XmlElement("VO")]
    public string VO { get; set; }    

    [XmlElement("Type")]
    public string Type { get; set; }
}


