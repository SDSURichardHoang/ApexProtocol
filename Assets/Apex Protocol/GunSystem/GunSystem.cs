using Boxophobic.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;


public class GunSystem : MonoBehaviour
{
    public static GunSystem Instance;
    public Animator animator;   
    public AudioSource audioSource;
    public AudioClip pistolShotSound;

    public bool shooting;
    public bool readytoShoot;
    public bool reloading;

    public GameObject muzzleFlash;
    public GameObject bulletHoleGraphic;
    public Camera GunCamera;
    public Transform attackPoint;
    public TextMeshProUGUI AmmoText;
    public TextMeshProUGUI dropBind;
    public GameObject gun1UI;
    public GameObject gun2UI;
    public GameObject gun1ActiveBckg;
    public GameObject gun2ActiveBckg;

    public RaycastHit RayHit;
    public LayerMask Enemy;
    public CinemachineVirtualCamera vCam;
    public Transform weaponEquipped;
    public GameObject weaponBarrel;
    public bool isAiming;
    public bool hasWeapon =false;
    public weaponObject currWeapon;
    public weaponObject[] wpnSlots= new weaponObject[2];
    public int activeSlot = 0;

    float soundVolume =0.5f;

    private void Awake()
    {
        Instance = this;
        readytoShoot = true;
    }
    private void Update()
    {
        slotChange();
        if (wpnSlots[0] != null && activeSlot == 0)
        {
            currWeapon = wpnSlots[0];
        }
        if (wpnSlots[1] != null && activeSlot == 1)
        {
            currWeapon = wpnSlots[1];
            

        }
        if (weaponEquipped!=null)
        {
            weaponEquipped = currWeapon.transform;
            ammoText();
            MyInput();
        }



    }
    private void MyInput()
    {
        
        if (currWeapon.allowButtonhold)
        {

            shooting = Input.GetKey(KeyCode.Mouse0);

        }
        else
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }





        if (Input.GetKeyDown(KeyCode.R) && currWeapon.bulletsLeft < currWeapon.magazineSize && !reloading)
        {
            Reload();
        }



        if(readytoShoot && shooting && !reloading && currWeapon.bulletsLeft > 0)
        {
            currWeapon.bulletsShot = currWeapon.bulletsPerTap;
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
            if (!PlayerController.Instance.isGrappling)
            {
                isAiming = false;
            }
        }
        
    }
    public void aimIn()
    {
        isAiming = true;
        setTransform("aiming");
        vCam.m_Lens.FieldOfView = 35f;
        animator.SetBool("isAiming", isAiming);
    }

    private void Reload()
    {
        reloading = true;
        //audioSource.PlayOneShot(weaponEquipped.GetComponent<weaponObject>().reloadSound);
        audioSource.PlayOneShot(currWeapon.reloadSound);
        Invoke("ReloadFinished", currWeapon.reloadTime);
        currWeapon.reloadDisplayTotalAmmo = currWeapon.totalAmmo;
    }

    private void ReloadFinished()
    {
        currWeapon.bulletsLeft = currWeapon.magazineSize;
        reloading = false;
    }


    private void Shoot()
    {
        currWeapon.bulletsLeft--;
        currWeapon.bulletsShot--;
        currWeapon.totalAmmo--;
        readytoShoot = false;


        float x = Random.Range(-currWeapon.spread, currWeapon.spread);
        float y = Random.Range(-currWeapon.spread, currWeapon.spread);

        Vector3 direction = GunCamera.transform.forward;



        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));

        if (Physics.Raycast(ray, out RaycastHit RayHit, currWeapon.range))
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
        AudioClip fireSoundAudio = weaponEquipped.GetComponent<weaponObject>().fireSound;
        audioSource.PlayOneShot(fireSoundAudio,weaponEquipped.GetComponent<weaponObject>().soundVol);
        //Delete bullet hole and muzzle flash objects after 10 and 2 seconds respectively 
        Destroy(bulletHoleClone, 10f);
        Destroy(muzzleFlashClone, 2f);


        Invoke("ResetShot", currWeapon.timeBetweenShooting);

        if(currWeapon.bulletsShot > 0 && currWeapon.bulletsLeft > 0)
        {
            Invoke("Shoot", currWeapon.timeBetweenShots);
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
            case "Grapple_Gun":
                if (IdleOrAiming == "idle")
                {
                    weaponEquipped.localRotation = Quaternion.Euler(-40.9f, 222.2f, 39.3f);
                }
                else if(IdleOrAiming == "aiming")
                {
                    weaponEquipped.localRotation = Quaternion.Euler(-26f, -109f, -51.3f);
                }
                break;

        }
    }
    private void ammoText()
    {
        if (!reloading)
        {
            AmmoText.fontSize = 36;
            AmmoText.SetText(currWeapon.bulletsLeft + " / " + currWeapon.reloadDisplayTotalAmmo );
        }
        else
        {

            AmmoText.fontSize = 24;
            AmmoText.SetText("Reloading...");
        }
    }
    private void slotChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            activeSlot = 0;
            gun1ActiveBckg.GetComponent<Image>().color = new Color(.9f, .9f, .9f, 0.9f);
            gun2ActiveBckg.GetComponent<Image>().color = new Color(.5f, .5f, .5f, .5f);
            handleSlotChange(activeSlot,1);
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            activeSlot = 1;
            gun2ActiveBckg.GetComponent<Image>().color = new Color(.9f, .9f, 0.9f, 0.9f);
            gun1ActiveBckg.GetComponent<Image>().color = new Color(.5f, .5f, .5f, .5f);
            handleSlotChange(activeSlot,0);
        }


    }
    void handleSlotChange(int slot,int otherSlot)
    {
        if(wpnSlots[otherSlot] != null)
        {
            //wpnSlots[otherSlot].GetComponent<weaponObject>().setWeaponValues(false);
            wpnSlots[otherSlot].gameObject.SetActive(false);
        }
        if (wpnSlots[slot] == null)
        {
            Debug.Log("no wepaon");
            
        }
        else
        {
            Debug.Log("We have:"+wpnSlots[slot].transform.tag);
            //wpnSlots[slot].GetComponent<weaponObject>().setWeaponValues(true);  
            wpnSlots[slot].gameObject.SetActive(true);
        }
    }
} 