using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //텍스트 수정용

namespace BNG
{


    public class key : MonoBehaviour
    {
        public Text missiontext; //미션 텍스트 수정변수
        GameObject keyob;//키 삭제용
        GameObject keysnap;//키 삭제용
        private GameObject keyora;//키오라 삭제용

        // Start is called before the first frame update
        void Start()
        {

        }

        Handlemanager handlemanager;//키잠김 처리

        private void OnCollisionEnter(Collision collision)
        {



            handlemanager = GameObject.Find("rooftophandle").GetComponent<Handlemanager>();
            //키와 접촉시 잠금/열림

            if (collision.gameObject.tag == "doorlock")//키와 접촉시
            {
                handlemanager.crushkey = false;//핸들매니저스크립트 중지

                missiontext.text = "좋았어. 옥상문을 제외한 통로가 " + "\n" + "있는지 찾아보자"; //미션텍스트 수정
   
                Debug.Log("잠김");

            }



        }


        void Update()
        {

        }



    }
}