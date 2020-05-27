using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifferentEnemy : MonoBehaviour
{
    public Rigidbody2D second_enemy;
  
    public float VerticalSpeed = 150f;

    // Start is called before the first frame update
    void Start()
    {
        second_enemy.velocity = new Vector2(0, -VerticalSpeed * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        WalkY();
        
        
    }

    void WalkY()
    {
        
        if(this.transform.position.y >= 3.1f)
        {
            second_enemy.velocity = new Vector2(-130f * Time.deltaTime, -VerticalSpeed * Time.deltaTime);
        }
        else if(this.transform.position.y <= -1.7)
        {
            second_enemy.velocity = new Vector2(-130f * Time.deltaTime, VerticalSpeed * Time.deltaTime);
        }
    }
   

}
