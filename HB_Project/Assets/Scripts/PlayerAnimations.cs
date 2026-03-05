using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private PlayerController _playerController;
    private PlayerMove _playerMove;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _playerMove = GetComponent<PlayerMove>();        
    }

    public void AnimationUpdate()
    {
        if (_playerController == null || _playerController.GetAnimator() == null) return;

        Animator anim = _playerController.GetAnimator();
        Vector2 input = _playerController.GetMoveInput();

        float multiplier = _playerMove.IsRunning ? 1f : 0.5f;

        if (_playerController.IsLockOn)
        {
            anim.SetFloat(AnimatorHash.InputX, input.x * multiplier, 0.1f, Time.deltaTime);
            anim.SetFloat(AnimatorHash.InputZ, input.y * multiplier, 0.1f, Time.deltaTime);
        }
        else
        {
            anim.SetFloat(AnimatorHash.InputX, 0f, 0.1f, Time.deltaTime);
            anim.SetFloat(AnimatorHash.InputZ, input.magnitude * multiplier, 0.1f, Time.deltaTime);
        }

        float moveSpeed = _playerController.IsAttack ? 0f : input.magnitude * multiplier;
        anim.SetFloat(AnimatorHash.MoveSpeed, moveSpeed);      
    }
}