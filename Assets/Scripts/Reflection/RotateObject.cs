using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateObject : EnvironmentObject
{
    public float rotationSpeed = 100f;

    public void Rotate(float direction)
    {
        transform.Rotate(Vector3.up, direction * rotationSpeed * Time.deltaTime, Space.World);
    }

    public override bool OnInteract()
    {
        HandleInteraction(CharacterManager.Instance.Player.gameObject);
        return true;
    }

    public virtual void HandleInteraction(GameObject player)
    {
        Debug.Log($"{gameObject.name} 입니다.");
    }
}
