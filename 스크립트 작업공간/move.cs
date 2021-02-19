using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveZ = 0f;

        float moveX = 0f;

        float moveY = 0f;


        if (Input.GetKey(KeyCode.W))

        {
          
            moveZ += 0.5f;

        }



        if (Input.GetKey(KeyCode.S))

        {
          
                moveZ -= 0.5f;

        }



        if (Input.GetKey(KeyCode.A))

        {
            
                moveX -= 0.5f;

        }



        if (Input.GetKey(KeyCode.D))

        {
           
                moveX += 0.5f;

        }

       



        transform.Translate(new Vector3(moveX, moveY, moveZ) * 0.1f);



    }
}
