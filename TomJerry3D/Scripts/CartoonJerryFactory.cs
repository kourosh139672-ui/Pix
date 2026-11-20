using UnityEngine;

/// <summary>
/// Builds a cute, rounded, Jerry-inspired mouse from lightweight Unity primitives.
/// Attach to an empty GameObject. The model faces local +Z.
/// This is a procedural placeholder model; replace it with a rigged mesh later.
/// </summary>
public class CartoonJerryFactory : MonoBehaviour
{
    [Header("Build")]
    public bool buildOnStart = true;
    public bool buildOnReset = false;
    public float size = 1f;

    [Header("Colors")]
    public Color fur = new Color(0.48f, 0.25f, 0.12f);
    public Color innerEar = new Color(1f, 0.48f, 0.55f);
    public Color cream = new Color(1f, 0.83f, 0.61f);
    public Color eyeWhite = new Color(1f, 0.98f, 0.91f);
    public Color pupil = new Color(0.06f, 0.035f, 0.025f);

    private Transform modelRoot;

    private void Start()
    {
        if (buildOnStart) Build();
    }

    [ContextMenu("Build Cute Jerry")]
    public void Build()
    {
        if (modelRoot != null)
        {
            if (Application.isPlaying) Destroy(modelRoot.gameObject);
            else DestroyImmediate(modelRoot.gameObject);
        }

        GameObject root = new GameObject("Jerry_Cute_Model");
        modelRoot = root.transform;
        modelRoot.SetParent(transform, false);
        modelRoot.localPosition = Vector3.zero;
        modelRoot.localRotation = Quaternion.identity;
        modelRoot.localScale = Vector3.one * Mathf.Max(0.1f, size);

        // Pear-shaped body and soft belly.
        Part("Body", PrimitiveType.Capsule, new Vector3(0, 0.76f, 0), new Vector3(0.64f, 0.78f, 0.48f), fur);
        Part("Cream Belly", PrimitiveType.Sphere, new Vector3(0, 0.78f, 0.405f), new Vector3(0.46f, 0.62f, 0.13f), cream);

        // Big round head, cheeks and muzzle.
        Part("Head", PrimitiveType.Sphere, new Vector3(0, 1.62f, 0.12f), new Vector3(0.88f, 0.78f, 0.70f), fur);
        Part("Left Cheek", PrimitiveType.Sphere, new Vector3(-0.23f, 1.38f, 0.43f), new Vector3(0.38f, 0.27f, 0.23f), cream);
        Part("Right Cheek", PrimitiveType.Sphere, new Vector3(0.23f, 1.38f, 0.43f), new Vector3(0.38f, 0.27f, 0.23f), cream);
        Part("Muzzle", PrimitiveType.Sphere, new Vector3(0, 1.34f, 0.53f), new Vector3(0.43f, 0.25f, 0.20f), cream);
        Part("Tiny Nose", PrimitiveType.Sphere, new Vector3(0, 1.48f, 0.70f), new Vector3(0.13f, 0.095f, 0.09f), new Color(0.35f, 0.12f, 0.13f));

        // Oversized ears with pink inner surfaces.
        Part("Left Ear", PrimitiveType.Sphere, new Vector3(-0.48f, 2.08f, 0.02f), new Vector3(0.48f, 0.55f, 0.19f), fur);
        Part("Right Ear", PrimitiveType.Sphere, new Vector3(0.48f, 2.08f, 0.02f), new Vector3(0.48f, 0.55f, 0.19f), fur);
        Part("Left Pink Ear", PrimitiveType.Sphere, new Vector3(-0.48f, 2.09f, 0.19f), new Vector3(0.31f, 0.38f, 0.055f), innerEar);
        Part("Right Pink Ear", PrimitiveType.Sphere, new Vector3(0.48f, 2.09f, 0.19f), new Vector3(0.31f, 0.38f, 0.055f), innerEar);

        // Large friendly eyes, pupils and tiny highlights.
        Eye(-0.19f);
        Eye(0.19f);

        // Arms, little hands and feet.
        Link("Left Arm", new Vector3(-0.35f, 1.02f, 0.12f), new Vector3(-0.55f, 0.68f, 0.35f), 0.13f, fur);
        Link("Right Arm", new Vector3(0.35f, 1.02f, 0.12f), new Vector3(0.55f, 0.68f, 0.35f), 0.13f, fur);
        Part("Left Hand", PrimitiveType.Sphere, new Vector3(-0.56f, 0.66f, 0.37f), new Vector3(0.19f, 0.13f, 0.17f), cream);
        Part("Right Hand", PrimitiveType.Sphere, new Vector3(0.56f, 0.66f, 0.37f), new Vector3(0.19f, 0.13f, 0.17f), cream);
        Part("Left Foot", PrimitiveType.Sphere, new Vector3(-0.25f, 0.12f, 0.20f), new Vector3(0.34f, 0.17f, 0.46f), cream);
        Part("Right Foot", PrimitiveType.Sphere, new Vector3(0.25f, 0.12f, 0.20f), new Vector3(0.34f, 0.17f, 0.46f), cream);

        // Curved-looking tapered tail made from small overlapping round segments.
        Vector3[] tail =
        {
            new Vector3(0, 0.45f, -0.34f),
            new Vector3(0.08f, 0.38f, -0.62f),
            new Vector3(0.20f, 0.34f, -0.88f),
            new Vector3(0.34f, 0.43f, -1.12f),
            new Vector3(0.39f, 0.60f, -1.30f)
        };
        for (int i = 0; i < tail.Length; i++)
        {
            float d = Mathf.Lerp(0.13f, 0.055f, i / (float)(tail.Length - 1));
            Part("Tail Segment " + i, PrimitiveType.Sphere, tail[i], Vector3.one * d, fur);
        }
    }

    private void Eye(float x)
    {
        Part(x < 0 ? "Left Eye" : "Right Eye", PrimitiveType.Sphere,
            new Vector3(x, 1.72f, 0.43f), new Vector3(0.19f, 0.24f, 0.12f), eyeWhite);
        Part(x < 0 ? "Left Pupil" : "Right Pupil", PrimitiveType.Sphere,
            new Vector3(x + 0.025f, 1.72f, 0.535f), new Vector3(0.095f, 0.15f, 0.065f), pupil);
        Part("Eye Sparkle", PrimitiveType.Sphere,
            new Vector3(x + 0.045f, 1.79f, 0.59f), new Vector3(0.045f, 0.055f, 0.025f), Color.white);
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
