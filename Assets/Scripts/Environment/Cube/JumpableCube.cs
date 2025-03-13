using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpableCube : Cube //상호작용시 순간적인 점프가 가능한 큐브
{
    public float jumpForce=10f; // cube를 밟았을 때의 점프력
    private bool canJump;
    private void Awake()
    {
        cubeType = "Jumpable";
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("충돌");
        if (other.TryGetComponent(out ThirdPersonController playerController))
        {
            StartCoroutine(WaitForInput(playerController));
        }
    }
    IEnumerator WaitForInput(ThirdPersonController playerController)
    {
        Debug.Log("대기 시작");
        yield return new WaitUntil(() => canJump);
        canJump = false;
        playerController.AddJumpForce(Vector3.up.normalized * jumpForce);
        Debug.Log("코드 실행");
    }
    public override void HandleInteraction(GameObject player)
    {
        canJump = true;
    }
}
