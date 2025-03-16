using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwitchManager : MonoBehaviour
{
    private Renderer switchRenderer;
    public GameObject[] blocks; // 사라질 블록들
    public Color activeColor = Color.green; // 활성화된 스위치 색상
    public Color defaultColor = Color.red; // 기본 스위치 색상

    private void Awake()
    {
        if (switchRenderer == null)
        {
            switchRenderer = GetComponent<Renderer>(); // 현재 오브젝트에서 Renderer 찾기
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))  // 스위치가 물에 닿으면
        {
            SwitchOn(); // 스위치 활성화
        }
    }

    void SwitchOn()
    {
        switchRenderer.material.color = activeColor;  // 빨간색에서 초록색으로 바뀜

        ActivateBlocks(); 
    }

    void ActivateBlocks()
    {
        foreach (GameObject block in blocks)
        {
            Destroy(block);   // 블록 사라지며 문 열림
        }
    }
}
