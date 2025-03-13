using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public string cubeType; // 큐브 타입을 지정하기 위한 변수

    private EnvironmentData data;
    private Interaction interaction;
    protected virtual void Start()
    {
        transform.localScale = new Vector3(50f, 50f, 50f); //기본적인 큐브의 사이즈
        // 필요시 큐브 초기화로직
    }
    public virtual void HandleInteraction(GameObject player)
    {
        Debug.Log($"{gameObject.name}은 {cubeType}의 Cube입니다.");
    }
}





