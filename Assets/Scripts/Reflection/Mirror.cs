using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    [SerializeField] private Color mirrorColor;     // 거울 색상

    private Renderer mirrorRenderer;
    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        mirrorRenderer = GetComponent<Renderer>();
        if (mirrorRenderer == null)
        {
            Debug.LogError($"Renderer가 없습니다! 오브젝트: {gameObject.name}", gameObject);
            return;
        }

        propertyBlock = new MaterialPropertyBlock();
        UpdateMirrorColor();
    }

    // 거울 색상 업데이트
    private void UpdateMirrorColor()
    {
        if (mirrorRenderer == null || propertyBlock == null) return;

        mirrorRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", mirrorColor);
        mirrorRenderer.SetPropertyBlock(propertyBlock);
    }

    // 반사각 계산
    public Vector3 Reflect(Vector3 incomingDirection, Vector3 normal)
    {
        return Vector3.Reflect(incomingDirection, normal);
    }

    // 거울 색상 반환
    public Color GetMirrorColor()
    {
        return mirrorColor;
    }

    // 들어온 빛 색상과 현재 거울 색상 믹스
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
