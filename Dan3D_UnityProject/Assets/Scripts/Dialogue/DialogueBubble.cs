using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBubble : MonoBehaviour
{
    DialogueManager dialogueManager;

    public Text bubbleText;
    Dialogue.Types dialogueType;

    //Typing
    float typeInterval_Normal = 0.025f;
    float typeInterval_HoldLong = 0.65f;
    float typeInterval_HoldShort = 0.45f;

    bool overrideTyping = false;
    float typeIntervalOverride = 0.005f;

    public GameObject completedVisual;

    //Text Length
    public Font font;
    public Canvas canvas;


    void Start()
    {
        dialogueManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<DialogueManager>();
    }

    public void SetText(string text, Dialogue.Types type)
    {
        dialogueType = type;
        overrideTyping = false;
        bubbleText.text = "";
        completedVisual.SetActive(false);

        StopAllCoroutines();
        StartCoroutine(TypeText(text));
    }

    public void Finish()
    {
        overrideTyping = true;
    }

    //Type text symbol after symbol. For better readability scale bubble up to size of a first line based on maximum size
    IEnumerator TypeText(string finalText)
    {
        //Index of currently typed symbol
        int typeIndex = 0;
        //Interval between typing symbols
        float timer = 0f;
        //Create break lines in the text based on the max bubble size. This text contains text with custom break lines for a given size of a bubble
        string formattedText = finalText; //FormatText(finalText);      
        //Set to Default interval
        float typeInterval = typeInterval_Normal;

        //Go through all symbols one by one
        while (typeIndex <= (formattedText.Length - 1))
        {

            timer += Time.deltaTime;
            //Because typing is based on deltatime, there might have to be more symbols typed in one frame. this says how many.
            int indexStep = 0;

            //Type next symbol afer the interval elapsed
            if (timer >= typeInterval)
            {

                //Find out how many steps/symbols to type in this frame
                indexStep = Mathf.FloorToInt(timer / typeInterval);

                timer = 0;

                //Reached end of the text, make sure the indexStep does not go over maximum of the full text ( typeIndex + indexStep cant be more than actaul length )
                if ((typeIndex + indexStep) >= formattedText.Length)
                {
                    indexStep = (formattedText.Length) - typeIndex;
                }

                //This is actual text, that will be typed, substring from final text. ie these are only the new symbols
                string newText = formattedText.Substring(typeIndex, indexStep);
                //See if the new text contains special symbol, if it does, make a pause
                SpecialSymbol symbolStruct = ContainsSpecialSymbol(newText);
                //-1 means there is no special symbol >= 0 means there is special symbol
                if (symbolStruct.index >= 0)
                {
                    //Pause at the special symbol, not after it
                    newText = newText.Substring(0, symbolStruct.index);
                    //Change the stepIndex to be at special symbol
                    indexStep = symbolStruct.index + 1;
                    //Wait for correct amount of time, "," has shorter interval than for example "."
                    typeInterval = symbolStruct.interval;
                    if (overrideTyping) { typeInterval = typeIntervalOverride; }
                }
                else
                {
                    //No special symbol in new text, use regular interval
                    typeInterval = typeInterval_Normal;
                    if (overrideTyping) { typeInterval = typeIntervalOverride; }
                }

                //Set shown text in the bubble to start at begging of finaltext to current index + indexStep
                string typedText = formattedText.Substring(0, typeIndex + indexStep);
                bubbleText.text = typedText;

                //Set current typed Index
                typeIndex = typeIndex + indexStep;
            }

            yield return null;
        }

        //Finished Typing
        dialogueManager.IsTyping = false;

        if (dialogueType == Dialogue.Types.Locked)
        {
            completedVisual.SetActive(true);
        }
    }

    //Check if this text contains any special symbol to trigger longer pause in interval
    SpecialSymbol ContainsSpecialSymbol(string text)
    {
        //No special symbol found
        int symbolIndex = -1;
        string[] symbols = new string[] { ".", "?", ",", "!" };

        foreach (string symbol in symbols)
        {

            bool contains = text.Contains(symbol);
            if (contains)
            {
                //If special symbol found, store its index and set correct interval for use by TypeText
                symbolIndex = text.IndexOf(symbol);
                float interval = (symbol == "," ? typeInterval_HoldShort : typeInterval_HoldLong);
                SpecialSymbol symbolStruct = new SpecialSymbol(symbolIndex, interval);
                return symbolStruct;
            }
        }

        return new SpecialSymbol(symbolIndex, 0f);
    }
}

//Used to store index of special symbol like "." or "," and what interval time should be used
struct SpecialSymbol
{
    public int index;
    public float interval;

    public SpecialSymbol(int Index, float Interval)
    {
        index = Index;
        interval = Interval;
    }
}
