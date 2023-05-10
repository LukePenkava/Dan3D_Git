using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IFocusUI 
{
    GameObject gameObject { get; } 

    UIScrollArea GetScrollArea();

    void Activate();
    void SetFocus();
    void UnFocus();
    bool CanBeFocused();

    Vector2 GetPosition(); 
    Vector2 GetLocalPosition(); 
    
}