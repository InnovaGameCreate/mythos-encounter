using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseUnlockInTitle : MonoBehaviour
{
    void Start()
    {
        // 現在のシーンが「タイトル」シーンか確認
        if (SceneManager.GetActiveScene().name == "Title")
        {
            // マウスロックを解除
            UnlockCursor();
        }
    }

    void UnlockCursor()
    {
        // マウスロックを解除する
        Cursor.lockState = CursorLockMode.None;

        // マウスカーソルを表示する
        Cursor.visible = true;
    }
}