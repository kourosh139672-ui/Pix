using UnityEngine;

/// <summary>
/// A simple floor trap. Give it a trigger collider and assign the cat AI.
/// The trap activates once when the cat enters its trigger.
/// </summary>
public class CatTrap : MonoBehaviour
{
    [SerializeField] private CatAI cat;
    [SerializeField] private float stunSeconds = 2.5f;
    [SerializeField] private bool singleUse = true;

    private bool used;

    private void OnTriggerEnter(Collider other)
    {
        if (used || cat == null) return;
        if (other.GetComponentInParent<CatAI>() != cat) return;

        cat.Stun(stunSeconds);
        if (singleUse)
        {
            used = true;
            gameObject.SetActive(false);
        }
    }
}
