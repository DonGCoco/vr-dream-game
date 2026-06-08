using UnityEngine;
using TMPro;

public class DiaryPokeUI : MonoBehaviour
{
    public DreamLayerManager dreamLayerManager;
    public GameObject diaryUI;

    [Header("Hint Arrows")]
    public GameObject hintArrow;   // 日记本红色箭头
    public GameObject arrow2;      // 水杯上方箭头

    public TMP_Text diaryText;

    [TextArea(3, 8)] public string layer0Text;
    [TextArea(3, 8)] public string layer1Text;
    [TextArea(3, 8)] public string layer2Text;
    [TextArea(3, 8)] public string layer3Text;
    [TextArea(3, 8)] public string layer4Text;

    private bool hasOpenedOnce = false;

    void Start()
    {
        if (diaryUI != null)
            diaryUI.SetActive(false);

        if (hintArrow != null)
            hintArrow.SetActive(true);

        if (arrow2 != null)
            arrow2.SetActive(false);
    }

    public void OnDiaryPoked()
    {
        if (diaryUI != null)
            diaryUI.SetActive(true);

        if (!hasOpenedOnce)
        {
            hasOpenedOnce = true;

            if (hintArrow != null)
                hintArrow.SetActive(false);

            if (arrow2 != null)
                arrow2.SetActive(true);
        }

        UpdateDiaryTextByLayer();
    }

    private void UpdateDiaryTextByLayer()
    {
        if (dreamLayerManager == null || diaryText == null) return;

        int layer = dreamLayerManager.GetCurrentLayer();

        if (layer == 0) diaryText.text = layer0Text;
        else if (layer == 1) diaryText.text = layer1Text;
        else if (layer == 2) diaryText.text = layer2Text;
        else if (layer == 3) diaryText.text = layer3Text;
        else if (layer == 4) diaryText.text = layer4Text;
    }

    public void CloseDiaryUI()
    {
        if (diaryUI != null)
            diaryUI.SetActive(false);
    }
}