using Boxophobic.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using KevinIglesias;


public class GunSystem : MonoBehaviour
{
    public Transform player;
    public static GunSystem Instance;
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip pistolShotSound;

    public bool shooting;
    public bool readytoShoot;
    public bool reloading;

    public GameObject muzzleFlash;
    public GameObject bloodSplatter;
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
    public bool hasWeapon = false;
    public weaponObject currWeapon;
    public weaponObject[] wpnSlots = new weaponObject[2];
    public int activeSlot = 0;
    public int otherSlot = 1;
    public GameObject glow;
    public GameObject laser;
    public GameObject flame;
    public GameObject head;
    public ParticleSystem flameParticles;

    float soundVolume = 0.5f;
    private string aimAnimate = "isAimingOneHanded";

    private void Awake()
    {
        Instance = this;
        readytoShoot = true;
    }
    public void Update()
    {
        flameEffect();
        assignAnimation();
        slotChange(-1);
        if (wpnSlots[0] != null && activeSlot == 0)
        {
            currWeapon = wpnSlots[0];
        }
        if (wpnSlots[1] != null && activeSlot == 1)
        {
            currWeapon = wpnSlots[1];


        }
        if (weaponEquipped != null)
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



        if (readytoShoot && shooting && !reloading && currWeapon.bulletsLeft > 0)
        {
            currWeapon.bulletsShot = currWeapon.bulletsPerTap;
            //Invoke("aimIn",0f);
            //Invoke("Shoot", 0.25f);
            aimIn();
            Shoot();

        }


        vCam.m_Lens.FieldOfView = 72f;
        animator.SetBool(aimAnimate, isAiming);

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
        animator.SetBool(aimAnimate, isAiming);
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


        //float x = Random.Range(-currWeapon.spread, currWeapon.spread);
        float y = Random.Range(-currWeapon.spread, currWeapon.spread);

        Vector3 direction = GunCamera.transform.forward;



        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        if (currWeapon.tag != "Flamethrower")
        {
            if (Physics.Raycast(ray, out RaycastHit RayHit, currWeapon.range))
            {

                if (RayHit.collider.CompareTag("Enemy"))
                {

                    RayHit.collider.GetComponent<EnemyHealth>().TakeDamage(currWeapon.damage);
                    Debug.Log(RayHit.collider.name);
                    switch (RayHit.collider.name)
                    {
                        case "BatEnemy":
                        case "SlimeEnemy":
                        case "GiantGolemBoss":
                            GameObject bloodSplatterTemp = Instantiate(bloodSplatter, RayHit.point + RayHit.normal * 0.01f, Quaternion.LookRotation(RayHit.normal * -1f));
                            Destroy(bloodSplatterTemp, .55f);
                            break;
                    }



                }
                else
                {
                    if (currWeapon.tag != "Flamethrower")
                    {
                        var bulletHoleClone = Instantiate(bulletHoleGraphic, RayHit.point + RayHit.normal * 0.01f, Quaternion.LookRotation(RayHit.normal * -1f));
                        Destroy(bulletHoleClone, 10f);
                    }
                }
            }
        }
        else
        {
            float radius = 0.3f;         // width of the flame
            float maxDistance = 10f;   // range
            Ray rayflame = new Ray(weaponBarrel.transform.transform.position, weaponBarrel.transform.forward);
            RaycastHit[] hits = Physics.SphereCastAll(ray, radius, maxDistance);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    hit.collider.GetComponent<EnemyHealth>().TakeDamage(currWeapon.damage);
                }
            }

        }

        Quaternion rotationBullet = Quaternion.LookRotation(RayHit.normal * -1f);
        GameObject muzzleFlashClone = null;
        if(currWeapon.tag == "Flamethrower")
        {
            Debug.Log("Test");
            flame.transform.position = weaponBarrel.transform.position;
            flame.transform.rotation = weaponBarrel.transform.rotation * Quaternion.Euler(90, 0, 0);
            //muzzleFlashClone = Instantiate(flame, weaponBarrel.transform.Iposition, rotationFlame);
            //muzzleFlashClone = Instantiate(flame, weaponBarrel.transform.position,player.rotation* Quaternion.Euler(0f,20f,0f) );
        }
        else if(currWeapon.tag== "Assault_Rifle")
        {

            muzzleFlashClone = Instantiate(laser, weaponBarrel.transform.position, Quaternion.identity);
            //Color flashColor = muzzleFlashClone.GetComponent<Color>();
            //flashColor = new Color(0f, 0.2f, 1f);
        }
        else
        {

            muzzleFlashClone = Instantiate(muzzleFlash, weaponBarrel.transform.position, Quaternion.identity);
        }
        AudioClip fireSoundAudio = weaponEquipped.GetComponent<weaponObject>().fireSound;
        audioSource.PlayOneShot(fireSoundAudio, weaponEquipped.GetComponent<weaponObject>().soundVol);
        Destroy(muzzleFlashClone, .2f);


        Invoke("ResetShot", currWeapon.timeBetweenShooting);

        //if (currWeapon.bulletsShot > 0 && currWeapon.bulletsLeft > 0)
        //{
            //Invoke("Shoot", currWeapon.timeBetweenShots);
        //}

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
                    weaponEquipped.localRotation = Quaternion.Euler(-44.1f, -168.2f, -298.5f);
                } else if (IdleOrAiming == "aiming")
                {
                    weaponEquipped.localRotation = Quaternion.Euler(-141.3f, -301.8f, -224.8f);
                }

                break;

            case "Revolver":
                if (IdleOrAiming == "idle")
                {
                    weaponEquipped.localRotation = Quaternion.Euler(168.1f, -20.4f, -120.4f);
                }
                else if (IdleOrAiming == "aiming")
                {
                    weaponEquipped.localRotation = Quaternion.Euler(206.5f, -52.7f, -141.2f);
                }
                break;
            case "Grapple_Gun":
                if (IdleOrAiming == "idle")
                {
                    weaponEquipped.localRotation = Quaternion.Euler(-40.9f, 222.2f, 39.3f);
                }
                else if (IdleOrAiming == "aiming")
                {
                    weaponEquipped.localRotation = Quaternion.Euler(-26f, -109f, -51.3f);
                }
                break;
            case "Assault_Rifle":
                
                if (IdleOrAiming == "idle")
                {
                    weaponEquipped.localRotation = Quaternion.Euler(-10.9f, 222.2f, 39.3f);
                    weaponEquipped.localPosition= new Vector3(-0.182f, 0.08f, -0.242f);
                }
                else if (IdleOrAiming == "aiming")
                {
                    weaponEquipped.localRotation = Quaternion.Euler(-93f, -11.1f, -75.3f);
                    weaponEquipped.localPosition= new Vector3(0.122f, 0.381f, -0.009f);
                }
                break;
            case "Flamethrower":
                
                if (IdleOrAiming == "idle")
                {
                    weaponEquipped.localRotation = Quaternion.Euler(-254.9f, -396.2f, 67.3f);
                    //weaponEquipped.localRotation = Quaternion.Euler(168f, -86f, 9.6f);
                    weaponEquipped.localPosition = new Vector3(0.19f, 0.315f, 0.17f);
                }
                else if (IdleOrAiming == "aiming")
                {
                    weaponEquipped.localRotation = Quaternion.Euler(168f, -86f, 9.6f);
                    weaponEquipped.localPosition = new Vector3(-0.134f, 0.697f, 0.03f);
                }
                break;


        }
    }
    private void ammoText()
    {
        if (!reloading)
        {
            if(currWeapon.tag== "Flamethrower")
            {
                AmmoText.fontSize = 22;
            }
            else
            {
                AmmoText.fontSize = 32;
            }
            AmmoText.SetText(currWeapon.bulletsLeft + " / " + currWeapon.reloadDisplayTotalAmmo);
        }
        else
        {

            AmmoText.fontSize = 24;
            AmmoText.SetText("Reloading...");
        }
    }
    // change slots, manual slot change call parameter allows for this function to be called
    // from other scripts to change the slot from code 
    public void slotChange(int manualSlotChangeCall)
    {
        if (activeSlot == 0)
        {
            otherSlot = 1;
        }
        if (activeSlot == 1)
        {
            otherSlot = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) || manualSlotChangeCall ==0)
        {
            activeSlot = 0;
            gun1ActiveBckg.GetComponent<Image>().color = new Color(.9f, .9f, .9f, 0.9f);
            gun2ActiveBckg.GetComponent<Image>().color = new Color(.5f, .5f, .5f, .5f);
            handleSlotChange(activeSlot, 1);

        }
        if (Input.GetKeyDown(KeyCode.Alpha2) || manualSlotChangeCall == 1)
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
    private void assignAnimation()
    {
        if (currWeapon != null)
        {
            if (currWeapon.twoHandedWeapon)
            {
                aimAnimate = "isAimingtwoHanded";
            }
            else
            {
                aimAnimate = "isAimingOneHanded";
            }
        }

    }
    private void flameEffect()
    {
        if(currWeapon!=null && currWeapon.tag == "Flamethrower" && shooting)
        {
            flameParticles.Play();
        }
        else
        {
            flameParticles.Stop();
        }
    }

} 

