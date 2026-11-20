using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// Cartoon slapstick: Tom knocks over tagged breakable props while moving quickly.
/// After enough mess, the owner gives him a comic scolding and sends him outside briefly.
/// No realistic damage or fighting is simulated.
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

    private void Awake() { agent = GetComponent<NavMeshAgent>(); }

    private void Update()
    {
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
        beingSentOut = true;
        onOwnerScolds?.Invoke();
        agent.isStopped = false;
        agent.SetDestination(outsidePoint.position);
        float timer = 0f;
        while (timer < 12f && agent.pathPending == false &&
               (agent.hasPath && agent.remainingDistance > 0.8f))
        {
            timer += Time.deltaTime;
            yield return null;
        }
        agent.isStopped = true;
        yield return new WaitForSeconds(timeoutOutside);
        transform.position = outsidePoint.position;
        agent.Warp(outsidePoint.position);
        agent.isStopped = false;
        mess = 0;
        onMessChanged?.Invoke(mess);
        beingSentOut = false;
    }
}

/// <summary>Put on a lightweight prop Tom can knock over.</summary>
public class BreakableProp : MonoBehaviour
{
    [SerializeField] private GameObject visualRoot;
    [SerializeField] private bool respawn = true;
    [SerializeField] private float respawnSeconds = 10f;
    private bool broken;

    public bool TryBreak()
    {
        if (broken) return false;
        broken = true;
        if (visualRoot != null) visualRoot.SetActive(false);
        else gameObject.SetActive(false);
        if (respawn) StartCoroutine(Respawn());
        return true;
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnSeconds);
        if (this == null) yield break;
        if (visualRoot != null) visualRoot.SetActive(true);
        else gameObject.SetActive(true);
        broken = false;
    }
}