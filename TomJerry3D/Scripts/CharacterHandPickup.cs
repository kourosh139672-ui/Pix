using UnityEngine;

/// <summary>
/// Add to Jerry or Tom. Assign a camera/eye transform and a hand socket.
/// Interact picks up or places food; ThrowHeld launches the carried item.
/// </summary>
public class CharacterHandPickup : MonoBehaviour
{
    [SerializeField] private Transform view;
    [SerializeField] private Transform handSocket;
    [SerializeField] private float reach = 2.4f;
    [SerializeField] private float throwForce = 5.5f;
    [SerializeField] private float throwLift = 1.4f;
    [SerializeField] private LayerMask itemMask = ~0;
    private CarryableItem held;
    private JerryHandVisual handVisual;

    public void Configure(Transform viewTransform, Transform socket)
    {
        view = viewTransform;
        handSocket = socket;
        handVisual = socket != null ? socket.GetComponent<JerryHandVisual>() : null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) Interact();
        if (Input.GetKeyDown(KeyCode.G)) ThrowHeld();
        if (held != null && handSocket != null)
        {
            held.transform.position = handSocket.position;
            held.transform.rotation = handSocket.rotation;
        }
    }

    public void Interact()
    {
        if (view == null) view = Camera.main != null ? Camera.main.transform : transform;

        if (held != null)
        {
            if (Physics.Raycast(view.position, view.forward, out RaycastHit targetHit,
                    reach, itemMask, QueryTriggerInteraction.Collide))
            {
                targetHit.collider.SendMessageUpwards("PlaceHeldBait", this, SendMessageOptions.DontRequireReceiver);
                if (!HasItem) return;
            }
            Drop();
            return;
        }

        if (Physics.Raycast(view.position, view.forward, out RaycastHit hit,
                reach, itemMask, QueryTriggerInteraction.Collide))
        {
            CarryableItem item = hit.collider.GetComponentInParent<CarryableItem>();
            if (item == null)
            {
                hit.collider.SendMessageUpwards("Interact", SendMessageOptions.DontRequireReceiver);
                return;
            }

            held = item;
            Rigidbody rb = held.GetComponent<Rigidbody>();
            if (rb != null) { rb.velocity = Vector3.zero; rb.angularVelocity = Vector3.zero; rb.isKinematic = true; rb.useGravity = false; }
            Collider[] colliders = held.GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders) c.enabled = false;
            held.transform.SetParent(handSocket != null ? handSocket : transform, true);
            if (handVisual != null) handVisual.PlayPickupMotion();
        }
    }

    public void ThrowHeld()
    {
        if (held == null) return;
        CarryableItem item = held;
        held = null;
        item.transform.SetParent(null, true);
        Collider[] colliders = item.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders) c.enabled = true;
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            Vector3 direction = view != null ? view.forward : transform.forward;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(direction * throwForce + Vector3.up * throwLift, ForceMode.VelocityChange);
            rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.VelocityChange);
        }
    }

    public void Drop()
    {
        if (held == null) return;
        CarryableItem item = held;
        held = null;
        item.transform.SetParent(null, true);
        Collider[] colliders = item.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders) c.enabled = true;
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null) { rb.isKinematic = false; rb.useGravity = true; }
    }

    public CarryableItem ReleaseHeldForPlacement()
    {
        CarryableItem item = held;
        held = null;
        if (item != null) item.transform.SetParent(null, true);
        return item;
    }

    public bool HasItem => held != null;
}
