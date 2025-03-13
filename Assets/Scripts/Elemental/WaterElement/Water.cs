using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public GameObject waterEffectPrefab; // 물줄기 이펙트
    public Transform fireObject; // 불 오브젝트
    public float waterForce = 10f; // 물줄기 발사 강도

    private void Update()
    {
       if (Input.GetMouseButtonDown(0))  // 마우스 클릭하면 물총 발사
       {
            FireWater(); 
       }
    }

    void FireWater()
    {
        GameObject waterEffect = Instantiate(waterEffectPrefab, transform.position, Quaternion.identity);
        Rigidbody rigid = waterEffect.GetComponent<Rigidbody>();
        Vector2 fireDirection = transform.up; // 물총이 향하는 방향

        rigid.velocity = fireDirection * waterForce; // 물줄기 발사

        Destroy(waterEffect, 2f);
    }
}
