using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pyramid : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    public float RotateSpeed = 10.0f;
    Vector3 RotationRate;
    // Start is called before the first frame update
    void Start()
    {
        int Ranx = Random.Range(-1, 1);
        int Rany = Random.Range(-1, 1);
        int Ranz = Random.Range(-1, 1);
        RotationRate = new Vector3(Ranx * RotateSpeed, Rany* RotateSpeed, Ranz * RotateSpeed);
    }

    // Update is called once per frame
    void Update()
    { 
        transform.Rotate(RotationRate * Time.deltaTime, Space.World) ;
    }
}
