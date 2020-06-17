using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeEnemyMenu : MonoBehaviour
{
    public float travelSpeed = 3.0f;


    public Transform[] stop;
    
    [SerializeField]
    private float timeWaiting;

    public float waitTime;

    private int randomPoint;
    bool mustStop;
    private Vector3 rotationValue;

    private Vector3 initialPosition;
    void Start()
    {
        initialPosition = this.transform.position;
        mustStop = false;
        rotationValue = new Vector3(travelSpeed*Time.deltaTime, 0f, 0f);
        timeWaiting = waitTime;
        randomPoint = Random.Range(0, stop.Length);
    }

    // Update is called once per frame
    void Update()
    {
        if(mustStop == false)
            this.transform.Translate(-rotationValue, Space.World);
        if(Vector2.Distance(this.transform.position, stop[randomPoint].transform.position)<= 0.2f)
        {
            mustStop = true;
            StartCoroutine(BackWalking());
        }
        if(this.transform.position.x <= -14)
        {
            this.transform.position = initialPosition;
            randomPoint = Random.Range(0, stop.Length);
        }
    }

    IEnumerator BackWalking()
    {
        yield return new WaitForSeconds(2);
        mustStop = false;
    }
}
/*     this.transform.position = Vector2.MoveTowards(transform.position, stopPoints[randomPoint].position, travelSpeed * Time.deltaTime);

        
        if (Vector2.Distance(transform.position,stopPoints[randomPoint].position) <= 0.2f)
        {
            if(timeWaiting <= 0)
            {
                randomPoint = Random.Range(0, stopPoints.Length);
                timeWaiting = waitTime;
                this.transform.LookAt(stopPoints[randomPoint].transform.position);
            }
            else
            {
                timeWaiting -= Time.deltaTime;
                //this.transform.LookAt(stopPoints[randomPoint].transform.position);
            }
        }
        */
