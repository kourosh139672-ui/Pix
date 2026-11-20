using UnityEngine;

/// <summary>Put this on food and small props that Jerry or Tom can carry.</summary>
[RequireComponent(typeof(Collider))]
public class CarryableItem : MonoBehaviour
{
    public string itemName = "Food";
    public Rigidbody Body => GetComponent<Rigidbody>();

    private void Awake()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = false;
        if (GetComponent<Rigidbody>() == null)
        {
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 0.25f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }
}