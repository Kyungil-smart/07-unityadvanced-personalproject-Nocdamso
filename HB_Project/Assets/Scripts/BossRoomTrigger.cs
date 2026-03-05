using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    private BoxCollider _collider;

    void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerExit(Collider other)
    {
        // 플레이어가 트리거를 통과했을 때
        if (other.CompareTag("Player"))
        {
            LockDoor();
        }
    }

    public void LockDoor()
    {
        // 트리거를 물리적인 벽으로 전환
        _collider.isTrigger = false; 


        Debug.Log("보스방이 봉쇄되었습니다! 이제 못 나갑니다.");
    }

    public void UnlockDoor()
    {
        // 보스를 잡았거나 리스폰 시 다시 열어주는 함수
        _collider.isTrigger = true;
    }
}
