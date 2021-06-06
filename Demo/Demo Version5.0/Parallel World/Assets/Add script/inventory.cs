using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



namespace BNG
{


    public class inventory : MonoBehaviour
    {

        public GameObject inventoryUI;
        public GameObject inventoryUI2;
        bool checkonoff = false;


        [Tooltip("(Optional) 지정된 경우 이 변환의 전방 방향이 이동 방향을 결정합니다. ")]
        public Transform ForwardDirection;

        [Tooltip("이동할 방향을 결정하는 데 사용됩니다. 예: 왼쪽 엄지스틱 축 또는 터치패드. ")]
        public List<InputAxis> inputAxis = new List<InputAxis>() { InputAxis.LeftThumbStickAxis };


        [Tooltip("True이면 응용 프로그램에 포커스 또는 재생 창이 있는 경우에만 이동 이벤트가 전송됩니다(Unity Editor에서 실행되는 경우).")]
        public bool RequireAppFocus = true;


        [Tooltip("인벤토리를 시작하는 데 사용할 키입니다.")]
        public List<ControllerBinding> invenInput = new List<ControllerBinding>() { ControllerBinding.None };

        [Tooltip("인벤토리를 활성화하는 데 사용되는 Unity Input Action")]
        public InputActionReference invenAction;




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


            if (Checkinven())
            {

                if (checkonoff == false)
                {
                    inventoryUI.SetActive(true);
                    inventoryUI2.SetActive(true);
                    checkonoff = true;
                }
                else if (checkonoff == true)
                {

                    inventoryUI.SetActive(false);
                    inventoryUI2.SetActive(false);
                    checkonoff = false;
                }

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


            return lastAxisValue;
        }


        public virtual bool Checkinven()
        {

            // 땅이 아닐시 점프 중복 방지
            if (playerController != null && !playerController.IsGrounded())
            {
                return false;
            }

            //바운드 컨트롤러 버튼 확인
            for (int x = 0; x < invenInput.Count; x++)
            {
                if (InputBridge.Instance.GetControllerBindingValue(invenInput[x]))
                {
                    return true;
                }
            }

            //Unity Input Action 값 확인
            if (invenAction != null && invenAction.action.ReadValue<float>() > 0)
            {
                return true;
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

