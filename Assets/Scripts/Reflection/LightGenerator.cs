using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class LightGenerator : MonoBehaviour
{
    [Header("Light")]
    [SerializeField] private GameObject lightBeamPrefab; // Cylinder 프리팹
    [SerializeField] private LayerMask reflectableLayer;
    [SerializeField] private LayerMask blockLayer;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private float contactTime = 2f;
    [SerializeField] private string targetTag = "TargetPoint";
    [SerializeField] private Color startLightColor;

    private List<GameObject> lightBeams = new List<GameObject>();
    
    private float contactStartTime = 0f;
    private bool isContactingTarget = false;
    private bool isClear = false;

    void Update()
    {
        GenerateLight();
    }

    void GenerateLight()
    {
        // 기존 빛 제거
        foreach (GameObject beam in lightBeams)
        {
            Destroy(beam);
        }
        lightBeams.Clear();

        Vector3 direction = transform.forward;
        Vector3 startPosition = transform.position;

        float currentLightDistance = 0f;        // 현재 빛의 사거리
        bool stillHittingTarget = false;        // 빛이 타겟을 컨택하고 있는 중인지 체크

        Color beamColor = startLightColor;        // 빛 색상 시작 색상으로 지정

        while (currentLightDistance < maxDistance)  // 최대사거리까지만 빛 기둥 생성
        {
            RaycastHit hit;

            // 지정한 레이어와 hit 되었을때
            if (Physics.Raycast(startPosition, direction, out hit, maxDistance - currentLightDistance, reflectableLayer | blockLayer))
            {
                float segmentLength = Vector3.Distance(startPosition, hit.point);
                CreateLightBeam(startPosition, hit.point, beamColor);
                currentLightDistance += segmentLength;

                // 목표지점 태그 되었을때
                if (hit.collider.CompareTag(targetTag))
                {
                    stillHittingTarget = true;

                    // 처음 컨택 되었을때
                    if (!isContactingTarget)
                    {
                        contactStartTime = Time.time;
                        isContactingTarget = true;
                        Debug.Log("목표에 접촉 시작!");
                    }
                    if (Time.time - contactStartTime >= contactTime)
                    {
                        OnLightHitTarget(hit, beamColor);
                        break;
                    }
                }

                // 거울과 접촉 시
                Mirror mirror = hit.collider.GetComponent<Mirror>();
                if (mirror != null)
                {
                    // 첫 반사에는 믹스가 안되게 함
                    if (beamColor == startLightColor)
                    {
                        // 첫 번째 반사일 경우, 거울 색상을 직접 적용
                        beamColor = mirror.GetMirrorColor();
                    }
                    else
                    {
                        // 이후 반사에는 2가지 색상 믹스
                        beamColor = mirror.MixColor(beamColor);
                    }

                    direction = mirror.Reflect(direction, hit.normal);
                    startPosition = hit.point;
                }
                else
                {
                    break;
                }
            }
            // 그 외에 경우
            else
            {
                // 최대 사거리까지 빛 기둥 생성
                Vector3 endPosition = startPosition + direction * (maxDistance - currentLightDistance);
                CreateLightBeam(startPosition, endPosition, beamColor);
                break;
            }
        }

        if (!stillHittingTarget && isContactingTarget && !isClear)
        {
            Debug.Log("초기화 진행");
            contactStartTime = 0f;
            isContactingTarget = false;
        }
    }

    // 빛 기둥 생성
    void CreateLightBeam(Vector3 start, Vector3 end, Color beamColor)
    {
        GameObject beam = Instantiate(lightBeamPrefab);
        lightBeams.Add(beam);

        // 위치 설정
        beam.transform.position = (start + end) / 2;

        // 빛의 방향 설정
        Quaternion rotation = Quaternion.LookRotation(end - start);
        beam.transform.rotation = rotation * Quaternion.Euler(90, 0, 0);

        // 크기 조정
        beam.transform.localScale = new Vector3(
            lightBeamPrefab.transform.localScale.x,   // 프리팹의 X 크기 유지
            (end - start).magnitude / 2,              // 길이 조정
            lightBeamPrefab.transform.localScale.z    // 프리팹의 Z 크기 유지
        );

        // 빛 기둥의 색상 결정
        Renderer beamRenderer = beam.GetComponent<Renderer>();
        if (beamRenderer != null)
        {
            beamRenderer.material.color = beamColor;
            beamRenderer.material.SetColor("_EmissionColor", beamColor);
        }

    }

    // 목표 지점 hit 되었을때
    void OnLightHitTarget(RaycastHit hit, Color beamColor)
    {
        TargetPoint target = hit.collider.GetComponent<TargetPoint>();
        if (target != null)
        {
            target.OnLightHit(beamColor);  // 목표지점에서 색상 일치 여부를 처리
        }
    }
}
