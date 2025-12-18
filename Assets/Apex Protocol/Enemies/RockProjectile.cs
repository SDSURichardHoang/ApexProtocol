using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockProjectile : MonoBehaviour
{
    //variables for launch force, upward force and lifespan timer
    public float launchForce = 10f;
    public float upwardForce = 5f;
    public float lifeTime = 5f;
    //rigidbody
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        //applies initial arc force (forward + upward)
        Vector3 launchDirection = transform.forward * launchForce + transform.up * upwardForce;
        rb.AddForce(launchDirection, ForceMode.Impulse);

        //destroy after lifetime if nothing is hit
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        //if it hits the player
        if (collision.gameObject.CompareTag("Player"))
        {
            //apply damage
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.takeDamage(10); // adjust damage value
            }
            Destroy(gameObject);
        }
    }
}
