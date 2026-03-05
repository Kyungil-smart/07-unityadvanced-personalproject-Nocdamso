using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossUI : MonoBehaviour
{
    [Header("참조")]
    public HealthPoint BossHealth;
    public BossAITest BossAI;

    [Header("UI 참조")]
    public GameObject UiParent;
    public Image BossHpBar;
    public TextMeshProUGUI BossNameText;

    [Header("보스 이름")]
    public string BossName = "미궁 속의 괴수 미노타우로스";

    void Start()
    {
        if (BossNameText != null) BossNameText.text = BossName;
        if (UiParent != null) UiParent.SetActive(false);
    }

    void Update()
    {
        if(BossHealth == null || BossAI == null || UiParent == null) return;

        BossHpBar.fillAmount = BossHealth._currentHp / BossHealth.MaxHp;
        
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(player.transform.position, 
                                              BossAI.transform.position);
            if (distance <= BossAI.DetectRange && BossHealth._currentHp > 0)
            {
                UiParent.SetActive(true);
            }
            else
            {
                UiParent.SetActive(false);
            }
        }

        if(BossHealth._currentHp <= 0)
        {
            Invoke("HideUI", 3f);
        }
    }

    public void HideUI()
    {
        if (UiParent != null) UiParent.SetActive(false);
    }
}
