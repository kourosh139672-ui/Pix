using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Lightweight procedural animation for the generated Tom model. Adds a soft
/// walk bounce, body sway, tail motion and head movement without external assets.
/// </summary>
public class CartoonCharacterMotion : MonoBehaviour
{
    [SerializeField] private float walkBounce = 0.055f;
    [SerializeField] private float walkSway = 5f;
    [SerializeField] private float tailSwing = 22f;
    private Transform model;
    private Transform head;
    private Transform tail;
    private Vector3 modelStart;
    private float phase;

    private void LateUpdate()
    {
        if (model == null)
        {
            model = transform.Find("Tom_Cartoon_Model");
            if (model == null) return;
            modelStart = model.localPosition;
            head = model.Find("Head");
            tail = model.Find("Tail Segment 5");
        }

        float speed = 0f;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null && agent.enabled && agent.isOnNavMesh)
            speed = agent.velocity.magnitude;

        phase += Time.deltaTime * Mathf.Lerp(1.5f, 10f, Mathf.Clamp01(speed / 4f));
        float amount = Mathf.Clamp01(speed / 2.5f);
        model.localPosition = modelStart + Vector3.up * Mathf.Abs(Mathf.Sin(phase)) * walkBounce * amount;
        model.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(phase * 0.5f) * walkSway * amount);

        if (head != null)
            head.localRotation = Quaternion.Euler(Mathf.Sin(phase * 0.35f) * 3f, Mathf.Sin(phase * 0.22f) * 5f, 0f);
        if (tail != null)
            tail.localRotation = Quaternion.Euler(0f, Mathf.Sin(phase * 0.8f) * tailSwing, Mathf.Cos(phase * 0.6f) * 12f);
    }
}
