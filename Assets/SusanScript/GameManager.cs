using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake() => Instance = this;

    public enum Player { Player1, Player2 }
    public Player currentPlayer;

    public BallMachine ballMachine;
    public Transform[] allTargets;
    public BallController currentBall;

    public bool IsWaitingForInput { get; private set; }

    private Transform expectedTarget;
    private float validHitTimeStart;
    private float validHitTimeEnd;

    void Start()
    {
     StartNewTurn(); // 开局就开始 Player1 回合，不手动调用 Highlight
    }

    public void StartNewTurn()
    {
        IsWaitingForInput = false;

        // 高亮当前轮玩家
        ScoreManager.Instance.HighlightActivePlayer(currentPlayer);

        StartCoroutine(ballMachine.LaunchBall());
    }


    public void StartInputWindow(Transform target)
    {
        expectedTarget = target;
        validHitTimeStart = Time.time;
        validHitTimeEnd = Time.time + 0.5f;
        IsWaitingForInput = true;
    }

    public void OnPlayerInput(int inputIndex)
    {
        if (!IsWaitingForInput) return;

        Transform playerInputTarget = GetTargetTransformByIndex(inputIndex);
        if (playerInputTarget == null || expectedTarget == null) return;

        float distance = Vector3.Distance(playerInputTarget.position, expectedTarget.position);
        bool isCorrectPosition = distance < 0.5f;

        if (isCorrectPosition)
        {
            currentBall?.TryHit(); // 由 BallController 自己判断是否在击球时间
        }

        IsWaitingForInput = false;
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

    public void OnBallReturned()
    {
        SwitchPlayerAndContinue();
    }

    public void OnBallMissed()
    {
        StartCoroutine(WaitThenNextTurn());
    }

    private void SwitchPlayerAndContinue()
    {
        currentPlayer = (currentPlayer == Player.Player1) ? Player.Player2 : Player.Player1;

        ScoreManager.Instance.HighlightActivePlayer(currentPlayer); // 高亮

        StartNewTurn();
    }


    private IEnumerator WaitThenNextTurn()
    {
        yield return new WaitForSeconds(1f);
        StartNewTurn();
    }


}
