using UnityEngine;
using UnityEngine.Events;

/// <summary>A mousetrap only catches Tom while a CarryableItem is installed as bait.</summary>
public class MouseTrapBaitSocket : MonoBehaviour
{
    [SerializeField] private Transform baitSocket;
    [SerializeField] private CatAI tom;
    [SerializeField] private float stunSeconds = 4f;
    [SerializeField] private bool consumeBait = true;
    private CarryableItem bait;
    private bool tomWasCaught;
    [SerializeField] private UnityEvent onTomCaught;

    public bool IsArmed => bait != null;
    public bool WasTriggered => tomWasCaught;

    public void Configure(Transform socket, CatAI cat)
    {
        baitSocket = socket;
        tom = cat;
    }

    public void PlaceBait(CarryableItem item)
    {
        if (item == null || bait != null) return;
        bait = item;
        Rigidbody rb = bait.GetComponent<Rigidbody>();
        if (rb != null) { rb.isKinematic = true; rb.useGravity = false; }
        Collider[] cols = bait.GetComponentsInChildren<Collider>();
        foreach (Collider c in cols) c.enabled = false;
        bait.transform.SetParent(baitSocket != null ? baitSocket : transform, false);
        bait.transform.localPosition = Vector3.zero;
        bait.transform.localRotation = Quaternion.identity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsArmed || tom == null || other.GetComponentInParent<CatAI>() != tom) return;
        if (tomWasCaught) return;
        tomWasCaught = true;
        tom.Stun(stunSeconds);
        onTomCaught?.Invoke();
        if (consumeBait)
        {
            Destroy(bait.gameObject);
            bait = null;
        }
    }

    public void PlaceHeldBait(CharacterHandPickup carrier)
    {
        if (carrier == null || !carrier.HasItem || bait != null) return;
        CarryableItem item = carrier.ReleaseHeldForPlacement();
        PlaceBait(item);
    }
}
