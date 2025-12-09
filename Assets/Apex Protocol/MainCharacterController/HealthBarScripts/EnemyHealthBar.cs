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

    private void Awake()
    {
        Instance = this;
    }
    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
    }


    void Update()
    {
        transform.rotation = camera.transform.rotation;
        transform.position = target.position + offset;
    }
}
