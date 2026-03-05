using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Header("스태미나 설정")]
    public float MaxStamina = 100f;
    public float CurrentStamina;

    [Header("소모량 설정")]
    public float RunCostPerSecond = 10f;
    public float RollCost = 20f;
    public float JumpCost = 10f;
    public float AttackCost = 15f;

    [Header ("스태미나 회복")]
    public float RegenRate = 30f;
    public float RegenDelay = 1f;

    private float _regenTimer;

    void Awake()
    {
        CurrentStamina = MaxStamina;
    }

    void Update()
    {
        if (_regenTimer > 0)
        {
            _regenTimer -= Time.deltaTime;
        }
        else if (CurrentStamina < MaxStamina)
        {
            CurrentStamina += RegenRate *Time.deltaTime;
            CurrentStamina = Mathf.Min(CurrentStamina, MaxStamina);
        }
    }

    public bool SpendStamina(float amount)
    {
        if(CurrentStamina >= amount)
        {
            CurrentStamina -= amount;
            _regenTimer = RegenDelay;
            return true;
        }

        return false;
    }

    public void SpendStaminaPerSec(float amount)
    {
        CurrentStamina -= amount * Time.deltaTime;
        CurrentStamina = Mathf.Max(CurrentStamina, 0);
        _regenTimer = RegenDelay;
    }

    public bool CanAction(float canAction)
    {
        return CurrentStamina >= canAction;
    }
}
