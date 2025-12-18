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
    private bool beginEncounter;
    float meleeTimer;
    float throwTimer;
    bool canAttack;
    bool meleeCalled, throwCalled;

    // Start is called before the first frame update
    void Start()
    {
        //neccesary components for NavMeshAgent and other keep factors
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        arenaCenter = transform.position;
        player = PlayerController.Instance.transform;
        throwTimer = 1f;
        meleeTimer= 0.5f;
        canAttack = true;
        meleeCalled = false;
        throwCalled = false; 
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        ChasePlayer();
        //code to detect the player and begin the encounter
        if (!beginEncounter && distanceToPlayer <= escapeRange)
        {
            beginEncounter = true;
        }

        //code to stop attacking if player escapes far enough
        if (beginEncounter && distanceToPlayer > escapeRange)
        {
            beginEncounter = false;
            agent.isStopped = true;
            return;
        }

        if (beginEncounter)
        {


            if (distanceToPlayer <= meleeRange && canAttack)
                //does a melee attack if close enough
            {
                meleeCalled = true;
            }
            else if (distanceToPlayer <= projectileRange && distanceToPlayer >= meleeRange && canAttack)

                //does a ranged attack if too far from player
            {
                throwCalled = true;
                RangedAttack();
            }
            if (meleeCalled)
            {
                MeleeAttack();
            }else if (throwCalled)
            {

                RangedAttack();
            }
        }
    }

    //code for the boss' melee attack
    void MeleeAttack()
    {
        meleeTimer-= Time.deltaTime;
        canAttack = false;
        animator.SetBool("MeleeAttack", true);
        if (meleeTimer<= 0f)
        {
            meleeCalled = false;
            canAttack = true;
            animator.SetBool("MeleeAttack", false);
            meleeTimer = 2f;
            player.GetComponent<Health>().takeDamage(meleeDamage);

        }
    }

    //code for the boss' projectile attack
    void RangedAttack()
    {
        throwTimer-=Time.deltaTime;
        animator.SetBool("ThrowAttack", true);
        canAttack = false;
        if (throwTimer <= 0f)
        {
            throwCalled = false;
            canAttack = true;
            animator.SetBool("ThrowAttack", false);
            throwTimer = 1f;


             // rock logic
            GameObject bossProjectile = Instantiate(projectilePrefab, projectilePoint.position, projectilePoint.transform.rotation);
            Rigidbody rb = bossProjectile.GetComponent<Rigidbody>();
            Vector3 direction = (player.position - projectilePoint.position).normalized;
            rb.AddForce(direction * 750f); // adjust force for balance
        }

    }

    //code to approach the player if too far to attack
    void ChasePlayer()
    {
        animator.SetBool("isWalking", true);
        agent.SetDestination(player.position);
    }
}
