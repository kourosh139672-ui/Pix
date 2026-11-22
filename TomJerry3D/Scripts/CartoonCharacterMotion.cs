using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Procedural cartoon animation for Tom: alternating legs and arms, body bounce,
/// head/ear motion, tail wave and occasional eye blink. No imported animation clips.
/// </summary>
public class CartoonCharacterMotion : MonoBehaviour
{
    [SerializeField] private float walkBounce = 0.055f;
    [SerializeField] private float walkSway = 5f;
    [SerializeField] private float tailSwing = 22f;

    private Transform model;
    private Transform head;
    private Transform leftArm;
    private Transform rightArm;
    private Transform leftLeg;
    private Transform rightLeg;
    private Transform leftEar;
    private Transform rightEar;
    private Transform leftEye;
    private Transform rightEye;
    private Transform[] tailSegments;
    private Vector3 modelStart;
    private Vector3 leftEyeScale;
    private Vector3 rightEyeScale;
    private float phase;
    private float blinkTimer;
    private float blinkAmount;
    private float smoothedSpeed;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        blinkTimer = Random.Range(2.5f, 5.5f);
    }

    private void LateUpdate()
    {
        if (model == null && !FindModelParts()) return;

        float speed = 0f;
        if (agent != null && agent.enabled && agent.isOnNavMesh)
            speed = agent.velocity.magnitude;
        smoothedSpeed = Mathf.Lerp(smoothedSpeed, speed, Time.deltaTime * 7f);

        float movement = Mathf.Clamp01(smoothedSpeed / 2.8f);
        phase += Time.deltaTime * Mathf.Lerp(1.8f, 11f, movement);
        float stride = Mathf.Sin(phase);
        float idleBreath = Mathf.Sin(Time.time * 2.1f) * 0.012f;

        model.localPosition = modelStart + Vector3.up *
            (idleBreath + Mathf.Abs(stride) * walkBounce * movement);
        model.localRotation = Quaternion.Euler(0f, 0f,
            Mathf.Sin(phase * 0.5f) * walkSway * movement);

        AnimateLimb(leftLeg, stride * 30f);
        AnimateLimb(rightLeg, -stride * 30f);
        AnimateLimb(leftArm, -stride * 24f);
        AnimateLimb(rightArm, stride * 24f);

        if (head != null)
            head.localRotation = Quaternion.Euler(
                Mathf.Sin(phase * 0.35f) * (2f + movement * 3f),
                Mathf.Sin(Time.time * 0.8f) * 5f,
                Mathf.Sin(phase * 0.25f) * 2f);

        if (leftEar != null)
            leftEar.localRotation = Quaternion.Euler(0f, 0f, -18f + Mathf.Sin(Time.time * 2.2f) * 3f);
        if (rightEar != null)
            rightEar.localRotation = Quaternion.Euler(0f, 0f, 18f - Mathf.Sin(Time.time * 2.2f) * 3f);

        if (tailSegments != null)
        {
            for (int i = 0; i < tailSegments.Length; i++)
            {
                if (tailSegments[i] == null) continue;
                float wave = Mathf.Sin(phase * 0.75f - i * 0.42f);
                tailSegments[i].localRotation = Quaternion.Euler(
                    Mathf.Cos(Time.time * 1.4f - i * 0.3f) * 4f,
                    wave * tailSwing * (0.45f + movement * 0.55f),
                    Mathf.Cos(phase * 0.55f - i * 0.35f) * 8f);
            }
        }

        AnimateBlink();
    }

    private bool FindModelParts()
    {
        model = transform.Find("Tom_Cartoon_Model");
        if (model == null) return false;

        modelStart = model.localPosition;
        head = model.Find("Head");
        leftArm = model.Find("Left Arm");
        rightArm = model.Find("Right Arm");
        leftLeg = model.Find("Left Leg");
        rightLeg = model.Find("Right Leg");
        leftEar = model.Find("Left Ear");
        rightEar = model.Find("Right Ear");
        leftEye = model.Find("Left Eye White");
        rightEye = model.Find("Right Eye White");
        if (leftEye != null) leftEyeScale = leftEye.localScale;
        if (rightEye != null) rightEyeScale = rightEye.localScale;

        tailSegments = new Transform[6];
        for (int i = 0; i < tailSegments.Length; i++)
            tailSegments[i] = model.Find("Tail Segment " + i);
        return true;
    }

    private static void AnimateLimb(Transform limb, float angle)
    {
        if (limb != null)
            limb.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    private void AnimateBlink()
    {
        blinkTimer -= Time.deltaTime;
        if (blinkTimer <= 0f)
        {
            blinkAmount = Mathf.MoveTowards(blinkAmount, 1f, Time.deltaTime * 12f);
            if (blinkAmount >= 1f)
                blinkTimer = -0.12f;
        }
        if (blinkTimer < 0f)
        {
            blinkAmount = Mathf.MoveTowards(blinkAmount, 0f, Time.deltaTime * 9f);
            if (blinkAmount <= 0f)
                blinkTimer = Random.Range(2.5f, 5.5f);
        }

        if (leftEye != null)
            leftEye.localScale = new Vector3(leftEyeScale.x, leftEyeScale.y * (1f - blinkAmount * 0.88f), leftEyeScale.z);
        if (rightEye != null)
            rightEye.localScale = new Vector3(rightEyeScale.x, rightEyeScale.y * (1f - blinkAmount * 0.88f), rightEyeScale.z);
    }
}
