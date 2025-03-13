using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPoint : MonoBehaviour
{
    [SerializeField] private Color targetColor;     //목표 색상
    [SerializeField] private Transform objectToMove;    // 움직일 오브젝트
    [SerializeField] private Vector3 targetPosition;  // 목표 위치
    [SerializeField] private float lerpTime;    // 러프 시간
    [SerializeField] private float elapsedTime;     // 경과 시간

    private bool isClear = false;

    private Renderer targetRenderer;
    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
        if (targetRenderer == null)
        {
            Debug.LogError($"Renderer가 없습니다! 오브젝트: {gameObject.name}", gameObject);
            return;
        }

        propertyBlock = new MaterialPropertyBlock();
        UpdateTargetColor();
    }

    private void Update()
    {
        if (isClear && objectToMove != null)
        {
            // 문이 이동할 시간 비율을 계산
            elapsedTime += Time.deltaTime;  // 경과 시간 추가
            float t = Mathf.Clamp01(elapsedTime / lerpTime);  // 0과 1 사이로 비율을 계산

            // Lerp를 사용하여 문을 서서히 이동
            objectToMove.position = Vector3.Lerp(objectToMove.position, targetPosition, t);

            // 문이 목표 위치에 도달한 후 이동을 멈추게 하기
            if (t >= 1f)
            {
                elapsedTime = 0f;  // 경과 시간 초기화 (추후 다른 동작을 위해 리셋)
            }
        }
    }

    // 목표지점 히트 시 호출
    public void OnLightHit(Color beamColor)
    {
        if (!isClear)
        {
            // 색상 일치 시 오브젝트 움직임
            if (CheckColorMatch(beamColor))
            {
                isClear = true;
                Debug.Log("목표 색상과 일치! 퍼즐 성공!");
                MoveObject();  
            }
            else
            {
                Debug.Log("색상이 다릅니다. 다시 반사하세요!");
            }
        }
    }

    // 목표 색상을 업데이트
    private void UpdateTargetColor()
    {
        targetRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", targetColor);
        propertyBlock.SetColor("_EmissionColor", targetColor);
        targetRenderer.SetPropertyBlock(propertyBlock);
    }

    // 목표지점 색상과 빛 색상 체크
    public bool CheckColorMatch(Color beamColor, float tolerance = 0.3f)
    {
        float rDiff = Mathf.Abs(targetColor.r - beamColor.r);
        float gDiff = Mathf.Abs(targetColor.g - beamColor.g);
        float bDiff = Mathf.Abs(targetColor.b - beamColor.b);

        return (rDiff < tolerance && gDiff < tolerance && bDiff < tolerance);
    }

    // 움직일 오브젝트의 목표 지점 설정
    private void MoveObject()
    {
        if (objectToMove != null)
        {
            targetPosition = objectToMove.position + Vector3.left * 5f;
        }
    }
}
