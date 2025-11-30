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
    public static GunSystem Instance;
    public Animator animator;   
    public AudioSource audioSource;
    public AudioClip pistolShotSound;

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
    public TextMeshProUGUI AmmoText;
    public TextMeshProUGUI dropBind;
    public GameObject gunUI;

    public RaycastHit RayHit;
    public LayerMask Enemy;
    public CinemachineVirtualCamera vCam;
    public Transform weaponEquipped;
    public GameObject weaponBarrel;
    public bool isAiming;
    public bool hasWeapon =false;


    private void Awake()
    {
        Instance = this;
        bulletsLeft = magazineSize;
        readytoShoot = true;
    }
    private void Update()
    {

        if (weaponEquipped!=null)
        {
            MyInput();
        }
            AmmoText.SetText(bulletsLeft + " / " + magazineSize);

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
            //Invoke("aimIn",0f);
            //Invoke("Shoot", 0.25f);
            aimIn();
            Shoot();

        }

        
        vCam.m_Lens.FieldOfView = 72f;
        animator.SetBool("isAiming", isAiming);
        setTransform("idle");
        if (Input.GetKey(KeyCode.Mouse1))
        {
            isAiming = true;
            aimIn();
        }
        else
        {

            isAiming = false;
        }
        
    }
    private void aimIn()
    {
        isAiming = true;
        setTransform("aiming");
        vCam.m_Lens.FieldOfView = 35f;
        animator.SetBool("isAiming", isAiming);
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
        bulletsLeft--;
        bulletsShot--;
        readytoShoot = false;


        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 direction = GunCamera.transform.forward;



        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));

        if (Physics.Raycast(ray, out RaycastHit RayHit, range))
        {
           // if (RayHit.collider.CompareTag("Enemy"))
            {

                //NOT COMPLETE UNTIL FIRST ENEMY SCRIPT IS CREATED
                //RayHit.collider
                //.GetComponent<>.TakeDamage(damage);




            }
        }

        Quaternion rotationBullet = Quaternion.LookRotation(RayHit.normal * -1f);
        var bulletHoleClone = Instantiate(bulletHoleGraphic, RayHit.point+RayHit.normal * 0.01f, rotationBullet);
        var muzzleFlashClone = Instantiate(muzzleFlash, weaponBarrel.transform.position, Quaternion.identity);
        AudioClip fireSoundAudio = weaponEquipped.GetComponent<EquipDropSystem>().fireSound;
        audioSource.PlayOneShot(fireSoundAudio,0.5f);
        //Delete bullet hole and muzzle flash objects after 10 and 2 seconds respectively 
        Destroy(bulletHoleClone, 10f);
        Destroy(muzzleFlashClone, 2f);


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
    private void setTransform(string IdleOrAiming)
    {
        
        switch (weaponEquipped.tag)
        {
            case "Pistol":
                if (IdleOrAiming == "idle")
                {
                    weaponEquipped.localRotation =Quaternion.Euler(-44.1f, -168.2f, -298.5f);
                }else if (IdleOrAiming == "aiming")
                {
                    weaponEquipped.localRotation= Quaternion.Euler(-141.3f, -301.8f, -224.8f);
                }

                break;

            case "Revolver":
                if (IdleOrAiming == "idle")
                {
                    weaponEquipped.localRotation = Quaternion.Euler(168.1f, -20.4f, -120.4f);
                }
                else if(IdleOrAiming == "aiming")
                {
                    weaponEquipped.localRotation = Quaternion.Euler(206.5f, -52.7f, -141.2f);
                }
                break;

        }
    }
}
