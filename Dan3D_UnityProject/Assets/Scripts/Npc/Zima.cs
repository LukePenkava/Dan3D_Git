using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zima : MonoBehaviour
{
    PlayerManager player;
    NavAgent navAgent;

    public float speed = 5f; // Speed of the character
    public float pathUpdateInterval = 0.2f; // Interval to update the path
    public float waypointTolerance = 1f; // Distance to waypoint to consider as reached
    public float arriveDistance = 1.0f;
    bool hasArrived = false;

    public float maxSpringConstant = 50f; // Maximum spring constant
    public float minSpringConstant = 5f;  // Minimum spring constant for smooth stopping
    public float stopThreshold = 3f;      // Distance threshold for starting to stop

    public float dampingRatio = 1f; // Damping ratio (1 for critical damping)
    public float mass = 1f;
    private Vector3 velocity; // Current velocity of the agent

    bool isFollowingDan = false;
    float followingTimer = 0f;
    float followingWaitTime = 0f;

    private int currentWaypointIndex;
    private float pathUpdateTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        navAgent = GetComponent<NavAgent>();

        currentWaypointIndex = 0;
        pathUpdateTimer = pathUpdateInterval;
    }


    void Update()
    {
        if (navAgent.Inited)
        {
            if (isFollowingDan)
            {
                pathUpdateTimer -= Time.deltaTime;
                if (pathUpdateTimer <= 0f && hasArrived == false)
                {
                    UpdatePath();
                    pathUpdateTimer = pathUpdateInterval;
                }

                FollowPath();

                hasArrived = (Vector3.Distance(transform.position, GetPlayerPos()) <= arriveDistance) ? true : false;
                
                if(hasArrived) {
                    isFollowingDan = false;
                    followingTimer = 0f;
                    followingWaitTime = Random.Range(2f, 8f);
                }
            }
            else
            {
                followingTimer += Time.deltaTime;
                if(followingTimer >= followingWaitTime) {
                    isFollowingDan = true;
                }
            }
        }
    }

    void UpdatePath()
    {
        navAgent.GetPath(transform.position, GetPlayerPos());
        currentWaypointIndex = 0; // Reset waypoint index whenever the path is updated
    }

    Vector3 GetPlayerPos()
    {
        Vector3 playerPos = player.gameObject.transform.position;
        Vector3 targetPos = new Vector3(playerPos.x, playerPos.y + 1.5f, playerPos.z);
        return targetPos;
    }

    void FollowPath()
    {
        //Linear Movement
        // if (navAgent.CurrentPath == null || navAgent.CurrentPath.Count == 0 || currentWaypointIndex >= navAgent.CurrentPath.Count)
        // {
        //     return;
        // }

        // // Move towards the current waypoint
        // Vector3 targetPosition = navAgent.CurrentPath[currentWaypointIndex].WorldPosition;
        // Vector3 movementDirection = (targetPosition - transform.position).normalized;

        // // Rotate towards the target position
        // if (movementDirection != Vector3.zero)
        // {
        //     Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
        //     transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        // }

        // transform.position += movementDirection * speed * Time.deltaTime;

        // // Check if the waypoint is reached
        // if (Vector3.Distance(transform.position, targetPosition) < waypointTolerance)
        // {
        //     currentWaypointIndex++;
        // }

        if (navAgent.CurrentPath == null || navAgent.CurrentPath.Count == 0 || currentWaypointIndex >= navAgent.CurrentPath.Count)
        {
            return;
        }

        // Target position
        Vector3 targetPosition = navAgent.CurrentPath[currentWaypointIndex].WorldPosition;
        Vector3 displacement = targetPosition - transform.position;
        float distanceToTarget = displacement.magnitude;

        // Adjust spring constant for smooth stopping
        float springK = Mathf.Lerp(minSpringConstant, maxSpringConstant, distanceToTarget / stopThreshold);
        springK = Mathf.Clamp(springK, minSpringConstant, maxSpringConstant);

        // Spring-damper calculation
        Vector3 springForce = springK * displacement;
        Vector3 dampingForce = 2f * dampingRatio * Mathf.Sqrt(springK) * velocity;
        Vector3 force = springForce - dampingForce;

        // Update velocity
        velocity += (force / mass) * Time.deltaTime;

        // Update position
        transform.position += velocity * Time.deltaTime;

        // Rotate towards the target position
        if (velocity != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        }

        // Check if the waypoint is reached
        if (distanceToTarget < waypointTolerance)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= navAgent.CurrentPath.Count)
            {
                // Optionally, reset the velocity when the final waypoint is reached
                velocity = Vector3.zero;
            }
        }
    }
}
