using System.Collections;
using UnityEngine;

public class BallMachine : MonoBehaviour
{
    public enum Side { Left, Right }

    public Transform[] leftTargets;
    public Transform[] rightTargets;

    public GameObject indicatorPrefab;
    public GameObject ballPrefab;

    public float indicatorDuration = 2f;
    public float ballTravelTime = 1f;

    private Transform currentTarget;
    private GameObject currentBallObject;


    public IEnumerator LaunchBall()
    {
        // 防止重复发球
        if (currentBallObject != null)
        {
            Debug.LogWarning("⚠️ 已经有球在场上，不能重复发球");
            yield break;
        }

        int index = Random.Range(0, 6);
        currentTarget = (index < 3) ? leftTargets[index] : rightTargets[index - 3];

        // 提示圈
        Instantiate(indicatorPrefab, currentTarget.position, Quaternion.identity);

        // 发球
        Vector3 firePoint = transform.position + new Vector3(0, 1f, 0);
        currentBallObject = Instantiate(ballPrefab, firePoint, Quaternion.identity);

        BallController ballController = currentBallObject.GetComponent<BallController>();
        ballController.start = firePoint;
        ballController.end = currentTarget.position;
        ballController.control = (firePoint + currentTarget.position) / 2 + new Vector3(0, 2f, 0);
        ballController.travelTime = ballTravelTime;

        GameManager.Instance.currentBall = ballController;
        GameManager.Instance.StartInputWindow(currentTarget);

        yield return null;
    }
    public void ClearCurrentBall()
    {
        currentBallObject = null;
    }


}
