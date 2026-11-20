using UnityEngine;

/// <summary>
/// Add to a mousetrap with a trigger collider. Drop a CarryableItem into its bait socket
/// (or call PlaceBait). Tom is caught only while bait is installed.
/// </summary>
public class MouseTrapBaitSocket : MonoBehaviour
{
    [SerializeField] private Transform baitSocket;
    [SerializeField] private CatAI tom;
    [SerializeField] private float stunSeconds = 4f;
    [SerializeField] private bool consumeBait = true;
    private CarryableItem bait;

    public bool IsArmed => bait != null;

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
        tom.Stun(stunSeconds);
        if (consumeBait)
        {
            Destroy(bait.gameObject);
            bait = null;
        }
    }

    // Call this from a UI action after the held food is selected.
    public void PlaceHeldBait(CharacterHandPickup carrier)
    {
        if (carrier == null || !carrier.HasItem || bait != null) return;
        CarryableItem item = carrier.ReleaseHeldForPlacement();
        PlaceBait(item);
    }
}