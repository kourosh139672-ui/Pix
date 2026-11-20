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

        collected = true;
        onCollected?.Invoke(cheeseValue);
        Destroy(gameObject);
    }
}
