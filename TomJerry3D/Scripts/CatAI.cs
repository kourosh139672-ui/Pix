using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Basic cat AI: patrols between points, chases the mouse when seen,
/// then searches the last known position.
/// Requires a baked NavMesh and a NavMeshAgent.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class CatAI : MonoBehaviour
{
    private enum State { Patrol, Chase, Search }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] patrolPoints;

    [Header("Detection")]
    [SerializeField] private float sightDistance = 9f;
    [SerializeField, Range(1f, 180f)] private float fieldOfView = 100f;
    [SerializeField] private float eyeHeight = 0.8f;
    [SerializeField] private LayerMask lineOfSightMask = ~0;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 1.8f;
    [SerializeField] private float chaseSpeed = 3.8f;
    [SerializeField] private float searchDuration = 3f;
    [SerializeField] private float arriveDistance = 0.6f;

    private NavMeshAgent agent;
    private State state = State.Patrol;
    private int patrolIndex;
    private Vector3 lastKnownPosition;
    private float searchTimer;
    private float stunnedUntil;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Time.time < stunnedUntil)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;
        bool seesPlayer = CanSeePlayer();

        if (seesPlayer)
        {
            state = State.Chase;
            lastKnownPosition = player.position;
        }

        switch (state)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                if (!seesPlayer)
                {
                    state = State.Search;
                    searchTimer = searchDuration;
                    agent.SetDestination(lastKnownPosition);
                }
                break;
            case State.Search:
                searchTimer -= Time.deltaTime;
                if (searchTimer <= 0f)
                    state = State.Patrol;
                break;
        }
    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Vector3 target = player.position + Vector3.up * 0.5f;
        Vector3 toPlayer = target - origin;

        if (toPlayer.magnitude > sightDistance) return false;
        if (Vector3.Angle(transform.forward, toPlayer) > fieldOfView * 0.5f) return false;

        if (Physics.Raycast(origin, toPlayer.normalized, out RaycastHit hit,
                sightDistance, lineOfSightMask, QueryTriggerInteraction.Ignore))
            return hit.transform == player || hit.transform.IsChildOf(player);

        return false;
    }

    private void Patrol()
    {
        agent.speed = patrolSpeed;
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= arriveDistance)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            if (patrolPoints[patrolIndex] != null)
                agent.SetDestination(patrolPoints[patrolIndex].position);
        }
        else if (!agent.hasPath && patrolPoints[patrolIndex] != null)
        {
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    }

    private void Chase()
    {
        if (player == null) return;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    public void Stun(float seconds)
    {
        stunnedUntil = Mathf.Max(stunnedUntil, Time.time + Mathf.Max(0f, seconds));
    }
}
