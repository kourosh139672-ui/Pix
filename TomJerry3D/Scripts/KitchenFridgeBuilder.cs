using UnityEngine;

/// <summary>
/// Creates a simple kitchen fridge with an opening door and edible objects to carry.
/// Attach to the kitchen root and use a CharacterHandPickup ray to interact with the fridge.
/// </summary>
public class KitchenFridgeBuilder : MonoBehaviour
{
    [SerializeField] private Vector3 position = new Vector3(10f, 0f, 3.2f);
    [SerializeField] private bool buildOnStart = true;
    private Material fridgeMat;
    private Material doorMat;
    private Material handleMat;
    private Material foodMat;

    private void Start()
    {
        if (buildOnStart) Build();
    }

    [ContextMenu("Build Kitchen Fridge")]
    public void Build()
    {
        fridgeMat = MakeMat(new Color(0.82f, 0.86f, 0.90f));
        doorMat = MakeMat(new Color(0.93f, 0.95f, 0.97f));
        handleMat = MakeMat(new Color(0.35f, 0.40f, 0.46f));
        foodMat = MakeMat(new Color(1f, 0.68f, 0.12f));

        GameObject root = new GameObject("KitchenFridge");
        root.transform.SetParent(transform, false);
        root.transform.localPosition = position;

        Cube("Fridge Body", root.transform, new Vector3(0, 1.2f, 0), new Vector3(1.35f, 2.4f, 1f), fridgeMat);

        GameObject hinge = new GameObject("Door Hinge");
        hinge.transform.SetParent(root.transform, false);
        hinge.transform.localPosition = new Vector3(-0.62f, 1.2f, 0.52f);

        GameObject door = Cube("Fridge Door", hinge.transform, new Vector3(0.62f, 0, 0), new Vector3(1.25f, 2.3f, 0.12f), doorMat);
        Cube("Door Handle", door.transform, new Vector3(0.42f, 0, 0.58f), new Vector3(0.08f, 0.72f, 0.08f), handleMat);

        GameObject spawn = new GameObject("Food Spawn");
        spawn.transform.SetParent(root.transform, false);
        spawn.transform.localPosition = new Vector3(0, 0.8f, 0.9f);

        GameObject food = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        food.name = "Cheese Food Prefab";
        food.transform.localScale = new Vector3(0.36f, 0.24f, 0.30f);
        food.GetComponent<Renderer>().sharedMaterial = foodMat;
        food.AddComponent<CarryableItem>().itemName = "Cheese";
        food.SetActive(false);

        var dispenser = root.AddComponent<FridgeFoodDispenser>();
        dispenser.Configure(hinge.transform, spawn.transform, food);
    }

    private GameObject Cube(string label, Transform parent, Vector3 localPos, Vector3 scale, Material material)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = label;
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = localPos;
        obj.transform.localScale = scale;
        obj.GetComponent<Renderer>().sharedMaterial = material;
        return obj;
    }

    private Material MakeMat(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        Material mat = new Material(shader);
        mat.color = color;
        return mat;
    }
}