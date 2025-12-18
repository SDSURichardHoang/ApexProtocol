using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossScript : MonoBehaviour
{
    //variables customizable based on boss
    public float speed = 1f;
    public float meleeRange, projectileRange, meleeDamage, cooldown, escapeRange;

    //variables for the arena
    public float arenaSize;
    private Vector3 arenaCenter;

    public Transform player;
    public Animator animator;
    public GameObject projectilePrefab;
    public Transform projectilePoint;

    private NavMeshAgent agent;
    private float attackTimer;
    private bool beginEncounter;

    // Start is called before the first frame update
    void Start()
    {
        //neccesary components for NavMeshAgent, starts Wander Coroutine after
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        arenaCenter = transform.position;
        attackTimer = cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
