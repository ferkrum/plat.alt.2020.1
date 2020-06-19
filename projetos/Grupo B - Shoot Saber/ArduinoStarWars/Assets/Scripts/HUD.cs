using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    Text pointsHUD;
    public GameObject lightSaber, player;
    int points;
    RawImage life;
    // Start is called before the first frame update
    void Start()
    {
        life = GameObject.Find("Life").GetComponent<RawImage>();
        pointsHUD = GameObject.Find("Points").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        life.rectTransform.localScale = new Vector3(player.GetComponent<Player>().life /4, 0.5263797f, 1);
        pointsHUD.text = lightSaber.GetComponent<LightSaber>().points.ToString();
    }

}
