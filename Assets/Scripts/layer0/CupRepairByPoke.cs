using System.Collections;
using UnityEngine;

public class CupRepairByPoke : MonoBehaviour
{
    public GameObject cupBroken;
    public GameObject cupFull;

    public float brokenFadeDuration = 0.15f;
    public float brokenShrinkTo = 0.85f;
    public float fullStartScale = 0.95f;
    public float fullAppearDuration = 0.08f;
    public bool onlyOnce = true;

    private bool hasTriggered = false;
    private Coroutine routine;

    public void TriggerRepair()
    {
        if (onlyOnce && hasTriggered) return;
        if (routine != null) return;

        hasTriggered = true;
        routine = StartCoroutine(RepairSequence());
    }

    private IEnumerator RepairSequence()
    {
        if (cupBroken == null || cupFull == null) yield break;

        Transform brokenTf = cupBroken.transform;
        Transform fullTf = cupFull.transform;

        Vector3 brokenOriginalScale = brokenTf.localScale;
        Vector3 fullOriginalScale = fullTf.localScale;

        // 先把 CupFull 对齐到 CupBroken
        fullTf.position = brokenTf.position;
        fullTf.rotation = brokenTf.rotation;

        // 第一阶段：CupBroken 闪动 + 缩小
        Renderer[] brokenRenderers = cupBroken.GetComponentsInChildren<Renderer>(true);

        float t = 0f;
        while (t < brokenFadeDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / brokenFadeDuration);

            float brokenScale = Mathf.Lerp(1f, brokenShrinkTo, p);
            brokenTf.localScale = brokenOriginalScale * brokenScale;

            bool visible = Mathf.Sin(p * Mathf.PI * 8f) > -0.15f;
            foreach (var r in brokenRenderers)
            {
                if (r != null) r.enabled = visible;
            }

            yield return null;
        }

        // 先彻底关闭 Broken，避免 collider 冲突
        foreach (var r in brokenRenderers)
        {
            if (r != null) r.enabled = true;
        }

        brokenTf.localScale = brokenOriginalScale;
        cupBroken.SetActive(false);

        // 第二阶段：再出现 CupFull
        cupFull.SetActive(true);
        fullTf.localScale = fullOriginalScale * fullStartScale;

        float t2 = 0f;
        while (t2 < fullAppearDuration)
        {
            t2 += Time.deltaTime;
            float p = Mathf.Clamp01(t2 / fullAppearDuration);

            float fullScale = Mathf.Lerp(fullStartScale, 1f, Mathf.SmoothStep(0f, 1f, p));
            fullTf.localScale = fullOriginalScale * fullScale;

            yield return null;
        }

        fullTf.localScale = fullOriginalScale;
        routine = null;
    }
}
