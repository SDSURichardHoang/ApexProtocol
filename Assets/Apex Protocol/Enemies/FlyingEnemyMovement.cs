using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class FlyingEnemyMovement : MonoBehaviour
{

    //speed variable, customizable based on enemy
    public float speed = 1f;
    //attack range variable, customizable based on enemy
    public float attackRange = 1f;

    private bool chase;
    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WanderCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (chase)
        {
            float distanceFromPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceFromPlayer <= attackRange) //stop chasing and enter attack function
            {
                //attack
            }

            else
            {
                Vector3 chaseDirection = (player.position - transform.position).normalized;
                if (chaseDirection != Vector3.zero) //code for the enemy to rotate in the direction of the player
                {
                    Quaternion targetRotation = Quaternion.LookRotation(chaseDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                }
                transform.position += Time.deltaTime * (speed * 2) * chaseDirection;
            }
        }
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
            while (timer < movementDuration && !chase)
            {
                //enemy moves in a random direction for a random amount of time
                transform.position += Time.deltaTime * speed * randomMovement;
                //code for the enemy to rotate in the direction it is moving in
                if (randomMovement != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(randomMovement);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                }

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
            player = other.transform;
            StopCoroutine(WanderCoroutine());
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
