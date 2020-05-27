using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    //SO NOTÍCIA BOA EM RELAÇÃO AO VÔ!

    public GameObject cam;
    public float intensity;
    

    private float lenght;
    private float startPos;
    // Start is called before the first frame update

    void Start()
    {
        startPos = transform.position.x;
        if (!GetComponent<SpriteRenderer>())
        {
            return;
        }
        else
        {
            lenght = GetComponent<SpriteRenderer>().bounds.size.x;
        }
        

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float temp = cam.transform.position.x * (1 - intensity);
        float dist = (cam.transform.position.x * intensity);
        transform.position = new Vector3(startPos+dist,transform.position.y,transform.position.z);
        if (temp > startPos + lenght) startPos += lenght;
        else if (temp < startPos - lenght) startPos -= lenght;
    }
}
