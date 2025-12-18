using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHealthBar : MonoBehaviour
{
    public static EnemyHealthBar Instance;
    private Slider slider;
    private Camera camera;
    private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f,2f,0f);
    private GameObject enemyGameObject;
    public GameObject healthbarPrefab;
    public GameObject healthBar;

    private void Awake()
    {
        target = this.transform;
        Instance = this;
            healthBar = Instantiate(healthbarPrefab);

            slider = healthBar.transform.GetChild(0).GetComponent<Slider>();
    }


    void Update()
    {
        camera = PlayerController.Instance.playerCamera;
        float health = 0;
        float maxHealth = 0;
        healthBar.transform.position = this.transform.position + offset;
        healthBar.transform.rotation= camera.transform.rotation;
        if (target != null && this.TryGetComponent<EnemyHealth>(out var enemyHealth))
        {
            health = enemyHealth.currHealth;
            maxHealth = enemyHealth.maxHealth;
        }

        slider.value = health / maxHealth;
        if (target == null ||  health <= 0) {
            Destroy(healthBar);
        }
        
    }
}
