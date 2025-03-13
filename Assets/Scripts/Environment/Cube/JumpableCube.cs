using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpableCube : Cube //상호작용시 순간적인 점프가 가능한 큐브
{
    public float jumpForce; // cube를 밟았을 때의 점프력
    private void Awake()
    {
        cubeType = "Jumpable";
    }
    public void JumpBlock()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    public override void HandleInteraction(GameObject player)
    {
        Debug.Log($"{gameObject.name}이(가) 점프를 실행함!");
        JumpBlock();
    }
}
