using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSelfDestroy : MonoBehaviour
{
    public float timer = 3.0f;

    void Awake()
    {
        StartCoroutine(SelfDestroy());
    }

    public void Collected() {
        Destroy(this.gameObject);
    }

    IEnumerator SelfDestroy() {
        yield return new WaitForSeconds(timer);
        Destroy(this.gameObject);
        GameObject.FindGameObjectWithTag("Managers").GetComponent<InteractionManager>().RemoveActiveInteraction();
    }

    
}
