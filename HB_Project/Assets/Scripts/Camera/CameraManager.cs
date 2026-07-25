using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [Header("카메라 설정")]
    public CinemachineCamera PlayerCamera; // 평상시 카메라
    public CinemachineCamera LockOnCamera; // 록온용 카메라
    public PlayerController Player;

    private bool _lastLockOn = false;

    void Update()
    {
        if (Player == null || PlayerCamera == null || LockOnCamera == null) return;

        bool currentLockOn = Player.IsLockOn && Player.LockOnTarget != null;
        // 플레이어의 록온 상태와 타겟 존재 여부 확인
        if (Player.IsLockOn && Player.LockOnTarget != null)
        {
            // 록온 카메라
            LockOnCamera.Priority = 10;
            PlayerCamera.Priority = 1;

            // 록온 카메라가 타겟을 바라보게 설정
            LockOnCamera.Target.LookAtTarget = Player.LockOnTarget;
        }
        else
        {
            if (_lastLockOn == true)
            {
                FixedLockOn();
            }

            // 평상시 카메라
            LockOnCamera.Priority = 1;
            PlayerCamera.Priority = 10;

        }

        _lastLockOn = currentLockOn;
    }
    private void FixedLockOn()
    {
        Quaternion syncRotation = LockOnCamera.transform.rotation;
        Vector3 rotate = syncRotation.eulerAngles;

        // PlayerCamera에서 Orbital Follow 컴포넌트를 가져옴
        var orbitalFollow = PlayerCamera.GetComponent<CinemachineOrbitalFollow>();

        if (orbitalFollow != null)
        {
            // 좌우 회전값(Horizontal Axis) 동기화
            orbitalFollow.HorizontalAxis.Value = rotate.y;

            // 상하 회전값(Vertical Axis) 동기화
            float pitch = rotate.x;
            // 오일러 각(0~360)을 시네머신 축 값(-180~180)으로 보정
            if (pitch > 180) pitch -= 360f;
            orbitalFollow.VerticalAxis.Value = pitch;

            // 유니티 6 시네머신에게 카메라 위치와 회전을 강제로 갱신하도록 명령
            PlayerCamera.ForceCameraPosition(PlayerCamera.transform.position, syncRotation);
        }
    }
}
