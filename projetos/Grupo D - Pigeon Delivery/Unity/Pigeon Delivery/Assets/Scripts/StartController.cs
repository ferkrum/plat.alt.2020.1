using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartController : MonoBehaviour
{
   private int sceneIndex;
   public Animator animator;
   void Update()
   {
        if (Input.anyKey)
        {
            FadeToLevel(1);
            //SceneManager.LoadScene(NextScene);
            //Debug.Log("Go to next scene");
        }
   }

    public void FadeToLevel(int index)
    {
        sceneIndex = index;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
