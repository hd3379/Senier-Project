using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointsController : MonoBehaviour
{
    public List<Transform> wayPoints = new List<Transform>();
    private Transform targetWayPoint;
    private int targetWayPointIndex = 0;
    private float minDistance = 0.1f;
    private int lastWayPointIndex;

    private float movementSpeed = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        targetWayPoint = wayPoints[targetWayPointIndex];
    }

    // Update is called once per frame
    void Update()
    {
        float mevementStep = movementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetWayPoint.position, mevementStep);
    }

    void CheckDistanceToWayPoint()
    {

    }

    void UpdateTargetWayPoint()
    {

    }
}
