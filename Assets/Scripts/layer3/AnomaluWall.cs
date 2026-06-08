using System.Collections;
using UnityEngine;

public class WallAnomalyByPoke : MonoBehaviour
{
    [Header("模型引用")]
    public GameObject wallIntact;    // 完好的墙面
    public GameObject wallBroken;    // 倒塌后的墙面（整体或带碎砖的模型）

    [Header("动画参数")]
    public float intactFadeDuration = 0.15f; // 完好墙消失的时长
    public float intactShrinkTo = 0.95f;     // 完好墙消失时缩小的比例
    public float brokenStartScale = 0.98f;   // 破碎墙出现时的初始缩放
    public float brokenAppearDuration = 0.1f; // 破碎墙出现的时长
    public bool onlyOnce = true;             // 异常是否只触发一次

    private bool hasTriggered = false;
    private Coroutine routine;

    // 当 Poke 交互触发时调用此方法
    public void TriggerAnomaly()
    {
        if (onlyOnce && hasTriggered) return;
        if (routine != null) return;

        hasTriggered = true;
        routine = StartCoroutine(AnomalySequence());
    }

    private IEnumerator AnomalySequence()
    {
        if (wallIntact == null || wallBroken == null) yield break;

        Transform intactTf = wallIntact.transform;
        Transform brokenTf = wallBroken.transform;

        Vector3 intactOriginalScale = intactTf.localScale;
        Vector3 brokenOriginalScale = brokenTf.localScale;

        // --- 第一阶段：完好墙面闪烁并缩小 ---
        Renderer[] intactRenderers = wallIntact.GetComponentsInChildren<Renderer>(true);
        float t = 0f;
        while (t < intactFadeDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / intactFadeDuration);

            // 缩小效果
            float s = Mathf.Lerp(1f, intactShrinkTo, p);
            intactTf.localScale = intactOriginalScale * s;

            // 快速闪烁效果
            bool visible = Mathf.Sin(p * Mathf.PI * 10f) > -0.1f;
            foreach (var r in intactRenderers)
            {
                if (r != null) r.enabled = visible;
            }

            yield return null;
        }

        // 还原渲染状态并关闭完好墙
        foreach (var r in intactRenderers) { if (r != null) r.enabled = true; }
        intactTf.localScale = intactOriginalScale;
        wallIntact.SetActive(false);

        // --- 第二阶段：破碎墙面弹出显示 ---
        wallBroken.SetActive(true);
        brokenTf.localScale = brokenOriginalScale * brokenStartScale;

        float t2 = 0f;
        while (t2 < brokenAppearDuration)
        {
            t2 += Time.deltaTime;
            float p = Mathf.Clamp01(t2 / brokenAppearDuration);

            // 平滑放大到原始大小
            float s2 = Mathf.Lerp(brokenStartScale, 1f, Mathf.SmoothStep(0f, 1f, p));
            brokenTf.localScale = brokenOriginalScale * s2;

            yield return null;
        }

        brokenTf.localScale = brokenOriginalScale;
        routine = null;
    }
}