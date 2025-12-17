using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroundedEnemyMovement : MonoBehaviour
{
    //speed variable, customizable based on enemy
    public float speed = 1f;
    //attack range variable, customizable based on enemy
    public float attackRange = 1f;

    //NavMesh used for grounded enemies compared to flying enemies
    private NavMeshAgent agent;
    //used for the sphere collider
    private SphereCollider sphereCollider;
    private bool chase;
    public Transform player;
    public float distanceFromPlayer;
    public float minChaseDistance = 10f;

    // Start is called before the first frame update
    void Start()
    {
        //neccesary components for NavMeshAgent, starts Wander Coroutine after
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        sphereCollider = GetComponent<SphereCollider>();
        StartCoroutine(WanderCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        distanceFromPlayer = Vector3.Distance(transform.position, player.position);
        if (chase)
        {

            if (distanceFromPlayer <= attackRange) //stop chasing and enter attack function
            {
                agent.isStopped = true;
                agent.ResetPath();
                //attack
            }

            else
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
        }
        chaseDetection();
    }

    //coroutine for Wandering, or moving around randomly while player is not nearby
    IEnumerator WanderCoroutine()
    {
        while (!chase)
        {
            //decides on a random direction for the enemy to move to
            Vector3 randomDirection = Random.insideUnitSphere * sphereCollider.radius;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, sphereCollider.radius, NavMesh.AllAreas);
            //sets the destination within the NavMesh for the enemy to go
            agent.SetDestination(hit.position);

            while (agent.remainingDistance > agent.stoppingDistance && !chase)
            {
                //enemy moves in a random direction for a random amount of time
                yield return null;
            }
            //pauses before engaging in a new movement
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    //this code is to enter and exit the "chase" mode
    private void chaseDetection()
    {
        if(distanceFromPlayer <= minChaseDistance)
        {
            chase = true;
            StartCoroutine(WanderCoroutine());
        }
        else
        {
            chase = false;
            StopCoroutine(WanderCoroutine());
        }
    }
}
