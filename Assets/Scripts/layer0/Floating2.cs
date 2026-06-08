using UnityEngine;

public class Floating2 : MonoBehaviour
{
    public float floatDistance = 0.08f;
    public float floatSpeed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float zOffset = Mathf.Sin(Time.time * floatSpeed) * floatDistance;

        // 按照箭头自己的 local Z 方向前后移动
        transform.localPosition = startPos + Vector3.forward * zOffset;
    }
}