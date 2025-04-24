using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    [Header("贝塞尔控制点")]
    public Vector3 start;
    public Vector3 control;
    public Vector3 end;
    public float travelTime = 2f;

    [Header("击球窗口设置 (百分比)")]
    [Range(0f, 1f)]
    public float hitWindowStartPercent = 0.85f;
    [Range(0f, 1f)]
    public float hitWindowEndPercent = 1.0f;

    [Header("调试")]
    public bool showDebugText = true;
    public Text debugText; // 拖入 UI Text 显示状态（可选）

    private float timer = 0f;
    private bool isReturning = false;
    private bool isHit = false;
    private bool isActive = true;
    private bool isDead = false;

    private Vector3 directionAfterEnd;

    private bool canBeHit = false;
    private float hitWindowStart;
    private float hitWindowEnd;

    void Start()
    {
        // 初始化击球窗口时间点
        hitWindowStart = travelTime * hitWindowStartPercent;
        hitWindowEnd = travelTime * hitWindowEndPercent;

        if (debugText != null)
            debugText.enabled = showDebugText;
    }

    void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;
        float t = timer / travelTime;

        if (t <= 1f)
        {
            // 贝塞尔插值运动
            Vector3 m1 = Vector3.Lerp(start, control, t);
            Vector3 m2 = Vector3.Lerp(control, end, t);
            transform.position = Vector3.Lerp(m1, m2, t);

            directionAfterEnd = (m2 - m1).normalized;

            // 判断击球窗口
            canBeHit = timer >= hitWindowStart && timer <= hitWindowEnd;

            // 调试文本
            if (debugText != null && showDebugText)
            {
                debugText.text = $"🕒 时间: {timer:F2}/{travelTime} s\n🎯 可击打: {canBeHit}";
            }
        }
        else
        {
            isActive = false;

            if (isHit)
            {
                GameManager.Instance.ballMachine.ClearCurrentBall();
                Destroy(gameObject);
                GameManager.Instance.OnBallReturned();
            }
            else
            {
                StartCoroutine(FlyOutThenDestroySmooth());


            }
        }
    }

    private IEnumerator FlyOutThenDestroySmooth()
    {
        float flySpeed = 8f;
        float bufferTime = 0.2f;
        float flyOutTime = 1.5f;
        float elapsed = 0f;

        // 🟢 在缓冲期内也继续飞
        while (elapsed < bufferTime)
        {
            transform.position += directionAfterEnd * flySpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        isDead = true; // ⚠️ 缓冲完再标记不可击球

        elapsed = 0f;
        while (elapsed < flyOutTime)
        {
            transform.position += directionAfterEnd * flySpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        GameManager.Instance.ballMachine.ClearCurrentBall();
        Destroy(gameObject);
        GameManager.Instance.OnBallMissed();
    }


    /// <summary>
    /// 被 GameManager 调用以尝试击球
    /// </summary>
    public void TryHit()
    {
        if (isHit)
        {
            Debug.Log("[球状态] 已经被击中，忽略重复击打");
            return;
        }

        if (isDead)
        {
            Debug.LogWarning("❌ 球已经开始飞出边界，不允许再击打！");
            return;
        }

        if (!canBeHit)
        {
            Debug.LogWarning("[MISS] 玩家按对了位置，但时机不对 ❌（球还没飞到圈圈位置）");
            return;
        }

        Debug.Log("✅ 击球成功！球开始回弹");

        // 加分
        ScoreManager.Instance.AddScore(GameManager.Instance.currentPlayer);

        isHit = true;
        isReturning = true;
        timer = 0f;
        isActive = true;

        // 设置回弹路径（目标为球机上方）
        start = transform.position;
        end = GameManager.Instance.ballMachine.transform.position + new Vector3(0, 1f, 0);
        control = (start + end) / 2 + new Vector3(0, 2f, 0);
    }
}



