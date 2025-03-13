using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    [SerializeField] private Color mirrorColor;     // �ſ� ����

    private Renderer mirrorRenderer;
    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        mirrorRenderer = GetComponent<Renderer>();
        if (mirrorRenderer == null)
        {
            Debug.LogError($"Renderer�� �����ϴ�! ������Ʈ: {gameObject.name}", gameObject);
            return;
        }

        propertyBlock = new MaterialPropertyBlock();
        UpdateMirrorColor();
    }

    // �ſ� ���� ������Ʈ
    private void UpdateMirrorColor()
    {
        if (mirrorRenderer == null || propertyBlock == null) return;

        mirrorRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", mirrorColor);
        mirrorRenderer.SetPropertyBlock(propertyBlock);
    }

    // �ݻ簢 ���
    public Vector3 Reflect(Vector3 incomingDirection, Vector3 normal)
    {
        return Vector3.Reflect(incomingDirection, normal);
    }

    // �ſ� ���� ��ȯ
    public Color GetMirrorColor()
    {
        return mirrorColor;
    }

    // ���� �� ����� ���� �ſ� ���� �ͽ�
    public Color MixColor(Color incomingColor)
    {
        Color mix = new Color(
            Mathf.Lerp(incomingColor.r, mirrorColor.r, 0.5f),
            Mathf.Lerp(incomingColor.g, mirrorColor.g, 0.5f),
            Mathf.Lerp(incomingColor.b, mirrorColor.b, 0.5f)
        );

        return mix;
    }
}
