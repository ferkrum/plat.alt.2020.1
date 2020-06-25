using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooteer : MonoBehaviour
{
    float timer, timerRandom;
    public GameObject target, shoot;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        timerRandom = Random.Range(2, 5);
    }

    // Update is called once per frame
    void Update()
    {
        if (target.transform.position.y > gameObject.transform.position.y   && target.transform.position.y < gameObject.transform.position.y + 2)
        {
            ChargeShoot();
        }
    }

    void ChargeShoot()
    {
        
        timer += Time.deltaTime;
        if (timer > timerRandom)
        {
            Instantiate(shoot, gameObject.transform.position, gameObject.transform.rotation);
            timer = 0;
        }

    }
}
