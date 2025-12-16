using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOver : MonoBehaviour
{
    
    //Resets level on input of restart button
    public void RestartButtton()
    {
        SceneManager.LoadSceneAsync(1);
    }

    //On hit of main menu button, changes scene to main menu scene
    public void QuitButton()
    {
        SceneManager.LoadSceneAsync(0);
    }


}
