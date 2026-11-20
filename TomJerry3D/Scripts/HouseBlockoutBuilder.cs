using UnityEngine;

/// <summary>
/// Builds a simple explorable house blockout at runtime.
/// Add to an empty GameObject in a Unity scene and press Play.
/// Replace primitive furniture with licensed/original models later.
/// </summary>
public class HouseBlockoutBuilder : MonoBehaviour
{
    [SerializeField] private Material floorMaterial;
    [SerializeField] private Material wallMaterial;
    [SerializeField] private Material accentMaterial;
    [SerializeField] private bool buildOnStart = true;

    private void Start()
    {
        if (buildOnStart) Build();
    }

    [ContextMenu("Build House Blockout")]
    public void Build()
    {
        // Main connected floor plan: living room, kitchen, bedroom and hall.
        Room("LivingRoom", new Vector3(0, 0, 0), new Vector2(12, 10));
        Room("Kitchen", new Vector3(10, 0, 0), new Vector2(8, 8));
        Room("Hallway", new Vector3(0, 0, 9), new Vector2(12, 4));
        Room("Bedroom", new Vector3(-9, 0, 9), new Vector2(8, 8));
        Room("Storage", new Vector3(10, 0, 9), new Vector2(6, 6));

        Furniture("Sofa", new Vector3(-2, 0.6f, -2), new Vector3(3, 1.2f, 1), accentMaterial);
        Furniture("CoffeeTable", new Vector3(1, 0.35f, -1), new Vector3(2, 0.7f, 1), wallMaterial);
        Furniture("KitchenIsland", new Vector3(10, 0.6f, 0), new Vector3(3, 1.2f, 2), accentMaterial);
        Furniture("Bed", new Vector3(-9, 0.45f, 9), new Vector3(3, 0.9f, 4), accentMaterial);
        Furniture("StorageCrates", new Vector3(11, 0.5f, 10), new Vector3(2, 1, 2), wallMaterial);

        // Cheese trail markers.
        CheeseSpot(new Vector3(-4, 0.25f, 1));
        CheeseSpot(new Vector3(3, 0.25f, -3));
        CheeseSpot(new Vector3(10, 0.25f, 3));
        CheeseSpot(new Vector3(-9, 0.25f, 11));
        CheeseSpot(new Vector3(11, 0.25f, 10));

        RenderSettings.ambientLight = new Color(0.55f, 0.55f, 0.58f);
        if (FindObjectOfType<Light>() == null)
        {
            var lightObject = new GameObject("HouseSun");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.15f;
            light.transform.rotation = Quaternion.Euler(50, -30, 0);
        }
    }

    private void Room(string roomName, Vector3 center, Vector2 size)
    {
        var root = new GameObject(roomName);
        root.transform.SetParent(transform, false);
        MakeCube("Floor", center + Vector3.down * 0.15f,
            new Vector3(size.x, 0.3f, size.y), floorMaterial, root.transform);

        float h = 3.2f;
        float t = 0.25f;
        // Four walls with openings left between room modules to suggest doorways.
        MakeCube("NorthWall", center + new Vector3(0, h / 2, size.y / 2),
            new Vector3(size.x, h, t), wallMaterial, root.transform);
        MakeCube("SouthWall", center + new Vector3(0, h / 2, -size.y / 2),
            new Vector3(size.x, h, t), wallMaterial, root.transform);
        MakeCube("EastWall", center + new Vector3(size.x / 2, h / 2, 0),
            new Vector3(t, h, size.y), wallMaterial, root.transform);
        MakeCube("WestWall", center + new Vector3(-size.x / 2, h / 2, 0),
            new Vector3(t, h, size.y), wallMaterial, root.transform);
    }

    private void Furniture(string label, Vector3 position, Vector3 scale, Material material)
    {
        MakeCube(label, position, scale, material, transform);
    }

    private void CheeseSpot(Vector3 position)
    {
        var cheese = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        cheese.name = "CheesePickup";
        cheese.transform.SetParent(transform, false);
        cheese.transform.position = position;
        cheese.transform.localScale = new Vector3(0.45f, 0.3f, 0.45f);
        var collider = cheese.GetComponent<SphereCollider>();
        collider.isTrigger = true;
        cheese.AddComponent<CheesePickup>();
        var glow = cheese.GetComponent<Renderer>();
        if (accentMaterial != null) glow.sharedMaterial = accentMaterial;
    }

    private static GameObject MakeCube(string label, Vector3 position, Vector3 scale,
        Material material, Transform parent)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = label;
        obj.transform.SetParent(parent, false);
        obj.transform.position = position;
        obj.transform.localScale = scale;
        if (material != null) obj.GetComponent<Renderer>().sharedMaterial = material;
        return obj;
    }
}
