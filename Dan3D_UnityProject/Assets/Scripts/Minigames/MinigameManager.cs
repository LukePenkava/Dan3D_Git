using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinigameManager : MonoBehaviour
{
    public delegate void MinigameDelegate(bool result);
    public MinigameDelegate mgDelegate;

    public GameObject minigameParent;

    public Image bg;
    public Image hitArea;
    public Image indicator;

    float bgWidth = 600f;
    float hitAreaMin = 60f;
    float hitAreaMax = 120f;
    float indicatorWidth = 20f;

    float hitAreaSuccessMin = 0f;
    float hitAreaSuccessMax = 0f;

    bool mgOn = false;
    float speed = 500f;

    void Start() {
        minigameParent.SetActive(false);
    }

    // Start is called before the first frame update
    public void Init()
    {
        bg.rectTransform.sizeDelta = new Vector2(bgWidth, bg.rectTransform.sizeDelta.y);

        float hitAreaWidth = Random.Range(hitAreaMin, hitAreaMax);
        hitArea.rectTransform.sizeDelta = new Vector2(hitAreaWidth, hitArea.rectTransform.sizeDelta.y);
        float minPos = hitAreaWidth/2f;
        float maxPos = bgWidth - hitAreaWidth/2f;
        float pos = Random.Range(minPos, maxPos) - bgWidth/2f; //minus bgWidth because center position is zero
        hitArea.transform.localPosition = new Vector3(pos, 0, 0);
        hitAreaSuccessMin = hitArea.transform.localPosition.x - hitAreaWidth/2f;
        hitAreaSuccessMax = hitArea.transform.localPosition.x + hitAreaWidth/2f;

        indicator.rectTransform.sizeDelta = new Vector2(indicatorWidth, indicator.rectTransform.sizeDelta.y);
        indicator.transform.localPosition = new Vector3(bgWidth/2f, 0, 0);

        minigameParent.SetActive(true);

        mgOn = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(mgOn) {
            indicator.transform.localPosition = new Vector3(indicator.transform.localPosition.x - (Time.deltaTime * speed), 0, 0);

            if(indicator.transform.localPosition.x <= (-bgWidth/2f)) {
                 indicator.transform.localPosition = new Vector3(-bgWidth/2f, 0, 0);

                mgDelegate(true);
                mgOn = false;
            }
        }
    }

    public void StopIndicator() 
    {
        mgOn = false;

        float pos = indicator.transform.localPosition.x;
        if(pos > hitAreaSuccessMin && pos < hitAreaSuccessMax) {
            mgDelegate(true);
        } else {
            mgDelegate(false);
        }

        minigameParent.SetActive(false);
    }
}
