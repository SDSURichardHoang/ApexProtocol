using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class weaponObject : MonoBehaviour
{

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

    public static weaponObject Instance;
    public GunSystem gunScript;
    public Transform player;
    public Transform gunContainer;
    public Transform ThirdPersonCam;
    public AudioClip fireSound;
    public AudioClip reloadSound;
    public Sprite uiImage;
    public GameObject popup;

    private float EquipRange = 1.75f;


    public bool isEquipped= false;
    private bool showPopup;
    private bool hasWeapon;
    public static bool slots;


    private void Start()
    {
        Instance = this;
        GunSystem.Instance.gunUI.SetActive(false);
        GunSystem.Instance.AmmoText.enabled = false;
        GunSystem.Instance.dropBind.enabled = false;
    }

    private void Update()
    {
        Vector3 distanceToPlayer = player.position - transform.position;
        if (distanceToPlayer.magnitude <= EquipRange && Input.GetKeyDown(KeyCode.E) && !isEquipped && !GunSystem.Instance.hasWeapon)
        {

            PickUp();
        }

        if(isEquipped && Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }
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

    private void PickUp()
    {
        setWeaponValues(true);
        Debug.Log(transform.tag);
        switch (transform.tag)
        {
            case "Pistol":
                transform.localPosition =new Vector3(-0.017f, 0.07f, -0.044f);
                //transform.localRotation= Quaternion.Euler(-44.3f, -168f, -298.8f);
                transform.localScale = new Vector3(0.4f,0.4f,0.4f);
                Debug.Log("pistol");
                break;
            case "Revolver":
                transform.localPosition =new Vector3(-0.058f, 0.114f, -0.104f);
                //transform.localRotation= Quaternion.Euler(-44.3f, -168f, -298.8f);
                transform.localScale = new Vector3(15f,15f,15f);
                Debug.Log("revolver");
                break;

              
        }
        //transform.localPosition = Vector3.zero;
        //transform.localRotation = Quaternion.Euler(Vector3.zero);
        //transform.localScale = new Vector3(0.3f,0.3f,0.3f);


    }

    private void Drop()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        setWeaponValues(false);
        

    }
    private void setWeaponValues(bool hasWeapon) 
    {
        
        isEquipped = hasWeapon;
        popup.SetActive(!hasWeapon);
        GunSystem.Instance.hasWeapon = hasWeapon;
        GunSystem.Instance.AmmoText.enabled = hasWeapon;
        GunSystem.Instance.dropBind.enabled = hasWeapon;
        GunSystem.Instance.gunUI.SetActive(hasWeapon);
        if (hasWeapon)
        {
            GunSystem.Instance.weaponEquipped = transform;
            transform.SetParent(gunContainer);
            Transform barrel = transform.Find("Barrel");
            GunSystem.Instance.weaponBarrel = barrel.gameObject;
            GunSystem.Instance.gunUI.GetComponent<Image>().sprite = uiImage;
        }
        else
        {

            GunSystem.Instance.weaponEquipped = null;
            transform.SetParent(null, true);
            Transform barrel = null;
            GunSystem.Instance.weaponBarrel = null;
            GunSystem.Instance.gunUI.GetComponent<Image>().sprite = null;
        }

    }
}
