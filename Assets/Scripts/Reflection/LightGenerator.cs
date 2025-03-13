using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGenerator : MonoBehaviour
{
    [Header("Light")]
    [SerializeField] private GameObject lightBeamPrefab; // Cylinder 프리팹
    [SerializeField] private LayerMask reflectableLayer;
    [SerializeField] private LayerMask blockLayer;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private float contactTime = 2f;
    [SerializeField] private string targetTag = "TargetPoint";

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
        float currentLightDistance = 0f;
        bool stillHittingTarget = false;

        while (currentLightDistance < maxDistance)
        {
            RaycastHit hit;
            if (Physics.Raycast(startPosition, direction, out hit, maxDistance - currentLightDistance, reflectableLayer | blockLayer))
            {
                float segmentLength = Vector3.Distance(startPosition, hit.point);
                CreateLightBeam(startPosition, hit.point);
                currentLightDistance += segmentLength;

                if (hit.collider.CompareTag(targetTag))
                {
                    stillHittingTarget = true;
                    if (!isContactingTarget)
                    {
                        contactStartTime = Time.time;
                        isContactingTarget = true;
                        Debug.Log("목표에 접촉 시작!");
                    }
                    if (Time.time - contactStartTime >= contactTime)
                    {
                        OnLightHitTarget();
                        break;
                    }
                }

                Mirror mirror = hit.collider.GetComponent<Mirror>();
                if (mirror != null)
                {
                    direction = mirror.Reflect(direction, hit.normal);
                    startPosition = hit.point;
                }
                else
                {
                    break;
                }
            }
            else
            {
                Vector3 endPosition = startPosition + direction * (maxDistance - currentLightDistance);
                CreateLightBeam(startPosition, endPosition);
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

    void CreateLightBeam(Vector3 start, Vector3 end)
    {
        GameObject beam = Instantiate(lightBeamPrefab);
        lightBeams.Add(beam);

        // 위치 설정
        beam.transform.position = (start + end) / 2;

        // 빛의 방향 설정
        Quaternion rotation = Quaternion.LookRotation(end - start);
        beam.transform.rotation = rotation;

        // 크기 조정 (Z축을 길이 방향으로 변경)
        beam.transform.localScale = new Vector3(0.05f, 0.05f, (end - start).magnitude);
    }

    void OnLightHitTarget()
    {
        if (!isClear)
        {
            isClear = true;
            Debug.Log("빛이 목표 지점에 도달했습니다! 기믹 성공!");
        }
    }
}
