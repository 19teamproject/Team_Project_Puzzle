using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 100f;

    public void Rotate(float direction)
    {
        transform.Rotate(Vector3.up, direction * rotationSpeed * Time.deltaTime, Space.World);
    }
}
