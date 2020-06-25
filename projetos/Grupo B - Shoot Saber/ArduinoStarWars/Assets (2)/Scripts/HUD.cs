using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    Text pointsHUD;
    public GameObject lightSaber, player;
    int points;
    RawImage life;
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.Current.PlayerDied += PlayerLost;
        GameEvents.Current.PlayerCalibrated += PlayerIsReady;
        GameEvents.Current.PlayerSurvived += PlayerWon;
        life = GameObject.Find("Life").GetComponent<RawImage>();
        pointsHUD = GameObject.Find("Points").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        life.rectTransform.localScale = new Vector3(player.GetComponent<Player>().life /4, 0.5263797f, 1);
        pointsHUD.text = "pontos:1" + lightSaber.GetComponent<LightSaber>().points.ToString();      
        if(points > 25)
        {
            GameEvents.Current.PlayerWon();
        }
    }

    private void PlayerLost()
    {
        SceneManager.LoadScene("Lose");
    }

    private void PlayerIsReady()
    {
        
    }

    private void PlayerWon()
    {
        SceneManager.LoadScene("Victory");
    }
}
