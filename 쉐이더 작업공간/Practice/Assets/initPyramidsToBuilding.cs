using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class initPyramidsToBuilding : MonoBehaviour
{
    public GameObject Pyramid;
    public Mesh mesh;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        CreatePyramids();
    }
    protected static float InitPyramid = 0;
    // Update is called once per frame

    void CreatePyramids()
    {
        Bounds bounds = mesh.bounds;
        Vector3 BoundSize;
        BoundSize.x = mesh.bounds.size.x * transform.localScale.x;
        BoundSize.y = mesh.bounds.size.y * transform.localScale.y;
        BoundSize.z = mesh.bounds.size.z * transform.localScale.z;
        int PyramidNum = 10;
        Pyramid.transform.localScale = (BoundSize / PyramidNum) * 500;
        for (int i = 0; i < PyramidNum; i++)
        {
            for (int j = 0; j < PyramidNum; j++)
            {
                //정면
                Vector3 PyramidPosition = transform.position - (BoundSize/2);
                //PyramidPosition.y += (BoundSize.y / 2);
                PyramidPosition.x += (BoundSize.x / PyramidNum) * i;
                PyramidPosition.y += (BoundSize.y / PyramidNum) * j;
                PyramidPosition.z += BoundSize.z;
                Instantiate(Pyramid, PyramidPosition, transform.rotation);

                //왼면
                PyramidPosition = transform.position - (BoundSize / 2);
                //PyramidPosition.y += (BoundSize.y / 2);
                PyramidPosition.y += (BoundSize.y / PyramidNum) * i;
                PyramidPosition.z += (BoundSize.z / PyramidNum) * j;
                Instantiate(Pyramid, PyramidPosition, transform.rotation);

                //오른면
                PyramidPosition = transform.position - (BoundSize / 2);
                //PyramidPosition.y += (BoundSize.y / 2);
                PyramidPosition.x += BoundSize.x;
                PyramidPosition.y += (BoundSize.y / PyramidNum) * i;
                PyramidPosition.z += (BoundSize.z / PyramidNum) * j;
                Instantiate(Pyramid, PyramidPosition, transform.rotation);

                //뒷면
                PyramidPosition = transform.position - (BoundSize / 2);
                //PyramidPosition.y += (BoundSize.y / 2);
                PyramidPosition.x += (BoundSize.x / PyramidNum) * i;
                PyramidPosition.y += (BoundSize.y / PyramidNum) * j;
                Instantiate(Pyramid, PyramidPosition, transform.rotation);

                //윗면
                PyramidPosition = transform.position - (BoundSize / 2);
                //PyramidPosition.y += (BoundSize.y / 2);
                PyramidPosition.x += (BoundSize.x / PyramidNum) * i;
                PyramidPosition.y += BoundSize.y;
                PyramidPosition.z += (BoundSize.z / PyramidNum) * j;
                Instantiate(Pyramid, PyramidPosition, transform.rotation);

                //아랫면
                PyramidPosition = transform.position - (BoundSize / 2);
                //PyramidPosition.y += (BoundSize.y / 2);
                PyramidPosition.x += (BoundSize.x / PyramidNum) * i;
                PyramidPosition.z += (BoundSize.z / PyramidNum) * j;
                Instantiate(Pyramid, PyramidPosition, transform.rotation);
            }
        }
    }
    void Update()
    {
    }
}