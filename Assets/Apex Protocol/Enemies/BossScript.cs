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
        //neccesary components for NavMeshAgent and other keep factors
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        arenaCenter = transform.position;
        attackTimer = cooldown;
        player = PlayerController.Instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        //code to detect the player and begin the encounter
        if (!beginEncounter && distanceToPlayer <= escapeRange)
        {
            beginEncounter = true;
        }

        //code to stop attacking if player escapes far enough
        if (beginEncounter && distanceToPlayer > escapeRange)
        {
            beginEncounter = false;
            animator.SetBool("isWalking", false);
            agent.isStopped = true;
            return;
        }

        if (beginEncounter)
        {
            if (agent.isStopped)
            {
                Vector3 lookPos = player.position - transform.position;
                lookPos.y = 0; // keep rotation flat
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime * 5f);
            }

            attackTimer -= Time.deltaTime;

            if (distanceToPlayer <= meleeRange && attackTimer <= 0f)
                //does a melee attack if close enough
            {
                MeleeAttack();
            }
            else if (distanceToPlayer <= projectileRange && attackTimer <= 0f)
                //does a ranged attack if too far from player
            {
                RangedAttack();
            }
            else
            {
                ChasePlayer();
            }
        }
    }

    //code for the boss' melee attack
    void MeleeAttack()
    {
        agent.isStopped = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("ThrowAttack", false);
        animator.SetBool("MeleeAttack", true);
        player.GetComponent<Health>().takeDamage(meleeDamage);
        attackTimer = cooldown;
    }

    //code for the boss' projectile attack
    void RangedAttack()
    {
        agent.isStopped = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("MeleeAttack", false);
        animator.SetBool("ThrowAttack", true);
        GameObject bossProjectile = Instantiate(projectilePrefab, projectilePoint.position, Quaternion.identity);
        Rigidbody rb = bossProjectile.GetComponent<Rigidbody>();
        Vector3 direction = (player.position - projectilePoint.position).normalized;
        rb.AddForce(direction * 500f); // adjust force for balance
        attackTimer = cooldown;
    }

    //code to approach the player if too far to attack
    void ChasePlayer()
    {
        agent.isStopped = false;
        animator.SetBool("MeleeAttack", false);
        animator.SetBool("ThrowAttack", false);
        animator.SetBool("isWalking", true);
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }
}
