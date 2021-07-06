using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    TIME uiTime;//유아이 시간 변경용
    AbsorbedTime absorbedTime;
    private bool IsColWithGhost = false;
    // Start is called before the first frame update
    void Start()
    {
        uiTime = GameObject.FindGameObjectWithTag("UI").GetComponent<TIME>();
        absorbedTime = gameObject.GetComponent<AbsorbedTime>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (IsColWithGhost == false)
            {
                absorbedTime.CreatePyramid(true, collision.transform.position);
                uiTime.SubTrackTime(true);
                IsColWithGhost = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (IsColWithGhost == true)
            {
                absorbedTime.CreatePyramid(true, collision.transform.position);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (IsColWithGhost == true)
            {
                absorbedTime.CreatePyramid(false, Vector3.zero);
                uiTime.SubTrackTime(false);
                IsColWithGhost = false;
            }
        }
    }

}
