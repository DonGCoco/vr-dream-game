using UnityEngine;
using System.Collections;

/// <summary>
/// 呼吸灯效果：平滑的忽明忽暗闪烁
/// </summary>
public class BreathingGlow : MonoBehaviour
{
    [Header("发光颜色设置")]
    [Tooltip("发光颜色（建议启用HDR）")]
    [ColorUsage(true, true)]
    public Color glowColor = new Color(1f, 0.5f, 0f, 1f); // 默认橙红色

    [Header("强度范围")]
    [Tooltip("最弱时的发光强度（0=完全不发光）")]
    [Range(0f, 5f)]
    public float minIntensity = 0f;

    [Tooltip("最强时的发光强度（建议2-5之间）")]
    [Range(0.1f, 10f)]
    public float maxIntensity = 3f;

    [Header("呼吸速度")]
    [Tooltip("呼吸频率（值越大闪烁越快）")]
    [Range(0.2f, 5f)]
    public float breathSpeed = 1f;

    [Header("可选：自动启动")]
    public bool startAutomatically = true;

    private Material materialInstance;
    private float timer = 0f;
    private bool isBreathing = true;

    void Start()
    {
        InitializeMaterial();

        if (startAutomatically)
        {
            StartBreathing();
        }
    }

    /// <summary>
    /// 初始化材质，确保支持Emission
    /// </summary>
    private void InitializeMaterial()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError($"物体 {gameObject.name} 没有Renderer组件！");
            enabled = false;
            return;
        }

        // 创建材质实例（避免修改原始材质资源）
        materialInstance = renderer.material;

        // 确保材质的Shader支持Emission
        if (!materialInstance.IsKeywordEnabled("_EMISSION"))
        {
            materialInstance.EnableKeyword("_EMISSION");
        }

        // 确保Global Illumination设置为实时或烘焙（可选）
        materialInstance.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

        Debug.Log($"✓ {gameObject.name} 的材质已初始化，支持Emission");
    }

    void Update()
    {
        if (!isBreathing || materialInstance == null) return;

        // 使用正弦波实现平滑呼吸效果
        timer += Time.deltaTime * breathSpeed;

        // 计算公式：将sin值(-1到1)映射到min到max之间
        float sinValue = Mathf.Sin(timer);
        float intensity = minIntensity + (sinValue + 1f) / 2f * (maxIntensity - minIntensity);

        // 应用发光强度
        ApplyGlowIntensity(intensity);
    }

    /// <summary>
    /// 应用当前的发光强度
    /// </summary>
    private void ApplyGlowIntensity(float intensity)
    {
        if (materialInstance == null) return;

        Color finalColor = glowColor * intensity;
        materialInstance.SetColor("_EmissionColor", finalColor);

        // 可选：强制更新全局光照（提高响应速度）
        // DynamicGI.UpdateEnvironment();
    }

    /// <summary>
    /// 开始呼吸效果
    /// </summary>
    public void StartBreathing()
    {
        isBreathing = true;
        timer = 0f; // 重置计时器，从最暗开始
        Debug.Log($"{gameObject.name} 开始呼吸灯效果");
    }

    /// <summary>
    /// 停止呼吸效果，恢复到指定强度
    /// </summary>
    public void StopBreathing(float fixedIntensity = 0f)
    {
        isBreathing = false;
        ApplyGlowIntensity(fixedIntensity);
        Debug.Log($"{gameObject.name} 停止呼吸灯效果，强度固定为 {fixedIntensity}");
    }

    /// <summary>
    /// 动态调整呼吸速度（运行时调用）
    /// </summary>
    public void SetBreathSpeed(float newSpeed)
    {
        breathSpeed = Mathf.Clamp(newSpeed, 0.2f, 5f);
    }

    /// <summary>
    /// 动态调整发光强度范围（运行时调用）
    /// </summary>
    public void SetIntensityRange(float newMin, float newMax)
    {
        minIntensity = Mathf.Clamp(newMin, 0f, newMax);
        maxIntensity = Mathf.Clamp(newMax, newMin, 10f);
    }

    void OnDestroy()
    {
        // 清理材质实例，避免内存泄漏
        if (materialInstance != null)
        {
            Destroy(materialInstance);
        }
    }

    // 可选：在编辑器中预览效果（仅在Play模式有效）
    void OnValidate()
    {
        if (Application.isPlaying && materialInstance != null)
        {
            // 运行时调整参数会立即生效
            if (isBreathing == false)
            {
                ApplyGlowIntensity(minIntensity);
            }
        }
    }
}