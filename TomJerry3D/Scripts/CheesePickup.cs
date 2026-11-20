using UnityEngine;
using UnityEngine.Events;

/// <summary>Attach to a cheese object with a trigger collider.</summary>
public class CheesePickup : MonoBehaviour
{
    [SerializeField] private int cheeseValue = 1;
    [SerializeField] private UnityEvent<int> onCollected;
    private bool collected;

    private void OnTriggerEnter(Collider other)
    {
        if (collected || !other.CompareTag("Player")) return;

        var counter = other.GetComponent<MouseCheeseCounter>();
        if (counter == null) counter = other.GetComponentInParent<MouseCheeseCounter>();
        if (counter != null) counter.AddCheese(cheeseValue);

        collected = true;
        onCollected?.Invoke(cheeseValue);
        Destroy(gameObject);
    }
}
