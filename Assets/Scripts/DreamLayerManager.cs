using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DreamLayerManager : MonoBehaviour
{
    [Header("Abnormal Layer Objects")]
    public GameObject layer0Normal;
    public GameObject layer0Weird;
    public GameObject layer1;
    public GameObject layer2Normal;
    public GameObject layer2Weird;
    public GameObject layer3;
    public GameObject layer3Mask;
    public GameObject layer4;

    [Header("Layer 1 Start Audio")]
    public AudioSource layer1StartAudio;
    public float layer1AudioDelay = 1f;


    [Header("Player / Camera Rig")]
    public Transform playerRig;
    public Transform bedSpawnPoint;

    [Header("VR Fade")]
    public OVRScreenFade ovrScreenFade;
    public float fadeDuration = 1f;
    public float blackHoldTime = 0.5f;

    private int currentLayer = 0;
    private bool isTransitioning = false;

    void Start()
    {
        SetDreamLayer(0);
    }

    public void CompleteCurrentLayer()
    {
        if (isTransitioning) return;

        if (currentLayer < 4)
        {
            StartCoroutine(GoToNextLayer(currentLayer + 1));
        }
        else
        {
            Debug.Log("Layer 4 completed. Ending is handled by another script.");
        }
    }

    private IEnumerator GoToNextLayer(int nextLayer)
    {
        isTransitioning = true;

        yield return StartCoroutine(FadeToBlack());

        TeleportToBed();

        SetDreamLayer(nextLayer);
        currentLayer = nextLayer;

        yield return new WaitForSeconds(blackHoldTime);

        yield return StartCoroutine(FadeFromBlack());

        isTransitioning = false;

        // 从 Layer 0 跳到 Layer 1，睁眼后 1 秒播放音频
        if (nextLayer == 1 && layer1StartAudio != null)
        {
            yield return new WaitForSeconds(layer1AudioDelay);
            layer1StartAudio.Play();
        }
    }

    private IEnumerator FadeToBlack()
    {
        if (ovrScreenFade == null)
        {
            Debug.LogWarning("OVRScreenFade 没有拖进 Inspector！");
            yield break;
        }

        ovrScreenFade.FadeOut();
        yield return new WaitForSeconds(fadeDuration);
    }

    private IEnumerator FadeFromBlack()
    {
        if (ovrScreenFade == null)
        {
            Debug.LogWarning("OVRScreenFade 没有拖进 Inspector！");
            yield break;
        }

        ovrScreenFade.FadeIn();
        yield return new WaitForSeconds(fadeDuration);
    }

    private void SetDreamLayer(int layerNumber)
    {
        SetActiveSafe(layer0Normal, false);
        SetActiveSafe(layer0Weird, false);
        SetActiveSafe(layer1, false);
        SetActiveSafe(layer2Normal, false);
        SetActiveSafe(layer2Weird, false);
        SetActiveSafe(layer3, false);
        SetActiveSafe(layer3Mask, false);
        SetActiveSafe(layer4, false);

        if (layerNumber == 0)
        {
            SetActiveSafe(layer0Weird, true);
            SetActiveSafe(layer2Normal, true);
            SetActiveSafe(layer3Mask, true);
        }
        else if (layerNumber == 1)
        {
            SetActiveSafe(layer0Normal, true);
            SetActiveSafe(layer1, true);
            SetActiveSafe(layer2Normal, true);
            SetActiveSafe(layer3Mask, true);
        }
        else if (layerNumber == 2)
        {
            SetActiveSafe(layer0Normal, true);
            SetActiveSafe(layer2Weird, true);
            SetActiveSafe(layer3Mask, true);
        }
        else if (layerNumber == 3)
        {
            SetActiveSafe(layer0Normal, true);
            SetActiveSafe(layer2Normal, true);
            SetActiveSafe(layer3, true);
        }
        else if (layerNumber == 4)
        {
            SetActiveSafe(layer0Normal, true);
            SetActiveSafe(layer2Normal, true);
            SetActiveSafe(layer3Mask, true);
            SetActiveSafe(layer4, true);
        }

        Debug.Log("Current Dream Layer: " + layerNumber);
    }

    private void SetActiveSafe(GameObject obj, bool active)
    {
        if (obj != null) obj.SetActive(active);
    }

    private void TeleportToBed()
    {
        if (playerRig == null || bedSpawnPoint == null) return;

        playerRig.position = bedSpawnPoint.position;
        playerRig.rotation = bedSpawnPoint.rotation;
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public int GetCurrentLayer()
    {
        return currentLayer;
    }
}