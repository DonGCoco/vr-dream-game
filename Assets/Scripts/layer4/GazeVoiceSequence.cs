using UnityEngine;

public class GazeVoiceSequence : MonoBehaviour
{
    public Transform playerCamera;

    public AudioSource voiceAudio;

    public AudioClip firstVoice;
    public AudioClip secondVoice;

    public float triggerDistance = 5f;
    public float triggerAngle = 40f;
    public float lookTimeRequired = 3f;

    private float lookTimer = 0f;
    private int gazeCount = 0;
    private bool isLookingTriggered = false;

    void Update()
    {
        if (playerCamera == null || voiceAudio == null) return;
        if (gazeCount >= 2) return;

        Vector3 dir = (transform.position - playerCamera.position).normalized;

        float angle = Vector3.Angle(playerCamera.forward, dir);

        float distance = Vector3.Distance(playerCamera.position, transform.position);

        bool looking = angle < triggerAngle && distance < triggerDistance;

        if (looking)
        {
            lookTimer += Time.deltaTime;

            if (!isLookingTriggered && lookTimer >= lookTimeRequired)
            {
                isLookingTriggered = true;

                gazeCount++;

                if (gazeCount == 1)
                {
                    voiceAudio.clip = firstVoice;
                    voiceAudio.Play();
                }
                else if (gazeCount == 2)
                {
                    voiceAudio.clip = secondVoice;
                    voiceAudio.Play();
                }
            }
        }
        else
        {
            lookTimer = 0f;
            isLookingTriggered = false;
        }
    }
}