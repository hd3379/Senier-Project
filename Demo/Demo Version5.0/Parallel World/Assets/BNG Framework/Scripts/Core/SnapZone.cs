using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//인벤토리 관련
namespace BNG 
{
    public class SnapZone : MonoBehaviour 
    {
        private GameObject keytool;//키튤팁  삭제용
        private GameObject maptool;//지도튤팁 삭제용

        [Header("Starting / Held Item")]
        [Tooltip("현재 보관 중인 항목")]
        public Grabbable HeldItem;


        [Header("Options")]
        [Tooltip("False인 경우 플레이어가 아이템을 떨어뜨리면 아이템이 인벤토리 공간으로 다시 이동.")]
        public bool CanDropItem = true;

        
      
       
        [Tooltip("false인 경우 스냅 존의 콘텐츠를 교체할 수 없음.")]
        public bool CanSwapItem = true;

      
        [Tooltip("Multiply Item Scale times this when in snap zone.")]
        public float ScaleItem = 1f;
        private float _scaleTo;

        public bool DisableColliders = true;
        List<Collider> disabledColliders = new List<Collider>();

        [Tooltip("True이면 SnapZone 내의 항목이 SnapZone에서 제거되는 대신 복제")]
        public bool DuplicateItemOnGrab = false;

        [Tooltip("최대 X초 전에 Grabbable을 떨어뜨린 경우에만 스냅")]
        public float MaxDropTime = 0.1f;

        [Header("Filtering")]
     
        [Tooltip("비어 있지 않은 경우 변환 이름에 다음 문자열 중 하나가 포함된 경우에만 개체를 스냅할 수 있음.")]
        public List<string> OnlyAllowNames;

     
        [Tooltip("변환에 다음 이름 중 하나가 포함된 경우 스냅 허용 안 함")]
        public List<string> ExcludeTransformNames;

        [Header("Audio")]//아이템 주을때 사운드
        public AudioClip SoundOnSnap;
        public AudioClip SoundOnUnsnap;


        [Header("Events")]
     
        /// Optional Unity Event  to be called when something is snapped to this SnapZone. Passes in the Grabbable that was attached.
      
        public GrabbableEvent OnSnapEvent;//그랩했을떄

        /// Optional Unity Event to be called when something has been detached from this SnapZone. Passes in the Grabbable is being detattached.
  
        public GrabbableEvent OnDetachEvent; //그랩멈출때

        GrabbablesInTrigger gZone;

        Rigidbody heldItemRigid;
        bool heldItemWasKinematic;
        Grabbable trackedItem; // 만약 물건을 놓을 수 없다면, 중단

        //컨트롤러에서 가장 근접한 물체 그랩
        [HideInInspector]
        public Grabbable ClosestGrabbable;

        SnapZoneOffset offset;

        // 첫 번째 프레임 업데이트 전에 시작이 호출
        void Start() {
            gZone = GetComponent<GrabbablesInTrigger>();
            _scaleTo = ScaleItem;

            // 아이템 자동 장비
            if (HeldItem != null) {
                GrabGrabbable(HeldItem);
            }
        }

        //업데이트는 프레임 당 한 번 호출.
        void Update() {

            ClosestGrabbable = getClosestGrabbable();

            // 무언가를 잡기
            if (HeldItem == null && ClosestGrabbable != null) {
                float secondsSinceDrop = Time.time - ClosestGrabbable.LastDropTime;
                if (secondsSinceDrop < MaxDropTime) {
                    GrabGrabbable(ClosestGrabbable);
                }
            }

            //  스냅을 계속 주거나 떨어뜨리기
            if (HeldItem != null) {

                // Something picked this up or changed transform parent
                if (HeldItem.BeingHeld || HeldItem.transform.parent != transform) {
                    ReleaseAll();
                }
                else {
                    // Scale Item while inside zone.                                            
                    float localScale = HeldItem.OriginalScale * _scaleTo;
                    HeldItem.transform.localScale = Vector3.Lerp(HeldItem.transform.localScale, new Vector3(localScale, localScale, localScale), Time.deltaTime * 30f);
                    
                    // Make sure this can't be grabbed from the snap zone
                    if(HeldItem.enabled || (disabledColliders != null && disabledColliders.Count > 0 && disabledColliders[0] != null && disabledColliders[0].enabled)) {
                        disableGrabbable(HeldItem);
                    }
                }
            }

            // Can't drop item. Lerp to position if not being held
            if (!CanDropItem && trackedItem != null && HeldItem == null) {
                if (!trackedItem.BeingHeld) {
                    GrabGrabbable(trackedItem);
                }
            }
        }

        Grabbable getClosestGrabbable() {

            Grabbable closest = null;
            float lastDistance = 9999f;

            if (gZone == null || gZone.NearbyGrabbables == null) {
                return null;
            }

            foreach (var g in gZone.NearbyGrabbables) {

                // Collider may have been disabled
                if(g.Key == null) {
                    continue;
                }

                float dist = Vector3.Distance(transform.position, g.Value.transform.position);
                if(dist < lastDistance) {

                    //  Not allowing secondary grabbables such as slides
                    if(g.Value.OtherGrabbableMustBeGrabbed != null) {
                        continue;
                    }

                    // Don't allow SnapZones in SnapZones
                    if(g.Value.GetComponent<SnapZone>() != null) {
                        continue;
                    }

                    // Don't allow InvalidSnapObjects to snap
                    if (g.Value.CanBeSnappedToSnapZone == false) {
                        continue;
                    }

                    // Must contain transform name
                    if (OnlyAllowNames != null && OnlyAllowNames.Count > 0) {
                        string transformName = g.Value.transform.name;
                        bool matchFound = false;
                        for (int x = 0; x < OnlyAllowNames.Count; x++) {
                            string name = OnlyAllowNames[x];
                            if (transformName.Contains(name)) {
                                matchFound = true;                                
                            }
                        }

                        // Not a valid match
                        if(!matchFound) {
                            continue;
                        }
                    }

                    // Check for name exclusion
                    if (ExcludeTransformNames != null) {
                        string transformName = g.Value.transform.name;
                        bool matchFound = false;
                        for (int x = 0; x < ExcludeTransformNames.Count; x++) {
                            // Not a valid match
                            if (transformName.Contains(ExcludeTransformNames[x])) {
                                matchFound = true;
                            }
                        }
                        // Exclude this
                        if (matchFound) {
                            continue;
                        }
                    }

                    // Only valid to snap if being held or recently dropped
                    if (g.Value.BeingHeld || (Time.time - g.Value.LastDropTime < MaxDropTime)) {
                        closest = g.Value;
                        lastDistance = dist;
                    }
                }
            }

            return closest;
        }

        public void GrabGrabbable(Grabbable grab) {

            // Grab is already in Snap Zone
            if(grab.transform.parent != null && grab.transform.parent.GetComponent<SnapZone>() != null) {
                return;
            }

            if(HeldItem != null) {
                ReleaseAll();
            }

            HeldItem = grab;
            heldItemRigid = HeldItem.GetComponent<Rigidbody>();

            // Mark as kinematic so it doesn't fall down
            if(heldItemRigid) {
                heldItemWasKinematic = heldItemRigid.isKinematic;
                heldItemRigid.isKinematic = true;
            }
            else {
                heldItemWasKinematic = false;
            }

            // Set the parent of the object 
            grab.transform.parent = transform;

            // Set scale factor            
            // Use SnapZoneScale if specified
            if (grab.GetComponent<SnapZoneScale>()) {
                _scaleTo = grab.GetComponent<SnapZoneScale>().Scale;
            }
            else {
                _scaleTo = ScaleItem;
            }

            // Is there an offset to apply?
            SnapZoneOffset off = grab.GetComponent<SnapZoneOffset>();
            if(off) {
                offset = off;
            }
            else {
                offset = grab.gameObject.AddComponent<SnapZoneOffset>();
                offset.LocalPositionOffset = Vector3.zero;
                offset.LocalRotationOffset = Vector3.zero;
            }

            // Lock into place
            if (offset) {
                HeldItem.transform.localPosition = offset.LocalPositionOffset;
                HeldItem.transform.localEulerAngles = offset.LocalRotationOffset;
            }
            else {
                HeldItem.transform.localPosition = Vector3.zero;
                HeldItem.transform.localEulerAngles = Vector3.zero;
            }

            // Disable the grabbable. This is picked up through a Grab Action
            disableGrabbable(grab);

            // Call Grabbable Event from SnapZone
            if (OnSnapEvent != null) {
                OnSnapEvent.Invoke(grab);
            }

            // Fire Off Events on Grabbable
            GrabbableEvents[] ge = grab.GetComponents<GrabbableEvents>();
            if (ge != null) {
                for (int x = 0; x < ge.Length; x++) {
                    ge[x].OnSnapZoneEnter();
                }
            }

            if (SoundOnSnap) {
                VRUtils.Instance.PlaySpatialClipAt(SoundOnSnap, transform.position, 0.75f);
            }
        }

        void disableGrabbable(Grabbable grab) {

            if (DisableColliders) {
                disabledColliders = grab.GetComponentsInChildren<Collider>(false).ToList();
                for (int x = 0; x < disabledColliders.Count; x++) {
                    disabledColliders[x].enabled = false;
                }
            }

            // Disable the grabbable. This is picked up through a Grab Action
            grab.enabled = false;
        }

        public Text missiontext; //미션 텍스트 수정변수
        Handlemanager handlemanager;//키잠김 처리

        public void GrabEquipped(Grabber grabber) {

                
            if (grabber != null) {
                if(HeldItem) {

                    
                    var g = HeldItem;

                    handlemanager = GameObject.Find("HandleLookAt").GetComponent<Handlemanager>();
                    //키와 접촉시 잠금/열림

                    //handlemanager.crushkey = false;//핸들매니저스크립트 중지

                    //missiontext.text = "미션:204호로가서 마음을 돌릴 수단 찾아보기"; //미션텍스트 수정

                    //Debug.Log("잠김");


                    keytool = GameObject.FindGameObjectWithTag("keytool");//미션포인트2 오라 받아오기
                    Destroy(keytool);//삭제

                    maptool = GameObject.FindGameObjectWithTag("maptool");//미션포인트2 오라 받아오기
                    Destroy(maptool);//삭제

                    if (DuplicateItemOnGrab) {

                        ReleaseAll();

                        // Position next to grabber if somewhat far away
                        if (Vector3.Distance(g.transform.position, grabber.transform.position) > 0.2f) {
                            g.transform.position = grabber.transform.position;
                        }

                        // Instantiate the object before it is grabbed
                        GameObject go = Instantiate(g.gameObject, transform.position, Quaternion.identity) as GameObject;
                        Grabbable grab = go.GetComponent<Grabbable>();

                        // Ok to attach it to snap zone now
                        this.GrabGrabbable(grab);

                        // Finish Grabbing the desired object
                        grabber.GrabGrabbable(g);

                       
                    }
                    else {
                        ReleaseAll();
                        
                        // Position next to grabber if somewhat far away
                        if (Vector3.Distance(g.transform.position, grabber.transform.position) > 0.2f) {
                            g.transform.position = grabber.transform.position;
                        }
                         // handlemanager.crushkey = true;//핸들매니저스크립트 시작
                        //Debug.Log("열림");
                        // Do grab
                        grabber.GrabGrabbable(g);


                    }
                }
            }
        }

      
        /// Release  everything snapped to us
        public void ReleaseAll() {

            // No need to keep checking
            if (HeldItem == null) {
                return;
            }

            // Still need to keep track of item if we can't fully drop it
            if (!CanDropItem && HeldItem != null) {
                trackedItem = HeldItem;
            }

            HeldItem.ResetScale();

            if (DisableColliders && disabledColliders != null) {
                foreach (var c in disabledColliders) {
                    if(c) {
                        c.enabled = true;
                    }
                }
            }
            disabledColliders = null;

            // Reset Kinematic status
            if(heldItemRigid) {
                heldItemRigid.isKinematic = heldItemWasKinematic;
            }

            HeldItem.enabled = true;
            HeldItem.transform.parent = null;

            // Play Unsnap sound
            if(HeldItem != null) {
                if (SoundOnSnap) {
                    VRUtils.Instance.PlaySpatialClipAt(SoundOnUnsnap, transform.position, 0.75f);
                }

                // Call event
                if (OnDetachEvent != null) {
                    OnDetachEvent.Invoke(HeldItem);
                }

                // Fire Off Grabbable Events
                GrabbableEvents[] ge = HeldItem.GetComponents<GrabbableEvents>();
                if (ge != null) {
                    for (int x = 0; x < ge.Length; x++) {
                        ge[x].OnSnapZoneExit();
                    }
                }
            }

            HeldItem = null;
        }
    }
}
