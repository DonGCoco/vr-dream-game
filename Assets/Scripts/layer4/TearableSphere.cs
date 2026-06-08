using UnityEngine;

public class TearableSphere : MonoBehaviour
{
    [Header("Controller")]
    public Transform controllerRayOrigin;

    [Header("Screen Mapping")]
    public Renderer targetRenderer;
    public Collider targetCollider;

    [Tooltip("数值越小，手移动一点就能擦更大范围；数值越大，擦得更精细。")]
    public float controllerXRange = 0.8f;

    [Tooltip("数值越小，手移动一点就能擦更大范围；数值越大，擦得更精细。")]
    public float controllerYRange = 0.55f;

    [Header("Tear Settings")]
    public int textureSize = 1024;
    public int tearRadius = 90;

    [Range(0.01f, 1f)]
    public float completePercent = 0.18f;

    [Header("After Complete")]
    public GameObject welcomeBackText;
    public bool hideBlackScreenWhenComplete = true;

    [Header("Input")]
    public bool autoTearWithoutTrigger = false;

    private Texture2D maskTexture;
    private Color[] pixels;

    private int erasedPixels;
    private int totalPixels;
    private bool completed;

    void Start()
    {
        InitMask();
    }

    void OnEnable()
    {
        // 如果 EndingRoot 运行中被重新打开，重置一次遮罩，避免上次擦过的状态残留
        if (Application.isPlaying && targetRenderer != null)
        {
            InitMask();
        }
    }

    void Update()
    {
        if (completed) return;
        if (controllerRayOrigin == null) return;

        if (autoTearWithoutTrigger)
        {
            TearByControllerPosition();
            return;
        }

        bool pressingTrigger =
            OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) ||
            OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);

        if (pressingTrigger)
        {
            TearByControllerPosition();
        }
    }

    void InitMask()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        if (targetCollider == null)
            targetCollider = GetComponent<Collider>();

        maskTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        maskTexture.wrapMode = TextureWrapMode.Clamp;
        maskTexture.filterMode = FilterMode.Bilinear;

        pixels = new Color[textureSize * textureSize];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        maskTexture.SetPixels(pixels);
        maskTexture.Apply(false);

        targetRenderer.material.SetTexture("_MaskTex", maskTexture);

        totalPixels = textureSize * textureSize;
        erasedPixels = 0;
        completed = false;

        if (welcomeBackText != null)
            welcomeBackText.SetActive(true);

        if (targetCollider != null)
            targetCollider.enabled = true;
    }

    void TearByControllerPosition()
    {
        Vector3 local = transform.InverseTransformPoint(controllerRayOrigin.position);

        float u = Mathf.InverseLerp(-controllerXRange, controllerXRange, local.x);
        float v = Mathf.InverseLerp(-controllerYRange, controllerYRange, local.y);

        u = Mathf.Clamp01(u);
        v = Mathf.Clamp01(v);

        TearAtUV(new Vector2(u, v));
    }

    void TearAtUV(Vector2 uv)
    {
        int centerX = Mathf.RoundToInt(uv.x * textureSize);
        int centerY = Mathf.RoundToInt(uv.y * textureSize);

        for (int y = -tearRadius; y <= tearRadius; y++)
        {
            for (int x = -tearRadius; x <= tearRadius; x++)
            {
                float distance = Mathf.Sqrt(x * x + y * y);
                float edgeNoise = Random.Range(-14f, 14f);

                if (distance > tearRadius + edgeNoise)
                    continue;

                int px = centerX + x;
                int py = centerY + y;

                if (px < 0 || px >= textureSize || py < 0 || py >= textureSize)
                    continue;

                int index = py * textureSize + px;

                if (pixels[index].r > 0.5f)
                {
                    pixels[index] = Color.black;
                    erasedPixels++;
                }
            }
        }

        maskTexture.SetPixels(pixels);
        maskTexture.Apply(false);

        float percent = (float)erasedPixels / totalPixels;

        if (percent >= completePercent)
        {
            CompleteTear();
        }
    }

    void CompleteTear()
    {
        completed = true;

        if (targetCollider != null)
            targetCollider.enabled = false;

        if (welcomeBackText != null)
            welcomeBackText.SetActive(true);

        if (hideBlackScreenWhenComplete)
            gameObject.SetActive(false);
    }
}