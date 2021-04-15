using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {

    public enum LocomotionType {
        Teleport,
        SmoothLocomotion,
        None
    }

   
    /// BNG Player 컨트롤러는 기본 플레이어 이동을 처리합니다.
  
    public class BNGPlayerController : MonoBehaviour
    {

        [Header("Camera Options : ")]//카메라 옵션

        [Tooltip("If true the CharacterController will move along with the HMD, as long as there are no obstacle's in the way")]//참일 경우, 문자 컨트롤러가 HMD와 함께 이동.
        public bool MoveCharacterWithCamera = true;

        [Tooltip("If true the CharacterController will rotate it's Y angle to match the HMD's Y angle")]//참이면 문자 컨트롤러가 Y 각도를 회전하여 HMD의 Y 각도와 일치시킵니다.
        public bool RotateCharacterWithCamera = true;

        [Header("Transform Setup ")]//변환 설정

        [Tooltip("The TrackingSpace represents your tracking space origin.")]//추적 공간은 추적 공간 원점을 나타냅니다.
        public Transform TrackingSpace;

        //CameraRig는 메인 카메라를 오프셋하는 데 사용되는 변환입니다. 메인 카메라는 이 부분에 대해 상위여야 합니다.
        [Tooltip("The CameraRig is a Transform that is used to offset the main camera. The main camera should be parented to this.")]
                 
        public Transform CameraRig;

        //일반적으로 CenterEyeAnchor는 메인 카메라를 포함하는 변환입니다.
        [Tooltip("The CenterEyeAnchor is typically the Transform that contains your Main Camera")]
        public Transform CenterEyeAnchor;

        [Header("Ground checks : ")]//점검
        [Tooltip("Raycast against these layers to check if player is grounded")]
        public LayerMask GroundedLayers;

     
        [Tooltip("How far off the ground the player currently is. 0 = Grounded, 1 = 1 Meter in the air.")] //현재 플레이어가 지상에서 얼마나 떨어져 있는지. 0 = 바닥, 1 = 1 미터 공중에서
        public float DistanceFromGround = 0;

        [Header("Player Capsule Settings : ")]//플레이어 접촉공간?
       
      
        [Tooltip("Minimum Height our Player's capsule collider can be (in meters)")]//플레이어의 캡슐 충돌 최소 높이(미터)
        public float MinimumCapsuleHeight = 0.4f;


       
        [Tooltip("Maximum Height our Player's capsule collider can be (in meters)")]//플레이어의 캡슐 충돌 최대 높이(미터)
        public float MaximumCapsuleHeight = 3f;        

        [HideInInspector]
        public float LastTeleportTime;

        [Header("Player Y Offset : ")]

        [Tooltip("Offset the height of the CharacterController by this amount")]//문자 컨트롤러의 높이를 오프셋합니다.
        public float CharacterControllerYOffset = -0.025f;


        /// Height of our camera in local coords 현지 좌표에서 카메라 높이

        [HideInInspector]
        public float CameraHeight;

        [Header("Misc : ")]
        //True이면 HMD가 활성화되거나 연결되지 않은 경우 카메라를 카메라 높이 상승으로 오프셋합니다. 이렇게 하면 카메라가 바닥으로 떨어지는 것을 방지하고 키보드 컨트롤을 사용할 수 있습니다.

        [Tooltip("If true the Camera will be offset by ElevateCameraHeight if no HMD is active or connected. This prevents the camera from falling to the floor and can allow you to use keyboard controls.")]
        public bool ElevateCameraIfNoHMDPresent = true;

        //HMD가 없는 경우 플레이어 카메라를 상승시키는 높이(미터) 및 카메라 상승 없음HMD가 있는 경우 카메라 상승. 1.65 = 약 5.4' 높이

        [Tooltip("How high (in meters) to elevate the player camera if no HMD is present and ElevateCameraIfNoHMDPresent is true. 1.65 = about 5.4' tall. ")]
        public float ElevateCameraHeight = 1.65f;


        /// If player goes below this elevation they will be reset to their initial starting position.
        /// If the player goes too far away from the center they may start to jitter due to floating point precisions.
        /// Can also use this to detect if player somehow fell through a floor. Or if the "floor is lava".
        
        /// 플레이어가 이 고도 아래로 내려가면 초기 시작 위치로 재설정됩니다.
        [Tooltip("Minimum Y position our player is allowed to go. Useful for floating point precision and making sure player didn't fall through the map.")]


        /// If player goes above this elevation they will be reset to their initial starting position.
        /// If the player goes too far away from the center they may start to jitter due to floating point precisions.

        //플레이어가 이 고도 이상으로 올라가면 초기 시작 위치로 재설정됩니다.
        public float MaxElevation = 6000f;

        [HideInInspector]
        public float LastPlayerMoveTime;

        // 조작할 컨트롤러
        CharacterController characterController;

        //smooth movement 사용 가능한 경우
        SmoothLocomotion smoothLocomotion;

        //선택적 구성 요소를 사용하여 마지막 이동 시간을 업데이트할 수 있습니다.
        PlayerClimbing playerClimbing;

        // 현재 아래에 있는 물체.
        public RaycastHit groundHit;
        Transform mainCamera;

        private Vector3 _initialPosition;

        void Start() {
            characterController = GetComponentInChildren<CharacterController>();
            smoothLocomotion = GetComponentInChildren<SmoothLocomotion>();

            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;

            if (characterController) {
                _initialPosition = characterController.transform.position;
            }
            else {
                _initialPosition = transform.position;
            }

            playerClimbing = GetComponentInChildren<PlayerClimbing>();
        }

        void Update() {

            // 카메라에 대한 온전성 검사
            if (mainCamera == null && Camera.main != null) {
                mainCamera = Camera.main.transform;
            }

            // 카메라 위치와 일치하도록 캐릭터 컨트롤러의 캡슐 높이 업데이트
            UpdateCharacterHeight();

            // 플레이어의 높이를 고려하여 카메라 리그 위치를 업데이트합니다.
            UpdateCameraRigPosition();

            // 카메라 장비를 배치한 후 메인 카메라의 높이를 업데이트할 수 있습니다.
            UpdateCameraHeight();

            CheckCharacterCollisionMove();

            if (characterController) {

                // TrackingSpace 카메라에 정렬
                if (RotateCharacterWithCamera) {
                    RotateTrackingSpaceToCamera();
                }
            }
        }
       
        void FixedUpdate() {

            UpdateDistanceFromGround();

            CheckPlayerElevationRespawn();
        }

        /// <summary>
        /// 플레이어가 지정된 최소/최대 고도 이상으로 이동했는지 확인합니다.
        ///플레이어는 부동 소수점 정밀도로 인해 물리학이 지터를 시작할 수 있으므로 6000 단위 이상 또는 이하로는 절대 안 됩니다

        public virtual void CheckPlayerElevationRespawn() {

            if(MinElevation == 0 && MaxElevation == 0) {
                return;
            }

            if(characterController != null && (characterController.transform.position.y < MinElevation || characterController.transform.position.y > MaxElevation)) {
                Debug.Log("플레이어가 범위를 벗어났습니다. 초기 위치로 돌아갑니다..");
                characterController.transform.position = _initialPosition;
            }
        }

        public virtual void UpdateDistanceFromGround() {

            if(characterController) {
                if (Physics.Raycast(characterController.transform.position, -characterController.transform.up, out groundHit, 20, GroundedLayers, QueryTriggerInteraction.Ignore)) {
                    DistanceFromGround = Vector3.Distance(characterController.transform.position, groundHit.point);
                    DistanceFromGround += characterController.center.y;
                    DistanceFromGround -= (characterController.height * 0.5f) + characterController.skinWidth;

                    // Round to nearest thousandth
                    DistanceFromGround = (float)Math.Round(DistanceFromGround * 1000f) / 1000f;
                }
                else {
                    DistanceFromGround = 9999f;
                }
            }
            // 문자 컨트롤러를 찾을 수 없습니다. 현재 변환 위치를 기준으로 거리 업데이트
            else
            {
                if (Physics.Raycast(transform.position, transform.up, out groundHit, 20, GroundedLayers, QueryTriggerInteraction.Ignore)) {
                    DistanceFromGround = Vector3.Distance(transform.position, groundHit.point);
                    // Round to nearest thousandth
                    DistanceFromGround = (float)Math.Round(DistanceFromGround * 1000f) / 1000f;
                }
                else {
                    DistanceFromGround = 9999f;
                }
            }
        }

        public virtual void RotateTrackingSpaceToCamera() {
            Vector3 initialPosition = TrackingSpace.position;
            Quaternion initialRotation = TrackingSpace.rotation;

            // 문자 컨트롤러를 올바른 회전/정렬로 이동합니다.
            characterController.transform.rotation = Quaternion.Euler(0.0f, CenterEyeAnchor.rotation.eulerAngles.y, 0.0f);

            // 이제 추적 공간을 초기 위치/회전 위치로 되돌릴 수 있습니다.
            TrackingSpace.position = initialPosition;
            TrackingSpace.rotation = initialRotation;
        }

        public virtual void UpdateCameraRigPosition() {

            float yPos = CharacterControllerYOffset;

            //캡슐 높이 및 중앙을 기준으로 캐릭터 컨트롤러 위치 가져오기
            if (characterController != null) {
                yPos = -(0.5f * characterController.height) + characterController.center.y + CharacterControllerYOffset;
            }

            //클라이밍 중에 캡슐을 약간 상쇄. 이를 통해 플레이어는 레지/플랫폼에 더 쉽게 몸을 올릴 수 있습니다.
            if (playerClimbing != null && playerClimbing.GrippingAtLeastOneClimbable()) {
                yPos -= 0.25f;
            }

            // HMD가 활성화되지 않은 경우 바닥에 앉지 않도록 리그를 약간 부딪치십시오.
            if (!InputBridge.Instance.HMDActive && ElevateCameraIfNoHMDPresent) {
                yPos += ElevateCameraHeight;
            }

            CameraRig.transform.localPosition = new Vector3(CameraRig.transform.localPosition.x, yPos, CameraRig.transform.localPosition.z);
        }

        public virtual void UpdateCharacterHeight() {
            float minHeight = MinimumCapsuleHeight;
            // HMD가 없는 경우 최소 높이를 높입니다. 이것은  플레이어가 정말 작아지는 것을 막는다.
            if (!InputBridge.Instance.HMDActive && minHeight < 1f) {
                minHeight = 1f;
            }

            // 카메라 높이를 기준으로 문자 높이를 업데이트합니다.
            if (characterController) {
                characterController.height = Mathf.Clamp(CameraHeight + CharacterControllerYOffset - characterController.skinWidth, minHeight, MaximumCapsuleHeight);

                // 만약 클라이밍을 한다면 캡슐 중심을 위로 올림.
                if (playerClimbing != null && playerClimbing.GrippingAtLeastOneClimbable()) {
                    characterController.height = playerClimbing.ClimbingCapsuleHeight;
                    characterController.center = new Vector3(0, playerClimbing.ClimbingCapsuleCenter, 0);
                }
                else {
                    characterController.center = new Vector3(0, -0.25f, 0);
                }
            }
        }

        public virtual void UpdateCameraHeight() {
            // 카메라 높이 업데이트
            if (CenterEyeAnchor) {
                CameraHeight = CenterEyeAnchor.localPosition.y;
            }
        }

     
        //캐릭터 컨트롤러를 새 카메라 위치로 이동합니다.
        public virtual void CheckCharacterCollisionMove() {

            if(!MoveCharacterWithCamera || characterController == null) {
                return;
            }
            
            Vector3 initialCameraRigPosition = CameraRig.transform.position;
            Vector3 cameraPosition = CenterEyeAnchor.position;
            Vector3 delta = cameraPosition - characterController.transform.position;
            
            // y 위치 무시
            delta.y = 0;

            // 캐릭터 컨트롤러와 Camera Rig 를 카메라 델타까지 이동
            if (delta.magnitude > 0.0f) {

                if(smoothLocomotion) {
                    smoothLocomotion.MoveCharacter(delta);
                }
                else if(characterController) {
                    characterController.Move(delta);
                }

                //카메라 리그를 제자리로 다시 이동
                CameraRig.transform.position = initialCameraRigPosition;
            }
        }

        public bool IsGrounded() {

            //캐릭터 컨트롤러에서 양성 반응이 있는지 여부
            if (characterController != null) {
                if(characterController.isGrounded) {
                    return true;
                }
            }

            // DistanceFromGround is a bit more reliable as we can give a bit of leniency in what's considered grounded
            return DistanceFromGround <= 0.001f;
        }
    }
}
