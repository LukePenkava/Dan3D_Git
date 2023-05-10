using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public GameObject player;
    Character_BaseData data;
    //AreaManager areaManager;
    //PlayerManager playerManager;    
    //InteractionManager interactionManager;
    public Inventory itemsInventory;
    int itemsFilterIndex = 0;
    public Text itemsFilterText;

    public GameObject uiParent;
    public GameObject canvas;
    public GameObject section_Items;
    public GameObject section_Recipes;
    public GameObject section_Quests;
    public GameObject section_Map;
    Dictionary<InventorySections, GameObject> sectionsDictionary = new Dictionary<InventorySections, GameObject>();

    public enum InventorySections {
        Items,
        Recipes,
        Quests,
        Map
    }
    
    
    Inventory.InventoryType activeInventoryType = Inventory.InventoryType.MainInventory;

    InventorySections activeSection = InventorySections.Items;
    public InventorySections ActiveSection { get { return activeSection; } }
    public UISwitch sectionButtons;
    
    public RectTransform mapMoveParent;
    public GameObject mapPlayerVisual;

    bool uiOpened = false;
    public bool UiOpened {
        get { return uiOpened; }
    }

    float canvasScale = 0f;


    void Start() {

        data = player.GetComponent<Character_BaseData>();  
        uiParent.SetActive(uiOpened);   
        GameObject managers = GameObject.FindGameObjectWithTag("Managers");
        //areaManager = managers.GetComponent<AreaManager>();
        //interactionManager = managers.GetComponent<InteractionManager>();
       // playerManager = GetComponent<PlayerManager>();
        canvasScale = canvas.GetComponent<RectTransform>().localScale.x;

        sectionsDictionary.Add(InventorySections.Items, section_Items);
        sectionsDictionary.Add(InventorySections.Recipes, section_Recipes);
        sectionsDictionary.Add(InventorySections.Quests, section_Quests);
        sectionsDictionary.Add(InventorySections.Map, section_Map);
    }

  
    //Update terrain color visual to light brown, player visual to green
    //Add Compas
  
   
    //Add items categories and filtering
    //Add quest object, info etc.

    public void SelectSection(int index) { //InventorySections section) {
        InventorySections section = (InventorySections)index;
        activeSection = section;               

        //Set Section Parent on
        foreach(KeyValuePair<InventorySections, GameObject> temp in sectionsDictionary) {
            temp.Value.gameObject.SetActive(false);
        }
        sectionsDictionary[section].SetActive(true);


        //Custom logic for each section
        
        //Each time Items are selected or deselected, init or destroy the items
        if(section == InventorySections.Items) {
            itemsFilterIndex = 0;
            Items.ItemTags filter = Items.ItemTags.Neutral;
            SetFilterText(filter);
            itemsInventory.Open(data, activeInventoryType, null, filter);
        } else {
            itemsInventory.Close();
        }

        // if(section == InventorySections.Recipes) {
        //     canvas.GetComponent<RecipesUI>().Init(RecipesUI.MenuType.Inventory);
        // }

        // if(section == InventorySections.Quests) {
        //     //canvas.GetComponent<QuestsUI>().Init();
        //     canvas.GetComponent<QuestsUI>().SelectQuests();
        // }

        // if(section == InventorySections.Map) {
        //     mapPlayerVisual.transform.localPosition = areaManager.GetPlayerNodePos();
        //     CenterMap();
        // }

    }
   
    //Comes from UIManager and GiveItem Interaction
    public void OpenInventory(int defaultSection = 0, Inventory.InventoryType inventoryType = Inventory.InventoryType.MainInventory) {
        uiOpened = !uiOpened;       
        uiParent.SetActive(uiOpened);  
        //playerManager.InputEnabled = !uiOpened;              
    
        //If closing Inventory, close items, destroy slots etc
        if(uiOpened == false) {         
            itemsInventory.Close();
            uiParent.SetActive(false);  
            UIManager.activeUISection = UIManager.UISection.Default;
           
            //if(activeInventoryType == Inventory.InventoryType.GiveItem) { interactionManager.GetCurrentInteraction().Close(); }
            //Set it at the end to use type when it was opened
            activeInventoryType = Inventory.InventoryType.MainInventory;

        } else {
            UIManager.activeUISection = UIManager.UISection.MainInventory;
            activeInventoryType = inventoryType;

            //Select Items section by default when opening inventory, 0 index is items button 
            //Open actual inventory through switch button, as if player clicked the button     
            sectionButtons.Switch(defaultSection);

            //If type is GiveItem, disable other section buttons
            for(int i = 1; i < sectionButtons.buttons.Count; i++) {
                sectionButtons.buttons[i].disabled = activeInventoryType == Inventory.InventoryType.GiveItem ? true : false;
            }           
            
        }
    }

    //Filter items in Inventory by their tag. Convert enums to list and go through them by index
    public void InventoryFilter() {
        List<Items.ItemTags> tagsList = new List<Items.ItemTags>();
        foreach(Items.ItemTags tag in Items.ItemTags.GetValues(typeof(Items.ItemTags))) {
            tagsList.Add(tag);
        }

        itemsFilterIndex++;
        if(itemsFilterIndex >= tagsList.Count) {
            itemsFilterIndex = 0;
        }
        Items.ItemTags newTag = tagsList[itemsFilterIndex];
        SetFilterText(newTag);

        itemsInventory.Open(data, activeInventoryType, null, newTag);
    }

    void SetFilterText(Items.ItemTags tag) {
        if(tag == Items.ItemTags.Neutral) {
            itemsFilterText.text = "ALL ITEMS";
        } else {
            itemsFilterText.text = tag.ToString();
        }
    }

    public void CenterMap() {
        mapMoveParent.localScale = new Vector3(1f, 1f, 1f);
        mapMoveParent.transform.localPosition = new Vector2(mapPlayerVisual.transform.localPosition.x * -1f, mapPlayerVisual.transform.localPosition.y * -1f);
    }

    public void ZoomOut() {
        mapMoveParent.localScale = new Vector3(0.5f, 0.5f, 1f);
        mapMoveParent.transform.localPosition = new Vector2(mapPlayerVisual.transform.localPosition.x / -2f, mapPlayerVisual.transform.localPosition.y / -2f);
    }

    public void DragMap(Vector2 dragInput) {     
        mapMoveParent.transform.localPosition = new Vector2(mapMoveParent.transform.localPosition.x + (dragInput.x / canvasScale), mapMoveParent.transform.localPosition.y + (dragInput.y / canvasScale));
    }

    public void MapZooming(float zoomInput) {
        if(zoomInput < 0f) {
            if(mapMoveParent.localScale.x > 0.1f) {
                mapMoveParent.localScale = new Vector3(mapMoveParent.localScale.x + (zoomInput * 0.1f), mapMoveParent.localScale.y + (zoomInput * 0.1f), 1f);
            } else {
                mapMoveParent.localScale = new Vector3(0.1f, 0.1f, 1f);
            }
        } else {
            if(mapMoveParent.localScale.x < 3f) {
                mapMoveParent.localScale = new Vector3(mapMoveParent.localScale.x + (zoomInput * 0.1f), mapMoveParent.localScale.y + (zoomInput * 0.1f), 1f);
            } else {
                mapMoveParent.localScale = new Vector3(3f, 3f, 1f);
            }
        }
    }
}
