using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDialogueInput 
{
    GameObject gameObject { get; } 

    void DialogueInput();
}
