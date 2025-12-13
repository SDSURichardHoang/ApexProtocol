using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class FlyingEnemyMovement : MonoBehaviour
{

    //speed variable, customizable based on enemy
    public float speed = 1f;

    private bool chase;
    //private Transform playerDirection;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WanderCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        //if (chase)
        //{
        //    GameObject player = 
        //    playerDirection = 
        //    transform.position += playerDirection * (speed * 2) * Time.deltaTime; 
        //}
    }

    //coroutine for Wandering, or moving around randomly while player is not nearby
    IEnumerator WanderCoroutine()
    {
        while (!chase)
        {
            //decides on a random direction for the enemy to move to
            Vector3 randomMovement = Random.onUnitSphere;
            //gives a random amount of time between 1 and 5 seconds
            float movementDuration = Random.Range(1f, 5f);
            //gives a random amount of time between 1 and 3 seconds
            float pauseDuration = Random.Range(1f, 3f);

            float timer = 0f;
            while (timer < movementDuration)
            {
                //enemy moves in a random direction for a random amount of time
                transform.position += Time.deltaTime * speed * randomMovement;
                timer += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(pauseDuration);
        }
    }

    //this code is to enter and exit the "chase" mode
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            chase = true;
            while (chase)
            {
                Vector3 playerDirection = (other.transform.position - transform.position).normalized;
                transform.position += Time.deltaTime * (speed * 2) * playerDirection;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
       if (other.CompareTag("Player"))
        {
            chase = false;
            StartCoroutine(WanderCoroutine());
        }
    }
}
