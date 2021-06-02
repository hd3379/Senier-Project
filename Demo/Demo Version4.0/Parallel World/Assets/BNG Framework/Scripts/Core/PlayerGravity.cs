using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {


    // 캐릭터 컨트롤러 또는 강체에 중력 적용
    public class PlayerGravity : MonoBehaviour {

        [Tooltip("참일 경우, 캐릭터 컨트롤러 구성 요소에 중력을 가하거나, CC가 없는 경우 강체 본체에 중력을 가합니다.")]

        public bool GravityEnabled = true;

        [Tooltip("캐릭터 컨트롤러 또는 강체 본체에 적용할 무게의 양. 기본값은 '물리.중력'입니다.")]
        public Vector3 Gravity = Physics.gravity;

        CharacterController characterController;
        SmoothLocomotion smoothLocomotion;

        Rigidbody playerRigidbody;

        private float _movementY;
        private Vector3 _initialGravityModifier;

        // FixedUpdate에서 Null 체크 저장
        private bool _validRigidBody = false;

        void Start() {
            characterController = GetComponent<CharacterController>();
            smoothLocomotion = GetComponentInChildren<SmoothLocomotion>();
            playerRigidbody = GetComponent<Rigidbody>();

            _validRigidBody = playerRigidbody != null;

            _initialGravityModifier = Gravity;
        }

        // 업데이트에서 문자 이동이 적용된 후 적용할 수 있도록 LateUpdate에서 Gravity 적용
        void LateUpdate() {

            // 플레이어 컨트롤러에 중력 적용
            if (GravityEnabled && characterController != null && characterController.enabled) {
                _movementY += Gravity.y * Time.deltaTime;

                // 부드러운 이동으로 기본 설정
                if (smoothLocomotion) {
                    smoothLocomotion.MoveCharacter(new Vector3(0, _movementY, 0) * Time.deltaTime);
                }
                // 플레이어 컨트롤러로 돌아가기
                else if (characterController) {
                    characterController.Move(new Vector3(0, _movementY, 0) * Time.deltaTime);
                }

                // 바닥에 닿았을 경우 Y 이동을 재설정합니다.
                if (characterController.isGrounded) {
                    _movementY = 0;
                }
            }
        }

        void FixedUpdate() {
            // 강체(Rigidbody) 컨트롤러에 중력 적용
            if (_validRigidBody && GravityEnabled) {
                playerRigidbody.AddForce(Gravity * (playerRigidbody.mass * playerRigidbody.mass));
            }
        }

        public void ToggleGravity(bool gravityOn) {

            GravityEnabled = gravityOn;

            if (gravityOn) {
                Gravity = _initialGravityModifier;
            }
            else {
                Gravity = Vector3.zero;
            }
        }
    }
}

