using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    public float speed = 10f;
    public Rigidbody rb;
    public bool Ontheground = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        float vertical = Input.GetAxis("Vertical") * Time.deltaTime * speed;
        
        

        transform.Translate(horizontal, 0, vertical);

        if (Input.GetButtonDown("Jump")&& Ontheground)
        {
        rb.AddForce(new Vector3(0,5,0), ForceMode.Impulse);
            Ontheground = false;
        }

    }
    //점프 한번만되게 
        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.tag=="Ground")//플레이어가 닿는 바닥에 고유한 태그 추가
        {
            Ontheground = true;
        }
        }

   
}
