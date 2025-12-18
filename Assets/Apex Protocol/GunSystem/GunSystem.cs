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
        // assign currweapon to the active slot 
        if (wpnSlots[0] != null && activeSlot == 0)
        {
            currWeapon = wpnSlots[0];
        }
        if (wpnSlots[1] != null && activeSlot == 1)
        {
            currWeapon = wpnSlots[1];


        }
        // assign weapon equipped the first time
        if (weaponEquipped != null)
        {
            weaponEquipped = currWeapon.transform;
            ammoText();
            MyInput();
        }



    }
    // handle user input
    private void MyInput()
    {

        // handle semi auto vs full auto weapon shooting 
        if (currWeapon.allowButtonhold)
        {

            shooting = Input.GetKey(KeyCode.Mouse0);

        }
        else
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }




        // reload if we have ammo left 
        if (Input.GetKeyDown(KeyCode.R) && currWeapon.bulletsLeft < currWeapon.magazineSize && !reloading)
        {
            Reload();
        }


        // shoot if not reloading, shooting, and have ammo
        if (readytoShoot && shooting && !reloading && currWeapon.bulletsLeft > 0)
        {
            currWeapon.bulletsShot = currWeapon.bulletsPerTap;
            //Invoke("aimIn",0f);
            //Invoke("Shoot", 0.25f);
            // aim in in case not already aiming in
            aimIn();
            Shoot();

        }

        // reset fov after aiming
        vCam.m_Lens.FieldOfView = 72f;
        // set aim animation
        animator.SetBool(aimAnimate, isAiming);
        

        // if not aiming then we set our weapon transform to the idle settings aka non aiming settings
        setTransform("idle");

        // aim input
        if (Input.GetKey(KeyCode.Mouse1))
        {
            isAiming = true;
            aimIn();
        }
        else
        {
            // fix for grapple animation to ensure it resets
            if (!PlayerController.Instance.isGrappling)
            {
                isAiming = false;
            }
        }

    }
    // when aiming increase fov and change animation
    public void aimIn()
    {
        isAiming = true;
        setTransform("aiming");
        vCam.m_Lens.FieldOfView = 35f;
        animator.SetBool(aimAnimate, isAiming);
    }

    // reload for certain time and restore ammo
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
        // subtract bullet from ammo and other tracking variables
        currWeapon.bulletsLeft--;
        currWeapon.bulletsShot--;
        currWeapon.totalAmmo--;
        // not ready to shoot again 
        readytoShoot = false;


        //float x = Random.Range(-currWeapon.spread, currWeapon.spread);
        float y = Random.Range(-currWeapon.spread, currWeapon.spread);

        Vector3 direction = GunCamera.transform.forward;



        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        // for traditional weapons we use rays to achieve bullet hit scan
        if (currWeapon.tag != "Flamethrower")
        {
            if (Physics.Raycast(ray, out RaycastHit RayHit, currWeapon.range))
            {
                // we use different effects for enemies such as blood etc
                if (RayHit.collider.CompareTag("Enemy"))
                {

                    RayHit.collider.GetComponent<EnemyHealth>().TakeDamage(currWeapon.damage);
                    Debug.Log(RayHit.collider.name);
                    switch (RayHit.collider.name)
                    {
                        case "BatEnemy":
                        case "SlimeEnemy":
                            GameObject bloodSplatterTemp = Instantiate(bloodSplatter, RayHit.point + RayHit.normal * 0.01f, Quaternion.LookRotation(RayHit.normal * -1f));
                            Destroy(bloodSplatterTemp, .55f);
                            break;
                    }



                }
                // if we hit a non enemy, so an object tehn we instatiate a bullet hole texture
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
        else // if we ARE using flamethrower
        {
            float radius = 0.3f;         // width of the flame
            float maxDistance = 10f;   // range
            // use a sphere ray so we can register hits on enemy within the area of the flame 
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


        // MUZZLE FLASHES
        Quaternion rotationBullet = Quaternion.LookRotation(RayHit.normal * -1f);
        GameObject muzzleFlashClone = null;
        // flamethrower has a flame 
        if(currWeapon.tag == "Flamethrower")
        {
            Debug.Log("Test");
            flame.transform.position = weaponBarrel.transform.position;
            flame.transform.rotation = weaponBarrel.transform.rotation * Quaternion.Euler(90, 0, 0);
            //muzzleFlashClone = Instantiate(flame, weaponBarrel.transform.Iposition, rotationFlame);
            //muzzleFlashClone = Instantiate(flame, weaponBarrel.transform.position,player.rotation* Quaternion.Euler(0f,20f,0f) );
        }
        // ar is a laser rifle so the muzzle flash is blue purple laser effect
        else if(currWeapon.tag== "Assault_Rifle")
        {

            muzzleFlashClone = Instantiate(laser, weaponBarrel.transform.position, Quaternion.identity);
            //Color flashColor = muzzleFlashClone.GetComponent<Color>();
            //flashColor = new Color(0f, 0.2f, 1f);
        }
        // all other are standard muzzle flashes
        else
        {

            muzzleFlashClone = Instantiate(muzzleFlash, weaponBarrel.transform.position, Quaternion.identity);
        }
        // play fire sound
        AudioClip fireSoundAudio = weaponEquipped.GetComponent<weaponObject>().fireSound;
        audioSource.PlayOneShot(fireSoundAudio, weaponEquipped.GetComponent<weaponObject>().soundVol);
        // remove the muzzle flash after .2 seconds
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
    // assigns the transform rotation for aiming or non aiming 
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
    // UI ammo update
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
    // invetory slot change ui update
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
    // this is to differentiate aiming with a two handed weapon and one handed weapon
    // and to use their respective animations
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
    // stop or play flame effect when firing 
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

