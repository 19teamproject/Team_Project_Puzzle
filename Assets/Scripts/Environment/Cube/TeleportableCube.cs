using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportableCube : Cube //텔레포트가 가능한 큐브
{
    public GameObject teleportTarget; //텔레포트를 위한 변수
    public Vector3 offset = Vector3.up;
    public void StartTeleport(GameObject player) //상호작용시 플레이어 텔레포트 기능 구현
    {
        if (teleportTarget != null) //이동할 큐브가 존재한다면
        {
            CharacterController controller=player.GetComponent<CharacterController>();
            controller.enabled = false;
            player.transform.position = teleportTarget.transform.position + offset; //큐브타겟 목표지점의 위에 텔레포트하여 충돌 방지
            Debug.Log("플레이어 이동 완료");
            controller.enabled = true;
        }
        else
        {
            Debug.Log("연결된 큐브가 없습니다.");
        }
    }
    public override void HandleInteraction(GameObject player)
    {
        StartTeleport(player);
    }
}
