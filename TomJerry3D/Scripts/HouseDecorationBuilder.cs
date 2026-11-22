using UnityEngine;

/// <summary>
/// Adds lightweight decorative details to the generated house: framed windows,
/// wall pictures, ceiling beams, a dining set and indoor plants.
/// All pieces are simple generated meshes so the demo needs no external assets.
/// </summary>
public class HouseDecorationBuilder : MonoBehaviour
{
    private Material wood;
    private Material frame;
    private Material glass;
    private Material leaf;
    private Material pot;
    private Material canvas;

    private void Start()
    {
        wood = Mat(new Color(0.30f, 0.17f, 0.10f));
        frame = Mat(new Color(0.88f, 0.82f, 0.68f));
        glass = Mat(new Color(0.34f, 0.67f, 0.82f));
        leaf = Mat(new Color(0.18f, 0.48f, 0.22f));
        pot = Mat(new Color(0.66f, 0.29f, 0.17f));
        canvas = Mat(new Color(0.92f, 0.62f, 0.28f));

        BuildWindows();
        BuildWallPictures();
        BuildDiningSet();
        BuildPlants();
        BuildCeilingBeams();
        BuildCeilingLights();
        BuildWallClock();
        BuildKitchenDetails();
    }

    private void BuildWindows()
    {
        Window(new Vector3(-11.82f, 2.05f, -3.2f), new Vector3(0.12f, 1.35f, 2.2f));
        Window(new Vector3(11.82f, 2.05f, -2.0f), new Vector3(0.12f, 1.35f, 2.2f));
        Window(new Vector3(-11.82f, 2.05f, 5.8f), new Vector3(0.12f, 1.35f, 2.2f));
    }

    private void Window(Vector3 center, Vector3 size)
    {
        Cube("Window Glass", center, size * new Vector3(0.35f, 0.82f, 0.82f), glass);
        float x = size.x * 0.65f;
        Cube("Window Frame Top", center + new Vector3(0, size.y * 0.5f, 0), new Vector3(x, 0.10f, size.z), frame);
        Cube("Window Frame Bottom", center - new Vector3(0, size.y * 0.5f, 0), new Vector3(x, 0.10f, size.z), frame);
        Cube("Window Frame Left", center - new Vector3(0, 0, size.z * 0.5f), new Vector3(x, size.y, 0.10f), frame);
        Cube("Window Frame Right", center + new Vector3(0, 0, size.z * 0.5f), new Vector3(x, size.y, 0.10f), frame);
        Cube("Window Crossbar", center, new Vector3(x, 0.07f, 0.08f), frame);
    }

    private void BuildWallPictures()
    {
        Picture(new Vector3(-4.2f, 2.05f, 9.78f), false);
        Picture(new Vector3(5.0f, 2.05f, 9.78f), false);
        Picture(new Vector3(-11.78f, 2.0f, 0.0f), true);
    }

    private void Picture(Vector3 center, bool sideWall)
    {
        Vector3 board = sideWall ? new Vector3(0.08f, 0.95f, 1.25f) : new Vector3(1.25f, 0.95f, 0.08f);
        Cube("Picture Frame", center, board, wood);
        Vector3 inset = board * 0.78f;
        inset.x = Mathf.Max(0.04f, inset.x);
        inset.y = Mathf.Max(0.04f, inset.y);
        inset.z = Mathf.Max(0.04f, inset.z);
        Vector3 offset = sideWall ? new Vector3(0.055f, 0, 0) : new Vector3(0, 0, -0.055f);
        Cube("Picture Artwork", center + offset, inset, canvas);
    }

    private void BuildDiningSet()
    {
        Vector3 table = new Vector3(5.1f, 0.78f, 3.4f);
        Cube("Dining Table Top", table, new Vector3(2.4f, 0.16f, 1.45f), wood);
        for (int x = -1; x <= 1; x += 2)
        for (int z = -1; z <= 1; z += 2)
            Cube("Dining Table Leg", table + new Vector3(x * 0.95f, -0.42f, z * 0.52f),
                new Vector3(0.14f, 0.84f, 0.14f), wood);

        Chair(new Vector3(3.6f, 0f, 3.4f));
        Chair(new Vector3(6.6f, 0f, 3.4f));
        Chair(new Vector3(5.1f, 0f, 2.25f), true);
    }

    private void Chair(Vector3 p, bool rotated = false)
    {
        GameObject seat = Cube("Dining Chair Seat", p + new Vector3(0, 0.48f, 0),
            new Vector3(0.85f, 0.14f, 0.82f), frame);
        GameObject back = Cube("Dining Chair Back", p + new Vector3(0, 0.95f, -0.34f),
            new Vector3(0.85f, 0.9f, 0.14f), frame);
        if (rotated)
        {
            seat.transform.RotateAround(p, Vector3.up, 90f);
            back.transform.RotateAround(p, Vector3.up, 90f);
        }
        for (int x = -1; x <= 1; x += 2)
        for (int z = -1; z <= 1; z += 2)
            Cube("Chair Leg", p + new Vector3(x * 0.31f, 0.22f, z * 0.29f),
                new Vector3(0.10f, 0.44f, 0.10f), wood);
    }

    private void BuildPlants()
    {
        Plant(new Vector3(-10.4f, 0f, -7.8f));
        Plant(new Vector3(10.5f, 0f, 7.8f));
    }

    private void Plant(Vector3 p)
    {
        GameObject vessel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        vessel.name = "Plant Pot";
        vessel.transform.SetParent(transform, false);
        vessel.transform.position = p + new Vector3(0, 0.28f, 0);
        vessel.transform.localScale = new Vector3(0.62f, 0.28f, 0.62f);
        vessel.GetComponent<Renderer>().sharedMaterial = pot;

        GameObject stem = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        stem.name = "Plant Stem";
        stem.transform.SetParent(transform, false);
        stem.transform.position = p + new Vector3(0, 0.95f, 0);
        stem.transform.localScale = new Vector3(0.10f, 0.55f, 0.10f);
        stem.GetComponent<Renderer>().sharedMaterial = leaf;

        for (int i = 0; i < 5; i++)
        {
            float angle = i * 72f * Mathf.Deg2Rad;
            Vector3 leafPos = p + new Vector3(Mathf.Cos(angle) * 0.38f, 1.25f + (i % 2) * 0.18f, Mathf.Sin(angle) * 0.38f);
            GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            blade.name = "Plant Leaf";
            blade.transform.SetParent(transform, false);
            blade.transform.position = leafPos;
            blade.transform.localScale = new Vector3(0.28f, 0.12f, 0.48f);
            blade.transform.rotation = Quaternion.Euler(0, i * 72f, -20f);
            blade.GetComponent<Renderer>().sharedMaterial = leaf;
        }
    }

    private void BuildCeilingBeams()
    {
        for (int i = -1; i <= 1; i++)
            Cube("Ceiling Decorative Beam", new Vector3(i * 7.5f, 3.18f, 0f),
                new Vector3(0.16f, 0.18f, 19.2f), wood);
    }

    private void BuildCeilingLights()
    {
        for (int i = -1; i <= 1; i++)
        {
            Vector3 p = new Vector3(i * 7.0f, 2.88f, -1.0f);
            GameObject shade = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            shade.name = "Warm Ceiling Lamp Shade";
            shade.transform.SetParent(transform, false);
            shade.transform.position = p;
            shade.transform.localScale = new Vector3(0.72f, 0.12f, 0.72f);
            shade.GetComponent<Renderer>().sharedMaterial = frame;

            GameObject bulb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bulb.name = "Ceiling Lamp Bulb";
            bulb.transform.SetParent(transform, false);
            bulb.transform.position = p + Vector3.down * 0.16f;
            bulb.transform.localScale = Vector3.one * 0.22f;
            bulb.GetComponent<Renderer>().sharedMaterial = canvas;

            GameObject lightObject = new GameObject("Warm Room Light");
            lightObject.transform.SetParent(transform, false);
            lightObject.transform.position = p + Vector3.down * 0.28f;
            Light point = lightObject.AddComponent<Light>();
            point.type = LightType.Point;
            point.color = new Color(1f, 0.78f, 0.52f);
            point.intensity = 0.65f;
            point.range = 7f;
            point.shadows = LightShadows.None;
        }
    }

    private void BuildWallClock()
    {
        Vector3 center = new Vector3(1.18f, 2.25f, 6.8f);
        GameObject clock = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        clock.name = "Kitchen Wall Clock";
        clock.transform.SetParent(transform, false);
        clock.transform.position = center;
        clock.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        clock.transform.localScale = new Vector3(0.72f, 0.10f, 0.72f);
        clock.GetComponent<Renderer>().sharedMaterial = frame;

        GameObject face = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        face.name = "Clock Face";
        face.transform.SetParent(transform, false);
        face.transform.position = center + Vector3.back * 0.065f;
        face.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        face.transform.localScale = new Vector3(0.59f, 0.035f, 0.59f);
        face.GetComponent<Renderer>().sharedMaterial = canvas;
    }

    private void BuildKitchenDetails()
    {
        // Small food jars and a fruit bowl make the counter feel lived in.
        for (int i = 0; i < 3; i++)
        {
            GameObject jar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            jar.name = "Kitchen Pantry Jar";
            jar.transform.SetParent(transform, false);
            jar.transform.position = new Vector3(5.2f + i * 0.75f, 2.02f, -8.05f);
            jar.transform.localScale = new Vector3(0.28f, 0.34f, 0.28f);
            jar.GetComponent<Renderer>().sharedMaterial = i == 1 ? glass : frame;
        }

        GameObject bowl = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bowl.name = "Fruit Bowl";
        bowl.transform.SetParent(transform, false);
        bowl.transform.position = new Vector3(6.7f, 1.98f, -4.3f);
        bowl.transform.localScale = new Vector3(0.9f, 0.20f, 0.58f);
        bowl.GetComponent<Renderer>().sharedMaterial = pot;

        for (int i = 0; i < 3; i++)
        {
            GameObject fruit = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            fruit.name = "Counter Fruit";
            fruit.transform.SetParent(transform, false);
            fruit.transform.position = new Vector3(6.35f + i * 0.32f, 2.14f, -4.3f + (i % 2) * 0.16f);
            fruit.transform.localScale = Vector3.one * 0.22f;
            fruit.GetComponent<Renderer>().sharedMaterial = i == 1 ? leaf : canvas;
        }
    }

    private GameObject Cube(string label, Vector3 position, Vector3 scale, Material material)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = label;
        obj.transform.SetParent(transform, false);
        obj.transform.position = position;
        obj.transform.localScale = scale;
        obj.GetComponent<Renderer>().sharedMaterial = material;
        return obj;
    }

    private Material Mat(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Diffuse");
        Material material = new Material(shader);
        material.color = color;
        return material;
    }
}
