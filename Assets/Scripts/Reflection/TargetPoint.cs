using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPoint : EnvironmentObject
{
    [SerializeField] private Color targetColor;     //목표 색상
    [SerializeField] private Transform objectToMove;    // 움직일 오브젝트
    [SerializeField] private Vector3 targetPosition;  // 목표 위치
    [SerializeField] private float lerpTime;    // 러프 시간
    [SerializeField] private float elapsedTime;     // 경과 시간
    [SerializeField] private float fadeDuration;    // 색상 변경 지속 시간
    [SerializeField] private bool isClear = false;
    [SerializeField] private AudioClip[] audioClips;

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

    public override bool OnInteract()
    {
        HandleInteraction(CharacterManager.Instance.Player.gameObject);
        return true;
    }
    public virtual void HandleInteraction(GameObject player)
    {
        Debug.Log($"{gameObject.name} 입니다.");
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
                MoveObject();

                StartCoroutine(ChangeToDisable());
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
        if (targetRenderer == null) return;

        // 새로운 머티리얼을 생성하여 개별 적용
        targetRenderer.material = new Material(targetRenderer.sharedMaterial);

        targetRenderer.material.SetColor("_Color", targetColor);
        targetRenderer.material.SetColor("_EmissionColor", targetColor);
    }

    // 목표지점 색상과 빛 색상 체크
    public bool CheckColorMatch(Color beamColor, float tolerance = 0.1f)
    {
        float rDiff = Mathf.Abs(targetColor.r - beamColor.r);
        float gDiff = Mathf.Abs(targetColor.g - beamColor.g);
        float bDiff = Mathf.Abs(targetColor.b - beamColor.b);

        return (rDiff < tolerance && gDiff < tolerance && bDiff < tolerance);
    }

    public bool isTargetClear()
    {
        return isClear;
    }

    // 움직일 오브젝트의 목표 지점 설정
    private void MoveObject()
    {
        SoundManager.PlayClip(audioClips[0]);
        if (objectToMove != null)
        {
            // 문이 바라보는 방향을 기준으로 왼쪽 방향 계산
            Vector3 leftDirection = -objectToMove.right;

            // 문이 바라보는 방향 기준 왼쪽으로 5m 이동
            targetPosition = objectToMove.position + leftDirection * 5f;
        }
    }

    IEnumerator ChangeToDisable()
    {
        SoundManager.PlayClip(audioClips[1]);
        Material mat = targetRenderer.material;

        Color startColor = mat.color;             // 원래 색상
        Color targetColor = Color.gray;   // 어두운 회색
        Color startEmission = mat.GetColor("_EmissionColor");  // 원래 에미션 색상
        Color targetEmission = targetColor * 0.5f; // 어두운 회색 기반으로 에미션 감소

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            mat.color = Color.Lerp(startColor, targetColor, t);
            mat.SetColor("_EmissionColor", Color.Lerp(startEmission, targetEmission, t)); // 에미션도 점점 어두워짐

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mat.color = targetColor;
        mat.SetColor("_EmissionColor", targetEmission); // 최종적으로 어두운 회색으로 설정
    }
}
