using UnityEngine;

/// <summary>
/// Simple interactive fridge. Aim at it and press E, or call Interact from a UI button.
/// Assign a food prefab that has CarryableItem and a collider.
/// </summary>
public class FridgeFoodDispenser : MonoBehaviour
{
    [SerializeField] private Transform door;
    [SerializeField] private Transform foodSpawn;
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private float openAngle = 105f;
    private bool isOpen;

    public void Configure(Transform fridgeDoor, Transform spawnPoint, GameObject food)
    {
        door = fridgeDoor;
        foodSpawn = spawnPoint;
        foodPrefab = food;
    }

    public void Interact()
    {
        isOpen = !isOpen;
        if (door != null)
            door.localRotation = Quaternion.Euler(0f, isOpen ? openAngle : 0f, 0f) * Quaternion.identity;
        if (isOpen && foodPrefab != null)
        {
            Transform spawn = foodSpawn != null ? foodSpawn : transform;
            GameObject food = Instantiate(foodPrefab, spawn.position, spawn.rotation);\n            food.SetActive(true);
        }
    }
}