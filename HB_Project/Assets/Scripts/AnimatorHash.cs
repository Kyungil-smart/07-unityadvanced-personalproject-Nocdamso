using UnityEngine;

public static class AnimatorHash
{
    // 애니메이터 선언해두고 꺼내 쓸 수 있게 관리
    public static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
    public static readonly int InputX = Animator.StringToHash("InputX");
    public static readonly int InputZ = Animator.StringToHash("InputZ");
    public static readonly int LightAttack = Animator.StringToHash("LightAttack");
    public static readonly int HeavyAttack = Animator.StringToHash("HeavyAttack");
    public static readonly int Roll = Animator.StringToHash("Roll");
    public static readonly int Jump = Animator.StringToHash("Jump");
    public static readonly int IsRunning = Animator.StringToHash("IsRunning");
    public static readonly int UseItem = Animator.StringToHash("UseItem");
    public static readonly int Hit = Animator.StringToHash("Hit");
    public static readonly int Die = Animator.StringToHash("Die");
}
