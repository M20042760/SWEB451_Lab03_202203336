using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PatrolHazard : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField, Min(0.1f)] private float speed = 2f;
    [SerializeField, Min(0.01f)]
    private float arriveDistance = 0.1f;

    [SerializeField, Min(0.1f)]
    private float waitTime = 0.75f;

    [Header("Respawn Extension")]
    [SerializeField] private bool resetOnPlayerRespawn = true;
    [SerializeField, Min(0)] private int resetWaypointIndex = 0;

    private Rigidbody body;
    private int targetIndex;
    private float waitTimer;
    private bool isWaiting;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        body.isKinematic = true;
    }

    private void OnEnable()
    {
        ResetZone.PlayerRespawned += ResetPatrol;
    }

    private void OnDisable()
    {
        ResetZone.PlayerRespawned -= ResetPatrol;
    }

    private void ResetPatrol()
    {
        if (!resetOnPlayerRespawn ||
            waypoints == null ||
            waypoints.Length == 0)
        {
            return;
        }

        int startIndex = Mathf.Clamp(
            resetWaypointIndex,
            0,
            waypoints.Length - 1);

        body.position = waypoints[startIndex].position;
        targetIndex = startIndex;
        waitTimer = 0f;
        isWaiting = false;
    }

    private void FixedUpdate()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            return;
        }

        if (isWaiting)
        {
            waitTimer -= Time.fixedDeltaTime;

            if (waitTimer > 0f)
            {
                return;
            }

            isWaiting = false;
        }

        Vector3 targetPosition = waypoints[targetIndex].position;

        Vector3 nextPosition = Vector3.MoveTowards(
            body.position,
            targetPosition,
            speed * Time.fixedDeltaTime);

        body.MovePosition(nextPosition);

        if (Vector3.Distance(
            nextPosition,
            targetPosition) <= arriveDistance)
        {
            targetIndex =
                (targetIndex + 1) % waypoints.Length;

            isWaiting = true;
            waitTimer = waitTime;
        }
    }
}