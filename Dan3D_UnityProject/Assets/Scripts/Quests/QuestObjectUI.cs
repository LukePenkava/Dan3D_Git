using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class QuestObjectUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IFocusUI
{
    QuestsUI questsUIManager;
    QuestObject questObj;   
    public Image focus;   
    public Text title;
    public Text state;
    UIScrollArea scrollArea;   

    public Sprite bg_Selected;
    public Sprite bg_Normal;

    Color textColor_Active = new Color(128f/255f, 99f/255f, 83f/255f, 1f);
    Color textColor_Disabled = new Color(150f/255f, 150f/255f, 150f/255f, 1f);
    Color bgColor_Active = new Color(159f/255f, 138f/255f, 112f/255f, 1f);
    Color bgColor_Disabled = new Color(160f/255f, 160f/255f, 160f/255f, 1f);
    Color focusColor_Active = new Color(222f/255f, 202f/255f, 165f/255f, 1f);
    Color focusColor_Disabled = new Color(220f/255f, 220f/255f, 220f/255f, 1f);

    public void Init(QuestObject Quest, QuestsUI QuestsUIManager, UIScrollArea ScrollArea, float width, float height, Character_BaseData playerBaseData) {

        //thisImg = this.GetComponent<Image>();
        questsUIManager = QuestsUIManager;
        questObj = Quest;
        scrollArea = ScrollArea;
        UnFocus();
        //Set size of this element. Child elements are based on this element;
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

        title.text = questObj.title;
        state.text = questObj.state.ToString();      
    }

    public void OnPointerClick(PointerEventData pointerEventData) {
        if(Director.inputType != Director.UI_InputType.Mouse) { return; }

        Activate();
    }

    public void OnPointerEnter(PointerEventData pointerEventData) {       
        if(Director.inputType != Director.UI_InputType.Mouse) { return; }
      
        SetFocus();    
    }

    public void OnPointerExit(PointerEventData pointerEventData) {
       if(Director.inputType != Director.UI_InputType.Mouse) { return; }

        UnFocus();     
    }

    public void Activate() {
        
        // if(canBeCrafted && UIManager.activeUISection == UIManager.UISection.RecipeBook) {
        //     GameObject.FindGameObjectWithTag("Managers").GetComponent<CraftingManager>().InitCrafting(recipe.recipeItem);
        // }
    }

    public void SetFocus() {        
        //focus.color = canBeCrafted ? focusColor_Active : focusColor_Disabled;
        //thisImg.sprite = bg_Selected;
        focus.enabled = true;    
        questsUIManager.Focused(questObj);    
    }

    public void UnFocus() {
        focus.enabled = false;
        //thisImg.sprite = bg_Normal;
    }

    public bool CanBeFocused() {
        return true;
    }

    public Vector2 GetPosition() {
        return new Vector2(this.transform.position.x, this.transform.position.y);
    }

    public Vector2 GetLocalPosition() {
        return new Vector2(this.transform.localPosition.x, this.transform.localPosition.y);
    }

    public UIScrollArea GetScrollArea() {
        return scrollArea;
    }
}
