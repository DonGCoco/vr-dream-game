using UnityEngine;

public class WaterSlosh : MonoBehaviour
{
    [Header("References")]
    public Transform cupRoot;
    

    [Header("Position Slosh")]
    public float positionInfluence = 12f;   // 平移对水倾斜的影响
    public float maxTiltAngle = 8f;         // 最大倾斜角度 这个

    [Header("Rotation Slosh")]
    public float rotationInfluence = 0.5f;  // 杯子旋转对水晃动的影响

    [Header("Smoothing")]
    public float smoothSpeed = 6f;          // 跟随平滑
    public float returnSpeed = 2f;          // 回正速度

    [Header("Bounce")]
    public float stopBounceMultiplier = 0.35f; // 突然停下时的回摆强度
    public float stopThreshold = 0.015f;       // 低于这个速度视为接近停下

    private Vector3 lastCupPosition;
    private Quaternion lastCupRotation;

    private Vector3 currentVelocity;
    private Vector3 lastVelocity;

    private Vector3 currentAngularVelocity;
    private Vector3 targetEuler;
    private Vector3 currentEulerVelocity; // SmoothDamp 用

    void Start()
    {
        if (cupRoot == null)
        {
            Debug.LogError("WaterSlosh: cupRoot is not assigned.");
            enabled = false;
            return;
        }

        lastCupPosition = cupRoot.position;
        lastCupRotation = cupRoot.rotation;
    }

    void LateUpdate()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        // 1. 计算杯子的线速度
        Vector3 worldDeltaPos = cupRoot.position - lastCupPosition;
        currentVelocity = worldDeltaPos / dt;

        // 转到杯子本地坐标，方便判断前后左右
        Vector3 localVelocity = cupRoot.InverseTransformDirection(currentVelocity);

        // 2. 计算杯子的角速度（近似）
        Quaternion deltaRotation = cupRoot.rotation * Quaternion.Inverse(lastCupRotation);
        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180f) angle -= 360f;
        if (Mathf.Approximately(angle, 0f) || float.IsNaN(axis.x))
        {
            currentAngularVelocity = Vector3.zero;
        }
        else
        {
            Vector3 worldAngular = axis.normalized * angle * Mathf.Deg2Rad / dt;
            currentAngularVelocity = cupRoot.InverseTransformDirection(worldAngular);
        }

        // 3. 根据平移生成目标倾斜
        // 杯子向前移动 -> 水面向后倾 => 绕X轴负方向
        float tiltXFromMove = -localVelocity.z * positionInfluence;

        // 杯子向右移动 -> 水面向左倾 => 绕Z轴正方向
        float tiltZFromMove = localVelocity.x * positionInfluence;

        // 4. 根据旋转生成延迟晃动
        // 杯子绕本地X/Z转动时，给水增加反向一点的倾斜感
        float tiltXFromRotation = -currentAngularVelocity.x * Mathf.Rad2Deg * rotationInfluence;
        float tiltZFromRotation = currentAngularVelocity.z * Mathf.Rad2Deg * rotationInfluence;

        float targetX = tiltXFromMove + tiltXFromRotation;
        float targetZ = tiltZFromMove + tiltZFromRotation;

        // 5. 突然停下时给一个回摆
        float speedNow = currentVelocity.magnitude;
        float speedBefore = lastVelocity.magnitude;

        bool suddenlyStopped = speedBefore > stopThreshold && speedNow < stopThreshold;

        if (suddenlyStopped)
        {
            Vector3 lastLocalVelocity = cupRoot.InverseTransformDirection(lastVelocity);

            // 停下时给一个反向小回摆
            targetX += lastLocalVelocity.z * positionInfluence * stopBounceMultiplier;
            targetZ += -lastLocalVelocity.x * positionInfluence * stopBounceMultiplier;
        }

        // 6. 限制最大角度
        targetX = Mathf.Clamp(targetX, -maxTiltAngle, maxTiltAngle);
        targetZ = Mathf.Clamp(targetZ, -maxTiltAngle, maxTiltAngle);

        // Y 不转
        targetEuler = new Vector3(targetX, 0f, targetZ);

        // 7. 平滑到目标，再自然回弹
        Vector3 currentLocalEuler = NormalizeEulerAngles(transform.localEulerAngles);

        // SmoothDamp 到目标值
        Vector3 smoothed = Vector3.SmoothDamp(
            currentLocalEuler,
            targetEuler,
            ref currentEulerVelocity,
            1f / smoothSpeed
        );

        // 如果杯子几乎不动，再更快一点回正
        if (speedNow < stopThreshold * 0.5f && currentAngularVelocity.magnitude < 0.05f)
        {
            smoothed.x = Mathf.Lerp(smoothed.x, 0f, returnSpeed * dt);
            smoothed.z = Mathf.Lerp(smoothed.z, 0f, returnSpeed * dt);
        }

        transform.localRotation = Quaternion.Euler(smoothed);

        // 8. 更新缓存
        lastVelocity = currentVelocity;
        lastCupPosition = cupRoot.position;
        lastCupRotation = cupRoot.rotation;
    }

    private Vector3 NormalizeEulerAngles(Vector3 euler)
    {
        euler.x = NormalizeAngle(euler.x);
        euler.y = NormalizeAngle(euler.y);
        euler.z = NormalizeAngle(euler.z);
        return euler;
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}