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

    //거울 색상 업데이트
    private void UpdateMirrorColor()
    {
        if (mirrorRenderer == null) return;

        // 기존 머티리얼을 복사하여 새 머티리얼을 생성 (개별 오브젝트 적용)
        mirrorRenderer.material = new Material(mirrorRenderer.sharedMaterial);
        mirrorRenderer.material.SetColor("_Color", mirrorColor);
    }

    // 반사각 계산
    public bool Reflect(Vector3 incomingDirection, Vector3 normal, out Vector3 reflectedDirection)
    {
        Vector3 mirrorForward = -transform.forward;

        // 뒤로 빛이 들어왔으면 반사 x
        if (Vector3.Dot(incomingDirection, mirrorForward) > 0)
        {
            reflectedDirection = Vector3.zero;
            return false;
        }

        // 반사각 계산
        reflectedDirection = Vector3.Reflect(incomingDirection, normal);
        return true;
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
