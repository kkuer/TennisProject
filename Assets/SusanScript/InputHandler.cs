using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public GameManager gameManager;

    void Update()
    {
        if (gameManager == null)
        {
            Debug.LogError(" GameManager 没有拖进来！");
            return;
        }

        // 显示当前回合状态（可选）
        Debug.Log("🎮 当前轮到: " + gameManager.currentPlayer);

        bool inputDetected = false;

        if (gameManager.currentPlayer == GameManager.Player.Player1)
        {
            if (Input.GetKeyDown(KeyCode.C)) { inputDetected = true; HandleInput(0, "C"); }
            if (Input.GetKeyDown(KeyCode.B)) { inputDetected = true; HandleInput(1, "B"); }
            if (Input.GetKeyDown(KeyCode.A)) { inputDetected = true; HandleInput(2, "A"); }
            if (Input.GetKeyDown(KeyCode.D)) { inputDetected = true; HandleInput(3, "D"); }
            if (Input.GetKeyDown(KeyCode.E)) { inputDetected = true; HandleInput(4, "E"); }
            if (Input.GetKeyDown(KeyCode.F)) { inputDetected = true; HandleInput(5, "F"); }
        }
        else if (gameManager.currentPlayer == GameManager.Player.Player2)
        {
            if (Input.GetKeyDown(KeyCode.Alpha3)) { inputDetected = true; HandleInput(0, "3"); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { inputDetected = true; HandleInput(1, "2"); }
            if (Input.GetKeyDown(KeyCode.Alpha1)) { inputDetected = true; HandleInput(2, "1"); }
            if (Input.GetKeyDown(KeyCode.Alpha4)) { inputDetected = true; HandleInput(3, "4"); }
            if (Input.GetKeyDown(KeyCode.Alpha5)) { inputDetected = true; HandleInput(4, "5"); }
            if (Input.GetKeyDown(KeyCode.Alpha6)) { inputDetected = true; HandleInput(5, "6"); }
        }

        // 有按键但不是当前玩家可用键
        if (!inputDetected && Input.anyKeyDown)
        {
            Debug.LogWarning("❌ 按下了无效按键（不是当前玩家的键位）");
        }
    }

    void HandleInput(int index, string keyLabel)
    {
        Debug.Log($"✅ [输入检测] 当前玩家: {gameManager.currentPlayer}，按下了 {keyLabel}，对应目标索引: {index}");
        gameManager.OnPlayerInput(index);
    }
}



