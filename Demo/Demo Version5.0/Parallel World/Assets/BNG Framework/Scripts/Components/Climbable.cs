using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Grabbable을 기반으로  플레이어가 등반을 할 수 있도록 하는 스크립트

namespace BNG
{

  

    public class Climbable : Grabbable {

        PlayerClimbing playerClimbing;

        void Start()
        {
            // Climbable이 듀얼 그랩으로 설정되어 있는지 확인,아닐 시 떨어짐

            SecondaryGrabBehavior = OtherGrabBehavior.DualGrab;

            //손으로 쥐고있는지 확인

            GrabPhysics = GrabPhysics.None;

            CanBeSnappedToSnapZone = false;

            TwoHandedDropBehavior = TwoHandedDropMechanic.None;

            //기본값이 사용된 경우 브레이크 거리 사용 안 함
          
            if (BreakDistance == 1) 
            {
                BreakDistance = 0;
            }

            if(player != null) {
                playerClimbing = player.gameObject.GetComponentInChildren<PlayerClimbing>();
            }
        }

        //아이템 보유시
        public override void GrabItem(Grabber grabbedBy) 
        {
            // 캐릭터 이동 위치를 추적할 수 있도록 클라이머를 추가.



            if (playerClimbing) {
                playerClimbing.AddClimber(this, grabbedBy);
            }
            
            base.GrabItem(grabbedBy);        
        }

        //아이템 드롭시
        public override void DropItem(Grabber droppedBy)
        {
            if(droppedBy != null && playerClimbing != null) {
                playerClimbing.RemoveClimber(droppedBy);
            }
            
            base.DropItem(droppedBy);
        }
    }
}