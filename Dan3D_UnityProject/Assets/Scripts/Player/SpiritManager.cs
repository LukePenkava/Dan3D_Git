using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiritManager : MonoBehaviour
{
    float maxSpirit = 40f;
    public float Spirit { get; private set; } = 0f;

    public Slider spiritSlider;

    void Start() {
        SetSpiritSlider();
    }
 

    public void AdjustSpirit(float value) {
        Spirit += value;
        SetSpiritSlider();
    }

    void SetSpiritSlider() {
        spiritSlider.value = Spirit/maxSpirit;
    }
}
