using UnityEngine;

/// <summary>
/// Add to Jerry or Tom. Assign a camera/eye transform and a hand socket.
/// Call Interact() from a mobile button; keyboard E also works.
/// </summary>
public class CharacterHandPickup : MonoBehaviour
{
    [SerializeField] private Transform view;
    [SerializeField] private Transform handSocket;
    [SerializeField] private float reach = 2.4f;
    [SerializeField] private LayerMask itemMask = ~0;
    private CarryableItem held;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) Interact();
        if (held != null && handSocket != null)
        {
            held.transform.position = handSocket.position;
            held.transform.rotation = handSocket.rotation;
        }
    }

    public void Interact()
    {
        if (held != null) { Drop(); return; }
        if (view == null) view = Camera.main != null ? Camera.main.transform : transform;
        if (Physics.Raycast(view.position, view.forward, out RaycastHit hit, reach, itemMask, QueryTriggerInteraction.Collide))
        {
            CarryableItem item = hit.collider.GetComponentInParent<CarryableItem>();
            if (item == null)
            {
                hit.collider.SendMessageUpwards("PlaceHeldBait", this, SendMessageOptions.DontRequireReceiver);
                hit.collider.SendMessageUpwards("Interact", SendMessageOptions.DontRequireReceiver);
                return;
            }
            held = item;
            Rigidbody rb = held.GetComponent<Rigidbody>();
            if (rb != null) { rb.isKinematic = true; rb.useGravity = false; }
            Collider[] colliders = held.GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders) c.enabled = false;
            held.transform.SetParent(handSocket != null ? handSocket : transform, true);
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