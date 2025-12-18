using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //variable to control speed
    public float speed;
    //variables to keep track of how long the projectile has been out
    private float timer;
    private float lifetime = 5f;
    private float damage;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
        timer += Time.deltaTime;
        if (timer > lifetime)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetDamage(float assignDamage)
    {
        damage = assignDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Health>().takeDamage(damage);
            Destroy(this.gameObject);
        }
    }
}
