using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;


public class LightSaber : MonoBehaviour
{
    int rotationZ;
    bool red;
    public int points;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        rotationZ = System.Convert.ToInt32(GetComponent<SerialController>().ReadSerialMessage());
        transform.eulerAngles = new Vector3(0, 0, -rotationZ + 85);
        Debug.Log(rotationZ.ToString() + "DALEEE");
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * 100 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * 100 * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Shot")
        {
            if (other.gameObject.GetComponent<Shot>().red == red)
            {
                Destroy(other.gameObject);
                points++;
            }
        }
    }
}
