using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class LightGenerator : MonoBehaviour
{
    [Header("Light")]
    [SerializeField] private GameObject lightBeamPrefab; // Cylinder ������
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
        // ���� �� ����
        foreach (GameObject beam in lightBeams)
        {
            Destroy(beam);
        }
        lightBeams.Clear();

        Vector3 direction = transform.forward;
        Vector3 startPosition = transform.position;

        float currentLightDistance = 0f;        // ���� ���� ��Ÿ�
        bool stillHittingTarget = false;        // ���� Ÿ���� �����ϰ� �ִ� ������ üũ

        Color beamColor = startLightColor;        // �� ���� ���� �������� ����

        while (currentLightDistance < maxDistance)  // �ִ��Ÿ������� �� ��� ����
        {
            RaycastHit hit;

            // ������ ���̾�� hit �Ǿ�����
            if (Physics.Raycast(startPosition, direction, out hit, maxDistance - currentLightDistance, reflectableLayer | blockLayer))
            {
                float segmentLength = Vector3.Distance(startPosition, hit.point);
                CreateLightBeam(startPosition, hit.point, beamColor);
                currentLightDistance += segmentLength;

                // ��ǥ���� �±� �Ǿ�����
                if (hit.collider.CompareTag(targetTag))
                {
                    stillHittingTarget = true;

                    // ó�� ���� �Ǿ�����
                    if (!isContactingTarget)
                    {
                        contactStartTime = Time.time;
                        isContactingTarget = true;
                        Debug.Log("��ǥ�� ���� ����!");
                    }
                    if (Time.time - contactStartTime >= contactTime)
                    {
                        OnLightHitTarget(hit, beamColor);
                        break;
                    }
                }

                // �ſ�� ���� ��
                Mirror mirror = hit.collider.GetComponent<Mirror>();
                if (mirror != null)
                {
                    // ù �ݻ翡�� �ͽ��� �ȵǰ� ��
                    if (beamColor == startLightColor)
                    {
                        // ù ��° �ݻ��� ���, �ſ� ������ ���� ����
                        beamColor = mirror.GetMirrorColor();
                    }
                    else
                    {
                        // ���� �ݻ翡�� 2���� ���� �ͽ�
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
            // �� �ܿ� ���
            else
            {
                // �ִ� ��Ÿ����� �� ��� ����
                Vector3 endPosition = startPosition + direction * (maxDistance - currentLightDistance);
                CreateLightBeam(startPosition, endPosition, beamColor);
                break;
            }
        }

        if (!stillHittingTarget && isContactingTarget && !isClear)
        {
            Debug.Log("�ʱ�ȭ ����");
            contactStartTime = 0f;
            isContactingTarget = false;
        }
    }

    // �� ��� ����
    void CreateLightBeam(Vector3 start, Vector3 end, Color beamColor)
    {
        GameObject beam = Instantiate(lightBeamPrefab);
        lightBeams.Add(beam);

        // ��ġ ����
        beam.transform.position = (start + end) / 2;

        // ���� ���� ����
        Quaternion rotation = Quaternion.LookRotation(end - start);
        beam.transform.rotation = rotation * Quaternion.Euler(90, 0, 0);

        // ũ�� ����
        beam.transform.localScale = new Vector3(
            lightBeamPrefab.transform.localScale.x,   // �������� X ũ�� ����
            (end - start).magnitude / 2,              // ���� ����
            lightBeamPrefab.transform.localScale.z    // �������� Z ũ�� ����
        );

        // �� ����� ���� ����
        Renderer beamRenderer = beam.GetComponent<Renderer>();
        if (beamRenderer != null)
        {
            beamRenderer.material.color = beamColor;
            beamRenderer.material.SetColor("_EmissionColor", beamColor);
        }

    }

    // ��ǥ ���� hit �Ǿ�����
    void OnLightHitTarget(RaycastHit hit, Color beamColor)
    {
        TargetPoint target = hit.collider.GetComponent<TargetPoint>();
        if (target != null)
        {
            target.OnLightHit(beamColor);  // ��ǥ�������� ���� ��ġ ���θ� ó��
        }
    }
}
