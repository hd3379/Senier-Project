using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Moveable_ghost : MonoBehaviour
{
    NavMeshAgent agent;

    [SerializeField]
    public List<Transform> waypoints;
    private Transform targetWaypoint;
    private int targetWaypointIndex = 0;
    private float minDistance = 0.5f;
    private int lastWaypointIndex;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        lastWaypointIndex = waypoints.Count - 1;
        targetWaypoint = waypoints[targetWaypointIndex];
        agent.SetDestination(targetWaypoint.position);
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, targetWaypoint.position);
        CheckDistanceToWaypoint(distance);
    }

    void CheckDistanceToWaypoint(float currentDistance)
    {
        if(currentDistance <= minDistance)
        {
            targetWaypointIndex = Random.Range(0, lastWaypointIndex);
            UpdateTargetWaypoint();
        }
    }

    void UpdateTargetWaypoint()
    {
        if(targetWaypointIndex > lastWaypointIndex)
        {
            targetWaypointIndex = 0;
        }
        targetWaypoint = waypoints[targetWaypointIndex];
        agent.SetDestination(targetWaypoint.position);
    }
}
