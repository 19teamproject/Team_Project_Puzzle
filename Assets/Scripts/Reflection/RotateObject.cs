using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateObject : EnvironmentObject
{
    [SerializeField] private TargetPoint targetPoint;
    [SerializeField] private GameObject lightGenerator; // LightGenerator 오브젝트
    [SerializeField] private float rotationSpeed = 100f; 
    [SerializeField] private bool isRotating = false; // E 키로 회전 활성화 여부

    public void Rotate(float direction)
    {
        transform.Rotate(Vector3.up, direction * rotationSpeed * Time.deltaTime, Space.World);
    }

    public override bool OnInteract()
    {
        HandleInteraction(CharacterManager.Instance.Player.gameObject);

        isRotating = !isRotating; // E 키 입력 시 토글

        if (lightGenerator != null)
        {
            LightGenerator lightGen = lightGenerator.GetComponent<LightGenerator>();
            if (lightGen != null)
            {
                lightGen.ToggleLightGeneration(isRotating);
            }
        }

        return true;
    }

    public bool IsClear()
    {
        return targetPoint != null && targetPoint.isTargetClear();
    }

    public virtual void HandleInteraction(GameObject player)
    {
        Debug.Log($"{gameObject.name} 입니다.");
    }
}
