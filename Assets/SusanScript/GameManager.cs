using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake() => Instance = this;

    public enum Player { Player1, Player2 }
    public Player currentPlayer;

    public BallMachine ballMachine;
    public Transform[] allTargets; // 目标点们，例如6个方向
    public BallController currentBall;

    private Transform expectedTarget; // 本回合目标点

    void Start()
    {
        StartNewTurn(); // 开局自动开始
    }

    public void StartNewTurn()
    {
        // 高亮当前玩家
        ScoreManager.Instance.HighlightActivePlayer(currentPlayer);

        // 发球（球会由 ballMachine 启动）
        StartCoroutine(ballMachine.LaunchBall());
    }

    /// <summary>
    /// 由 BallMachine 调用，设置期望玩家击打的方向（目标点）
    /// </summary>
    public void SetExpectedTarget(Transform target)
    {
        expectedTarget = target;
    }

    /// <summary>
    /// 玩家点击输入按钮时触发
    /// </summary>
    public void OnPlayerInput(int inputIndex)
    {
        Transform playerInputTarget = GetTargetTransformByIndex(inputIndex);
        if (playerInputTarget == null || expectedTarget == null) return;

        float distance = Vector3.Distance(playerInputTarget.position, expectedTarget.position);
        bool isCorrectPosition = distance < 0.5f;

        if (isCorrectPosition)
        {
            currentBall?.TryHit(); // 让 BallController 决定是否在击球时机内
        }
        else
        {
            Debug.Log("[MISS] 方向错了");
        }
    }

    /// <summary>
    /// 由 BallController 调用，表示球成功击中并返回
    /// </summary>
    public void OnBallReturned()
    {
        SwitchPlayerAndContinue();
    }

    /// <summary>
    /// 由 BallController 调用，球飞出边界未击中
    /// </summary>
    public void OnBallMissed()
    {
        StartCoroutine(WaitThenNextTurn());
    }

    private void SwitchPlayerAndContinue()
    {
        currentPlayer = (currentPlayer == Player.Player1) ? Player.Player2 : Player.Player1;

        ScoreManager.Instance.HighlightActivePlayer(currentPlayer);
        StartNewTurn();
    }

    private IEnumerator WaitThenNextTurn()
    {
        yield return new WaitForSeconds(1f);
        StartNewTurn();
    }

    private Transform GetTargetTransformByIndex(int index)
    {
        if (index < 0 || index >= allTargets.Length)
        {
            Debug.LogError("Invalid index: " + index);
            return null;
        }
        return allTargets[index];
    }
}


