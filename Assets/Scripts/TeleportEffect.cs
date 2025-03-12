using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TeleportEffect : MonoBehaviour
{
    public TeleportEffect teleportTarget;
    public float teleportDelay;

    public void StartTeleport(GameObject player) //플레이어 텔레포트 기능 구현
    {
        if(teleportTarget != null) //이동할 큐브가 존재한다면
        {
            StartCoroutine(TeleportPlayer(player)); //코루틴을 사용하여 메서드로
        }
        else
        {
            Debug.Log("연결된 큐브가 없습니다.");
        }
    }
    private IEnumerator TeleportPlayer(GameObject player)
    {
        yield return new WaitForSeconds(teleportDelay); //지연시간 만큼 지연

        player.transform.position = teleportTarget.transform.position + Vector3.up * 1.5f; //큐브타겟 목표지점의
                                                                                           //위에 텔레포트하여 충돌 방지
        Debug.Log("플레이어 이동 완료");
    }
}
