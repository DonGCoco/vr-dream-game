using UnityEngine;

public class StartGuideController : MonoBehaviour
{
    public GameObject startGuideCanvas;

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One) || 
            OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
            Input.GetKeyDown(KeyCode.Space))
        {
            CloseGuide();
        }
    }

    public void CloseGuide()
    {
        if (startGuideCanvas != null)
        {
            startGuideCanvas.SetActive(false);
        }
    }
}