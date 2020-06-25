using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float life;
    public RawImage vinheta;

    void Start()
    {
        vinheta = GameObject.Find("Vinheta").GetComponent<RawImage>();
        life = 20;
    }

    // Update is called once per frame
    void Update()
    {
        if(life < 1)
        {
            GameEvents.Current.PlayerLost();
        }
    }

    public void TakeDamage()
    {
        life -= 1;
        StartCoroutine(Blood());
    }

    private IEnumerator Blood()
    {
        vinheta.enabled = true;
        yield return new WaitForSeconds(1f);
        vinheta.enabled = false;
    }
}
