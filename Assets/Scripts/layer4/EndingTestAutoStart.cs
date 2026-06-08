using UnityEngine;

public class EndingTestAutoStart : MonoBehaviour
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

    [Header("Timing")]
    public float delay = 3f;

    [Header("Screen Placement")]
    public float screenDistance = 2.0f;
    public Vector3 screenScale = new Vector3(4f, 2.4f, 1f);

    [Header("Welcome Back Placement")]
    public float welcomeBackDistance = 3.2f;
    public float welcomeBackDownOffset = 0.25f;

    private bool endingStarted = false;

    void Start()
    {
        if (endingRoot != null)
            endingRoot.SetActive(false);

        if (welcomeBackText != null)
            welcomeBackText.gameObject.SetActive(true);

        // 开始时先不强制关 passthrough，避免 Meta Building Block 初始化失败
        if (passthroughLayer != null)
            passthroughLayer.hidden = true;

        Invoke(nameof(StartEnding), delay);
    }

    void LateUpdate()
    {
        if (!endingStarted) return;

        FollowHead();
    }

    void StartEnding()
    {
        endingStarted = true;

        // 1. 关闭 VR 房间
        if (roomRoot != null)
            roomRoot.SetActive(false);

        // 2. 打开 Passthrough Object
        if (passthroughObject != null)
            passthroughObject.SetActive(true);

        // 3. 强制启用 OVRManager passthrough
        if (OVRManager.instance != null)
        {
            OVRManager.instance.isInsightPassthroughEnabled = true;
        }

        // 4. 打开 Passthrough Layer
        if (passthroughLayer != null)
        {
            passthroughLayer.hidden = false;
            passthroughLayer.overlayType = OVROverlay.OverlayType.Underlay;
        }

        // 5. 让相机背景透明，否则 underlay passthrough 会被相机清屏挡住
        if (centerEyeCamera != null)
        {
            centerEyeCamera.clearFlags = CameraClearFlags.SolidColor;
            centerEyeCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
        }

        // 6. 打开结尾黑屏
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