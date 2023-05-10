using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    public Image icon;
    public Text titleText;
    public Text amountText;

    public void Init(Items.ItemName itemName, int amount) {
        icon.sprite = Resources.Load<Sprite>("Icons/Items/" + itemName.ToString());
        string title = ItemsDirectory.ItemsData[itemName].title;
        titleText.text = title.ToString();
        amountText.text = "x" + amount.ToString();
    }
}