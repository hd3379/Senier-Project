using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



namespace BNG
{

    public enum MovementVector
    {
        HMD,
        Controller
    }

    public class SmoothLocomotion : MonoBehaviour
    {
      

        [Header("이동처리 : ")]
        public float MovementSpeed = 1.25f;

        [Tooltip("(Optional) 지정된 경우 이 변환의 전방 방향이 이동 방향을 결정합니다. ")]
        public Transform ForwardDirection;

        [Tooltip("이동할 방향을 결정하는 데 사용됩니다. 예: 왼쪽 엄지스틱 축 또는 터치패드. ")]
        public List<InputAxis> inputAxis = new List<InputAxis>() { InputAxis.LeftThumbStickAxis };

        [Tooltip("이동에 영향을 주는 데 사용되는 입력 작업")]
        public InputActionReference MoveAction;

        [Tooltip("참일 경우 W,A,S,D를 사용하여 이동할 수 있습니다. 테스트에 유용함.")]
        public bool AllowKeyboardInputs = true;

        [Tooltip("True이면 응용 프로그램에 포커스 또는 재생 창이 있는 경우에만 이동 이벤트가 전송됩니다(Unity Editor에서 실행되는 경우).")]
        public bool RequireAppFocus = true;

        [Header("걷기 : ")]
        public float SprintSpeed = 1.5f;//초기속도

        [Tooltip("스프린트를 시작하는 데 사용할 키. 또한 스프린트 키 다운() 기능을 재정의하여 스프린트 기준을 결정할 수 있습니다..")]
        public List<ControllerBinding> SprintInput = new List<ControllerBinding>() { ControllerBinding.None };

        [Tooltip("스프린트를 활성화하는 데 사용되는 Unity Input Action")]
        public InputActionReference SprintAction;

        [Header("Strafe : ")]
        public float StrafeSpeed = 1f;
        public float StrafeSprintSpeed = 1.25f;

        [Header("Jump : ")]
        [Tooltip("점프 중에 플레이어에게 적용되는 '힘'의 양")]
        public float JumpForce = 3f;

        [Tooltip("점프를 시작하는 데 사용할 키입니다. 점프 점검() 기능을 재정의하여 점프 기준을 결정할 수도 있습니다.")]
        public List<ControllerBinding> JumpInput = new List<ControllerBinding>() { ControllerBinding.None };

        [Tooltip("점프를 활성화하는 데 사용되는 Unity Input Action")]
        public InputActionReference JumpAction;

        [Header("Air Control : ")]
        [Tooltip("접지되지 않았을 때 플레이어가 움직일 수 있습니까? 조이스틱을 움직일 수 있고 플레이어가 접지되지 않은 경우에도 입력에 응답하도록 하려면 참으로 설정.")]
        public bool AirControl = true;

        [Tooltip("에어 컨트롤 = 참일 경우 플레이어가 공중에서 얼마나 빠르게 움직일 수 있는지 여부. 예: 0.5 = 플레이어가 이동 속도의 절반으로 이동합니다.")]
        public float AirControlSpeed = 1f;

        BNGPlayerController playerController;
        CharacterController characterController;

        // Left / Right
        float movementX;

        // Up / Down
        float movementY;

        // Forwards / Backwards
        float movementZ;

        bool movementDisabled = false;

        private float _verticalSpeed = 0; // Keep track of vertical speed

        #region Events
        public delegate void OnBeforeMoveAction();
        public static event OnBeforeMoveAction OnBeforeMove;

        public delegate void OnAfterMoveAction();
        public static event OnAfterMoveAction OnAfterMove;
        #endregion

        public virtual void Update()
        {
            CheckControllerReferences();
            UpdateInputs();
            MoveCharacter();
        }

        public virtual void CheckControllerReferences()
        {
            // 비활성화된 동안 구성 요소가 호출될 수 있으므로 여기서 참조를 확인하십시오.
            if (playerController == null)
            {
                playerController = GetComponentInParent<BNGPlayerController>();
            }

            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }
        }

        public virtual void UpdateInputs()
        {

            // 먼저 이전 프레임의 입력을 재설정합니다.
            movementX = 0;
            movementY = 0;
            movementZ = 0;

            //VR 컨트롤러 입력으로 시작
            Vector2 primaryAxis = GetMovementAxis();
            if (playerController && playerController.IsGrounded())
            {
                movementX = primaryAxis.x;
                movementZ = primaryAxis.y;
            }
            else if (AirControl)
            {
                movementX = primaryAxis.x * AirControlSpeed;//에어컨트롤 중력 적용
                movementZ = primaryAxis.y * AirControlSpeed;
            }

            // VR 입력이 사용되지 않는 경우 키보드 입력 확인
            if (AllowKeyboardInputs && movementX == 0 && movementZ == 0)
            {
                GetKeyBoardInputs();
            }

            if (CheckJump())
            {
                movementY += JumpForce;
            }

            if (CheckSprint())
            {
                movementX *= StrafeSprintSpeed;
                movementZ *= SprintSpeed;
            }
            else
            {
                movementX *= StrafeSpeed;
                movementZ *= MovementSpeed;
            }
        }

        public virtual Vector2 GetMovementAxis()
        {

            //입력 목록에서 0이 아닌 가장 큰 값 사용
            Vector3 lastAxisValue = Vector3.zero;

            // Check raw input bindings
            if (inputAxis != null)
            {
                for (int i = 0; i < inputAxis.Count; i++)
                {
                    Vector3 axisVal = InputBridge.Instance.GetInputAxisValue(inputAxis[i]);

                    //마지막 항목이 0이면 항상 이 값을 사용하십시오.
                    if (lastAxisValue == Vector3.zero)
                    {
                        lastAxisValue = axisVal;
                    }
                    else if (axisVal != Vector3.zero && axisVal.magnitude > lastAxisValue.magnitude)
                    {
                        lastAxisValue = axisVal;
                    }
                }
            }

            // 응용 프로그램 포커스가 있는 경우 Unity Input Action 확인
            bool hasRequiredFocus = RequireAppFocus == false || RequireAppFocus && Application.isFocused;
            if (MoveAction != null && hasRequiredFocus)
            {
                Vector3 axisVal = MoveAction.action.ReadValue<Vector2>();

                // 마지막 항목이 0이면 항상 이 값을 사용하십시오.
                if (lastAxisValue == Vector3.zero)
                {
                    lastAxisValue = axisVal;
                }
                else if (axisVal != Vector3.zero && axisVal.magnitude > lastAxisValue.magnitude)
                {
                    lastAxisValue = axisVal;
                }
            }

            return lastAxisValue;
        }

        public virtual void MoveCharacter() //이동처리
        {

            // Bail early if no elligible for movement
            if (movementDisabled || characterController == null)
            {
                return;
            }

            Vector3 moveDirection = new Vector3(movementX, movementY, movementZ);

            if (ForwardDirection != null)
            {
                moveDirection = ForwardDirection.TransformDirection(moveDirection);
            }
            else
            {
                moveDirection = transform.TransformDirection(moveDirection);
            }

            // 점프 입력값 체크
            if (playerController != null && playerController.IsGrounded() && !movementDisabled)
            {
                // 땅에 닿을 시 점프 초기화
                _verticalSpeed = 0;
                if (CheckJump())
                {
                    _verticalSpeed = JumpForce;
                }
            }

            moveDirection.y = _verticalSpeed;

            if (playerController)
            {
                playerController.LastPlayerMoveTime = Time.time;
            }

            if (moveDirection != Vector3.zero)
            {
                MoveCharacter(moveDirection * Time.deltaTime);
            }
        }

        public float Magnitude;

        public virtual void MoveCharacter(Vector3 motion)
        {

            // 이동이 필요하지 않은 경우 즉시 탈출 가능

            if (motion == null || motion == Vector3.zero)
            {
                return;
            }

            Magnitude = (float)Math.Round(motion.magnitude * 1000f) / 1000f;

            bool callEvents = Magnitude > 0.0f;

            CheckControllerReferences();

            // 이벤트 이동 전 호출
            if (callEvents)
            {
                OnBeforeMove?.Invoke();
            }

            if (characterController && characterController.enabled)
            {
                characterController.Move(motion);
            }

            //이벤트 이동 후 호출
            if (callEvents)
            {
                OnAfterMove?.Invoke();
            }
        }

        public virtual void GetKeyBoardInputs()
        {
            // 앞
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                movementZ += 1f;
            }
            // 뒤
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                movementZ -= 1f;
            }
            // 좌
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                movementX -= 1f;
            }
            // 우
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                movementX += 1f;
            }
        }

        public virtual bool CheckJump()
        {

            // 땅이 아닐시 점프 중복 방지
            if (playerController != null && !playerController.IsGrounded())
            {
                return false;
            }

            //바운드 컨트롤러 버튼 확인
            for (int x = 0; x < JumpInput.Count; x++)
            {
                if (InputBridge.Instance.GetControllerBindingValue(JumpInput[x]))
                {
                    return true;
                }
            }

            //Unity Input Action 값 확인
            if (JumpAction != null && JumpAction.action.ReadValue<float>() > 0)
            {
                return true;
            }

            return false;
        }

        public virtual bool CheckSprint()
        {

            // 바운드 컨트롤러 버튼 확인
            for (int x = 0; x < SprintInput.Count; x++)
            {
                if (InputBridge.Instance.GetControllerBindingValue(SprintInput[x]))
                {
                    return true;
                }
            }

            // Unity Input Action 값 확인
            if (SprintAction != null)
            {
                return SprintAction.action.ReadValue<float>() == 1f;
            }

            return false;
        }



        public virtual void EnableMovement()
        {
            movementDisabled = false;
        }

        public virtual void DisableMovement()
        {
            movementDisabled = true;
        }

    

        
    }
}

