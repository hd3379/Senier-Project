﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {
    public class Tooltip : MonoBehaviour {


        private GameObject keytool;//키튤팁  삭제용
        private GameObject maptool;//지도튤팁 삭제용
        /// <summary>
        /// Offset from Object we are providing tip to
        /// </summary>
        public Vector3 TipOffset = new Vector3(1.5f, 0.2f, 0);

        /// <summary>
        /// If true Y axis will be in World Coordinates. False for local coords.
        /// </summary>
        public bool UseWorldYAxis = true;

        /// <summary>
        /// Hide the tooltip if Camera is farther away than this. In meters.
        /// </summary>
        public float MaxViewDistance = 10f;

        /// <summary>
        /// Hide this if farther than MaxViewDistance
        /// </summary>
        Transform childTransform;

        public Transform DrawLineTo;
        LineToTransform lineTo;
        Transform lookAt;
        
        void Start() {
            lookAt = Camera.main.transform;
            lineTo = GetComponentInChildren<LineToTransform>();

            childTransform = transform.GetChild(0);

            if (DrawLineTo && lineTo) {
                lineTo.ConnectTo = DrawLineTo;
            }
        }

        void Update() {
            if(lookAt) {
                transform.LookAt(Camera.main.transform);
            }
            else {
                lookAt = Camera.main.transform;
            }

            transform.parent = DrawLineTo;
            transform.localPosition = TipOffset;

            if(UseWorldYAxis) {
                transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
                transform.position += new Vector3(0, TipOffset.y, 0);
            }
            
            if(childTransform) {
                float dist = Vector3.Distance(transform.position, Camera.main.transform.position);
                childTransform.gameObject.SetActive(dist <= MaxViewDistance);

             
            }
           
        }
    }
}
