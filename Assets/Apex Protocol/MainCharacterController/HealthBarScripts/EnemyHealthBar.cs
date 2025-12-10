using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHealthBar : MonoBehaviour
{
    public static EnemyHealthBar Instance;
    [SerializeField] private Slider slider;
    [SerializeField] private Camera camera;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] public GameObject enemyGameObject;

    private void Awake()
    {
        Instance = this;
    }


    void Update()
    {
        float health = 0;
        float maxHealth = 0;
        if (target != null && target.TryGetComponent<EnemyHealth>(out var enemyHealth))
        {
            health = enemyHealth.currHealth;
            maxHealth = enemyHealth.maxHealth;
            transform.rotation = camera.transform.rotation;
            transform.position = target.position + offset;
        }

        slider.value = health / maxHealth;
        if (target == null ||  health <= 0) {
            Destroy(enemyGameObject);
        }
    }
}
