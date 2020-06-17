using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO.Ports;

public class ArduinoCMenu : MonoBehaviour
{
    public float Speed;
    public static bool isOver = false;
    private Vector3 dir;
    int dataAux = 0;

    private string[] lineInfo;
    private char[] splitter = { ':', ' ' };

    private int valueOnX;
    SerialPort sp = new SerialPort("COM3", 1200);

    public Transform firePoint;
    public GameObject BY, BR, BB;
    private GameObject instanceCopy;
    public GameObject crateHolder;

    [SerializeField]
    private int randomCrate;

    private bool crateTime;
    public static bool sendState;
    public int crateSpawnTime = 5;
    bool canread;
    bool canAskSound;
    // Start is called before the first frame update
    void Start()
    {
        crateTime = true;
        randomCrate = Random.Range(0, 2);
        Time.timeScale = 1.0f;
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
                if (isOver != true)
                {

                    if (crateTime == true)
                    {
                        //TIME TO SPAWN CRATE!
                        crateSpawnTime = Random.Range(2, 8);
                        StartCoroutine(WaitToSpawn());
                        crateTime = false;
                    }
                    if(canAskSound == true)
                    {
                        AskForSoundData();
                    }
                    AskForData();
                    if (canread == true)
                    {
                        string line = sp.ReadLine(); // reads the line
                       
                        if(line == "S")
                        {
                            FreeCrate();
                            canAskSound = false;
                            crateTime = true;
                        }
                        else
                        {
                            valueOnX = int.Parse(line);

                            Debug.Log(valueOnX);
                            //Debug.Log(valueOnX);

                            PigeonMotor(valueOnX);
                            // PigeonMotorX(valueOnX);
                            canread = false;
                        }
                        
                    }

                }
                else
                {
                   
                    Time.timeScale = 0f;
                    sp.Close();
                }

            }
            catch (System.Exception)
            {

            }
        }
    }
    void PigeonMotor(int _data)
    {
        Vector3 immediatePostion = new Vector3((float)_data, 0.0f, 0.0f);
        transform.position = Vector3.Lerp(immediatePostion, this.transform.position, 0.75f);
        
    }
    void FreeCrate()
    {
        instanceCopy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        instanceCopy.transform.parent = crateHolder.transform;
    }
    void SpawnCrate()
    {

        CrateToSpawn();
        if (randomCrate == 0 || randomCrate == 1)
        {
            instanceCopy = Instantiate(BY, firePoint.position, firePoint.rotation);
        }
        else if (randomCrate == 3 || randomCrate == 4 || randomCrate == 5)
        {
            instanceCopy = Instantiate(BR, firePoint.position, firePoint.rotation);
        }
        else if (randomCrate == 6 || randomCrate == 7 || randomCrate == 8 || randomCrate == 9)
        {
            instanceCopy = Instantiate(BB, firePoint.position, firePoint.rotation);
        }
        instanceCopy.transform.parent = this.gameObject.transform;
        instanceCopy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        canAskSound = true;



    }
    void CrateToSpawn()
    {
        randomCrate = Random.Range(0, 10);
        Debug.Log(randomCrate);
    }
    void OnCollisionEnter2D(Collision2D _col)
    {
        if (_col.collider.tag == "Obstacles")
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
    void AskForData()
    {
        sp.WriteLine("X");
        canread = true;
    }
    void AskForSoundData()
    {
        sp.WriteLine("C");
        
    }
}
