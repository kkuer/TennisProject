using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    private void Awake() => Instance = this;

    public int scoreP1 = 0;
    public int scoreP2 = 0;

    public TextMeshProUGUI scoreTextP1;
    public TextMeshProUGUI scoreTextP2;

    public Color defaultColor = Color.black;
    public Color highlightColor = Color.red;

    void Start()
    {
        UpdateUI();
        HighlightActivePlayer(GameManager.Player.Player1); // 游戏开始先高亮 Player1
    }

    public void AddScore(GameManager.Player player)
    {
        if (player == GameManager.Player.Player1)
            scoreP1++;
        else
            scoreP2++;

        UpdateUI();
    }

    void UpdateUI()
    {
        scoreTextP1.text = "Player 1 Score: " + scoreP1;
        scoreTextP2.text = "Player 2 Score: " + scoreP2;
    }

    public void HighlightActivePlayer(GameManager.Player current)
    {
        if (current == GameManager.Player.Player1)
        {
            scoreTextP1.color = highlightColor;
            scoreTextP2.color = defaultColor;
        }
        else
        {
            scoreTextP1.color = defaultColor;
            scoreTextP2.color = highlightColor;
        }
    }
}
