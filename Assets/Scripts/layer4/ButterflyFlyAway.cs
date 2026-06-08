using UnityEngine;
using UnityEngine.Video;

public class ButterflyFlyAway : MonoBehaviour
{
    [Header("Fly Away")]
    public Transform[] flyPoints;
    public Animator animator;
    public AudioSource flyVoiceAudio;
    public float flySpeed = 2f;

    [Header("Old Ending - Not Used Now")]
    public GameObject endingCanvas;
    public VideoPlayer videoPlayer;

    [Header("XR Ending")]
    public EndingXRController endingXRController;

    private bool isFlying = false;
    private int currentPoint = 0;
    private bool hasStartedFlying = false;
    private bool endingStarted = false;

    void Update()
    {
        if (!isFlying) return;
        if (flyPoints == null || flyPoints.Length == 0) return;
        if (currentPoint >= flyPoints.Length)
        {
            isFlying = false;
            return;
        }

        Transform target = flyPoints[currentPoint];

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            flySpeed * Time.deltaTime
        );

        transform.LookAt(target);
        transform.Rotate(0, 180f, 0);

        float distance = Vector3.Distance(
            transform.position,
            target.position
        );

        if (distance < 0.1f)
        {
            currentPoint++;
        }
    }

    public void OnButterflyClicked()
    {
        // 第一次点击：蝴蝶飞走 + 播放语音
        if (!hasStartedFlying)
        {
            hasStartedFlying = true;
            isFlying = true;

            if (animator != null)
            {
                animator.enabled = true;
            }

            if (flyVoiceAudio != null)
            {
                flyVoiceAudio.Play();
            }

            return;
        }

        // 第二次点击：进入 XR 结尾
        if (!endingStarted)
        {
            endingStarted = true;

            if (endingXRController != null)
            {
                endingXRController.StartEnding();
            }
            else
            {
                Debug.LogWarning("ButterflyFlyAway: EndingXRController is not assigned.");
            }
        }
    }
}