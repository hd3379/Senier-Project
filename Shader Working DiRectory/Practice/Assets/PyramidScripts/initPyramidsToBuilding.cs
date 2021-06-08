using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class initPyramidsToBuilding : MonoBehaviour
{
    public GameObject pyramid;
    public Renderer renderer;
    private bool state;
    float movingTime;

    // Start is called before the first frame update
    void Start()
    {
        state = true;
        CreatePyramids();
        movingTime = Time.time;
    }
    protected static float InitPyramid = 0;
    // Update is called once per frame

    void CreatePyramids()
    {
        Bounds bounds = GetComponent<Renderer>().bounds;
        Vector3 BoundSize;
        BoundSize.x = bounds.size.x;
        BoundSize.y = bounds.size.y;
        BoundSize.z = bounds.size.z;
        int PyramidNum = 10;
        pyramid.transform.localScale = (BoundSize / PyramidNum)*300;
        for (int i = 0; i < PyramidNum; i++)
        {
            for (int j = 0; j < PyramidNum; j++)
            {
                //정면
                Vector3 PyramidPosition = transform.position - (BoundSize/2);
                PyramidPosition.y += (BoundSize.y / 2); //3D모델의 좌표가 (0,0,0)이 바닥이냐 중심이냐에 따라 다르더라
                PyramidPosition.x += (BoundSize.x / PyramidNum) * i;
                PyramidPosition.y += (BoundSize.y / PyramidNum) * j;
                PyramidPosition.z += BoundSize.z;
                
                Instantiate(pyramid, PyramidPosition, transform.rotation);

                //왼면
                PyramidPosition = transform.position - (BoundSize / 2);
                PyramidPosition.y += (BoundSize.y / 2);
                PyramidPosition.y += (BoundSize.y / PyramidNum) * i;
                PyramidPosition.z += (BoundSize.z / PyramidNum) * j;
                Instantiate(pyramid, PyramidPosition, transform.rotation);

                //오른면
                PyramidPosition = transform.position - (BoundSize / 2);
                PyramidPosition.y += (BoundSize.y / 2);
                PyramidPosition.x += BoundSize.x;
                PyramidPosition.y += (BoundSize.y / PyramidNum) * i;
                PyramidPosition.z += (BoundSize.z / PyramidNum) * j;
                Instantiate(pyramid, PyramidPosition, transform.rotation);

                //뒷면
                PyramidPosition = transform.position - (BoundSize / 2);
                PyramidPosition.y += (BoundSize.y / 2);
                PyramidPosition.x += (BoundSize.x / PyramidNum) * i;
                PyramidPosition.y += (BoundSize.y / PyramidNum) * j;
                Instantiate(pyramid, PyramidPosition, transform.rotation);

                //윗면
                PyramidPosition = transform.position - (BoundSize / 2);
                PyramidPosition.y += (BoundSize.y / 2);
                PyramidPosition.x += (BoundSize.x / PyramidNum) * i;
                PyramidPosition.y += BoundSize.y;
                PyramidPosition.z += (BoundSize.z / PyramidNum) * j;
                Instantiate(pyramid, PyramidPosition, transform.rotation);

                //아랫면
                PyramidPosition = transform.position - (BoundSize / 2);
                PyramidPosition.y += (BoundSize.y / 2);
                PyramidPosition.x += (BoundSize.x / PyramidNum) * i;
                PyramidPosition.z += (BoundSize.z / PyramidNum) * j;
                Instantiate(pyramid, PyramidPosition, transform.rotation);
            }
        }
    }
    void Update()
    {
        if (state == true)
        {
            if (Time.time - movingTime >= 2.0f) //2초후에 시작
            {
                gameObject.GetComponent<Renderer>().material.SetFloat("isCapture", 0);
                state = false;
                movingTime = Time.time;
            }
        }
    }

    public void FinishWorldChange()
    {
    }
}