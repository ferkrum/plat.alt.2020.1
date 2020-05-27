using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    Text pointsHUD;
    public GameObject lightSaber;
    int points;
    // Start is called before the first frame update
    void Start()
    {
        pointsHUD = GameObject.Find("Points").GetComponent<Text>();
        points = lightSaber.GetComponent<LightSaber>().points;
    }

    // Update is called once per frame
    void Update()
    {
        pointsHUD.text = points.ToString();
    }
}
