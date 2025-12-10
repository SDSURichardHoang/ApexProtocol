using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class NextScenePortalTransition : MonoBehaviour
{
    public string LvL2scene;

    void OnTriggerEnter(Collider Transition)
    {
        if (Transition.CompareTag("Player"))
        {
            SceneManager.LoadScene(LvL2scene);
        }
    }


}
