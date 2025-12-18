using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoHealthPack : MonoBehaviour
{
    public float equipRange = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    if((PlayerController.Instance.spineTransform.position - transform.position).magnitude <= equipRange)
        {
            switch (transform.tag)
            {
                case "HealthPack":
                    if (PlayerController.Instance.GetComponent<Health>().health < 100)
                    {
                        PlayerController.Instance.GetComponent<Health>().health += 10;
                        Destroy(gameObject);
                    }
                    break;

                case "AmmoPack":
                    if (GunSystem.Instance != null && GunSystem.Instance.currWeapon != null && GunSystem.Instance.currWeapon.tag !="Grapple_Gun")
                    {
                        GunSystem.Instance.currWeapon.totalAmmo += 20;
                        GunSystem.Instance.currWeapon.reloadDisplayTotalAmmo+= 20;
                        Destroy(gameObject);
                    }
                    break;
            }
        }
    }
}
