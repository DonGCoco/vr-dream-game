using UnityEngine;

public class EndingFollowHead : MonoBehaviour
{
    public Transform centerEye;
    public Transform blackSphere;
    public Transform welcomeBackText;

    public float sphereScale = 8f;
    public float welcomeBackDistance = 3.5f;
    public float welcomeBackDownOffset = 0.2f;

    void LateUpdate()
    {
        if (centerEye == null) return;

        Vector3 headPos = centerEye.position;

        Vector3 forward = centerEye.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
            forward = centerEye.forward;

        forward.Normalize();

        if (blackSphere != null)
        {
            blackSphere.position = headPos;
            blackSphere.localScale = Vector3.one * sphereScale;
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