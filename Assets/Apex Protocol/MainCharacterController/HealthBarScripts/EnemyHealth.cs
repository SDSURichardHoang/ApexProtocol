using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100;
    public float currHealth;
    // Start is called before the first frame update
    void Start()
    {
        currHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        EnemyHealthBar.Instance.UpdateHealthBar(currHealth,maxHealth);
    }
    public void TakeDamage(float damage)
    {
        currHealth-= damage; 
    }
}
