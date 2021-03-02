using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class initPyramids : MonoBehaviour
{
    public GameObject Pyramid;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 PyramidPosition = transform.position;
            PyramidPosition.x += 3.0f * i;
            PyramidPosition.y += 5.0f;
            GameObject instance = Instantiate(Pyramid, PyramidPosition,
                  transform.rotation) as GameObject;
            //Destroy(instance, 2.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
