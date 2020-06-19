using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;


public class LightSaber : MonoBehaviour
{
    int rotationZ;
    string arduino;
    string[] arduinoMessages;
    
    public bool red, button;
    public int points;

    private int _button;
    private float _potenciometer;
    private float _leftMax = 0, _rightMax = 360;
    private bool right;
    public bool _isAdjusted;
    // Start is called before the first frame update
    void Start()
    {
        _isAdjusted = false;
        right = true;
        button = false;
        red = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        char[] splitChar = { ' ' };
        arduino = GetComponent<SerialController>().ReadSerialMessage();
        rotationZ = System.Convert.ToInt32(GetComponent<SerialController>().ReadSerialMessage());
        arduinoMessages = arduino.Split(splitChar);

        //Debug.Log(arduinoMessages[1] + "POTENCIOMETRO");
       // Debug.Log( "botao" + arduinoMessages[0]);

        _potenciometer = System.Convert.ToInt32(arduinoMessages[1]) - 90;
        
        if (_isAdjusted)
        {
            float angle = Mathf.Lerp(_leftMax, _rightMax, _potenciometer);
            transform.eulerAngles = new Vector3(0, 0, angle);
            Debug.Log(angle + "DALEEE");
        }
        else
        {
            
            transform.eulerAngles = new Vector3(0, 0,_potenciometer );
            adjustSensibility();
        }
       
        _button = System.Convert.ToInt32(arduinoMessages[0]);
        if (_button == 1)
        {
            red = false;
        }
        else
        {
            red = true;

        }


        
        //if (Input.GetKey(KeyCode.A))
        //{
        //    transform.Rotate(Vector3.forward * 100 * Time.deltaTime);
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    transform.Rotate(-Vector3.forward * 100 * Time.deltaTime);
        //}
       
    }

    public void adjustSensibility()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (right)
            {
                _rightMax = _potenciometer;
                right = false;
            }
            else
            {
                _leftMax = _potenciometer;
                right = true;
                _isAdjusted = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Shot")
        {
            
           
            if (other.gameObject.GetComponent<Shot>().red == red)
            {
                Debug.Log("COLIDIU BOLINHA");
                points++;
                Destroy(other.gameObject);
                
            }
        }
    }
}
