using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    private GameObject time;
    private bool IsColWithPlayer = false;
    // Start is called before the first frame update
    void Start()
    {
        time = GameObject.FindGameObjectWithTag("time");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if (IsColWithPlayer == false)
            {
                //time.GetComponent<TIME>().SubTrackTime(true);
                IsColWithPlayer = true;
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (IsColWithPlayer == true)
            {
               // time.GetComponent<TIME>().SubTrackTime(false);
                IsColWithPlayer = false;
            }
        }
    }
}
