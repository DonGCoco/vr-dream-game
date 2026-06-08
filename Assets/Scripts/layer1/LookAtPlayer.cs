using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform target;
    public Vector3 rotationOffset = new Vector3(-90, 0, 0); // 在这里预设你的 -90 度修正
    private Vector3 fixedPosition;

    void Start()
    {
        fixedPosition = transform.position;
        if (target == null && Camera.main != null)
            target = Camera.main.transform;
    }

    void Update()
    {
        if (target != null)
        {
            // 1. 保持位置不动
            // transform.position = fixedPosition;

            // 2. 计算朝向玩家的方向（只看左右）
            Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
            Vector3 direction = targetPosition - transform.position;

            if (direction != Vector3.zero)
            {
                // 3. 计算“看向玩家”的基础旋转
                Quaternion lookRotation = Quaternion.LookRotation(direction);

                // 4. 【关键步骤】在基础旋转之上，加上你的模型偏移量
                // Quaternion相乘代表旋转叠加
                transform.rotation = lookRotation * Quaternion.Euler(rotationOffset);
            }
        }
    }
}