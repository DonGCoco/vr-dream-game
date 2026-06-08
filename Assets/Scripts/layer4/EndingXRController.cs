using UnityEngine;

public class EndingXRController : MonoBehaviour
{
    [Header("Roots")]
    public GameObject roomRoot;
    public GameObject endingRoot;

    [Header("Passthrough")]
    public GameObject passthroughObject;
    public OVRPassthroughLayer passthroughLayer;

    [Header("Camera")]
    public Camera centerEyeCamera;
    public Transform centerEye;

    [Header("Ending Objects")]
    public Transform blackTearScreen;
    public Transform welcomeBackText;

    [Header("Screen Placement")]
    public float screenDistance = 2.0f;
    public Vector3 screenScale = new Vector3(6f, 3.6f, 1f);

    [Header("Welcome Back Placement")]
    public float welcomeBackDistance = 1.8f;
    public float welcomeBackDownOffset = 0.1f;

    private bool endingStarted = false;

    void Start()
    {
        if (endingRoot != null)
            endingRoot.SetActive(false);

        if (welcomeBackText != null)
            welcomeBackText.gameObject.SetActive(false);

        if (passthroughLayer != null)
            passthroughLayer.hidden = true;
    }

    void LateUpdate()
    {
        if (!endingStarted) return;

        FollowHead();
    }

    public void StartEnding()
    {
        endingStarted = true;

        if (roomRoot != null)
            roomRoot.SetActive(false);

        if (passthroughObject != null)
            passthroughObject.SetActive(true);

        if (OVRManager.instance != null)
        {
            OVRManager.instance.isInsightPassthroughEnabled = true;
        }

        if (passthroughLayer != null)
        {
            passthroughLayer.hidden = false;
            passthroughLayer.overlayType = OVROverlay.OverlayType.Underlay;
        }

        if (centerEyeCamera != null)
        {
            centerEyeCamera.clearFlags = CameraClearFlags.SolidColor;
            centerEyeCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
        }

        if (endingRoot != null)
            endingRoot.SetActive(true);

        if (blackTearScreen != null)
            blackTearScreen.gameObject.SetActive(true);

        if (welcomeBackText != null)
            welcomeBackText.gameObject.SetActive(false);

        FollowHead();
    }

    void FollowHead()
    {
        if (centerEye == null)
            return;

        Vector3 headPos = centerEye.position;

        Vector3 forward = centerEye.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
            forward = centerEye.forward;

        forward.Normalize();

        if (blackTearScreen != null)
        {
            blackTearScreen.position = headPos + forward * screenDistance;
            blackTearScreen.rotation = Quaternion.LookRotation(blackTearScreen.position - headPos);
            blackTearScreen.localScale = screenScale;
        }

        if (welcomeBackText != null)
        {
            welcomeBackText.position =
                headPos + forward * welcomeBackDistance + Vector3.down * welcomeBackDownOffset;

            welcomeBackText.rotation =
                Quaternion.LookRotation(welcomeBackText.position - headPos);
        }
    }
}