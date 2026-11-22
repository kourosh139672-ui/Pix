using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// Cartoon slapstick: Tom bumps harmless props. After enough mess,
/// an owner-scolding event fires and Tom takes a short trip outside before returning.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class TomHouseChaos : MonoBehaviour
{
    [SerializeField] private float smashRadius = 0.8f;
    [SerializeField] private float checkEvery = 0.25f;
    [SerializeField] private int messLimit = 4;
    [SerializeField] private Transform outsidePoint;
    [SerializeField] private float timeoutOutside = 6f;
    [SerializeField] private UnityEvent<int> onMessChanged;
    [SerializeField] private UnityEvent onOwnerScolds;

    private NavMeshAgent agent;
    private int mess;
    private float nextCheck;
    private bool beingSentOut;
    private Vector3 homePosition;

    public void Configure(Transform exitPoint) { outsidePoint = exitPoint; }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        homePosition = transform.position;
    }

    private void Update()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh) return;
        if (beingSentOut || Time.time < nextCheck || agent.velocity.magnitude < 1.2f) return;

        nextCheck = Time.time + checkEvery;
        Collider[] hits = Physics.OverlapSphere(transform.position, smashRadius);
        foreach (Collider hit in hits)
        {
            BreakableProp prop = hit.GetComponentInParent<BreakableProp>();
            if (prop != null && prop.TryBreak())
            {
                mess++;
                onMessChanged?.Invoke(mess);
                if (mess >= messLimit && outsidePoint != null)
                    StartCoroutine(OwnerSendsTomOut());
                break;
            }
        }
    }

    private IEnumerator OwnerSendsTomOut()
    {
        if (!agent.isOnNavMesh) yield break;
        beingSentOut = true;
        onOwnerScolds?.Invoke();
        agent.isStopped = false;
        agent.SetDestination(outsidePoint.position);

        float timer = 0f;
        while (timer < 12f)
        {
            if (!agent.pathPending && (!agent.hasPath || agent.remainingDistance <= 0.8f)) break;
            timer += Time.deltaTime;
            yield return null;
        }

        agent.isStopped = true;
        yield return new WaitForSeconds(timeoutOutside);

        if (NavMesh.SamplePosition(homePosition, out NavMeshHit homeHit, 2f, NavMesh.AllAreas))
            agent.Warp(homeHit.position);

        agent.isStopped = false;
        mess = 0;
        onMessChanged?.Invoke(mess);
        beingSentOut = false;
    }
}
