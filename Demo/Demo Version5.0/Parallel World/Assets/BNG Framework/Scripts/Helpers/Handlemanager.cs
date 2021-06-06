using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {

    public class Handlemanager: MonoBehaviour {

        public Transform LookAt;


        //손잡이가 Grabbable이면 가동가능.

        public Grabbable HandleGrabbable;

        public float Speed = 5f;

        public float LocalYMin = 215f;
        public float LocalYMax = 270f;

        Vector3 initialRot;

       public bool crushkey = true;

        void Start() {
            initialRot = transform.localEulerAngles;
        }

        


        void Update() {

            if (crushkey == true)
            {
                if (HandleGrabbable != null && HandleGrabbable.BeingHeld)
                {
                    Quaternion rot = Quaternion.LookRotation(HandleGrabbable.GetPrimaryGrabber().transform.position - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10);

                    Vector3 currentPos = transform.localEulerAngles;
                    float constrainedY = Mathf.Clamp(currentPos.y, LocalYMin, LocalYMax);

                    transform.localEulerAngles = new Vector3(initialRot.x, constrainedY, initialRot.z);
                }
                // LookAt Transform을 가리킵니다.
                else
                {
                    Quaternion rot = Quaternion.LookRotation(LookAt.position - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * Speed);

                    Vector3 currentPos = transform.localEulerAngles;
                    float constrainedY = Mathf.Clamp(currentPos.y, LocalYMin, LocalYMax);

                    transform.localEulerAngles = new Vector3(initialRot.x, constrainedY, initialRot.z);
                }

            }
        }


       
}
}