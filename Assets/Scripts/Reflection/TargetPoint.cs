using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPoint : MonoBehaviour
{
    [SerializeField] private Color targetColor;     //��ǥ ����
    [SerializeField] private Transform objectToMove;    // ������ ������Ʈ
    [SerializeField] private Vector3 targetPosition;  // ��ǥ ��ġ
    [SerializeField] private float lerpTime;    // ���� �ð�
    [SerializeField] private float elapsedTime;     // ��� �ð�

    private bool isClear = false;

    private Renderer targetRenderer;
    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
        if (targetRenderer == null)
        {
            Debug.LogError($"Renderer�� �����ϴ�! ������Ʈ: {gameObject.name}", gameObject);
            return;
        }

        propertyBlock = new MaterialPropertyBlock();
        UpdateTargetColor();
    }

    private void Update()
    {
        if (isClear && objectToMove != null)
        {
            // ���� �̵��� �ð� ������ ���
            elapsedTime += Time.deltaTime;  // ��� �ð� �߰�
            float t = Mathf.Clamp01(elapsedTime / lerpTime);  // 0�� 1 ���̷� ������ ���

            // Lerp�� ����Ͽ� ���� ������ �̵�
            objectToMove.position = Vector3.Lerp(objectToMove.position, targetPosition, t);

            // ���� ��ǥ ��ġ�� ������ �� �̵��� ���߰� �ϱ�
            if (t >= 1f)
            {
                elapsedTime = 0f;  // ��� �ð� �ʱ�ȭ (���� �ٸ� ������ ���� ����)
            }
        }
    }

    // ��ǥ���� ��Ʈ �� ȣ��
    public void OnLightHit(Color beamColor)
    {
        if (!isClear)
        {
            // ���� ��ġ �� ������Ʈ ������
            if (CheckColorMatch(beamColor))
            {
                isClear = true;
                Debug.Log("��ǥ ����� ��ġ! ���� ����!");
                MoveObject();  
            }
            else
            {
                Debug.Log("������ �ٸ��ϴ�. �ٽ� �ݻ��ϼ���!");
            }
        }
    }

    // ��ǥ ������ ������Ʈ
    private void UpdateTargetColor()
    {
        targetRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", targetColor);
        propertyBlock.SetColor("_EmissionColor", targetColor);
        targetRenderer.SetPropertyBlock(propertyBlock);
    }

    // ��ǥ���� ����� �� ���� üũ
    public bool CheckColorMatch(Color beamColor, float tolerance = 0.3f)
    {
        float rDiff = Mathf.Abs(targetColor.r - beamColor.r);
        float gDiff = Mathf.Abs(targetColor.g - beamColor.g);
        float bDiff = Mathf.Abs(targetColor.b - beamColor.b);

        return (rDiff < tolerance && gDiff < tolerance && bDiff < tolerance);
    }

    // ������ ������Ʈ�� ��ǥ ���� ����
    private void MoveObject()
    {
        if (objectToMove != null)
        {
            targetPosition = objectToMove.position + Vector3.left * 5f;
        }
    }
}
