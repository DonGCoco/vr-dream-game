using System.Collections;
using UnityEngine;

public class CupFullAutoNextLayer : MonoBehaviour
{
    public DreamLayerManager dreamLayerManager;
    public float delayBeforeNextLayer = 2f;

    private bool hasTriggered = false;

    private void OnEnable()
    {
        if (hasTriggered) return;

        hasTriggered = true;
        StartCoroutine(GoNextLayerAfterDelay());
    }

    private IEnumerator GoNextLayerAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeNextLayer);

        if (dreamLayerManager != null)
        {
            dreamLayerManager.CompleteCurrentLayer();
        }
        else
        {
            Debug.LogWarning("DreamLayerManager Ã»ÓÐÍÏ½ø Inspector£¡");
        }
    }
}