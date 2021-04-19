using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pyramid : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float RotateSpeed = 200.0f;
    Vector3 RotationRate;
    Vector3 WorldPos;
    int WorldChange = 1;
    public Vector3 BlackhallPos;
    private Vector3 OriginPos;
    private Vector3 MovingVector;
    float movingTime;
    bool IsMove;
    // Start is called before the first frame update
    void Start()
    {
        float Ranx = Random.Range(-1.0f, 1.0f);
        float Rany = Random.Range(-1.0f, 1.0f);
        float Ranz = Random.Range(-1.0f, 1.0f);

        BlackhallPos = new Vector3(0.0f, 20.0f, 0.0f);
        RotationRate = new Vector3(Ranx * RotateSpeed, Rany * RotateSpeed, Ranz * RotateSpeed);
        WorldPos = this.gameObject.transform.position;
        OriginPos = WorldPos;
        IsMove = false;
        movingTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (WorldChange == 1)
        {
            if (IsMove == false)
            {
                if (Random.value > 0.995f) {
                    IsMove = true;
                    MovingVector = (BlackhallPos - WorldPos);
                }
                if (Time.time - movingTime >= 4.0f) //이동 못하고 있으면 4초후에는 출발
                {
                    IsMove = true;
                    MovingVector = (BlackhallPos - WorldPos);
                }
            }
            if (IsMove == true)
            {
                Vector3 DoMove = MovingVector * Time.deltaTime * 0.5f; //2초후에 도달
                if ((BlackhallPos - WorldPos).sqrMagnitude < (BlackhallPos - (WorldPos + DoMove)).sqrMagnitude)
                {
                    MovingVector.Set(0, 0, 0); // 움직이는게 더멀어지는경우 -> 다도착했다는 의미로 더 안움직이게
                    DoMove.Set(0, 0, 0);
                }
                WorldPos = WorldPos + (DoMove); 
                transform.Translate(DoMove, Space.World);
                if (Time.time - movingTime >= 8.0f)
                {
                    WorldChange = 2;
                    movingTime = Time.time;
                    IsMove = false;
                }
            }
        }
        else if (WorldChange == 2)
        {
            if (IsMove == false)
            {
                if (Random.value > 0.995f)
                {
                    IsMove = true;
                    MovingVector = (OriginPos - WorldPos);
                }
                if (Time.time - movingTime >= 4.0f) //이동 못하고 있으면 4초후에는 출발
                {
                    IsMove = true;
                    MovingVector = (OriginPos - WorldPos);
                }
            }
            if (IsMove == true)
            {
                Vector3 DoMove = MovingVector * Time.deltaTime * 0.5f; //2초후에 도달
                if ((OriginPos - WorldPos).sqrMagnitude < (OriginPos - (WorldPos + DoMove)).sqrMagnitude)
                {
                    MovingVector.Set(0, 0, 0); // 움직이는게 더멀어지는경우 -> 다도착했다는 의미로 더 안움직이게
                    DoMove.Set(0, 0, 0);
                }
                WorldPos = WorldPos + (DoMove);
                transform.Translate(DoMove, Space.World);
                if (Time.time - movingTime >= 8.0f)
                {
                    WorldChange = 2;
                    movingTime = Time.time;
                    IsMove = false;
                }
            }
        }
        else
        {
        }
        transform.Rotate(RotationRate * Time.deltaTime, Space.Self);
    }

}
