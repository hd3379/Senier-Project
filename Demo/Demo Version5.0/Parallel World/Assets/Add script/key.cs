using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //텍스트 수정용

namespace BNG
{


    public class key : MonoBehaviour
    {
        public Text missiontext; //미션 텍스트 수정변수
        public Text questtext; //미션 텍스트 수정변수
        GameObject keyob;//키 삭제용
        GameObject keysnap;//키 삭제용
        private GameObject keyora;//키오라 삭제용

        bool keyoff = true;

        public AudioClip keycardsound;

        // Start is called before the first frame update
        void Start()
        {
            handlemanager = GameObject.Find("rooftophandle").GetComponent<Handlemanager>();
        }

        void Update()
        {
            //VRUtils.Instance.PlaySpatialClipAt(keycardsound, transform.position, 1f, 1f);
        }

        Handlemanager handlemanager;//키잠김 처리

        void m()
        {
            missiontext.gameObject.SetActive(false);

        }

        void OnCollisionEnter(Collision collision)
        {
            //
            handlemanager = GameObject.Find("rooftophandle").GetComponent<Handlemanager>();

            //키와 접촉시 잠금/열림
            if (keyoff == true)//문은 초기에 열려있음
            {
                if (collision.gameObject.tag == "doorlock")//키와 접촉시
                {
                
                     handlemanager.GetComponent<Handlemanager>().enabled = false;


                    missiontext.text = "좋았어. 옥상문을 제외한 통로가 " + "\n" + "있는지 찾아보자"; //미션텍스트 수정
                    questtext.text = "지도를 참고하여 옥상통로 찾아서 잠그기";

                    GetComponent<AudioSource>().Play();//키 찍을시 사운드발생

             
                    Invoke("m", 6.0f);

                    keyoff = false;
                    Debug.Log("잠김");

                }
            }

            else if (keyoff == false)//문은 초기에 열려있음
            {
                if (collision.gameObject.tag == "doorlock")//키와 접촉시
                {

                    handlemanager.GetComponent<Handlemanager>().enabled = true;

                    GetComponent<AudioSource>().Play();//키 찍을시 사운드발생


                    keyoff = true;
                    Debug.Log("열림");

                }
            }

        }


      


    }
}