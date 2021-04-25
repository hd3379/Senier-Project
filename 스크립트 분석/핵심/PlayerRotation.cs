using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BNG {
    //오른쪽 컨트롤러를 이용한 플레이어 회전 좁은공간에서 사용하지만 굳이 필요 없을거같다..!
    public enum RotationMechanic {
        Snap,
        Smooth
    }
    public class PlayerRotation : MonoBehaviour {

        [Header("Input")]
        [Tooltip("업데이트를 건너뛰려면 false로 설정")]
        public bool AllowInput = true;

        [Tooltip("왼쪽/오른쪽으로 회전할지 여부를 결정하는 데 사용됩니다. 예를 들어, 엄지스틱의 X축 일때 . -1은 왼쪽으로, 1은 오른쪽으로 스냅할 수 있습니다..")]
        public List<InputAxis> inputAxis = new List<InputAxis>() { InputAxis.RightThumbStickAxis };

        [Tooltip("플레이어 회전에 사용되는 Unity Input Action")]
        public InputActionReference RotateAction;

        [Header("Smooth / Snap Turning")]
        [Tooltip("스냅 회전은 고정된 양의 도를 차례로 회전합니다. 부드럽게 하면 플레이어가 선형으로 회전합니다..")]
        public RotationMechanic RotationType = RotationMechanic.Snap;

        [Header("Snap Turn Settings")]
        [Tooltip("회전각")]
        public float SnapRotationAmount = 45f;

        [Tooltip("입력으로 간주하려면 썸스틱 X축이 >= 이어야 합니다.")]
        public float SnapInputAmount = 0.75f;

        [Header("Smooth Turn Settings")]
        [Tooltip("회전 시 플레이어 회전 속도유형이 '매끄러움'으로 설정되었습니다.'")]
        public float SmoothTurnSpeed = 40f;

        [Tooltip("최소값")]
        public float SmoothTurnMinInput = 0.1f;

        float recentSnapTurnTime;

  
        //이 프레임의 회전 수

        float rotationAmount = 0;

        float xAxis;
        float previousXInput; //이전값

        #region Events
        public delegate void OnBeforeRotateAction();
        public static event OnBeforeRotateAction OnBeforeRotate;

        public delegate void OnAfterRotateAction();
        public static event OnAfterRotateAction OnAfterRotate;
        #endregion

        void Update() {

            if(!AllowInput) {
                return;
            }

            xAxis = GetAxisInput();

            if (RotationType == RotationMechanic.Snap) {
                DoSnapRotation(xAxis);
            }

            else if (RotationType == RotationMechanic.Smooth) {
                DoSmoothRotation(xAxis);
            }

            // 이후 검사를 위한 입력 저장
            previousXInput = xAxis;
        }

     
        // -1과 1 사이의 소수값을 반환하여 문자를 회전시킬 방향을 결정합니다.
      
        public virtual float GetAxisInput() {

            //입력 목록에서 0이 아닌 가장 큰 값 사용
            float lastVal = 0;

            // 값 입력 확인
            if (inputAxis != null) {
                for (int i = 0; i < inputAxis.Count; i++) {
                    float axisVal = InputBridge.Instance.GetInputAxisValue(inputAxis[i]).x;

                    //마지막 항목이 0이면 항상 이 값을 사용.
                    if (lastVal == 0) {
                        lastVal = axisVal;
                    }
                    else if (axisVal != 0 && axisVal > lastVal) {
                        lastVal = axisVal;
                    }
                }
            }

            //Unity 입력 작업 확인
            if (RotateAction != null) {
                float axisVal = RotateAction.action.ReadValue<Vector2>().x;
                //마지막 항목이 0이면 항상 이 값을 사용.
                if (lastVal == 0) {
                    lastVal = axisVal;
                }
                else if (axisVal != 0 && axisVal > lastVal) {
                    lastVal = axisVal;
                }
            }

            return lastVal;
        }

        public virtual void DoSnapRotation(float xInput) {

            // 입력을 검색하기 전에 회전량 재설정
            rotationAmount = 0;

            // 오른쪽 스냅
            if (xInput >= 0.1f && previousXInput < 0.1f) {
                rotationAmount += SnapRotationAmount;
            }
            // 왼쪽 스냅
            else if (xInput <= -0.1f && previousXInput > -0.1f) {
                rotationAmount -= SnapRotationAmount;
            }

            if(Math.Abs(rotationAmount) > 0) {

                // 회전 전 이벤트 호출
                OnBeforeRotate?.Invoke();

                // 회전 적용
                transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + rotationAmount, transform.eulerAngles.z));

                recentSnapTurnTime = Time.time;

                //회전 후 이벤트 호출
                OnAfterRotate?.Invoke();
            }
        }

        public virtual bool RecentlySnapTurned() {
            return Time.time - recentSnapTurnTime <= 0.1f;
        }

        public virtual void DoSmoothRotation(float xInput) {

            // 입력을 검색하기 전에 회전량 재설정
            rotationAmount = 0;

            // 부드럽게 오른쪽으로 회전
            if (xInput >= SmoothTurnMinInput) {
                rotationAmount += xInput * SmoothTurnSpeed * Time.deltaTime;
            }
            // 부드럽게 왼쪽으로 회전
            else if (xInput <= -SmoothTurnMinInput) {
                rotationAmount += xInput * SmoothTurnSpeed * Time.deltaTime;
            }

            // 회전 적용
            transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + rotationAmount, transform.eulerAngles.z));
        }
    }
}

