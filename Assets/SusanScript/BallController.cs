using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Vector3 start;
    public Vector3 control;
    public Vector3 end;
    public float travelTime = 1f;

    private float timer = 0f;
    private bool isReturning = false;
    private bool isHit = false;
    private bool isActive = true;

    private Vector3 directionAfterEnd;

    // 击球判定窗口
    private bool canBeHit = false;
    private float hitWindowStart;
    private float hitWindowEnd;

    void Start()
    {
        // 击球窗口在飞行的 85% ~ 100% 之间（可调）
        hitWindowStart = travelTime * 0.85f;
        hitWindowEnd = travelTime * 1.0f;
    }

    void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;
        float t = timer / travelTime;

        if (t <= 1f)
        {
            // 贝塞尔插值，模拟弧线
            Vector3 m1 = Vector3.Lerp(start, control, t);
            Vector3 m2 = Vector3.Lerp(control, end, t);
            transform.position = Vector3.Lerp(m1, m2, t);

            directionAfterEnd = (m2 - m1).normalized;

            // 启用击球判定窗口
            canBeHit = timer >= hitWindowStart && timer <= hitWindowEnd;
        }
        else
        {
            isActive = false;

            if (isHit)
            {
                Destroy(gameObject);
                GameManager.Instance.OnBallReturned();
            }
            else
            {
                StartCoroutine(FlyOutThenDestroy());
            }
        }
    }

    private IEnumerator FlyOutThenDestroy()
    {
        float extraFlyTime = 1.5f;
        float flySpeed = 8f;

        float elapsed = 0f;
        while (elapsed < extraFlyTime)
        {
            transform.position += directionAfterEnd * flySpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
        GameManager.Instance.OnBallMissed();
    }

    // 被 GameManager 调用
    public void TryHit()
    {
        if (isHit)
        {
            Debug.Log("[球状态] 已经被击中，忽略重复击打");
            return;
        }

        if (!canBeHit)
        {
            Debug.LogWarning("[MISS] 玩家按对了位置，但时机不对 ❌（球还没飞到圈圈位置）");
            return;
        }

        Debug.Log("✅ 击球成功！球开始回弹");

        // ✅ 关键：加分逻辑
        ScoreManager.Instance.AddScore(GameManager.Instance.currentPlayer);

        isHit = true;
        isReturning = true;
        timer = 0f;
        isActive = true;

        start = transform.position;
        end = GameManager.Instance.ballMachine.transform.position + new Vector3(0, 1f, 0);
        control = (start + end) / 2 + new Vector3(0, 2f, 0);
    }


}
