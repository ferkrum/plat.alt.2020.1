using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooteer : MonoBehaviour
{
    float timer;
    public GameObject target, shoot;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (target.transform.position.y > gameObject.transform.position.y - 5 && target.transform.position.y < gameObject.transform.position.y + 5)
        {
            ChargeShoot();
        }
    }

    void ChargeShoot()
    {
        timer += Time.deltaTime;
        if (timer > 2)
        {
            Instantiate(shoot, gameObject.transform.position, gameObject.transform.rotation);
            timer = 0;
        }

    }
}
