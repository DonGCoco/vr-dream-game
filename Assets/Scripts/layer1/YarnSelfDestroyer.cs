using UnityEngine;
using System.Collections;

public class YarnSelfDestroyer : MonoBehaviour
{
    [Header("设置")]
    public float destroyDelay = 3f; // 抓住后几秒消失

    [Header("Dream Layer Manager")]
    public DreamLayerManager dreamLayerManager;

    private bool _hasBeenGrabbed = false;
    private Vector3 _startPos;

    void Start()
    {
        _startPos = transform.position;
    }

    // 方案 A：如果你用的是标准的 Unity 物理触发
    void OnMouseDown()
    {
        StartTimer();
    }

    // 方案 B：由现有抓取脚本在“抓住”时调用
    public void StartTimer()
    {
        if (_hasBeenGrabbed) return;

        _hasBeenGrabbed = true;
        Debug.Log("毛线球已被抓住，3秒后消失...");
        StartCoroutine(DestroySequence());
    }

    IEnumerator DestroySequence()
    {
        yield return new WaitForSeconds(destroyDelay);

        Debug.Log("毛线球已消失，第一层完成");

        // 先通知 DreamLayerManager 切到下一层
        if (dreamLayerManager != null)
        {
            dreamLayerManager.CompleteCurrentLayer();
        }
        else
        {
            Debug.LogWarning("DreamLayerManager 没有拖进 Inspector！");
        }

        // 再隐藏自己
        gameObject.SetActive(false);
    }

    // 方案 C：检测线团是否离开了原始位置
    void Update()
    {
        if (!_hasBeenGrabbed && Vector3.Distance(transform.position, _startPos) > 0.1f)
        {
            StartTimer();
        }
    }
}