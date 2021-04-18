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
    }

    // Update is called once per frame
    void Update()
    {
        if (WorldChange == 1)
        {
            MovingVector = (BlackhallPos - WorldPos) * Time.deltaTime * 0.2f;  //5초에 걸쳐 적용
            WorldPos = WorldPos + (MovingVector);
            transform.Translate(MovingVector);
            if((MovingVector.sqrMagnitude) < 0.1f * Time.deltaTime * 0.2f)
            {
                WorldChange = 2;
            }
        }
        else if (WorldChange == 2)
        {
            MovingVector = (OriginPos - WorldPos) * Time.deltaTime * 0.2f;
            WorldPos = WorldPos + (MovingVector);
            transform.Translate(MovingVector);
            if ((MovingVector.sqrMagnitude) < 0.1f * Time.deltaTime * 0.2f)
            {
                WorldChange = 0;
            }
        }
        else
        {
        }
        transform.Rotate(RotationRate * Time.deltaTime, Space.World);
    }

}
