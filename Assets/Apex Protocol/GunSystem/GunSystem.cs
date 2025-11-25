using Boxophobic.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.DebugUI.Table;


public class GunSystem : MonoBehaviour
{
    public Animator animator;   

    public float timeBetweenShooting;
    public float spread;
    public float range;
    public float reloadTime;
    public float timeBetweenShots;


    public int magazineSize;
    public int bulletsPerTap;
    public int damage;
    public int bulletsLeft;
    public int bulletsShot;

    public bool allowButtonhold;
    public bool shooting;
    public bool readytoShoot;
    public bool reloading;

    public GameObject muzzleFlash;
    public GameObject bulletHoleGraphic;
    public Camera GunCamera;
    public Transform attackPoint;
    public TextMeshProUGUI text;

    public RaycastHit RayHit;
    public LayerMask Enemy;
    public CinemachineVirtualCamera vCam;
    public GameObject weaponEquipped;
    private Quaternion aimedInWeaponRotation;
    private Quaternion defaultWeaponRotation;
    public GameObject weaponBarrel;


    private void Awake()
    {
        defaultWeaponRotation = weaponEquipped.transform.localRotation;
        bulletsLeft = magazineSize;
        readytoShoot = true;
        aimedInWeaponRotation = Quaternion.Euler(-46.3f, -108.8f, -58.8f);
    }
    private void Update()
    {

        MyInput();

        text.SetText(bulletsLeft + " / " + magazineSize);
    }

    private void MyInput()
    {
        if (allowButtonhold)
        {

            shooting = Input.GetKey(KeyCode.Mouse0);

        }
        else
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }





        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }



        if(readytoShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Invoke("aimIn",0f);
            Invoke("Shoot", 0.2f);

        }


        vCam.m_Lens.FieldOfView = 72f;
        animator.SetBool("isAiming", false);
        weaponEquipped.transform.localRotation = defaultWeaponRotation;
        if (Input.GetKey(KeyCode.Mouse1))
        {
            aimIn();
        }
        
    }
    private void aimIn()
    {
        weaponEquipped.transform.localRotation= aimedInWeaponRotation;
        vCam.m_Lens.FieldOfView = 45f;
        animator.SetBool("isAiming", true);
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }


    private void Shoot()
    {
        readytoShoot = false;


        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 direction = GunCamera.transform.forward;



        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));

        if (Physics.Raycast(ray, out RaycastHit RayHit, range))
        {
            Debug.Log(RayHit.transform.position);
           // if (RayHit.collider.CompareTag("Enemy"))
            {

                //NOT COMPLETE UNTIL FIRST ENEMY SCRIPT IS CREATED
                //RayHit.collider
                //.GetComponent<>.TakeDamage(damage);




            }
        }

        Quaternion rotationBullet = Quaternion.LookRotation(RayHit.normal * -1f);
        Instantiate(bulletHoleGraphic, RayHit.point+RayHit.normal * 0.01f, rotationBullet);
        Instantiate(muzzleFlash, weaponBarrel.transform.position, Quaternion.identity);

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if(bulletsShot > 0 && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
       
    }

    private void ResetShot()
    {
        readytoShoot = true;
    }
  
}
