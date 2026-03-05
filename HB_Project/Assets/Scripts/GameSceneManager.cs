using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameSceneManager : MonoBehaviour
{
    // 어디서든 접근 하도록 싱글톤
    public static GameSceneManager Instance;

    [Header("UI 패널")]
    public GameObject YouDiedPanel;
    public GameObject VictoryPanel;
    public GameObject EscMenuPanel;

    public Transform RespawnPoint;
    public float RespawnDelay = 5f;
    public BossHealth Boss;
    public BossUI _bossUi;


    private PlayerInput _playerInput;

    private bool _isGameOver = false;
    
    // 메뉴창이 열렸는지 확인
    public bool IsMenuOpen => EscMenuPanel != null && EscMenuPanel.activeSelf;
    public bool IsGameOver => _isGameOver;

    void Awake()
    {    
        // 싱글톤 초기화
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

    }

    void Start()
    {
        // 처음에 모든 패널 꺼두기
        if (YouDiedPanel) YouDiedPanel.SetActive(false);
        if (VictoryPanel) VictoryPanel.SetActive(false);
        if (EscMenuPanel) EscMenuPanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnEscMenu(InputValue value)
    {
        if(_isGameOver) return;

        if (value.isPressed)
        {
            ToggleEscMenu();
        }
    }

    public void ToggleEscMenu()
    {
        if (EscMenuPanel == null) return;

        bool isActive = !EscMenuPanel.activeSelf;
        EscMenuPanel.SetActive(isActive);

        // 마우스 커서 제어
        Cursor.visible = isActive;
        Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;

        if(_playerInput != null)
        {
            string mapName = isActive ? "UI" : "PlayerAction";
            _playerInput.SwitchCurrentActionMap(mapName);
        }
    }

    public void PlayerDied()
    {
        if(YouDiedPanel) YouDiedPanel.SetActive(true);
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(RespawnDelay);

        GameObject player = GameObject.FindWithTag("Player");
        if(player != null)
        {
            var moveLocate = player.GetComponent<CharacterController>();
            if(moveLocate != null) moveLocate.enabled = false;
            player.transform.position = RespawnPoint.position;
            if(moveLocate != null) moveLocate.enabled = true;

            var health = player.GetComponent<PlayerHealth>();
            if (health != null) health.ResetHealth();

            var items = player.GetComponent<Items>();
            if (items != null) items.ResetItems();

            player.GetComponent<PlayerController>().enabled = true;
            player.GetComponent<PlayerMove>().enabled = true;


            if (Boss != null)
            {
                Boss.ResetBoss();

                if (_bossUi != null)
                {
                    _bossUi.HideUI();
                }
                else
                {
                    if(_bossUi != null) _bossUi.HideUI();
                }
            }

            _isGameOver = false;
            if (YouDiedPanel) YouDiedPanel.SetActive(false);
        }
    }

    public void BossDefeated()
    {
        if (_isGameOver) return;
        _isGameOver = true;

        if(VictoryPanel) VictoryPanel.SetActive(true);
        Invoke("GoToTitle", 5f);
    }

    public void RestartCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
