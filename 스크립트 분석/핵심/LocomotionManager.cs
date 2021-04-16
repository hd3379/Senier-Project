using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BNG {
    public class LocomotionManager : MonoBehaviour {

        [Header("Locomotion Type")]
        
        [Tooltip("플레이어에 저장된 항목이 없을 경우 사용할 기본 이동입니다. 0 = 순간이동. 1 = 부드러운 이동")]
        public LocomotionType DefaultLocomotion = LocomotionType.Teleport;

        LocomotionType selectedLocomotion = LocomotionType.Teleport;
        public LocomotionType SelectedLocomotion {
            get { return selectedLocomotion; }
        }

        [Header("Save / Loading")]
        [Tooltip("참일 경우 이동 유형이 저장되고 플레이어 프리프에서 로드됩니다.")]
        public bool LoadLocomotionFromPrefs = false;

        [Header("Input")]
        [Tooltip("이동 유형을 전환하는 데 사용할 키")]
        public List<ControllerBinding> locomotionToggleInput = new List<ControllerBinding>() { ControllerBinding.None };

        [Tooltip("이동 유형을 전환하는 데 사용되는 작업")]
        public InputActionReference LocomotionToggleAction;

        BNGPlayerController player;
        PlayerTeleport teleport;
        SmoothLocomotion smoothLocomotion;

        void Start() {
            player = GetComponent<BNGPlayerController>();
            teleport = GetComponent<PlayerTeleport>();

            // Load Locomotion Preference
            if (LoadLocomotionFromPrefs) {
                ChangeLocomotion(PlayerPrefs.GetInt("LocomotionSelection", 0) == 0 ? LocomotionType.Teleport : LocomotionType.SmoothLocomotion, false);
            }
            else {
                ChangeLocomotion(DefaultLocomotion, false);
            }
        }

        void OnEnable() //가능
        {
            if(LocomotionToggleAction) {
                LocomotionToggleAction.action.Enable();
                LocomotionToggleAction.action.performed += OnLocomotionToggle;
            }
        }

        void OnDisable() //불가능 
        {
            if (LocomotionToggleAction) {
                LocomotionToggleAction.action.Disable();
                LocomotionToggleAction.action.performed -= OnLocomotionToggle;
            }
        }

        public void OnLocomotionToggle(InputAction.CallbackContext context) {
            // Toggle the locomotion
            ChangeLocomotion(SelectedLocomotion == LocomotionType.SmoothLocomotion ? LocomotionType.Teleport : LocomotionType.SmoothLocomotion, LoadLocomotionFromPrefs);
        }

        public void UpdateTeleportStatus()//텔레포트상태
        {
            teleport.enabled = SelectedLocomotion == LocomotionType.Teleport;
        }

        public void ChangeLocomotion(LocomotionType locomotionType, bool save) {
            ChangeLocomotionType(locomotionType);

            if (save) {
                PlayerPrefs.SetInt("LocomotionSelection", locomotionType == LocomotionType.Teleport ? 0 : 1);
            }

            UpdateTeleportStatus();
        }

        public void ChangeLocomotionType(LocomotionType loc) {

            //  Smooth Locomotion 가능하게 만들기
            if (smoothLocomotion == null) {
                smoothLocomotion = GetComponent<SmoothLocomotion>();
            }

            selectedLocomotion = loc;

            if (teleport == null) {
                teleport = GetComponent<PlayerTeleport>();
            }

            toggleTeleport(selectedLocomotion == LocomotionType.Teleport);
            toggleSmoothLocomotion(selectedLocomotion == LocomotionType.SmoothLocomotion);
        }

        void toggleTeleport(bool enabled) {
            if (enabled) {
                teleport.EnableTeleportation();
            }
            else {
                teleport.DisableTeleportation();
            }
        }

        void toggleSmoothLocomotion(bool enabled) {
            if (smoothLocomotion) {
                smoothLocomotion.enabled = enabled;
            }
        }

        public void ToggleLocomotionType() {
            // Toggle 마지막 값을 기준으로 전환
            if (selectedLocomotion == LocomotionType.SmoothLocomotion) {
                ChangeLocomotionType(LocomotionType.Teleport);
            }
            else {
                ChangeLocomotionType(LocomotionType.SmoothLocomotion);
            }
        }
    }
}