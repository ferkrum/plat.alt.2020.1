using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    public float velocity, timer;
    bool launch;
    public bool red;
    // Start is called before the first frame update
    void Start()
    {
        red = true;
        launch = false;
        GetComponent<Rigidbody>().useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > 1)
        {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().AddForce((transform.forward + new Vector3(0,.2f,0)) * velocity);
            launch = false;
        }
        if (timer > 4)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag ("Player"))
        {
            collision.gameObject.GetComponent<Player>().life-= 1;
            Destroy(gameObject);
        }
      
        if (collision.gameObject.CompareTag ("Floor"))
        {
           
            Destroy(gameObject);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Saber"))
    //    {

    //        Destroy(gameObject);
    //    }
    //}
}
