using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHookRotation : MonoBehaviour
{
    // Start is called before the first frame update
    public class RotateGun : MonoBehaviour
    {

        public GrapplingHook GrapplingHook;

        private Quaternion desiredRotation;
        private float rotationSpeed = 5f;

        void Update()
        {
            if (!GrapplingHook.IsGrappling())
            {
                desiredRotation = transform.parent.rotation;
            }
            else
            {
                desiredRotation = Quaternion.LookRotation(GrapplingHook.GetGrapplePoint() - transform.position);
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
        }

    }
}