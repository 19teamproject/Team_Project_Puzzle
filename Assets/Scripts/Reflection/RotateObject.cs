using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateObject : MonoBehaviour
{
    // �ӽ÷� ����� ������Ʈ ȸ��, ���߿� ���� �� ���� ����

    private Vector3 lastMousePosition;
    private bool isDragging = false;
    public float rotationSpeed = 0.5f;

    void OnMouseDown()
    {
        isDragging = true;
        lastMousePosition = Input.mousePosition;
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 mouseDelta = currentMousePosition - lastMousePosition;

            float rotationAmount = mouseDelta.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, -rotationAmount, Space.World);

            lastMousePosition = currentMousePosition;
        }
    }
}
