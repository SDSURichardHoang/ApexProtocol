using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //used to establish basic position vector
        Vector3 position = transform.position;
        float movementValue = Random.value;
        if (movementValue <= 0.17) //moves in the positive x direction
        {
            position.x += Time.deltaTime; //will update with a speed variable later
        }
        else if (movementValue > 0.17 && movementValue <= 0.33) //moves in the negative x direction
        {
            position.x -= Time.deltaTime; //will update with a speed variable later
        }
        else if (movementValue > 0.33 && movementValue <= 0.5) //moves in the positive y direction
        {
            position.y += Time.deltaTime; //will update with a speed variable later
        }
        else if (movementValue > 0.5 && movementValue <= 0.66) //moves in the negative y direction
        {
            position.y -= Time.deltaTime; //will update with a speed variable later
        }
        else if (movementValue > 0.66 && movementValue <= 0.83) //moves in the positive z direction
        {
            position.z += Time.deltaTime; //will update with a speed variable later
        }
        else if (movementValue > 0.83) //moves in the negative z direction
        {
            position.z -= Time.deltaTime; //will update with a speed variable later
        }
    }
}
