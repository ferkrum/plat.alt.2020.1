using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuContoller : MonoBehaviour
{
    public GameObject generalPlay, generalSettings, generalExit;
    public GameObject generalSound, generalScreenSize, generalBack;


    public bool isPlay, isSettings, isExit;
    public static bool deleteAll;
    void Start()
    {
        deleteAll = false;
        generalPlay.SetActive(true);
        generalSettings.SetActive(true);
        generalExit.SetActive(true);

        generalSound.SetActive(false);
        generalScreenSize.SetActive(false);
        generalBack.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D _col)
    {
  
        if (_col.gameObject.name == "Box01(Clone)" || _col.gameObject.name == "Box02(Clone)" || _col.gameObject.name == "Box03(Clone)")
        {
            if(isPlay == true)
            {
                Debug.Log("aaaaaajsdhjkdfhjfbgdhfffj");
                SceneManager.LoadScene(2);
            }
            if(isSettings == true)
            {
                generalPlay.SetActive(false);
                generalSettings.SetActive(false);
                generalExit.SetActive(false);
                deleteAll = true;

                Debug.Log("HITED ME");
                generalSound.SetActive(true);
                generalScreenSize.SetActive(true);
                generalBack.SetActive(true);
            }
            if(isExit == true)
            {
                Application.Quit();
            }
        }
        
    }

}
