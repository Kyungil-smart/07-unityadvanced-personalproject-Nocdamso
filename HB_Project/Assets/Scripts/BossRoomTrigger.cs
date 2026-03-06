using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    public GameObject DoorObject;
    
    private bool _isLocked = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("감지기 접촉: " + other.name);

        if(!_isLocked && other.CompareTag("Player"))
        {
            Debug.Log("플레이어 태그 확인됨! 문을 닫습니다.");
            LockDoor();
        }
    }

    public void LockDoor()
    {
        _isLocked = true;
        if(DoorObject != null) DoorObject.SetActive(true);
        Debug.Log("보스방을 봉쇄합니다.");
    }

    public void UnlockDoor()
    {
        _isLocked = false;
        if(DoorObject != null) DoorObject.SetActive(false);
    }
}
