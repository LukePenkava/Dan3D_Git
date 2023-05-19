using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;   
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public delegate void FadeCompleted();

    GameDirector gameDirector;
    PlayerUI playerUI;
    // TradeManager tradeScript;
    // PlayerInput playerInput;  
    // PlayerManager playerManager;
    // InteractionManager interactionManager;   
    // GameMenu gameMenu;    

    public enum UISection {
        Default,
        MainInventory,
        RecipeBook,
        Trade,
        GameMenu
    }
    public static UISection activeUISection = UISection.Default; 
    UISection prevUISection;  

    float navigationAngle = 89f; 

    // public GameObject tradeParent;
    // public GameObject gameMenuParent;  
    // public GameObject mapParent; 
    // public GameObject demoEndParent;

    //Buttons
    Dictionary<ButtonColors, ButtonVisual> buttons = new Dictionary<ButtonColors, ButtonVisual>();
    public Dictionary<ButtonColors, ButtonVisual> Buttons {
        get { return buttons;}
    }

    //Navigation
    IFocusUI selectedElement;
    bool hasSelectedElement = false;

    public enum UIElementType {
        InventoryObject,
        Slot,
        Button
    };
    
    public Vector2 virtualCursorPosition = Vector2.zero; 

    //Gameplay
    //public Image fadeOverlay;
    //public Camera sceneCamera;
    //public Transform channelingAnchor;
    //public GameObject progressBar;
    //public Slider slider;
    //public Text text;
    //public GameObject hintParent;
    //public Text hintText;
    //public GameObject hintParent2;
    //public Text hintText2;
    

    //Notifications  
    //public Text notificationText;

    //Items    
    public Transform itemInfoParent;
    public GameObject itemInfoPrefab;
    List<GameObject> infoItems = new List<GameObject>();
    float itemInfoWidth = 300f;
    float itemInfoGap = 50f;   

    GameDirector.GameState gameStateBeforeInvenotryOpened = GameDirector.GameState.World;

    //Resources
    bool resourceInfoActive = false;
    public GameObject resourceInfoParent;
    public Text resourceAmountText;
    public Image resourceIcon;
    int resourceAmount = 0;
    public int ResourceAmount { set { resourceAmount = value; }}
    Items.ItemName resourceName; 
    public Items.ItemName ResourceName { set { resourceName = value; }}

    //Game Menu
    public GameObject gameMenu;
    public GameObject loadOverlay;
    public GameObject endGameScreen;

    public Text qualityText;

    public void Init()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        gameDirector = GetComponent<GameDirector>();
        playerUI = GetComponent<PlayerUI>(); 
        // tradeScript = GetComponent<TradeManager>();
        // playerInput = player.GetComponent<PlayerInput>();
          
        // playerManager = player.GetComponent<PlayerManager>();
        // interactionManager = GetComponent<InteractionManager>();  
        // gameMenu = gameMenuParent.GetComponent<GameMenu>();    

        activeUISection = UISection.Default;
        prevUISection = activeUISection;
        InitButtons();

        resourceInfoParent.SetActive(false);
        gameMenu.SetActive(false);
        endGameScreen.SetActive(false);

        //gameMenuParent.SetActive(false);
        //mapParent.SetActive(false);
        //demoEndParent.SetActive(false);
        //SetChanneling(false);
        //notificationText.gameObject.SetActive(false);

        //fadeOverlay.gameObject.SetActive(false);      

         
    }    

    void Update() {

        if(resourceAmount > 0) {
            if(resourceInfoActive == false) {
                resourceInfoActive = true;

                resourceInfoParent.SetActive(true);
                resourceAmountText.text = resourceAmount.ToString();
                resourceIcon.sprite = Resources.Load<Sprite>("Icons/Items/" + resourceName.ToString());
            } else {
                resourceAmountText.text = resourceAmount.ToString();
            }
        } else {
            if(resourceInfoActive == true) {
                resourceInfoActive = false;
                resourceInfoParent.SetActive(false);
            }
        }        

    }  

    public void SetGameMenu() {
        if(gameMenu.activeSelf) {
            gameMenu.SetActive(false);
            GameDirector.gameState = GameDirector.GameState.World;
        }
        else {
            gameMenu.SetActive(true);

            GameDirector.gameState = GameDirector.GameState.UI;
            if(Director.inputType == Director.UI_InputType.Buttons) {
                NavigateUI();
            }
        }
    }

    public void SetEndGameScreen() {
        endGameScreen.SetActive(true);

        GameDirector.gameState = GameDirector.GameState.UI;
        if(Director.inputType == Director.UI_InputType.Buttons) {
            NavigateUI();
        }
    }


    public void OpenGameMenu() {

        // if(gameMenuParent.activeSelf) {
        //     gameMenuParent.SetActive(false);
        //     activeUISection = prevUISection;
        //     gameDirector.PauseTime(false);
        // } else {
        //     prevUISection = activeUISection;
        //     activeUISection = UISection.GameMenu;
        //     gameMenuParent.SetActive(true);
        //     //gameMenu.Init();
        //     gameDirector.PauseTime(true);
        // }
    } 

    

    //Called From PlayerIpnut, only place that open Inventory
    public void PlayerInventoryInput(int defaultSection = 0) {  
        
        if(activeUISection == UISection.Trade || activeUISection == UISection.RecipeBook || activeUISection == UISection.GameMenu) { return; }

        hasSelectedElement = false;
        playerUI.OpenInventory(defaultSection);        
        
        //Set State to Menu and preselect first element by default with NavigateUI, if there is some item in inventory
        if(playerUI.UiOpened) {

            gameStateBeforeInvenotryOpened = GameDirector.gameState;
            GameDirector.gameState = GameDirector.GameState.UI;
            //interactionManager.ShowSelectionMenu(false);

            if(Director.inputType == Director.UI_InputType.Buttons) {
                NavigateUI();
            }
        } 
        else {
            //gameDirector.gameState = GameDirector.GameState.World;  
            GameDirector.gameState = gameStateBeforeInvenotryOpened; 
            //interactionManager.CheckSelectionMenuVisibility();          
        }
    }

    //From interaction
    // public void OpenTrade(BaseData traderData) {

    //     activeUISection = UISection.Trade;
    //     tradeParent.SetActive(true);
    //     tradeScript.OpenTrade(traderData);

    //     //Set State to Menu and preselect first element by default with NavigateUI, if there is some item in inventory
    //     GameDirector.gameState = GameDirector.GameState.UI; 

    //     if(Director.inputType == Director.InputType.Keys) {
    //         NavigateUI();
    //     }
    // }

    // public void CloseTrade() {
        
    //     activeUISection = UISection.Default;
    //     tradeScript.CloseTrade();
    //     tradeParent.SetActive(false);
    //     GameDirector.gameState = GameDirector.GameState.World;
    //     hasSelectedElement = false;
    // }  

   


    #region Gameplay 

    

    public void DisplayItems(List<Items.ItemName> items){
        //Take all items, which player received and show them as UI        

        //To display items and thery amounts correctly, create dictionary with itemname and its amount ( ie 2x LavenderSprout, 1x MeadowTail)
        Dictionary<Items.ItemName, int> itemsDict = new Dictionary<Items.ItemName, int>();
        for(int i = 0; i < items.Count; i++) {
            if(itemsDict.ContainsKey(items[i])) {
                itemsDict[items[i]]++;
            } else {
                itemsDict.Add(items[i], 1);
            }
        }

        foreach(GameObject temp in infoItems) {
            Destroy(temp.gameObject);
        }
        infoItems.Clear();
        
        int index = 0;
        foreach(KeyValuePair<Items.ItemName, int> item in itemsDict) {           

            float totalWidth = (itemsDict.Count * itemInfoWidth) + ( (itemsDict.Count -1) * itemInfoGap);
            float posXStart = (totalWidth / -2f) + itemInfoWidth/2f;
            float posX = posXStart + (index*itemInfoWidth) + (index*itemInfoGap);           
           
            GameObject newInfoItem = (GameObject)Instantiate(itemInfoPrefab, Vector3.zero, Quaternion.identity);
            newInfoItem.transform.SetParent(itemInfoParent);
            newInfoItem.transform.localPosition = new Vector2(posX, 0);
            newInfoItem.transform.localScale = Vector3.one;
            newInfoItem.GetComponent<ItemInfo>().Init(item.Key, item.Value);
            index++;

            infoItems.Add(newInfoItem);
        }

        StartCoroutine(HideItems());
    }

    // public void DsiplayLostItems(List<Items.ItemName> items) {       
        
    //     if(items.Count > 1) {
    //         notificationText.text = items.Count.ToString() + " items lost";
    //     } else {
    //         notificationText.text = items.Count.ToString() + " item lost";
    //     }
        
    //     notificationText.gameObject.SetActive(true);

    //     StartCoroutine(DisplayLostItems(items));
    // }

    // IEnumerator DisplayLostItems(List<Items.ItemName> items) {

    //     for(int i = 0; i < items.Count; i++) {
            
    //         GameObject newInfoItem = (GameObject)Instantiate(itemInfoPrefab, Vector3.zero, Quaternion.identity);
    //         newInfoItem.transform.SetParent(itemInfoParent);
    //         newInfoItem.transform.localPosition = new Vector2(0, 0);
    //         newInfoItem.transform.localScale = Vector3.one;
    //         newInfoItem.GetComponent<ItemInfo>().Init(items[i], 1);

    //         yield return new WaitForSeconds(1.0f);

    //         Destroy(newInfoItem);
    //     }

    //     notificationText.gameObject.SetActive(false);
    // }

    IEnumerator HideItems() {
        yield return new WaitForSeconds(Director.displayItemTime);
        
        foreach(GameObject temp in infoItems) {
            Destroy(temp.gameObject);
        }
        infoItems.Clear();
    }


    // //fadeCompleted is a delegate to LoadNewArea, but it needs parameter node, it has to be received as other parameter
    // public void TriggerFade(bool fadeIn, FadeCompleted fadeCompleted, float holdTime = 0f, string fadeColor = "black") {   
    //     StartCoroutine(FadeCoroutine(fadeIn, fadeCompleted, holdTime, fadeColor));
    // }

    // IEnumerator FadeCoroutine(bool fadeIn, FadeCompleted fadeCompleted, float holdTime, string FadeColor) {
    //     float timer = 0;
    //     float fadeTimer = 0.3f;
    //     float alphaValue = fadeIn ? 0f : 1.0f;
    //     Color fadeColor = (FadeColor == "black") ? Color.black : Color.white;
    //     fadeOverlay.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alphaValue);
    //     fadeOverlay.gameObject.SetActive(true);

    //     while(timer < fadeTimer) {            
    //         timer += Time.deltaTime;

    //         if(fadeIn) {
    //             alphaValue = timer / fadeTimer;
    //         } else {
    //             alphaValue = 1.0f - (timer / fadeTimer);
    //         }            
    //         fadeOverlay.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alphaValue);

    //         yield return null;
    //     }

    //     fadeOverlay.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1.0f);

    //     //Keep the screen black for holdTime length
    //     if(holdTime > 0f) {
    //         yield return new WaitForSeconds(holdTime);
    //     }

    //     //There is no delegate, so dont trigger anything. Used for fading in
    //     if(fadeCompleted != null) {
    //     fadeCompleted();
    //     }

    //     if(!fadeIn) {
    //         fadeOverlay.gameObject.SetActive(false);
    //     }
    // }    

    // public void SetChanneling(bool on) {
    //     progressBar.SetActive(on);
    // }

    // public void UpdateChanneling(float timer, float channelTime, string textValue) {
    //     float percent = timer / channelTime;
    //     slider.value = percent;
    //     text.text = textValue;

    //     Vector3 viewPos = sceneCamera.WorldToScreenPoint(channelingAnchor.position);
    //     progressBar.transform.position = viewPos;
    // }    

    // public void ShowNarrator(string textValue, float? timeOn = null, float? timeDelay = null) {
        
    //     StartCoroutine(ShowNarratorCoroutine(textValue, timeOn, timeDelay));        
    // }

    // IEnumerator ShowNarratorCoroutine(string textValue, float? timeOn, float? timeDelay){
    //     float delay = 0f;
    //     if(timeDelay != null) { delay = (float)timeDelay; }

    //     yield return new WaitForSeconds(delay);

    //     hintParent.SetActive(true);
    //     hintText.text = textValue;

    //     if(timeOn != null) {
    //         float timeActive = (float)timeOn;
    //         StartCoroutine(HideNarratorCoroutine(timeActive));
    //     }
    // }

    // IEnumerator HideNarratorCoroutine(float timeActive){
    //     yield return new WaitForSeconds(timeActive);
    //     hintParent.SetActive(false);
    // }

    // public void ShowNarrator2(string textValue, float? timeOn = null, float? timeDelay = null) {
        
    //     StartCoroutine(ShowNarrator2Coroutine(textValue, timeOn, timeDelay));        
    // }

    // IEnumerator ShowNarrator2Coroutine(string textValue, float? timeOn, float? timeDelay){
    //     float delay = 0f;
    //     if(timeDelay != null) { delay = (float)timeDelay; }

    //     yield return new WaitForSeconds(delay);

    //     hintParent2.SetActive(true);
    //     hintText2.text = textValue;

    //     if(timeOn != null) {
    //         float timeActive = (float)timeOn;
    //         StartCoroutine(HideNarrator2Coroutine(timeActive));
    //     }
    // }

    // IEnumerator HideNarrator2Coroutine(float timeActive){
    //     yield return new WaitForSeconds(timeActive);
    //     hintParent2.SetActive(false);
    // }

    // public void HideNarrator() {
    //     hintParent.SetActive(false);
    //     hintParent2.SetActive(false);
    // }

    // public void ShowDemoEnd() {
    //     demoEndParent.SetActive(true);
    // }

    #endregion

    #region UI Navigation

    //When something gets close like Select Recipe menu and items get delted, it would otherwise keep here selectedElement altho it does not exist naymore
    public void DeselectElement() {
        hasSelectedElement = false;
        selectedElement = null;
    }

    //Select closest UI Element in input direction
    public void NavigateUI(Vector2? input = null) {            
     
        int selectedElementIndex = 0;
        GameObject[] elements = GameObject.FindGameObjectsWithTag("UIElement");       

        if(elements.Length > 0) {
            
            //There is no selected Element. For now select the one at most top left
            if(!hasSelectedElement) { 
                //Find the elemenet at top left by adding each element posX and posY. Whatever element has lowest sum should be at top left
                float sum = Mathf.Infinity;

                for(int i = 0; i < elements.Length; i++) {
                   
                    if(elements[i].GetComponent<IFocusUI>().CanBeFocused() == false) { continue; }

                    Vector2 pos = elements[i].GetComponent<IFocusUI>().GetPosition();
                    float elementSum = pos.x - pos.y;                  

                    //This element sum is lower, so its closer to the top left
                    if(elementSum < sum) {                        
                        sum = elementSum;
                        selectedElementIndex = i;
                        hasSelectedElement = true;                        
                    }
                }

                if(hasSelectedElement) {
                    selectedElement = elements[selectedElementIndex].GetComponent<IFocusUI>();
                    selectedElement.SetFocus();
                    //print(selectedElement.gameObject.name);
                }                

            } else {
                
                //There should always be input, but if there is not, set it to something, so its not null
                Vector2 inputVector = input == null ? Vector2.zero : (Vector2)input;
                Vector2 curPos = selectedElement.GetPosition(); 
                bool foundNextItem = false;
                int nextItemIndex = 0;
                float closestDistance = Mathf.Infinity;

                for(int i = 0; i < elements.Length; i++) {                

                    //Same lement in array as the one selected, skip it
                    if(elements[i] == selectedElement.gameObject) { continue; }
                    if(elements[i].GetComponent<IFocusUI>().CanBeFocused() == false) { continue; }

                    Vector2 nextElementPos = elements[i].GetComponent<IFocusUI>().GetPosition();
                    Vector2 direction = (nextElementPos - curPos).normalized;                    
                    float angle = Vector2.Angle(inputVector, direction);
                    float distance = Vector2.Distance(nextElementPos, curPos);

                    //Angle needs to affect distance, if angle is 0, ie same direction as input, distance is as is, but if the angle is higher, ie not direct angle, distance should get multiplied by angle
                    float index = Mathf.Abs(angle) / navigationAngle; //0 when same direction, 1 when at the max allowed off angle
                    distance += (distance * index);
                    
                    //print("Direction " + direction + " distance " + distance + " Angle " + angle);

                    //Angle should be 0 if the next item is in same direction as input
                    //Check if element is in correct direction based on input. If it is, then get the one, which is closest to this element.
                    if(Mathf.Abs(angle) < navigationAngle && distance < closestDistance) { //89
                        closestDistance = distance;
                        foundNextItem = true;
                        nextItemIndex = i;                        
                    }                    
                }

                if(foundNextItem) {
                    selectedElement.UnFocus();
                    selectedElement = elements[nextItemIndex].GetComponent<IFocusUI>();
                    selectedElement.SetFocus();
                    //print(selectedElement.gameObject.name);
                }
            }
        }

        if(hasSelectedElement) {
            //Selected Element can be Item, which is inside slot, so its localPositin is 0, always get slot position
            //Check if the Element has access to some ScrollArea, if it does, then it is inside one and notify the ScrollArea to adjust position of the Scroll Area
            if(selectedElement.GetScrollArea() != null) {
                Vector2 slotPos = selectedElement.GetLocalPosition();
                selectedElement.GetScrollArea().AdjustPosition(slotPos);
            }
        }
    }

    public void ActivateSelectedElement() {
        if(hasSelectedElement) {
            selectedElement.Activate();
        }
    }

    

    //Empty slot activated, after item was selected => Moving item to this new slot.     
    public void ItemMoved(ref InventoryObject invObj) {     
        if(invObj == null) { 
            return; 
        }
        
        //Deselect Slot        
        selectedElement.UnFocus();

        //Get Access to the Inventory item and make it the selectedElement      
        selectedElement = invObj.GetComponent<IFocusUI>();     
    }
  

    //When player switches from mouse to keyboard or joystick
    public void SwitchedInput() {

        if(Director.inputType == Director.UI_InputType.Mouse) {
            if(hasSelectedElement && selectedElement.gameObject != null) {             
                selectedElement.UnFocus();
            }
            hasSelectedElement = false;
            Cursor.visible = true;

        } else {
            Cursor.visible = false;

            //Unfocus all elements
            GameObject[] elements = GameObject.FindGameObjectsWithTag("UIElement");
            foreach(GameObject temp in elements) {
                temp.GetComponent<IFocusUI>().UnFocus();
            }
            hasSelectedElement = false;
        }
    }

    #endregion


     void InitButtons() {

        //Green
        ButtonVisual buttonGreen = new ButtonVisual();
        buttonGreen.buttonColorType = ButtonColors.Green;
        buttonGreen.buttonSprite =  Resources.Load<Sprite>("UI/Buttons/ButtonGreen");
        buttonGreen.buttonColor = Color.white;
        buttonGreen.buttonTextColor = new Color(28f/255f, 122f/255f, 48f/255f, 1f);

        //Purple
        ButtonVisual buttonPurple = new ButtonVisual();
        buttonPurple.buttonColorType = ButtonColors.Purple;
        buttonPurple.buttonSprite =  Resources.Load<Sprite>("UI/Buttons/ButtonPurple");
        buttonPurple.buttonColor = Color.white;
        buttonPurple.buttonTextColor = new Color(78f/255f, 23f/255f, 118f/255f, 1f);

        //Red
        ButtonVisual buttonRed = new ButtonVisual();
        buttonRed.buttonColorType = ButtonColors.Red;
        buttonRed.buttonSprite =  Resources.Load<Sprite>("UI/Buttons/ButtonRed");
        buttonRed.buttonColor = Color.white;
        buttonRed.buttonTextColor = new Color(131f/255f, 23f/255f, 32f/255f, 1f);

        //Grey
        ButtonVisual buttonGrey = new ButtonVisual();
        buttonGrey.buttonColorType = ButtonColors.Grey;
        buttonGrey.buttonSprite =  Resources.Load<Sprite>("UI/Buttons/ButtonGrey");     
        buttonGrey.buttonColor = new Color(1,1,1, 150f/255f);
        buttonGrey.buttonTextColor = new Color(106f/255f, 106f/255f, 106f/255f, 150f/255f);

        buttons.Add(ButtonColors.Green, buttonGreen);
        buttons.Add(ButtonColors.Purple, buttonPurple);
        buttons.Add(ButtonColors.Red, buttonRed);
        buttons.Add(ButtonColors.Grey, buttonGrey);
    }

    public void GoToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit() {
        Application.Quit();
    }

    public void SetQualityMedium() {
        qualityText.text = "QUALITY MEDIUM";
        Director.quality = "medium";
        GameObject.FindGameObjectWithTag("Area").GetComponent<Area>().SetQuality();        
    }

    public void SetQualityHigh() {
        qualityText.text = "QUALITY HIGH";
        Director.quality = "high";
        GameObject.FindGameObjectWithTag("Area").GetComponent<Area>().SetQuality();        
    }

}

public enum ButtonColors {
    Green,
    Purple,
    Red,
    Grey
}

public class ButtonVisual {
    
    public ButtonColors buttonColorType;
    public Sprite buttonSprite;

    public Color buttonColor;
    public Color buttonTextColor;   
}

