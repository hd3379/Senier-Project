using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimePyramid : MonoBehaviour
{
    GameObject ghost;
    Vector3 ghostPosition;

    float lifeTime;
    bool isCreated = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isCreated)
        {
            ghostPosition = ghost.transform.position;
            ghostPosition.y += 1.5f;
            Vector3 MovingVector = ghostPosition - transform.position;
            Vector3 DoMove = MovingVector * (Time.deltaTime*2.0f);
            lifeTime -= Time.deltaTime;
            if(lifeTime < 0.0f)
            {
                Destroy(gameObject);
            }
            transform.Translate(DoMove, Space.World);
        }
    }
    public void SetTimer(float time)
    {
        lifeTime = time;
        isCreated = true;
    }
    public void SetGhost(GameObject g)
    {
        ghost = g;
    }
}
