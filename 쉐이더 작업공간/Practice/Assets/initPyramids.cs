using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class initPyramids : MonoBehaviour
{
    public GameObject Pyramid;
    // Start is called before the first frame update
    void Start()
    {
    }
    protected static float InitPyramid = 0;
    // Update is called once per frame
    void Update()
    {
        InitPyramid += Time.deltaTime;
        
        while(InitPyramid > 0.001f)
        { 
            InitPyramid -= 0.001f;
            Vector3 PyramidPosition = transform.position;
            GameObject instance = Instantiate(Pyramid, PyramidPosition,
                  transform.rotation);
            Destroy(instance, 1.0f);
        }
    }
        
}

