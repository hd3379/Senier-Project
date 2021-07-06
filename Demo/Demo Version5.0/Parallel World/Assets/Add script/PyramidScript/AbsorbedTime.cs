using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbedTime : MonoBehaviour
{
    bool isCreated = false;
    public GameObject pyramid;
    private GameObject instancePyramid;
    TimePyramid timePyramid;

    Vector3 playerPosition;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isCreated)
        {
            
            //playerPosition.y += 0.1f;

            float Ranx = Random.Range(-1.0f, 1.0f);
            float Rany = Random.Range(-1.0f, 1.0f);
            float Ranz = Random.Range(-1.0f, 1.0f);
            playerPosition.x += Ranx;
            playerPosition.y += Rany;
            playerPosition.z += Ranz;

            instancePyramid = Instantiate(pyramid, playerPosition, transform.rotation);
            timePyramid = instancePyramid.GetComponent<TimePyramid>();
            timePyramid.SetTimer(1.0f);
            timePyramid.SetGhost(gameObject);
        }
    }

    //고스트에서 absorbedTime의 createpyramid를 호출 (player방향으로, 방출하고 사라지는 피라미드를..)
    public void CreatePyramid(bool isAbsorved, Vector3 p_Position){
        if (isAbsorved)
        {
            isCreated = true;
            playerPosition = p_Position;
        }
        else
        {
           isCreated = false;
        }
    }
}
