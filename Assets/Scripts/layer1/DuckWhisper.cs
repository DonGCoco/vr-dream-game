using UnityEngine;

public class DuckWhisper : MonoBehaviour
{
    [Header("玩家")]
    public Transform player;

    [Header("音频")]
    public AudioSource whisperAudio;

    [Header("触发距离")]
    public float triggerDistance = 2f;

    [Header("停留时间")]
    public float stayTime = 2f;

    private bool hasPlayed = false;

    private float timer = 0f;

    void Update()
    {
        // 已播放过则不再触发
        if (hasPlayed) return;

        // 计算距离
        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        // 玩家进入范围
        if (distance < triggerDistance)
        {
            // 开始计时
            timer += Time.deltaTime;

            // 停留够时间
            if (timer >= stayTime)
            {
                whisperAudio.Play();

                hasPlayed = true;
            }
        }
        else
        {
            // 离开范围则重置计时
            timer = 0f;
        }
    }
}