using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGenerator : MonoBehaviour
{
    [Header("Light")]
    [SerializeField] private LineRenderer lineRenderer;   // 빛을 시각적으로 표현

    [SerializeField] private LayerMask reflectableLayer;  // 반사 가능한 레이어 (거울)
    [SerializeField] private LayerMask blockLayer;        // 빛을 막는 레이어 (벽, 장애물)

    [SerializeField] private float maxDistance = 100f;    // 빛이 진행할 수 있는 최대 거리 
    [SerializeField] private float contactTime = 2f;  // 목표에 접촉해야 하는 시간 (초)
    [SerializeField] private float contactStartTime = 0f;  // 목표에 접촉한 시간 기록

    [SerializeField] private string targetTag = "TargetPoint";  // 목표 지점의 태그

    private bool isContactingTarget = false;    // 빛이 목표에 처음 컨택할때 체크
    private bool isClear = false;       // 목표가 클리어 되었을때 체크

    void Update()
    {
        GenerateLight();
    }

    void GenerateLight()
    {
        List<Vector3> lightPath = new List<Vector3>();
        Vector3 direction = transform.forward;      // 생성기가 바라보는 방향
        Vector3 startPosition = transform.position;     // 빛이 나오기 시작하는 위치
        lightPath.Add(startPosition);

        float currentLightDistance = 0f;        // 빛의 현재 사거리
        bool stillHittingTarget = false;            // 빛이 목표를 컨택하고 있는 중인지 체크

        while (currentLightDistance < maxDistance)
        {
            RaycastHit hit;

            if (Physics.Raycast(startPosition, direction, out hit, maxDistance - currentLightDistance, reflectableLayer | blockLayer))
            {
                lightPath.Add(hit.point);
                currentLightDistance += Vector3.Distance(startPosition, hit.point);         // 빛의 거리 추가될때마다 누적

                if (hit.collider.CompareTag(targetTag))
                {
                    stillHittingTarget = true;      // 빛이 목표를 컨택하는 중임

                    if (!isContactingTarget)
                    {
                        // 목표에 처음 접촉한 경우, 접촉 시작 시간 기록
                        contactStartTime = Time.time;
                        isContactingTarget = true;
                        Debug.Log("목표에 접촉 시작!");
                    }

                    // 목표에 접촉한 시간이 2초 이상인 경우 기믹 성공 처리
                    if (Time.time - contactStartTime >= contactTime)
                    {
                        OnLightHitTarget();
                        break;
                    }
                }

                // 거울을 맞았으면 반사
                Mirror mirror = hit.collider.GetComponent<Mirror>();
                if (mirror != null)
                {
                    direction = mirror.Reflect(direction, hit.normal);
                    startPosition = hit.point;
                }
                else
                {
                    // 벽이나 장애물에 부딪히면 멈춤
                    break;
                }
            }
            else
            {
                // 아무것도 막지 않으면 최대 거리까지 진행
                lightPath.Add(startPosition + direction * (maxDistance - currentLightDistance));
                break;
            }
        }

        if (!stillHittingTarget && isContactingTarget && !isClear)
        {
            Debug.Log("초기화 진행");
            contactStartTime = 0f;
            isContactingTarget = false;
        }

        // 라인 렌더러 업데이트
        lineRenderer.positionCount = lightPath.Count;
        lineRenderer.SetPositions(lightPath.ToArray());
    }

    // 기믹 성공 시 처리
    void OnLightHitTarget()
    {
        if (!isClear)
        {
            isClear = true;
            Debug.Log("빛이 목표 지점에 도달했습니다! 기믹 성공!");
        }
    }
}
