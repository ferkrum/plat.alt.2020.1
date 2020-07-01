using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpPlataform : MonoBehaviour
{
    private float _speed;
    public GameObject Saber;

    void Start()
    {
        _speed = 0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        
        gameObject.transform.position += new Vector3(0, _speed, 0) * Time.deltaTime;
        if(Saber!= null)
        {
            Saber.transform.position += new Vector3(0, _speed, 0) * Time.deltaTime;
        }
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "UpPlataform")
        {
            _speed = 0.4f;
        }
        if (other.gameObject.tag == "DownPlataform")
        {
            _speed = 0.05f;
        }
    }
}
