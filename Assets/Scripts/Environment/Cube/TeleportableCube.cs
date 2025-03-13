using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportableCube : Cube //텔레포트가 가능한 큐브
{
    public GameObject teleportTarget; //텔레포트를 위한 변수
    public float teleportDelay = 2f; //텔레포트동안 지연시간

    public void Awake()
    {
        cubeType = "Teleportable";
    }
    public void StartTeleport(GameObject player) //상호작용시 플레이어 텔레포트 기능 구현
    {
        if (teleportTarget != null) //이동할 큐브가 존재한다면
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

        player.transform.position = teleportTarget.transform.position + Vector3.up * 1.5f; //큐브타겟 목표지점의 위에 텔레포트하여 충돌 방지
        Debug.Log("플레이어 이동 완료");
    }
    public override void HandleInteraction(GameObject player)
    {
        StartTeleport(player);
    }
}
