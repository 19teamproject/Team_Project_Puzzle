using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwitchManager : MonoBehaviour
{
    private Renderer switchRenderer;
    public GameObject[] blocks; // 사라질 블록들
    public Material secondMat;

    public AudioSource audioSource;
    public AudioClip switchSound; 

    private void Awake()
    {
        if (switchRenderer == null)
        {
            switchRenderer = GetComponent<Renderer>(); // 현재 오브젝트에서 Renderer 찾기
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("무언가 스위치에 닿음: " + other.gameObject.name);
        if (other.CompareTag("Water"))  // 스위치가 물에 닿으면
        {
            Debug.Log("물이 닿았음!");
            SwitchOn(); // 스위치 활성화
        }
    }

    void SwitchOn()
    {
        switchRenderer.material = secondMat;  // 빨간색에서 초록색으로 바뀜

        if (audioSource != null && switchSound != null)
        {
            audioSource.PlayOneShot(switchSound);
        }

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
