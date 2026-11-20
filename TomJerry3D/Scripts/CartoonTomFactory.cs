using UnityEngine;

/// <summary>
/// Builds a rounded, classic-cartoon-style gray cat inspired by Tom.
/// Attach to an empty GameObject. The character faces local +Z.
/// This is a lightweight procedural prototype made from Unity primitives.
/// </summary>
public class CartoonTomFactory : MonoBehaviour
{
    [Header("Build")]
    public bool buildOnStart = true;
    public float size = 1f;

    [Header("Colors")]
    public Color fur = new Color(0.38f, 0.43f, 0.50f);
    public Color lightFur = new Color(0.88f, 0.82f, 0.72f);
    public Color innerEar = new Color(0.88f, 0.48f, 0.52f);
    public Color eyeColor = new Color(0.78f, 0.78f, 0.30f);
    public Color noseColor = new Color(0.88f, 0.43f, 0.48f);
    public Color dark = new Color(0.12f, 0.11f, 0.12f);

    private Transform modelRoot;

    private void Start()
    {
        if (buildOnStart) Build();
    }

    [ContextMenu("Build Cartoon Tom")]
    public void Build()
    {
        if (modelRoot != null)
        {
            if (Application.isPlaying) Destroy(modelRoot.gameObject);
            else DestroyImmediate(modelRoot.gameObject);
        }

        GameObject root = new GameObject("Tom_Cartoon_Model");
        modelRoot = root.transform;
        modelRoot.SetParent(transform, false);
        modelRoot.localPosition = Vector3.zero;
        modelRoot.localRotation = Quaternion.identity;
        modelRoot.localScale = Vector3.one * Mathf.Max(0.1f, size);

        // Broad torso and pale chest.
        Part("Torso", PrimitiveType.Capsule, new Vector3(0, 0.91f, -0.02f), new Vector3(0.82f, 0.91f, 0.58f), fur);
        Part("Chest Bib", PrimitiveType.Sphere, new Vector3(0, 0.93f, 0.43f), new Vector3(0.58f, 0.70f, 0.16f), lightFur);

        // Large cat head, cheeks and muzzle.
        Part("Head", PrimitiveType.Sphere, new Vector3(0, 1.88f, 0.12f), new Vector3(1.04f, 0.82f, 0.72f), fur);
        Part("Left Cheek", PrimitiveType.Sphere, new Vector3(-0.32f, 1.58f, 0.42f), new Vector3(0.48f, 0.34f, 0.28f), lightFur);
        Part("Right Cheek", PrimitiveType.Sphere, new Vector3(0.32f, 1.58f, 0.42f), new Vector3(0.48f, 0.34f, 0.28f), lightFur);
        Part("Muzzle", PrimitiveType.Sphere, new Vector3(0, 1.55f, 0.56f), new Vector3(0.50f, 0.28f, 0.24f), lightFur);
        Part("Nose", PrimitiveType.Sphere, new Vector3(0, 1.70f, 0.76f), new Vector3(0.15f, 0.11f, 0.09f), noseColor);

        // Tall triangular-looking ears assembled from tapered primitives.
        Ear(-0.62f, "Left");
        Ear(0.62f, "Right");

        // Big expressive eyes with colored irises and dark pupils.
        Eye(-0.25f, "Left");
        Eye(0.25f, "Right");

        // Whisker lines on both sides of the muzzle.
        Whisker(new Vector3(-0.34f, 1.55f, 0.68f), new Vector3(-0.88f, 1.62f, 0.70f), "Left Whisker Upper");
        Whisker(new Vector3(-0.34f, 1.48f, 0.68f), new Vector3(-0.91f, 1.43f, 0.70f), "Left Whisker Lower");
        Whisker(new Vector3(0.34f, 1.55f, 0.68f), new Vector3(0.88f, 1.62f, 0.70f), "Right Whisker Upper");
        Whisker(new Vector3(0.34f, 1.48f, 0.68f), new Vector3(0.91f, 1.43f, 0.70f), "Right Whisker Lower");

        // Arms and oversized soft paws.
        Link("Left Arm", new Vector3(-0.42f, 1.22f, 0.12f), new Vector3(-0.66f, 0.72f, 0.38f), 0.17f, fur);
        Link("Right Arm", new Vector3(0.42f, 1.22f, 0.12f), new Vector3(0.66f, 0.72f, 0.38f), 0.17f, fur);
        Part("Left Paw", PrimitiveType.Sphere, new Vector3(-0.67f, 0.67f, 0.40f), new Vector3(0.27f, 0.18f, 0.24f), lightFur);
        Part("Right Paw", PrimitiveType.Sphere, new Vector3(0.67f, 0.67f, 0.40f), new Vector3(0.27f, 0.18f, 0.24f), lightFur);

        // Strong legs and broad feet.
        Link("Left Leg", new Vector3(-0.28f, 0.60f, 0.00f), new Vector3(-0.34f, 0.20f, 0.10f), 0.20f, fur);
        Link("Right Leg", new Vector3(0.28f, 0.60f, 0.00f), new Vector3(0.34f, 0.20f, 0.10f), 0.20f, fur);
        Part("Left Foot", PrimitiveType.Sphere, new Vector3(-0.38f, 0.12f, 0.30f), new Vector3(0.40f, 0.18f, 0.52f), lightFur);
        Part("Right Foot", PrimitiveType.Sphere, new Vector3(0.38f, 0.12f, 0.30f), new Vector3(0.40f, 0.18f, 0.52f), lightFur);

        // Long curved tail, built from overlapping tapered segments.
        Vector3[] tail =
        {
            new Vector3(0.18f, 0.80f, -0.38f),
            new Vector3(0.42f, 0.70f, -0.65f),
            new Vector3(0.70f, 0.72f, -0.87f),
            new Vector3(0.92f, 0.91f, -1.03f),
            new Vector3(1.00f, 1.18f, -1.10f),
            new Vector3(0.94f, 1.40f, -1.12f)
        };
        for (int i = 0; i < tail.Length; i++)
        {
            float diameter = Mathf.Lerp(0.19f, 0.085f, i / (float)(tail.Length - 1));
            Part("Tail Segment " + i, PrimitiveType.Sphere, tail[i], Vector3.one * diameter, fur);
        }
    }

    private void Ear(float x, string side)
    {
        GameObject outer = Part(side + " Ear", PrimitiveType.Capsule,
            new Vector3(x, 2.55f, 0.02f), new Vector3(0.40f, 0.48f, 0.17f), fur);
        outer.transform.localRotation = Quaternion.Euler(0, 0, x < 0 ? -18f : 18f);

        GameObject pink = Part(side + " Inner Ear", PrimitiveType.Sphere,
            new Vector3(x, 2.56f, 0.17f), new Vector3(0.23f, 0.32f, 0.055f), innerEar);
        pink.transform.localRotation = outer.transform.localRotation;
    }

    private void Eye(float x, string side)
    {
        Part(side + " Eye White", PrimitiveType.Sphere,
            new Vector3(x, 1.98f, 0.53f), new Vector3(0.22f, 0.30f, 0.14f), lightFur);
        Part(side + " Iris", PrimitiveType.Sphere,
            new Vector3(x, 1.98f, 0.655f), new Vector3(0.13f, 0.22f, 0.07f), eyeColor);
        Part(side + " Pupil", PrimitiveType.Sphere,
            new Vector3(x + 0.015f, 1.98f, 0.715f), new Vector3(0.065f, 0.16f, 0.045f), dark);
        Part(side + " Eye Shine", PrimitiveType.Sphere,
            new Vector3(x + 0.04f, 2.07f, 0.755f), new Vector3(0.04f, 0.055f, 0.025f), Color.white);
    }

    private void Whisker(Vector3 start, Vector3 end, string partName)
    {
        LinkBetween(partName, start, end, 0.018f, lightFur);
    }

    private GameObject Part(string partName, PrimitiveType type, Vector3 localPosition, Vector3 localScale, Color color)
    {
        GameObject part = GameObject.CreatePrimitive(type);
        part.name = partName;
        part.transform.SetParent(modelRoot, false);
        part.transform.localPosition = localPosition;
        part.transform.localScale = localScale;

        Collider col = part.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Renderer renderer = part.GetComponent<Renderer>();
        renderer.sharedMaterial = MakeMaterial(color);
        return part;
    }

    private void Link(string partName, Vector3 start, Vector3 end, float radius, Color color)
    {
        LinkBetween(partName, start, end, radius, color);
    }

    private void LinkBetween(string partName, Vector3 start, Vector3 end, float radius, Color color)
    {
        Vector3 delta = end - start;
        GameObject link = Part(partName, PrimitiveType.Capsule, (start + end) * 0.5f,
            new Vector3(radius * 2f, delta.magnitude * 0.5f + radius, radius * 2f), color);
        link.transform.localRotation = Quaternion.FromToRotation(Vector3.up, delta.normalized);
    }

    private Material MakeMaterial(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        Material material = new Material(shader);
        material.color = color;
        return material;
    }
}
