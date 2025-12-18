using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100;
    public float currHealth;
    public GameObject ammoPack;
    public GameObject HealthPack;
    // Start is called before the first frame update
    void Start()
    {
        currHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (currHealth < 0)
        {
            Destroy(GetComponent<EnemyHealthBar>().healthBar);
            Destroy(gameObject);
            float rand = Random.value;
            if(rand < .5)
            {
                Instantiate(HealthPack);
            }
            else
            {
                Instantiate(ammoPack);

            }
            
        }
    }
    public void TakeDamage(float damage)
    {
        currHealth-= damage; 
    }
}
