using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ExplodingBullet : MonoBehaviour
{
    public Rigidbody RigB;
    public GameObject Explosion;
    public LayerMask whatIsEnemies;

    [Range(0f, 1f)]
    public float bounciness;
    public float ExplosionRange;
    public float LifeSpanG;

    public bool useGravity;
    public bool ExplodeOnContact = true;

    public int ExplosionDamage;
    public int Collisions;
    public int MaxCollisions;


    PhysicMaterial physics;

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        if(Collisions > MaxCollisions)
        {
            Explode();
        }

        LifeSpanG -= Time.deltaTime;
        
        if(LifeSpanG <= 0)
        {
            Explode();
        }
    }


    private void OnCollisionEnter(Collision collision)
    {

         Collisions++;

        if(collision.collider.CompareTag("Enemy") && ExplodeOnContact)
        {
            Explode();
        }
    }



    private void Explode()
    {
        if (Explosion != null)
        {
            Instantiate(Explosion, transform.position, Quaternion.identity);

            Collider[] enemies = Physics.OverlapSphere(transform.position, ExplosionRange, whatIsEnemies);

            for(int i = 0; i < enemies.Length; i++)
            {
                //THIS LINE IS TO IMPLEMENT DAMAGE TO THE ENEMY
                //enemies[i].GetComponent<ShootingAi>().TakeDamage(ExplosionDamage);

            }
        }
    }


    private void Setup()
    {
        physics = new PhysicMaterial();
        physics.bounciness = bounciness;
        physics.frictionCombine = PhysicMaterialCombine.Minimum;
        physics.bounceCombine = PhysicMaterialCombine.Maximum;

        GetComponent<SphereCollider>().material = physics;

        RigB.useGravity = useGravity;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, ExplosionRange);
    }
}
