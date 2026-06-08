using System.Collections;
using UnityEngine;

public class SmoothCarWallLoopGrab : MonoBehaviour
{
    [Header("Straight Movement")]
    public float firstBackDistance = 0.2f;
    public float backDistance = 0.4f;
    public float upDistance = 0.8f;
    public float forwardDistance = 0.4f;
    public float downDistance = 0.8f;

    [Header("Corner Turn")]
    public float cornerRadius = 0.12f;
    public float moveSpeed = 0.25f;
    public float turnSpeed = 90f;

    [Header("Dream Layer Manager")]
    public DreamLayerManager dreamLayerManager;

    [Header("Trigger Point")]
    public Collider triggerPoint;
    public bool snapToTriggerPoint = true;

    [Header("Grab Audio")]
    public AudioSource grabAudioSource;
    public float grabAudioDelay = 1f;

    private bool grabAudioStarted = false;

    private Rigidbody rb;
    private Coroutine loopRoutine;

    private bool grabbed = false;
    private bool firstLoop = true;
    private bool layerCompleted = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rb.useGravity = false;
        rb.isKinematic = true;

        loopRoutine = StartCoroutine(LoopMove());
    }

    void Update()
    {
        if (grabbed && !layerCompleted)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    IEnumerator LoopMove()
    {
        while (!grabbed && !layerCompleted)
        {
            float currentBackDistance = firstLoop ? firstBackDistance : backDistance;

            yield return MoveStraight(Vector3.back, currentBackDistance);

            yield return SmoothTurn(Vector3.back);

            yield return MoveStraight(Vector3.up, upDistance);

            yield return SmoothTurn(Vector3.up);

            yield return MoveStraight(Vector3.forward, forwardDistance);

            yield return SmoothTurn(Vector3.forward);

            yield return MoveStraight(Vector3.down, downDistance);

            yield return SmoothTurn(Vector3.down);

            firstLoop = false;
        }
    }

    IEnumerator MoveStraight(Vector3 direction, float distance)
    {
        float moved = 0f;
        direction.Normalize();

        while (moved < distance && !grabbed && !layerCompleted)
        {
            float step = moveSpeed * Time.deltaTime;
            step = Mathf.Min(step, distance - moved);

            transform.position += direction * step;
            moved += step;

            yield return null;
        }
    }

    IEnumerator SmoothTurn(Vector3 startDirection)
    {
        float turned = 0f;

        while (turned < 90f && !grabbed && !layerCompleted)
        {
            float angleStep = turnSpeed * Time.deltaTime;
            angleStep = Mathf.Min(angleStep, 90f - turned);

            Vector3 currentDir = Quaternion.AngleAxis(turned, Vector3.right) * startDirection;
            currentDir.Normalize();

            float arcStep = cornerRadius * angleStep * Mathf.Deg2Rad;
            transform.position += currentDir * arcStep;

            transform.Rotate(Vector3.right, angleStep, Space.World);

            turned += angleStep;

            yield return null;
        }
    }

    // 绑定到 When Select
    public void OnGrab()
    {
        StopAutoMoveAndEnablePhysics();

        if (!grabAudioStarted)
        {
            grabAudioStarted = true;
            StartCoroutine(PlayGrabAudioAfterDelay());
        }
    }

    // 绑定到 When Unselect
    public void OnRelease()
    {
        StopAutoMoveAndEnablePhysics();
        StartCoroutine(ForcePhysicsAfterRelease());
    }



    void StopAutoMoveAndEnablePhysics()
    {
        grabbed = true;

        if (loopRoutine != null)
        {
            StopCoroutine(loopRoutine);
            loopRoutine = null;
        }

        rb.isKinematic = false;
        rb.useGravity = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    IEnumerator ForcePhysicsAfterRelease()
    {
        yield return null;

        rb.isKinematic = false;
        rb.useGravity = true;

        yield return new WaitForFixedUpdate();

        rb.isKinematic = false;
        rb.useGravity = true;
    }

    IEnumerator PlayGrabAudioAfterDelay()
    {
        yield return new WaitForSeconds(grabAudioDelay);

        if (grabAudioSource != null)
        {
            grabAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("Grab Audio Source 没有拖进 Inspector！");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckTriggerPoint(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CheckTriggerPoint(other);
    }

    private void CheckTriggerPoint(Collider other)
    {
        if (layerCompleted) return;

        // 只允许指定的 TriggerPoint 触发
        if (triggerPoint != null && other != triggerPoint) return;

        CompleteCarLayer();
    }

    private void CompleteCarLayer()
    {
        layerCompleted = true;

        Debug.Log("小车到达 TriggerPoint，当前层完成，准备黑屏跳转。");

        // 停止自动循环
        if (loopRoutine != null)
        {
            StopCoroutine(loopRoutine);
            loopRoutine = null;
        }

        // 停住小车
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;

        // 可选：吸附到 TriggerPoint 中心
        if (snapToTriggerPoint && triggerPoint != null)
        {
            transform.position = triggerPoint.transform.position;
            transform.rotation = triggerPoint.transform.rotation;
        }

        // 调用 DreamLayerManager 黑屏跳下一层
        if (dreamLayerManager != null)
        {
            dreamLayerManager.CompleteCurrentLayer();
        }
        else
        {
            Debug.LogWarning("DreamLayerManager 没有拖进 Inspector！");
        }
    }
}