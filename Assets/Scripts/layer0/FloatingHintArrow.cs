using UnityEngine;

public class FloatingHintArrow : MonoBehaviour
{
    public float floatHeight = 0.08f;
    public float floatSpeed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.localPosition = startPos + new Vector3(0, yOffset, 0);
    }
}