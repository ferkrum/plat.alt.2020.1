using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shot : MonoBehaviour
{
    public float velocity, timer;
    bool launch;
    public bool red;
    public AudioSource shotSound;
    // Start is called before the first frame update
    void Start()
    {
        // transform.eulerAngles = new Vector3(-Random.Range(95,105) - 90, -100, 0);
        transform.eulerAngles += new Vector3(-100, 0, 0);
      
        launch = true;
        GetComponent<Rigidbody>().useGravity = false;
        if (transform.rotation.eulerAngles.y < 0)
        {
            transform.eulerAngles += new Vector3(0, 180, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        timer += Time.deltaTime;
        if(timer > 0)
        {
            GetComponent<Rigidbody>().useGravity = false;
            if(launch == true)
            {
                shotSound.Play();
            }
            GetComponent<Rigidbody>().AddForce((transform.forward + new Vector3(0, .2f, 0)) * velocity);
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
            collision.gameObject.GetComponent<Player>().TakeDamage();
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
