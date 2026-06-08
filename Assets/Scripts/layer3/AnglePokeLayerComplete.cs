using UnityEngine;

public class AnglePokeLayerComplete : MonoBehaviour
{
    [Header("Dream Layer Manager")]
    public DreamLayerManager dreamLayerManager;

    [Header("Optional")]
    public GameObject objectToHideAfterPoke;
    public bool hideAfterPoke = false;

    private bool hasTriggered = false;

    // 绑定到 Poke Interaction 的事件
    public void OnPoked()
    {
        if (hasTriggered) return;
        hasTriggered = true;

        Debug.Log("Angle 被 poke，第三层完成，准备黑屏跳转。");

        if (hideAfterPoke && objectToHideAfterPoke != null)
        {
            objectToHideAfterPoke.SetActive(false);
        }

        if (dreamLayerManager != null)
        {
            dreamLayerManager.CompleteCurrentLayer();
        }
        else
        {
            Debug.LogWarning("DreamLayerManager 没有拖进 Inspector！");
        }
    }
}