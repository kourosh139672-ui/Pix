using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Cat AI with patrol, visual pursuit and a timed last-known-position search.
/// Search ends when Tom reaches the clue and looks around, or when its timer expires.
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
    [SerializeField] private float targetHeight = 0.5f;
    [SerializeField] private LayerMask lineOfSightMask = ~0;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 1.8f;
    [SerializeField] private float chaseSpeed = 3.8f;
    [SerializeField] private float searchDuration = 4.5f;
    [SerializeField] private float arriveDistance = 0.6f;
    [SerializeField] private float searchTurnSpeed = 100f;

    private NavMeshAgent agent;
    private State state = State.Patrol;
    private int patrolIndex;
    private Vector3 lastKnownPosition;
    private float searchTimer;
    private float searchTurn;
    private float stunnedUntil;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Configure(Transform target, Transform[] points)
    {
        player = target;
        patrolPoints = points;
        patrolIndex = 0;
    }

    private void Update()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh) return;
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
                    searchTurn = 0f;
                    agent.isStopped = false;
                    agent.SetDestination(lastKnownPosition);
                }
                break;
            case State.Search:
                Search();
                break;
        }
    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;
        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Vector3 target = player.position + Vector3.up * targetHeight;
        Vector3 toPlayer = target - origin;
        float distance = toPlayer.magnitude;
        if (distance > sightDistance || distance < 0.01f) return false;
        if (Vector3.Angle(transform.forward, toPlayer) > fieldOfView * 0.5f) return false;

        if (Physics.Raycast(origin, toPlayer.normalized, out RaycastHit hit,
                distance + 0.1f, lineOfSightMask, QueryTriggerInteraction.Ignore))
            return hit.transform == player || hit.transform.IsChildOf(player);

        return false;
    }

    private void Patrol()
    {
        agent.speed = patrolSpeed;
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        if (agent.pathPending) return;

        if (!agent.hasPath || agent.remainingDistance <= arriveDistance)
        {
            for (int attempt = 0; attempt < patrolPoints.Length; attempt++)
            {
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                if (patrolPoints[patrolIndex] != null)
                {
                    agent.SetDestination(patrolPoints[patrolIndex].position);
                    break;
                }
            }
        }
    }

    private void Chase()
    {
        if (player == null) return;
        agent.speed = chaseSpeed;
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    private void Search()
    {
        agent.speed = patrolSpeed * 0.85f;
        searchTimer -= Time.deltaTime;
        if (!agent.pathPending && (!agent.hasPath || agent.remainingDistance <= arriveDistance))
        {
            agent.isStopped = true;
            transform.Rotate(Vector3.up, searchTurnSpeed * Time.deltaTime);
            searchTurn += searchTurnSpeed * Time.deltaTime;
            if (searchTurn >= 300f) searchTimer = Mathf.Min(searchTimer, 0.5f);
        }
        if (searchTimer <= 0f)
        {
            agent.isStopped = false;
            state = State.Patrol;
            agent.ResetPath();
        }
    }

    public void Stun(float seconds)
    {
        stunnedUntil = Mathf.Max(stunnedUntil, Time.time + Mathf.Max(0f, seconds));
        if (agent != null && agent.enabled && agent.isOnNavMesh)
            agent.isStopped = true;
    }
}
