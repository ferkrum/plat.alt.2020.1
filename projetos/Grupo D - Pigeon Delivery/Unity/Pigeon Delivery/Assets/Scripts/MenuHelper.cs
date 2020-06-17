using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHelper : MonoBehaviour
{
    public GameObject playerHelper;
    public GameObject Play, Settings, Exit;


    private bool isPlay, isSetting, isExit;

    void Update()
    {
        MakeBigger();
    }
    void MakeBigger()
    {
        if (this.playerHelper.transform.position.x >= -5.5f && this.playerHelper.transform.position.x <= -3.5f && isSetting != true && isExit !=true)
        {

            Play.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
            isPlay = true;
        }
        else
        {
            Play.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
            isPlay = false;
        }

        if (this.playerHelper.transform.position.x >= -1.1f && this.playerHelper.transform.position.x <= 1.1f && isPlay!=true && isExit != true)
        {
           
            Settings.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
            isSetting = true;
        }
        else
        {
            Settings.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
            isSetting = false;
        }

        if (this.playerHelper.transform.position.x >= 2.5f && this.playerHelper.transform.position.x <= 4.5f && isPlay != true && isSetting != true)
        {
          
            Exit.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
            isExit = true;
        }
        else
        {

            Exit.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
            isExit = false;
        }


    }
}
