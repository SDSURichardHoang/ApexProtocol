using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    public static GrapplingHook Instance;
    public bool isGrappling = false;
    public Transform gunTip;
    public LayerMask grappleMask;
    public LineRenderer grappleLine;
    public float grapplePullSpeed = 40f;
    public float grappleSwingStrength = 5f;
    public float grappleStopDistance = 2f;
    public float grappleMaxRange = 25f;

    public Vector3 grapplePoint;
    public float cooldownTime = 2f;
    public float cooldownTimer = 0f;
    public AudioSource audSource;
    private void Awake()
    {
         Instance = this;
    }
    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
        if (cooldownTimer <=0 && Input.GetKey(KeyCode.Mouse0)&& GunSystem.Instance.currWeapon != null && GunSystem.Instance.currWeapon.tag =="Grapple_Gun")
        {
            StartGrapple();
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }
    }
    public void StartGrapple()
    {
        audSource.clip = this.GetComponent<weaponObject>().fireSound;
        audSource.loop = false;
        GunSystem.Instance.isAiming = true;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        if (Physics.Raycast(ray, out RaycastHit hit,grappleMaxRange, grappleMask))
        {

            cooldownTimer = cooldownTime;
            audSource.Play();
            grapplePoint = hit.point;
            isGrappling = true;
            PlayerController.Instance.isGrappling = true;
            if (grappleLine != null) grappleLine.positionCount = 2;
            GunSystem.Instance.isAiming = true;
            PlayerController.Instance.animator.SetBool("isAiming", true);
            PlayerController.Instance.isGrappling = true;
        }
    }

    public void StopGrapple()
    {
        audSource.Stop();
        GunSystem.Instance.isAiming = false;
        PlayerController.Instance.animator.SetBool("isAiming", false);
        PlayerController.Instance.isGrappling = false;
        isGrappling = false;
        if (grappleLine != null) grappleLine.positionCount = 0;
    }
}
