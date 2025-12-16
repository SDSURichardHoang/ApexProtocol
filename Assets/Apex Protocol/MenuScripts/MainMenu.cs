using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //On hit of Play Game button, sets scene to start of level 1
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }


    //On hit of Quit button, ends program
    public void QuitGame()
    {
        Application.Quit();
    }
}
