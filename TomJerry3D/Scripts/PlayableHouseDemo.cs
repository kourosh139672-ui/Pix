using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// One-component playable prototype scene. Add to an empty GameObject and press Play.
/// It builds a connected house, a first-person mouse controller, a cartoon cat,
/// touch controls, a fridge, cheese and a baitable mousetrap.
/// </summary>
public class PlayableHouseDemo : MonoBehaviour
{
    [Header("Build")]
    [SerializeField] private bool buildOnStart = true;
    [SerializeField] private bool buildNavigationAtRuntime = true;

    private Transform houseRoot;
    private NavMeshDataInstance navMeshInstance;
    private Material floorMat;
    private Material wallMat;
    private Material trimMat;
    private Material furnitureMat;
    private Material cheeseMat;

    private void Start()
    {
        if (buildOnStart) BuildDemo();
    }

    [ContextMenu("Build Playable House Demo")]
    public void BuildDemo()
    {
        if (transform.Find("Generated TomJerry House") != null) return;

        floorMat = MakeMaterial(new Color(0.66f, 0.48f, 0.31f));
        wallMat = MakeMaterial(new Color(0.91f, 0.84f, 0.70f));
        trimMat = MakeMaterial(new Color(0.40f, 0.24f, 0.15f));
        furnitureMat = MakeMaterial(new Color(0.28f, 0.39f, 0.49f));
        cheeseMat = MakeMaterial(new Color(1f, 0.67f, 0.08f));

        houseRoot = new GameObject("Generated TomJerry House").transform;
        houseRoot.SetParent(transform, false);

        BuildHouseGeometry();
        if (buildNavigationAtRuntime) BuildNavMesh();

        Transform player = BuildPlayer();
        CatAI cat = BuildTom(player);
        BuildFridge();
        BuildCheese(new Vector3(-5f, 0.25f, -1.5f));
        BuildCheese(new Vector3(1.8f, 0.25f, -5.8f));
        BuildCheese(new Vector3(7.5f, 0.25f, -1.8f));
        BuildMousetrap(cat, new Vector3(0.2f, 0f, 4.8f));
        BuildBreakableProps();

        Debug.Log("Tom & Jerry 3D demo built. Use the left joystick to move, swipe right to look, and USE to interact.");
    }

    private void BuildHouseGeometry()
    {
        // One connected floor plan: living room (left), kitchen (right),
        // hallway (middle), bedroom (back-left), storage (back-right).
        Cube("Whole House Floor", houseRoot, new Vector3(0f, -0.16f, 0f),
            new Vector3(24f, 0.32f, 20f), floorMat);

        const float wallHeight = 3.4f;
        const float wallThickness = 0.28f;

        // Outside walls.
        Cube("North Outer Wall", houseRoot, new Vector3(0, wallHeight / 2f, 10f),
            new Vector3(24f, wallHeight, wallThickness), wallMat);
        Cube("South Outer Wall", houseRoot, new Vector3(0, wallHeight / 2f, -10f),
            new Vector3(24f, wallHeight, wallThickness), wallMat);
        Cube("East Outer Wall", houseRoot, new Vector3(12f, wallHeight / 2f, 0),
            new Vector3(wallThickness, wallHeight, 20f), wallMat);
        Cube("West Outer Wall", houseRoot, new Vector3(-12f, wallHeight / 2f, 0),
            new Vector3(wallThickness, wallHeight, 20f), wallMat);

        // Internal partitions with wide door openings.
        WallSegment("Kitchen Divider A", new Vector3(1f, wallHeight / 2f, -7.5f), new Vector3(wallThickness, wallHeight, 5f));
        WallSegment("Kitchen Divider B", new Vector3(1f, wallHeight / 2f, -1.5f), new Vector3(wallThickness, wallHeight, 3f));
        WallSegment("Bedroom Divider A", new Vector3(-8f, wallHeight / 2f, 1f), new Vector3(8f, wallHeight, wallThickness));
        WallSegment("Bedroom Divider B", new Vector3(2f, wallHeight / 2f, 1f), new Vector3(6f, wallHeight, wallThickness));
        WallSegment("Storage Divider", new Vector3(9f, wallHeight / 2f, 1f), new Vector3(6f, wallHeight, wallThickness));

        // Doorway lintels and colored skirting help the rooms read clearly.
        Cube("Living Room Rug", houseRoot, new Vector3(-5.5f, 0.015f, -5.2f),
            new Vector3(5.5f, 0.03f, 3.4f), trimMat);
        Cube("Kitchen Mat", houseRoot, new Vector3(6.2f, 0.018f, -5.3f),
            new Vector3(3.2f, 0.035f, 2.2f), furnitureMat);

        // Simple furniture blockout.
        Cube("Sofa Base", houseRoot, new Vector3(-7.2f, 0.55f, -3.4f),
            new Vector3(3.2f, 1.1f, 1.2f), furnitureMat);
        Cube("Sofa Back", houseRoot, new Vector3(-7.2f, 1.1f, -2.85f),
            new Vector3(3.2f, 1.2f, 0.25f), furnitureMat);
        Cube("Coffee Table", houseRoot, new Vector3(-3.8f, 0.42f, -5.1f),
            new Vector3(2.2f, 0.12f, 1.2f), trimMat);
        Cube("Kitchen Counter", houseRoot, new Vector3(7.4f, 0.95f, -8.2f),
            new Vector3(6.8f, 1.9f, 0.8f), wallMat);
        Cube("Kitchen Island", houseRoot, new Vector3(5.3f, 0.55f, -4.3f),
            new Vector3(2.7f, 1.1f, 1.5f), trimMat);
        Cube("Bedroom Bed Base", houseRoot, new Vector3(-7.6f, 0.35f, 6.6f),
            new Vector3(3.4f, 0.7f, 4.2f), furnitureMat);
        Cube("Bedroom Pillow", houseRoot, new Vector3(-7.6f, 0.78f, 8.1f),
            new Vector3(1.8f, 0.18f, 0.8f), wallMat);
        Cube("Storage Crate One", houseRoot, new Vector3(7.8f, 0.55f, 5.6f),
            new Vector3(1.3f, 1.1f, 1.3f), trimMat);
        Cube("Storage Crate Two", houseRoot, new Vector3(9.5f, 0.4f, 7.4f),
            new Vector3(1.1f, 0.8f, 1.1f), trimMat);

        RenderSettings.ambientLight = new Color(0.62f, 0.60f, 0.56f);
        GameObject sunObject = new GameObject("House Directional Light");
        sunObject.transform.SetParent(houseRoot, false);
        sunObject.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
        Light sun = sunObject.AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.intensity = 1.1f;
    }

    private void WallSegment(string label, Vector3 center, Vector3 scale)
    {
        Cube(label, houseRoot, center, scale, wallMat);
    }

    private Transform BuildPlayer()
    {
        GameObject playerObject = new GameObject("Jerry Player - First Person");
        playerObject.transform.SetParent(houseRoot, false);
        playerObject.transform.position = new Vector3(-7.2f, 0f, -5.2f);
        playerObject.transform.rotation = Quaternion.Euler(0f, 38f, 0f);
        playerObject.tag = "Player";

        CharacterController characterController = playerObject.AddComponent<CharacterController>();
        characterController.height = 1.1f;
        characterController.radius = 0.28f;
        characterController.center = new Vector3(0f, 0.55f, 0f);
        characterController.stepOffset = 0.25f;

        GameObject pivot = new GameObject("Mouse Eye Camera Pivot");
        pivot.transform.SetParent(playerObject.transform, false);
        pivot.transform.localPosition = new Vector3(0f, 0.94f, 0f);

        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.SetParent(pivot.transform, false);
        cameraObject.transform.localPosition = Vector3.zero;
        cameraObject.transform.localRotation = Quaternion.identity;
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.fieldOfView = 78f;
        camera.nearClipPlane = 0.04f;
        cameraObject.AddComponent<AudioListener>();

        MousePlayerController controller = playerObject.AddComponent<MousePlayerController>();
        controller.Configure(pivot.transform);
        playerObject.AddComponent<MouseCheeseCounter>();

        GameObject hand = new GameObject("Jerry Hand Socket");
        hand.transform.SetParent(cameraObject.transform, false);
        hand.transform.localPosition = new Vector3(0.28f, -0.22f, 0.55f);
        CharacterHandPickup pickup = playerObject.AddComponent<CharacterHandPickup>();
        pickup.Configure(cameraObject.transform, hand.transform);

        MobileFirstPersonHUD hud = playerObject.AddComponent<MobileFirstPersonHUD>();
        hud.Configure(controller, pickup);
        return playerObject.transform;
    }

    private CatAI BuildTom(Transform player)
    {
        GameObject tom = new GameObject("Tom - Cat AI");
        tom.transform.SetParent(houseRoot, false);
        tom.transform.position = new Vector3(7.2f, 0.05f, -3.8f);
        tom.transform.rotation = Quaternion.LookRotation(player.position - tom.transform.position);

        CapsuleCollider body = tom.AddComponent<CapsuleCollider>();
        body.height = 1.55f;
        body.radius = 0.38f;
        body.center = new Vector3(0f, 0.78f, 0f);

        Rigidbody bodyPhysics = tom.AddComponent<Rigidbody>();
        bodyPhysics.isKinematic = true;
        bodyPhysics.useGravity = false;

        NavMeshAgent agent = tom.AddComponent<NavMeshAgent>();
        agent.height = 1.55f;
        agent.radius = 0.34f;
        agent.speed = 3.8f;
        agent.angularSpeed = 240f;
        agent.acceleration = 12f;
        agent.stoppingDistance = 0.8f;

        CartoonTomFactory model = tom.AddComponent<CartoonTomFactory>();
        model.size = 0.68f;
        model.buildOnStart = true;

        CatAI ai = tom.AddComponent<CatAI>();
        Transform[] patrol = new Transform[4];
        patrol[0] = MakePoint("Tom Patrol Kitchen", new Vector3(8f, 0.05f, -6f));
        patrol[1] = MakePoint("Tom Patrol Hall", new Vector3(0f, 0.05f, 2.8f));
        patrol[2] = MakePoint("Tom Patrol Living", new Vector3(-6f, 0.05f, -1f));
        patrol[3] = MakePoint("Tom Patrol Storage", new Vector3(8f, 0.05f, 6.5f));
        ai.Configure(player, patrol);

        if (agent.isOnNavMesh) agent.Warp(tom.transform.position);
        return ai;
    }

    private Transform MakePoint(string label, Vector3 position)
    {
        GameObject point = new GameObject(label);
        point.transform.SetParent(houseRoot, false);
        point.transform.position = position;
        return point.transform;
    }

    private void BuildFridge()
    {
        GameObject fridgeBuilderObject = new GameObject("Fridge Builder");
        fridgeBuilderObject.transform.SetParent(houseRoot, false);
        KitchenFridgeBuilder builder = fridgeBuilderObject.AddComponent<KitchenFridgeBuilder>();
        builder.Configure(new Vector3(9.4f, 0f, -7.6f));
        builder.Build();
    }

    private void BuildCheese(Vector3 position)
    {
        GameObject cheese = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        cheese.name = "Collectible Cheese";
        cheese.transform.SetParent(houseRoot, false);
        cheese.transform.position = position;
        cheese.transform.localScale = new Vector3(0.42f, 0.28f, 0.42f);
        cheese.GetComponent<Renderer>().sharedMaterial = cheeseMat;
        SphereCollider trigger = cheese.GetComponent<SphereCollider>();
        trigger.isTrigger = true;
        cheese.AddComponent<CheesePickup>();
    }

    private void BuildMousetrap(CatAI cat, Vector3 position)
    {
        GameObject trap = new GameObject("Baitable Mousetrap");
        trap.transform.SetParent(houseRoot, false);
        trap.transform.position = position;

        GameObject basePlate = Cube("Trap Base", trap.transform,
            new Vector3(0f, 0.08f, 0f), new Vector3(0.95f, 0.16f, 0.72f), trimMat);
        basePlate.transform.localPosition = new Vector3(0f, 0.08f, 0f);

        BoxCollider trigger = trap.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(1.2f, 0.8f, 1.1f);
        trigger.center = new Vector3(0f, 0.35f, 0f);

        Rigidbody rb = trap.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        GameObject socket = new GameObject("Trap Bait Socket");
        socket.transform.SetParent(trap.transform, false);
        socket.transform.localPosition = new Vector3(0f, 0.28f, 0f);

        MouseTrapBaitSocket baitSocket = trap.AddComponent<MouseTrapBaitSocket>();
        baitSocket.Configure(socket.transform, cat);
    }

    private void BuildBreakableProps()
    {
        CreateBreakable("Breakable Vase", new Vector3(-2.2f, 0.45f, -2.1f), new Vector3(0.55f, 0.9f, 0.55f));
        CreateBreakable("Breakable Box", new Vector3(3.4f, 0.4f, 4.8f), new Vector3(0.8f, 0.8f, 0.8f));
        CreateBreakable("Breakable Stool", new Vector3(5.5f, 0.35f, -1.2f), new Vector3(0.75f, 0.7f, 0.75f));
    }

    private void CreateBreakable(string label, Vector3 position, Vector3 scale)
    {
        GameObject prop = Cube(label, houseRoot, position, scale, cheeseMat);
        prop.AddComponent<BreakableProp>();
    }

    private void BuildNavMesh()
    {
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        NavMeshBuilder.CollectSources(houseRoot, ~0, NavMeshCollectGeometry.PhysicsColliders,
            0, new List<NavMeshBuildMarkup>(), sources);

        NavMeshBuildSettings settings = NavMesh.GetSettingsByID(0);
        Bounds bounds = new Bounds(new Vector3(0f, 1.5f, 0f), new Vector3(24f, 5f, 20f));
        NavMeshData data = NavMeshBuilder.BuildNavMeshData(settings, sources, bounds,
            Vector3.zero, Quaternion.identity);

        if (data == null)
        {
            Debug.LogWarning("Tom & Jerry 3D: runtime NavMesh build failed. Tom will stay in place until a NavMesh is baked.");
            return;
        }

        navMeshInstance = NavMesh.AddNavMeshData(data);
    }

    private GameObject Cube(string label, Transform parent, Vector3 position, Vector3 scale, Material material)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = label;
        obj.transform.SetParent(parent, false);
        obj.transform.position = position;
        obj.transform.localScale = scale;
        obj.GetComponent<Renderer>().sharedMaterial = material;
        return obj;
    }

    private Material MakeMaterial(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Diffuse");
        Material material = new Material(shader);
        material.color = color;
        return material;
    }

    private void OnDestroy()
    {
        if (navMeshInstance.valid) navMeshInstance.Remove();
    }
}
