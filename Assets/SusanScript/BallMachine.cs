using System.Collections;
using UnityEngine;

public class BallMachine : MonoBehaviour
{
    public enum Side { Left, Right }

    [Header("目标点")]
    public Transform[] leftTargets;
    public Transform[] rightTargets;

    [Header("预制体")]
    public GameObject indicatorPrefab;
    public GameObject ballPrefab;

    [Header("参数")]
    public float indicatorDuration = 2f;
    public float ballTravelTime = 1f;

    private Transform currentTarget;
    private GameObject currentBallObject;

    public IEnumerator LaunchBall()
    {
        // 防止重复发球
        if (currentBallObject != null)
        {
            Debug.LogWarning("已经有球在场上，不能重复发球");
            yield break;
        }

        // 随机选择一个目标点（共6个）
        int index = Random.Range(0, 6);
        currentTarget = (index < 3) ? leftTargets[index] : rightTargets[index - 3];

        // 创建提示圈（indicator）
        if (indicatorPrefab != null)
        {
            Instantiate(indicatorPrefab, currentTarget.position, Quaternion.identity);
        }

        // 生成并配置球
        Vector3 firePoint = transform.position + new Vector3(0, 1f, 0);
        currentBallObject = Instantiate(ballPrefab, firePoint, Quaternion.identity);

        BallController ballController = currentBallObject.GetComponent<BallController>();
        ballController.start = firePoint;
        ballController.end = currentTarget.position;
        ballController.control = (firePoint + currentTarget.position) / 2 + new Vector3(0, 2f, 0);
        ballController.travelTime = ballTravelTime;

        // 告诉 GameManager 当前球是谁、目标点是哪里
        GameManager.Instance.currentBall = ballController;
        GameManager.Instance.SetExpectedTarget(currentTarget); // 替代 StartI

        yield return null;
    }

    public void ClearCurrentBall()
    {
        currentBallObject = null;
    }
}



