using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO.Ports;

public class ArduinoController : MonoBehaviour
{
    public float Speed;
    public static bool isOver = false;
    private Vector3 dir;
    int dataAux = 0;
    public GameObject textElement;

    public GameObject bElement;
    private string[] lineInfo;
    private char[] splitter = { ':', ' ' };
    private int valueOnY;
    private int valueOnX;
    SerialPort sp = new SerialPort("COM3",9600);

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        bElement.SetActive(false);
        textElement.SetActive(false);
        sp.Open();
        sp.ReadTimeout = 1;
        isOver = false;
        dir = new Vector3(0, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (sp.IsOpen)
        {
            try
            {
                if(isOver != true)
                {
                    
                    //GetData(sp.ReadLine());
                    string line = sp.ReadLine();
                    //Debug.Log(line);
                    lineInfo = line.Split(':');
                    if(lineInfo[0] == "Y")
                    {
                        valueOnY = int.Parse(lineInfo[1]);
                        //Debug.Log("Valor Y: " + valueOnY.ToString());
                        PigeonMotorY(valueOnY);
                    }   
                    else if(lineInfo[0] == "X")
                    {
                        valueOnX = int.Parse(lineInfo[1]);
                        //Debug.Log("Valor X: " + valueOnX);
                        PigeonMotorX(valueOnX);
                    }
                    //for (int i =0; i < lineInfo.Length; i++)
                    //{
                    //    if(lineInfo[i] == "Y")
                    //    {
                            
                    //    }
                    //    if(lineInfo[i] == "X")
                    //    {
                    //        valueOnX = int.Parse(lineInfo[i + 1]);
                    //        //Debug.Log("Valor X: " + valueOnX);
                    //        //PigeonMotorX(valueOnX);
                    //    }
                    //}
                    
                }
                else
                {
                    textElement.SetActive(true);
                    bElement.SetActive(true);
                    Time.timeScale = 0f;
                    sp.Close();
                }
                
            }
            catch(System.Exception)
            {

            }
        }
    }
    void PigeonMotorY(int _data)
    {
        this.transform.position = new Vector3(this.transform.position.x, (Speed * ((_data) * 2) * Time.deltaTime), this.transform.position.z);
    }
    void PigeonMotorX(int _data)
    {
        this.transform.position = new Vector3((Speed * ((_data) * 2) * Time.deltaTime), this.transform.position.y, this.transform.position.z);
    }
    void OnCollisionEnter2D(Collision2D _col)
    {
        if(_col.collider.tag == "Obstacles")
        {
            isOver = true;
            Debug.Log("DEAD");
        }
    }

    public void ButtonTest()
    {
        SceneManager.LoadScene("GameScene");   
    }
}
