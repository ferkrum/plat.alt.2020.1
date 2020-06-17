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
    SerialPort sp = new SerialPort("COM3", 9600);

    public Transform firePoint;
    public GameObject BY, BR, BB;
    private GameObject instanceCopy;
    public GameObject crateHolder;

    [SerializeField]
    private int randomCrate;

    private bool crateTime;
    public static bool sendState;
    public int crateSpawnTime=5;
    bool canread;
   
    // Start is called before the first frame update
    void Start()
    {
        crateTime = true;
        randomCrate = Random.Range(0, 2);
        Time.timeScale = 1.0f;
        bElement.SetActive(false);
        textElement.SetActive(false);
        sp.Open();
        sp.ReadTimeout = 2;
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

                    if (crateTime == true)
                    {
                        //TIME TO SPAWN CRATE!
                        crateSpawnTime = Random.Range(2, 8);
                        StartCoroutine(WaitToSpawn());
                        crateTime = false;
                    }

                    string line = sp.ReadLine();
                    Debug.Log(line);// reads the line
                    lineInfo = line.Split(':'); // divide the string when ":" is found
                    Debug.Log(lineInfo[0]);

                    //check what kind of input was recived
                    if (lineInfo[0] == "Y")
                    {
                        valueOnY = int.Parse(lineInfo[1]);
                        PigeonMotor(valueOnY);
                    }
                    else if (lineInfo[0] == "X")
                    {
                        valueOnX = int.Parse(lineInfo[1]);
                        PigeonMotorX(valueOnX);
                    }
                    else if (lineInfo[0] == "S" && crateTime == false)
                    {
                        FreeCrate();
                       
                        Debug.Log("DROP");
                    }

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
    void PigeonMotor(int _data)
    {
        Vector3 immediatePostion = new Vector3(0.0f, (float)_data , 0.0f);
        transform.position = Vector3.Lerp(transform.position, immediatePostion, 0.75f);
        // this.transform.position = new Vector3(this.transform.position.x, ((int)Speed * (int)_data)/2, this.transform.position.z);
    }
    void PigeonMotorX(int _data)
    {
        this.transform.position = new Vector3(((int)Speed * (int)_data)/2, this.transform.position.y, this.transform.position.z);
    }
    void FreeCrate()
    {
        instanceCopy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        instanceCopy.transform.parent = crateHolder.transform;
        crateTime = true;
        Debug.Log("ENTREI AKI");
    }
    void SpawnCrate()
    {

        CrateToSpawn();
        if(randomCrate == 0 || randomCrate == 1)
        {
            instanceCopy = Instantiate(BY, firePoint.position, firePoint.rotation);
        }
        else if(randomCrate == 3 || randomCrate == 4 || randomCrate == 5)
        {
            instanceCopy = Instantiate(BR, firePoint.position, firePoint.rotation);
        }
        else if(randomCrate == 6 || randomCrate == 7 || randomCrate == 8 || randomCrate == 9)
        {
            instanceCopy = Instantiate(BB, firePoint.position, firePoint.rotation);
        }
        instanceCopy.transform.parent = this.gameObject.transform;
        instanceCopy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;



    }
    void CrateToSpawn()
    {
        randomCrate = Random.Range(0, 10);
        Debug.Log(randomCrate);
    }
    void OnCollisionEnter2D(Collision2D _col)
    {
        if(_col.collider.tag == "Obstacles")
        {
            isOver = true;
            Debug.Log("DEAD");
        }
    }
 
    IEnumerator WaitToSpawn()
    {
        Debug.Log("I am here");
        yield return new WaitForSeconds(crateSpawnTime);
        SpawnCrate();
    }
    IEnumerator AskForData()
    {
        yield return new WaitForEndOfFrame();
        sp.WriteLine("A");
        canread = true;
    }
}
