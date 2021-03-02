using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pyramid : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float RotateSpeed = 200.0f;
    Vector3 RotationRate;
    Vector3 TransRate;
    // Start is called before the first frame update
    void Start()
    {
        float Ranx = Random.Range(-1.0f , 1.0f);
        float Rany = Random.Range(-1.0f, 1.0f);
        float Ranz = Random.Range(-1.0f, 1.0f);
        RotationRate = new Vector3(Ranx * RotateSpeed, Rany* RotateSpeed, Ranz * RotateSpeed);
        Ranx = Random.Range(-1.0f, 1.0f);
        Rany = Random.Range(-1.0f, 1.0f);
        Ranz = Random.Range(-1.0f, 1.0f);
        TransRate = new Vector3(Ranx * moveSpeed, Rany * moveSpeed, Ranz * moveSpeed);
    }

    // Update is called once per frame
    void Update()
    { 
        transform.Translate(TransRate * Time.deltaTime);
        transform.Rotate(RotationRate * Time.deltaTime, Space.World) ;
    }

}
