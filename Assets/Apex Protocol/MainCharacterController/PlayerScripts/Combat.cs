using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    [SerializeField] public Animator animator;
    // Start is called before the first frame update
    float meleeTimer;
    bool meleeCalled = false;
    void Start()
    { 
            animator.SetBool("Melee",false);
        meleeTimer = 1.3f;
    }

    // Update is called once per frame
    void Update()
    {

        if (meleeCalled)
        {
            melee();
        }

        if (Input.GetKey(KeyCode.F))
        {
            meleeCalled= true;


        }
     

    }

    void melee()
    {
        meleeTimer -= Time.deltaTime;
        animator.SetBool("Melee", true);
        if (meleeTimer < 0)
        {
            meleeCalled = false;
            animator.SetBool("Melee", false);
            meleeTimer = 1.1f;
        }
    }
}
