using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// particle system을 사용하여 물줄기를 구현하였습니다.

public class Water : MonoBehaviour
{
    public Transform firePoint; // 물이 나오는 위치
    public float waterRange = 10f; // 물줄기 사거리
    public ParticleSystem waterParticle; // Particle system으로 물줄기 효과

    private void Update()
    {
       if (Input.GetMouseButtonDown(0))  // 마우스 누르면 물줄기 발사
       {
            waterParticle.Play();
       }
       else if (Input.GetMouseButtonUp(0))
       {
            waterParticle.Stop();  // 마우스 떼면 물 효과 중지
       }

       if (Input.GetMouseButton(0))
       {
            CheckForFire();
       }
        
    }

    void CheckForFire()
    {
        RaycastHit hit;  // 물줄기가 맞은 물체의 정보를 저장하는 변수
        if(Physics.Raycast(firePoint.position, firePoint.forward, out hit, waterRange))  // firePoint에서 앞쪽으로 레이저를 쏨
        {
            if(hit.collider.CompareTag("Fire"))  // 레이저에 맞은 물체가 Fire 태그를 갖고 있으면
            {
                hit.collider.GetComponent<Fire>().ExtinguishFire();  // 불을 꺼라
            }
        }
    }

}
