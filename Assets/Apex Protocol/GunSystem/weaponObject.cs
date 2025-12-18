using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class weaponObject : MonoBehaviour
{

    public float timeBetweenShooting;
    public float spread;
    public float range;
    public int bulletsPerTap;
    public float reloadTime;
    public float timeBetweenShots;
    public int magazineSize;
    public int totalAmmo;
    public int reloadDisplayTotalAmmo;
    public int damage;
    public int bulletsLeft;
    public int bulletsShot;
    public bool allowButtonhold;

    public static weaponObject Instance;
    public GunSystem gunScript;
    public Transform player;
    public Transform gunContainer;
    public Transform ThirdPersonCam;
    public AudioClip fireSound;
    public AudioClip reloadSound;
    public Sprite uiImage;
    public GameObject popup;
    private GameObject glowPrefab;
    private GameObject glow;

    private float EquipRange = 1.75f;


    public bool isEquipped= false;
    private bool showPopup;
    private bool hasWeapon;
    public static bool slots;
    public float soundVol = 0.5f;
    public int slot = -1;
    weaponObject wpnInSlot;
    public bool twoHandedWeapon = false;


    private void Start()
    {
        Instance = this;
        reloadDisplayTotalAmmo = totalAmmo;
        GunSystem.Instance.gun1UI.SetActive(false);
        GunSystem.Instance.gun2UI.SetActive(false);
        GunSystem.Instance.AmmoText.enabled = false;
        GunSystem.Instance.dropBind.enabled = false;
        glowPrefab  = GunSystem.Instance.glow;
        glow = Instantiate(glowPrefab);
        glow.transform.localScale= new Vector3(.7f, .30f, .7f);
        glow.SetActive(false);
    }

    private void Update()
    {
       
        Vector3 distanceToPlayer = player.position - transform.position;
        wpnInSlot = GunSystem.Instance.wpnSlots[GunSystem.Instance.activeSlot];
        // if within range and we try to pick up 
        if (distanceToPlayer.magnitude <= EquipRange && Input.GetKeyDown(KeyCode.E) && !isEquipped)
        {
            // if we have an empty slot active then just equip regular
          if(wpnInSlot == null)
            {
                Equip();
            }
          // if our slot is full but we have an open slot that is non active then switch to that one and equip
            else if (GunSystem.Instance.wpnSlots[GunSystem.Instance.otherSlot]==null)
            {
                GunSystem.Instance.activeSlot = GunSystem.Instance.otherSlot;
                Equip();
                GunSystem.Instance.slotChange(GunSystem.Instance.activeSlot);

            }
        }

        // if we have a gun and want to drop it
        if(isEquipped && Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
            GunSystem.Instance.currWeapon = null;
            GunSystem.Instance.weaponEquipped= null;
            GunSystem.Instance.wpnSlots[GunSystem.Instance.activeSlot]= null;
        }

        // keep glow and popup aligned with weapon and rotate with player camera
        glow.transform.position = transform.position;
        // enable glow from further away than equip range
        if (!isEquipped && distanceToPlayer.magnitude <= EquipRange + 10f)
        {

            transform.RotateAround(transform.position, Vector3.up, 50f * Time.deltaTime);
            glow.transform.rotation = player.rotation;
            glow.SetActive(true);
        }
        else
        {
            glow.SetActive(false);
        }
        // if within equip range sohw popup
        if (!isEquipped && distanceToPlayer.magnitude<=EquipRange)
        {
            //spin and show popup
            popup.SetActive(true);
            popup.transform.rotation = player.rotation;



        }
        else
        {
            popup.SetActive(false); 
        }
    }

    private void Equip()
    {
        GunSystem.Instance.wpnSlots[GunSystem.Instance.activeSlot] = this;
        slot = GunSystem.Instance.activeSlot;
        setWeaponValues(true);
        Debug.Log(transform.tag);

        // assign the position and scale of the weapon upon pickup
        switch (transform.tag)
        {
            case "Pistol":
                transform.localPosition =new Vector3(-0.017f, 0.07f, -0.044f);
                transform.localScale = new Vector3(0.4f,0.4f,0.4f);
                Debug.Log("pistol");
                break;
            case "Revolver":
                transform.localPosition =new Vector3(-0.058f, 0.114f, -0.104f);
                transform.localScale = new Vector3(15f,15f,15f);
                Debug.Log("revolver");
                break;
            case "Grapple_Gun":
                transform.localPosition =new Vector3(0f, 0.14f, -0.06f);
                transform.localScale = new Vector3(0.03f,0.03f,0.03f);
                Debug.Log("Grapple_Gun");
                break;
            case "Assault_Rifle":
                //transform.localPosition =new Vector3(0.058f, 0.39f, -0.115f);
                transform.localPosition =new Vector3(-0.182f, 0.08f, -0.242f);
                transform.localScale = new Vector3(0.5f,0.5f,0.5f);
                Debug.Log("ar");
                break;
            case "Flamethrower":
                transform.localPosition = new Vector3(-0.134f, 0.697f, 0.03f);
                transform.localScale = new Vector3(15f,15f,15f);
                Debug.Log("fthrow");
                break;
            default:
                Debug.Log("empty");
                break;

              
        }


    }

    
    private void Drop()
    {
        if (GunSystem.Instance.activeSlot == slot)
        {
            // if we drop a weapon set it 0 rotation, flamethrower model is offset so we account for that 
            if(transform.tag == "Flamethrower")
            {   
               transform.rotation = Quaternion.Euler(90f,0f,0f);
            }
            else 
            { 
                transform.rotation = Quaternion.Euler(Vector3.zero);
            }
            setWeaponValues(false);
        }
        

    }
    // when we pickup or drop a weapon handle the enable/disabling of relevant weapon settings
    public void setWeaponValues(bool hasWeapon) 
    {
        
        isEquipped = hasWeapon;
        popup.SetActive(!hasWeapon);
        GunSystem.Instance.hasWeapon = hasWeapon;
        GunSystem.Instance.AmmoText.enabled = hasWeapon;
        GunSystem.Instance.dropBind.enabled = hasWeapon;
        if (slot == 0)
        {
            GunSystem.Instance.gun1UI.SetActive(hasWeapon);
        }else if(slot == 1)
        {
            GunSystem.Instance.gun2UI.SetActive(hasWeapon);
        }
        if (hasWeapon)
        {
            GunSystem.Instance.weaponEquipped = transform;
            transform.SetParent(gunContainer);
            Transform barrel = transform.Find("Barrel");
            GunSystem.Instance.weaponBarrel = barrel.gameObject;
            if(slot == 0)
            {
                GunSystem.Instance.gun1UI.GetComponent<Image>().sprite = uiImage;
            }else if(slot == 1)
            {

                GunSystem.Instance.gun2UI.GetComponent<Image>().sprite = uiImage;
            }
        }
        else
        {

            GunSystem.Instance.weaponEquipped = null;
            transform.SetParent(null, true);
            Transform barrel = null;
            GunSystem.Instance.weaponBarrel = null;
            if(slot == 0)
            {
                GunSystem.Instance.gun1UI.GetComponent<Image>().sprite = null;
            }else if(slot == 1)
            {

                GunSystem.Instance.gun2UI.GetComponent<Image>().sprite = null;
            }
        }

    }
}
