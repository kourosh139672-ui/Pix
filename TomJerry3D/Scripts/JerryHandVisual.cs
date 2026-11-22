using UnityEngine;

/// <summary>
/// Procedural first-person Jerry arm and paw. Motion responds to player movement,
/// landing and item interaction instead of using a constant idle-only sway.
/// </summary>
public class JerryHandVisual : MonoBehaviour
{
    private Transform armVisual;
    private Transform pawVisual;
    private MousePlayerController player;
    private Vector3 armRestPosition;
    private Vector3 pawRestPosition;
    private Quaternion armRestRotation;
    private Quaternion pawRestRotation;
    private float phase;
    private float actionPulse;

    private void Start()
    {
        player = GetComponentInParent<MousePlayerController>();

        GameObject arm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        arm.name = "Jerry Soft Arm";
        arm.transform.SetParent(transform, false);
        arm.transform.localPosition = new Vector3(0f, -0.02f, 0.08f);
        arm.transform.localRotation = Quaternion.Euler(18f, 0f, -18f);
        arm.transform.localScale = new Vector3(0.16f, 0.22f, 0.16f);
        DisableCollider(arm);
        arm.GetComponent<Renderer>().sharedMaterial = MakeMaterial(new Color(0.48f, 0.31f, 0.20f));
        armVisual = arm.transform;

        GameObject paw = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        paw.name = "Jerry Paw";
        paw.transform.SetParent(transform, false);
        paw.transform.localPosition = new Vector3(0.015f, -0.10f, 0.23f);
        paw.transform.localScale = new Vector3(0.22f, 0.13f, 0.19f);
        DisableCollider(paw);
        paw.GetComponent<Renderer>().sharedMaterial = MakeMaterial(new Color(0.66f, 0.47f, 0.34f));
        pawVisual = paw.transform;

        armRestPosition = armVisual.localPosition;
        pawRestPosition = pawVisual.localPosition;
        armRestRotation = armVisual.localRotation;
        pawRestRotation = pawVisual.localRotation;
    }

    private void Update()
    {
        if (player == null) return;
        Vector3 horizontalVelocity = player.GetComponent<CharacterController>() != null
            ? player.GetComponent<CharacterController>().velocity
            : Vector3.zero;
        horizontalVelocity.y = 0f;
        float movement = Mathf.Clamp01(horizontalVelocity.magnitude / 3.5f);
        phase += Time.deltaTime * Mathf.Lerp(1.8f, 10f, movement);
        actionPulse = Mathf.MoveTowards(actionPulse, 0f, Time.deltaTime * 2.8f);

        float stride = Mathf.Sin(phase);
        float bob = Mathf.Abs(Mathf.Cos(phase * 2f)) * movement;
        armVisual.localPosition = armRestPosition + new Vector3(
            stride * 0.018f * movement,
            -bob * 0.025f + actionPulse * 0.035f,
            -bob * 0.018f);
        armVisual.localRotation = armRestRotation * Quaternion.Euler(
            stride * 7f * movement - actionPulse * 28f,
            0f,
            -stride * 9f * movement);

        pawVisual.localPosition = pawRestPosition + new Vector3(
            stride * 0.025f * movement,
            -bob * 0.035f + actionPulse * 0.055f,
            -bob * 0.025f);
        pawVisual.localRotation = pawRestRotation * Quaternion.Euler(
            -actionPulse * 35f,
            stride * 5f * movement,
            stride * 12f * movement);
    }

    public void PlayPickupMotion()
    {
        actionPulse = 1f;
    }

    private static void DisableCollider(GameObject obj)
    {
        Collider col = obj.GetComponent<Collider>();
        if (col != null) col.enabled = false;
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
