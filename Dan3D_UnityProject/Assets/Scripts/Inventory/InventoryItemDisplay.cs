using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemDisplay : MonoBehaviour
{
    public Image itemIcon;
    public Text itemName;
    public Text itemDescription;

    void Awake() {
        SetItemInfo(false);
    }

    public void SetItemInfo(bool show, Item itemData = null) {
        
        if(show) {
            itemIcon.gameObject.SetActive(true);
            itemIcon.sprite = Resources.Load<Sprite>("Icons/Items/" + itemData.itemName.ToString());
            itemName.text = itemData.title;
            itemDescription.text = itemData.description;
        } else {
            itemIcon.gameObject.SetActive(false);
            itemIcon.sprite = null;
            itemName.text = "";
            itemDescription.text = "";
        }
    }
}
