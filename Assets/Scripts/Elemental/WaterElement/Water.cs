using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public GameObject waterEffectPrefab; // ���ٱ� ����Ʈ
    public Transform fireObject; // �� ������Ʈ
    public float waterForce = 10f; // ���ٱ� �߻� ����

    private void Update()
    {
       if (Input.GetMouseButtonDown(0))  // ���콺 Ŭ���ϸ� ���� �߻�
       {
            FireWater(); 
       }
    }

    void FireWater()
    {
        GameObject waterEffect = Instantiate(waterEffectPrefab, transform.position, Quaternion.identity);
        Rigidbody rigid = waterEffect.GetComponent<Rigidbody>();
        Vector2 fireDirection = transform.up; // ������ ���ϴ� ����

        rigid.velocity = fireDirection * waterForce; // ���ٱ� �߻�

        Destroy(waterEffect, 2f);
    }
}
