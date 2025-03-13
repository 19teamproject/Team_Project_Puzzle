using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum CubeType
{
    Scalable,
    Jumpable,
    Teleportable
}
public class Cube : EnvironmentObject
{
    public string cubeType; // 큐브 타입을 지정하기 위한 변수

    private Interaction interaction;
    protected virtual void Start()
    {
        transform.localScale = Vector3.one; //기본적인 큐브의 사이즈
        // 필요시 큐브 초기화로직
    }
    public override bool OnInteract()
    {
        HandleInteraction(CharacterManager.Instance.Player.gameObject);
        return false;
    }
    public virtual void HandleInteraction(GameObject player)
    {
        Debug.Log($"{gameObject.name}은 {cubeType}의 Cube입니다.");
    }
}





