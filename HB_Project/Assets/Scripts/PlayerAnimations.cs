using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private PlayerController _playerController;

    void Awake() => _playerController = GetComponent<PlayerController>();

    public void AnimationUpdate()
    {
        if (_playerController == null || _playerController.GetAnimator() == null) return;

        Animator anim = _playerController.GetAnimator();
        Vector2 input = _playerController.GetMoveInput();

        if (_playerController.IsLockOn)
        {
            anim.SetFloat("InputX", input.x, 0.1f, Time.deltaTime);
            anim.SetFloat("InputZ", input.y, 0.1f, Time.deltaTime);
        }
        else
        {
            anim.SetFloat("InputX", 0f, 0.1f, Time.deltaTime);
            anim.SetFloat("InputZ", input.magnitude, 0.1f, Time.deltaTime);
        }

        float moveSpeed = _playerController.IsAttack ? 0f : input.magnitude;
        anim.SetFloat("MoveSpeed", moveSpeed);
        
    }

}