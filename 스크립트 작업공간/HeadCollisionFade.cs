using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //텍스트 수정용

namespace BNG {

    public class HeadCollisionFade : MonoBehaviour {

        public Text missiontext; //미션 텍스트 수정변수
        private GameObject firstora;//미션1지점 오라 삭제용
        private GameObject secondora;//미션2지점 오라 삭제용


        ScreenFader fader;

        public float FadeDistance = 0.1f;
        public float FadeOutDistance = 0.045f;

        // 페이드 시작 위치

        public float MinFade = 0.5f;
        public float MaxFade = 0.95f;
        public float FadeSpeed = 1f;

        [Tooltip("HMD가 활성으로 등록 중인 경우에만 화면을 페이드합니다.")]
        public bool CheckOnlyIfHMDActive = false;

        public bool IgnoreHeldGrabbables = true;

        public Transform DistanceTransform;
        public int cols = 0;
        private float currentFade = 0;
        private float lastFade = 0;

        public List<Collider> collisions;

        void Start() {
            if(Camera.main) {
                fader = Camera.main.transform.GetComponent<ScreenFader>();
            }
           
        }

        void LateUpdate() {

            bool headColliding = false;

            // hmd가 장착된 경우 헤드 충돌 확인
            if (CheckOnlyIfHMDActive == false || InputBridge.Instance.HMDActive) {
                for (int x = 0; x < collisions.Count; x++) {
                    if (collisions[x] != null && collisions[x].enabled) {
                        headColliding = true;
                        break;
                    }
                }
            }

            if (headColliding) {
                FadeDistance = Vector3.Distance(transform.position, DistanceTransform.position);
            }
            else {
                FadeDistance = 0;
            }

            if (fader) {
                // 멀어지면 시커멓게
                if (FadeDistance > FadeOutDistance) {
                    currentFade += Time.deltaTime * FadeSpeed;

                    if (headColliding && currentFade < MinFade) {
                        currentFade = MinFade;
                    }

                    if (currentFade > MaxFade) {
                        currentFade = MaxFade;
                    }

                    // 값이 변경된 경우에만 업데이트 페이드
                    if (currentFade != lastFade) {
                        fader.SetFadeLevel(currentFade);
                        lastFade = currentFade;
                    }
                    
                }
                // 페이드 백
                else {
                    currentFade -= Time.deltaTime * FadeSpeed;

                    if (currentFade < 0) {
                        currentFade = 0;
                    }

                    // 값이 변경된 경우에만 업데이트 페이드
                    if (currentFade != lastFade) {
                        fader.SetFadeLevel(currentFade);
                        lastFade = currentFade;
                    }
                }
            }
        }

        void OnCollisionEnter(Collision col) {
            if(collisions == null) {
                collisions = new List<Collider>();
            }

            // 보류 중인 붙잡기 가능한 물리학 개체 무시
            bool ignorePhysics = IgnoreHeldGrabbables && col.gameObject.GetComponent<Grabbable>() != null && col.gameObject.GetComponent<Grabbable>().BeingHeld;

            // 또한 이 물체에 이음매가 부착되어 있으면 물리학을 무시.
            if (!ignorePhysics && col.collider.GetComponent<Joint>()) {
                ignorePhysics = true;
            }
            
            if (ignorePhysics) {
                Physics.IgnoreCollision(col.collider, GetComponent<Collider>(), true);
                return;
            }

            if(!collisions.Contains(col.collider)) {
                collisions.Add(col.collider);
                cols++;
            }
            
            //----------------------------------------------------------------------


            //미션 포인트 충돌시 미션변경
            if (col.gameObject.tag == "mission1")//미션포인트1 도달시
            {
                missiontext.text = "미션:남자를 따라가 집주소를 알아내기";

                firstora = GameObject.FindGameObjectWithTag("mission1");//미션포인트1 오라 받아오기
                Destroy(firstora);//삭제
           

            }

            if (col.gameObject.tag == "mission2")//미션포인트1 도달시
            {
                missiontext.text = "미션:빈집에서 자물쇠를 찾아 옥상문 잠그기";

                secondora = GameObject.FindGameObjectWithTag("mission2");//미션포인트1 오라 받아오기
                Destroy(secondora);//삭제


            }

        }

      
        void OnCollisionExit(Collision col) {
            if (collisions == null) {
                collisions = new List<Collider>();
            }

            if (collisions.Contains(col.collider)) {
                collisions.Remove(col.collider);
                cols--;
            }
        }
    }
}