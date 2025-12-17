using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossScript : MonoBehaviour
{
    //speed variable, customizable based on enemy
    public float speed = 1f;

    //NavMesh used for grounded enemies compared to flying enemies
    private NavMeshAgent agent;
    //used for the sphere collider
    private SphereCollider sphereCollider;
    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        //neccesary components for NavMeshAgent, starts Wander Coroutine after
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        sphereCollider = GetComponent<SphereCollider>();
        //StartCoroutine(WanderCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
