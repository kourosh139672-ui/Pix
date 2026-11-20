using System.Collections;
using UnityEngine;

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