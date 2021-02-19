using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//vr기기 미연동시 시야체험을 위한 스크립트
public class SightControl : MonoBehaviour
{
    //마우스 좌우값을 받아오는 문자열.
    private const string AXIS_MOUSE_X = "Mouse X";

    //마우스 상하값을 받아오는 문자열.
    private const string AXIS_MOUSE_Y="Mouse Y";

    //마우스 좌표 움직임을 멤버변수로 저장한다
    private float m_fMouseX = 0f;//좌우

    private float m_fMouseY = 0f;

    // Start is called before the first frame update
    void Start()
    {
        m_fMouseX = 0f;
        m_fMouseY = 0f;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        m_fMouseX += Input.GetAxis(AXIS_MOUSE_X) * 1.0f;
         //m_fMouseY -= Input.GetAxis(AXIS_MOUSE_Y) * 1.0f;
        transform.transform.localRotation = Quaternion.Euler(m_fMouseY,m_fMouseX, 0);

#endif
    }
}
