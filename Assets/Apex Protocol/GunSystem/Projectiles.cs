using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Projectiles : MonoBehaviour
{
    public GameObject bullet;

    public float FShoot;
    public float Fupward;
    public float timebetweenShots;
    public float TimebetweenShooting;
    public float spread;
    public float ReloadTime;

    public int MagSize;
    public int BulletsOnTap;
    public int bulletsLeft;
    public int bulletsShot;

    public bool HoldDownButtonFN;
    public bool Shooting;
    public bool readyToShoot;
    public bool Reload;
    public bool allowInvoke = true;


    public Camera ThirdPersonCam;
    public Transform Attackpoint;
    public TextMeshProUGUI AmmoText;
    public GameObject muzzleFlash;


    private void Awake()
    {
        bulletsLeft = MagSize;

        readyToShoot = true;
        
    }

    private void Update()
    {
        MyInput();

        if(AmmoText != null)
        {
            AmmoText.SetText(bulletsLeft / BulletsOnTap + " / " + MagSize / BulletsOnTap);
        }
    }


    private void MyInput()
    {

        if(Input.GetKeyDown(KeyCode.R) && bulletsLeft < MagSize && !Reload)
        {
            Reloading();
        }

        if (readyToShoot && Shooting && !Reload && bulletsLeft <= 0)
        {
            Reloading();
        }



        if (HoldDownButtonFN)
        {
            Shooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            Shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }





        if(readyToShoot && Shooting && !Reload && bulletsLeft > 0)
        {
            bulletsShot = 0;

            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        Ray ray = ThirdPersonCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));


        RaycastHit hit;

        Vector3 Target;

        if (Physics.Raycast(ray, out hit))
        {
            Target = hit.point;
        }
        else
        {
            Target = ray.GetPoint(75);
        }

        Vector3 directionWithoutSpread = Target - Attackpoint.position;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        GameObject currentBullet = Instantiate(bullet, Attackpoint.position, Quaternion.identity);

        currentBullet.transform.forward = directionWithSpread.normalized;

        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * FShoot, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(ThirdPersonCam.transform.up * Fupward, ForceMode.Impulse);

        if(muzzleFlash != null)
        {
            Instantiate(muzzleFlash, Attackpoint.position, Quaternion.identity);
        }

        bulletsLeft--;
        bulletsShot++;

        if (allowInvoke)
        {
            Invoke("ResetShot", TimebetweenShooting);
            allowInvoke = false;
        }

        if(bulletsShot < BulletsOnTap && bulletsLeft > 0)
        {
            Invoke("Shoot", timebetweenShots);
        }
    }


    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reloading()
    {
        Reload = true;
        Invoke("ReloadFinished", ReloadTime);

    }

    private void ReloadFinished()
    {
        bulletsLeft = MagSize;
        Reload = false;
    }
}
