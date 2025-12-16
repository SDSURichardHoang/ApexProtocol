using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOver : MonoBehaviour
{
    
    public void RestartButtton()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void QuitButton()
    {
        SceneManager.LoadSceneAsync(0);
    }


}
