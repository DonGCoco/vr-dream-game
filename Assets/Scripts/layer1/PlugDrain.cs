using UnityEngine;

public class PlugDrain : MonoBehaviour
{
    [Header("引用设置")]
    public Transform bloodWater;    // 拖入血水模型
    public Transform abnormalDucks; // 拖入所有鸭子的父物体

    [Header("触发参数")]
    public float pullThreshold = 0.4f; // 拉开多远触发计时
    public float delayTime = 2.0f;     // 延迟触发时间（秒）

    [Header("动画参数")]
    public float sinkSpeed = 0.01f;     // 下降速度
    public float targetY = -2.0f;      // 消失高度
    public float disappearSpeed = 0.5f;

    private Vector3 originalPosition;
    private bool isCountingDown = false; // 是否正在倒计时
    private bool isDraining = false;     // 是否正式开始排水
    private float timer = 0f;            // 计时器变量

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        // 1. 检测是否被拉开并开始倒计时
        if (!isDraining && !isCountingDown)
        {
            if (Vector3.Distance(transform.position, originalPosition) > pullThreshold)
            {
                isCountingDown = true;
                Debug.Log("线团已拔出，2秒倒计时开始...");
            }
        }

        // 2. 倒计时逻辑
        if (isCountingDown && !isDraining)
        {
            timer += Time.deltaTime;
            if (timer >= delayTime)
            {
                isDraining = true;
                isCountingDown = false;
                Debug.Log("倒计时结束，开始排水！");
            }
        }

        // 3. 执行排水动画
        if (isDraining)
        {
            PerformSinkAnimation();
        }
    }

void PerformSinkAnimation()
{
    float dt = Time.deltaTime;

    // 1. 血水只缩放，不下移，避免穿模
    if (bloodWater != null && bloodWater.gameObject.activeSelf)
    {
        Vector3 scale = bloodWater.localScale;

        scale.y -= sinkSpeed * dt;

        // 不让它无限变成负数
        if (scale.y < 0.01f)
        {
            scale.y = 0.01f;
        }

        bloodWater.localScale = scale;
    }

    // 2. 鸭子慢慢下降、旋转、缩小
    if (abnormalDucks != null && abnormalDucks.gameObject.activeSelf)
    {
        // 鸭子的下降速度单独降低，避免穿浴缸
        abnormalDucks.Translate(Vector3.down * sinkSpeed * 0.25f * dt, Space.World);

        abnormalDucks.localScale = Vector3.Lerp(
            abnormalDucks.localScale,
            Vector3.zero,
            dt * disappearSpeed
        );
    }

    // 3. 血水缩到很薄后，直接整体隐藏
    if (bloodWater != null && bloodWater.localScale.y <= 0.2f)
    {
        bloodWater.gameObject.SetActive(false);

        if (abnormalDucks != null)
        {
            abnormalDucks.gameObject.SetActive(false);
        }

        isDraining = false;

        Debug.Log("异常已彻底清除。");
      }
  }
}