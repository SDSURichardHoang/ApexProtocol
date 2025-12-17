using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public static Health Instance;
    public Slider HealthSlider;
    public float maxHealth = 100f;
    public float health;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (health > 100)
        {
            health = 100;
        }
        if (HealthSlider.value != health)
        {
            HealthSlider.value = health;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            takeDamage(10);
        }
        checkForDeath();


       
    }
    public void takeDamage(float damage)
    {
        health -= damage;
    }
    void checkForDeath()
    {
        if(health< 0)
        {
            SceneManager.LoadScene("Game Over");
        }
    }
}
