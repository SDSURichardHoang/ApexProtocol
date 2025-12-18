using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class laserGunDrop : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject weapon;
    float x, z;
    void Start()
    {
        weapon.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        weapon.transform.position = this.transform.position;
        if(this.GetComponent<EnemyHealth>().currHealth <= 75)
        {
            weapon.transform.position += new Vector3(0f,1f,0f);
            weapon.SetActive(true);
        }
    }
}
