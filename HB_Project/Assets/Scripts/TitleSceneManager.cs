using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class TitleSceneManager : MonoBehaviour
{
    void Start()
    {
        InputSystem.actions.Enable();

        Cursor.visible = true;

        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 1f;
    }
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            // 에디터에서 테스트할 때 꺼질 수 있게
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // 빌드된 게임이 종료
            Application.Quit();
        #endif

        Debug.Log("게임 종료");
    }
    
}
