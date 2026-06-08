using UnityEngine;

public class ButterflyWatcher : MonoBehaviour
{
    public Animator animator;
    public Transform playerCamera;

    public AudioSource startVoiceAudio;
    public GameObject interactionHint;

    public float triggerDistance = 5f;
    public float triggerAngle = 25f;
    public float lookTimeRequired = 2f;

    private float lookTimer = 0f;
    private bool activated = false;

    void Start()
    {
        if (interactionHint != null)
        {
            interactionHint.SetActive(false);
        }
    }

    void Update()
    {
        if (activated) return;

        if (playerCamera == null || animator == null)
            return;

        Vector3 dir =
            (transform.position - playerCamera.position).normalized;

        float angle =
            Vector3.Angle(playerCamera.forward, dir);

        float distance =
            Vector3.Distance(playerCamera.position, transform.position);

        bool looking =
            angle < triggerAngle &&
            distance < triggerDistance;

        if (looking)
        {
            lookTimer += Time.deltaTime;

            if (lookTimer >= lookTimeRequired)
            {
                activated = true;

                animator.enabled = true;

                if (startVoiceAudio != null)
                {
                    startVoiceAudio.Play();
                }

                if (interactionHint != null)
                {
                    interactionHint.SetActive(true);
                }
            }
        }
        else
        {
            lookTimer = 0f;
        }
    }
}