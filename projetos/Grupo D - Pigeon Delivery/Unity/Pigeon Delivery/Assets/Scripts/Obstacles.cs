using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour
{

    public Rigidbody2D obstacleRB;
    public float Speed = 130f;

    
    public float MAX_HEIGHT;
    public float MIN_HEIGHT;
    public float MAX_WIDTH;
    public float MIN_WIDTH;


    private float _randX;
    private float _randY;

    private GameObject _obstacleGO;
 

    // Start is called before the first frame update
    void Start()
    {
        _obstacleGO = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        MakeThemWalk();
    }

    void MakeThemWalk()
    {
        obstacleRB.velocity = new Vector2(-Speed * Time.deltaTime, 0);
    }

    void ChangePosition()
    {
        _randX = Random.Range(MIN_WIDTH, MAX_WIDTH);
        _randY = Random.Range(MIN_HEIGHT, MAX_HEIGHT);

        _obstacleGO.transform.position = new Vector3(_randX, _randY, 0f);
    }

    void OnTriggerEnter2D(Collider2D _col)
    {
        ChangePosition();
    }

}
