using UnityEngine;

/// <summary>
/// Procedural first-person Jerry paw and arm. Adds a gentle idle sway and
/// movement bob to the hand socket; carried items remain visible in front.
/// </summary>
public class JerryHandVisual : MonoBehaviour
{
    private Transform visual;
    private Vector3 restPosition;
    private float phase;

    private void Start()
    {
        GameObject arm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        arm.name = "Jerry Soft Arm";
        arm.transform.SetParent(transform, false);
        arm.transform.localPosition = new Vector3(0f, -0.02f, 0.08f);
        arm.transform.localRotation = Quaternion.Euler(18f, 0f, -18f);
        arm.transform.localScale = new Vector3(0.16f, 0.22f, 0.16f);
        Collider armCollider = arm.GetComponent<Collider>();
        if (armCollider != null) armCollider.enabled = false;
        arm.GetComponent<Renderer>().sharedMaterial = MakeMaterial(new Color(0.48f, 0.31f, 0.20f));

        GameObject paw = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        paw.name = "Jerry Paw";
        paw.transform.SetParent(transform, false);
        paw.transform.localPosition = new Vector3(0.015f, -0.10f, 0.23f);
        paw.transform.localScale = new Vector3(0.22f, 0.13f, 0.19f);
        Collider pawCollider = paw.GetComponent<Collider>();
        if (pawCollider != null) pawCollider.enabled = false;
        paw.GetComponent<Renderer>().sharedMaterial = MakeMaterial(new Color(0.66f, 0.47f, 0.34f));
        visual = paw.transform;
        restPosition = visual.localPosition;
    }

    private void LateUpdate()
    {
        if (visual == null) return;
        phase += Time.deltaTime * 2.4f;
        visual.localPosition = restPosition + new Vector3(
            Mathf.Sin(phase) * 0.008f,
            Mathf.Sin(phase * 2f) * 0.012f,
            0f);
        visual.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(phase) * 4f);
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
