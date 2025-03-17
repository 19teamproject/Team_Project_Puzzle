using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateObject : EnvironmentObject
{
    [SerializeField] private TargetPoint targetPoint;
    [SerializeField] private GameObject lightGenerator; // LightGenerator 오브젝트

    public bool isRotating = false; // E 키로 회전 활성화 여부
    public float rotationSpeed = 100f;
    public float lastInputTime = 0;
    private bool hasToggledLight = false;

    public void Rotate(float angle, Vector3 axis)
    {
        transform.Rotate(axis, angle, Space.World); // 주어진 축(axis)과 각도(angle)로 회전
    }

    public void RotateTimeOutCheck()
    {
        if (isRotating && Time.time - lastInputTime > 2f)
        {
            isRotating = false;
            Debug.Log("입력 시간 초과로 회전 모드 해제");
        }
    }

    public override bool OnInteract()
    {
        HandleInteraction(CharacterManager.Instance.Player.gameObject);

        if (lightGenerator != null && !hasToggledLight)
        {
            LightGenerator lg = lightGenerator.GetComponent<LightGenerator>();

            lg.ToggleLightMaterial();
            lg.ToggleLightGeneration(true);
            hasToggledLight = true;
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
