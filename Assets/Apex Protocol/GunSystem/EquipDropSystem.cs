using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipDropSystem : MonoBehaviour
{
    public GunSystem gunScript;
    public Rigidbody RigB;
    public BoxCollider Collider;
    public Transform player;
    public Transform gunContainer;
    public Transform ThirdPersonCam;

    public float EquipRange;
    public float DropForceF;
    public float DropForceUp;


    public bool Equip;
    public static bool slots;


    private void Start()
    {
        if (Equip)
        {
            gunScript.enabled = true;
            RigB.isKinematic = true;
            Collider.isTrigger = true;
            slots = true;
        }

        if (!Equip)
        {
            gunScript.enabled = false;
            RigB.isKinematic = false;
            Collider.isTrigger = false;
        }
       
    }

    private void Update()
    {
        Vector3 distanceToPlayer = player.position - transform.position;

        if (!Equip && distanceToPlayer.magnitude <= EquipRange && Input.GetKeyDown(KeyCode.E) && !slots)
        {
            PickUp();
        }

        if(Equip && Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }
           
    }

    private void PickUp()
    {
        Equip = true;
        slots = true;

        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        RigB.isKinematic = true;
        Collider.isTrigger = true;

        gunScript.enabled = true;
    }

    private void Drop()
    {
        Equip = false;
        slots = false;

        transform.SetParent(null);

        RigB.isKinematic = false;
        Collider.isTrigger = false;

        RigB.velocity = player.GetComponent<Rigidbody>().velocity;

        RigB.AddForce(ThirdPersonCam.forward * DropForceF, ForceMode.Impulse);
        RigB.AddForce(ThirdPersonCam.up * DropForceUp, ForceMode.Impulse);

        float random = Random.Range(-1f, 1f);

        RigB.AddTorque(new Vector3(random, random, random) * 10);

        gunScript.enabled = false;


    }
}
