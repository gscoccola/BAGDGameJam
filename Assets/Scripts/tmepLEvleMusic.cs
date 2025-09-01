using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tmepLEvleMusic : LevelMusic
{
   


    protected override void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (!MainMenu.languageChoose) return;
        }
        
        
        base.Start();
    }

    public void OnChooseLanguage()
    {
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
    }
}
