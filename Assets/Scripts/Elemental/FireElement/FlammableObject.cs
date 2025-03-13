using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableObject : MonoBehaviour
{
    public Renderer targetRenderer; // 불에 타는 동안 색을 변경할 Renderer
    public Rigidbody rigid;
    public GameObject flameEffect,endEffect;  // 화염 효과, 마무리 효과 Prefab
    private bool isBurning = false;  // 불이 이미 붙었는지 확인하는 값 (중복 실행방지)
    public Color startColor = new Vector4(1,1,1,1);
    public Color endColor = new Vector4(0,0,0, 1);

    public float duration = 2f; // 색이 변하는 데 걸리는 시간

    public void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Fire") && !isBurning)
        {
            StartCoroutine(BurnWood());
        }
    }

    private IEnumerator BurnWood()
    {
        rigid.isKinematic = true;  //   // 오브젝트가 마구 움직이지 않게 고정
        isBurning = true;
        GameObject[] flame = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            Vector3 myPos = transform.position;
            flame[i] = Instantiate(flameEffect,myPos,Quaternion.identity);
            flame[i].transform.SetParent(transform); // 불꽃이 오브젝트 따라다니게 함
            flame[i].transform.position = new Vector3(myPos.x + Random.Range(0f, 1f), myPos.y + Random.Range(0f, 1f), myPos.z + Random.Range(0f, 1f));
        }

        float elapsedTime = 0f;  // 경과 시간 초기화

        while (elapsedTime < duration)  // duration 동안 실행
        {
            targetRenderer.material.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;  
            yield return null;
        }

        targetRenderer.material.color = endColor;
        Instantiate(endEffect, transform.position, Quaternion.identity);
        Destroy(transform.parent.gameObject);
    }

}
