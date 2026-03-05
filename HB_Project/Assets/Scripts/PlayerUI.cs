using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    private HealthPoint _healthPoint;
    private PlayerStamina _playerStamina;
    private Items _items;

    [Header("체력 UI")]
    public Image HpBar;

    [Header("스태미너 UI")]
    public Image StminBar;

    [Header("포션 UI")]
    public TextMeshProUGUI PotionText;


    void Awake()
    {
        // 같은 오브젝트(Player)에 붙어있는 스크립트들을 찾아옵니다.
        _healthPoint = GetComponent<HealthPoint>();
        _playerStamina = GetComponent<PlayerStamina>();
        _items = GetComponent<Items>();
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        // 체력 데이터와 연동
        if(HpBar != null && _healthPoint != null) 
            HpBar.fillAmount = _healthPoint._currentHp / _healthPoint.MaxHp;

        // 스태미나 데이터와 연동
        if(StminBar != null && _playerStamina != null) 
            StminBar.fillAmount = _playerStamina.CurrentStamina / _playerStamina.MaxStamina;

        // 포션 개수 데이터와 연동
        if(PotionText != null && _items != null) 
            PotionText.text = $"{_items.PotionCount}"; 
    }
    
}
