using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//무시해도되는 충돌
namespace BNG 
{
    public class IgnoreColliders : MonoBehaviour {

        public List<Collider> CollidersToIgnore;

        // Start is called before the first frame update
        void Start() {
            var thisCol = GetComponent<Collider>();
            if(CollidersToIgnore != null) {
                foreach(var col in CollidersToIgnore) {
                    if(col && col.enabled) {
                        Physics.IgnoreCollision(thisCol, col, true);
                    }
                }
            }
        }
    }
}