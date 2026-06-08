using UnityEngine;

public class GazeVoiceTrigger : MonoBehaviour
{
    public Transform playerCamera;
    public AudioSource voiceAudio;

    public float triggerDistance = 5f;
    public float triggerAngle = 20f;
    public float lookTimeRequired = 3f;

    private float lookTimer = 0f;
    private bool hasPlayed = false;

    void Update()
    {
        if (hasPlayed) return;
        if (playerCamera == null || voiceAudio == null) return;

        Vector3 dir = (transform.position - playerCamera.position).normalized;
        float angle = Vector3.Angle(playerCamera.forward, dir);
        float distance = Vector3.Distance(playerCamera.position, transform.position);

        bool looking = angle < triggerAngle && distance < triggerDistance;

        if (looking)
        {
            lookTimer += Time.deltaTime;

            if (lookTimer >= lookTimeRequired)
            {
                hasPlayed = true;
                voiceAudio.Play();
            }
        }
        else
        {
            lookTimer = 0f;
        }
    }
}