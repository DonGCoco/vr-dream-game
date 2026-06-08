using UnityEngine;

public class CupGazeArrowTrigger : MonoBehaviour
{
    [Header("Player / Camera")]
    public Transform playerHead;

    [Header("Arrows")]
    public GameObject arrow2;
    public GameObject arrow3;

    [Header("Audio")]
    public AudioSource triggerAudio;

    [Header("Gaze Settings")]
    public float triggerDistance = 2f;
    public float stayTime = 2f;
    public float gazeAngle = 20f;

    private float timer = 0f;
    private bool hasTriggered = false;

    void Start()
    {
        if (arrow3 != null)
        {
            arrow3.SetActive(false);
        }
    }

    void Update()
    {
        if (hasTriggered) return;
        if (playerHead == null || arrow2 == null || arrow3 == null) return;

        if (!arrow2.activeInHierarchy)
        {
            timer = 0f;
            return;
        }

        float distance = Vector3.Distance(transform.position, playerHead.position);

        if (distance > triggerDistance)
        {
            timer = 0f;
            return;
        }

        Vector3 directionToCup = (transform.position - playerHead.position).normalized;
        float angle = Vector3.Angle(playerHead.forward, directionToCup);

        if (angle <= gazeAngle)
        {
            timer += Time.deltaTime;

            if (timer >= stayTime)
            {
                TriggerArrow3();
            }
        }
        else
        {
            timer = 0f;
        }
    }

    private void TriggerArrow3()
    {
        hasTriggered = true;

        arrow2.SetActive(false);
        arrow3.SetActive(true);

        if (triggerAudio != null)
        {
            triggerAudio.Play();
        }

        Debug.Log("用户注视水杯 2 秒，arrow2 关闭，arrow3 开启，并播放音频。");
    }
}