using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGenerator : MonoBehaviour
{
    [Header("Light")]
    [SerializeField] private LineRenderer lineRenderer;   // ���� �ð������� ǥ��

    [SerializeField] private LayerMask reflectableLayer;  // �ݻ� ������ ���̾� (�ſ�)
    [SerializeField] private LayerMask blockLayer;        // ���� ���� ���̾� (��, ��ֹ�)

    [SerializeField] private float maxDistance = 100f;    // ���� ������ �� �ִ� �ִ� �Ÿ� 
    [SerializeField] private float contactTime = 2f;  // ��ǥ�� �����ؾ� �ϴ� �ð� (��)
    [SerializeField] private float contactStartTime = 0f;  // ��ǥ�� ������ �ð� ���

    [SerializeField] private string targetTag = "TargetPoint";  // ��ǥ ������ �±�

    private bool isContactingTarget = false;    // ���� ��ǥ�� ó�� �����Ҷ� üũ
    private bool isClear = false;       // ��ǥ�� Ŭ���� �Ǿ����� üũ

    void Update()
    {
        GenerateLight();
    }

    void GenerateLight()
    {
        List<Vector3> lightPath = new List<Vector3>();
        Vector3 direction = transform.forward;      // �����Ⱑ �ٶ󺸴� ����
        Vector3 startPosition = transform.position;     // ���� ������ �����ϴ� ��ġ
        lightPath.Add(startPosition);

        float currentLightDistance = 0f;        // ���� ���� ��Ÿ�
        bool stillHittingTarget = false;            // ���� ��ǥ�� �����ϰ� �ִ� ������ üũ

        while (currentLightDistance < maxDistance)
        {
            RaycastHit hit;

            if (Physics.Raycast(startPosition, direction, out hit, maxDistance - currentLightDistance, reflectableLayer | blockLayer))
            {
                lightPath.Add(hit.point);
                currentLightDistance += Vector3.Distance(startPosition, hit.point);         // ���� �Ÿ� �߰��ɶ����� ����

                if (hit.collider.CompareTag(targetTag))
                {
                    stillHittingTarget = true;      // ���� ��ǥ�� �����ϴ� ����

                    if (!isContactingTarget)
                    {
                        // ��ǥ�� ó�� ������ ���, ���� ���� �ð� ���
                        contactStartTime = Time.time;
                        isContactingTarget = true;
                        Debug.Log("��ǥ�� ���� ����!");
                    }

                    // ��ǥ�� ������ �ð��� 2�� �̻��� ��� ��� ���� ó��
                    if (Time.time - contactStartTime >= contactTime)
                    {
                        OnLightHitTarget();
                        break;
                    }
                }

                // �ſ��� �¾����� �ݻ�
                Mirror mirror = hit.collider.GetComponent<Mirror>();
                if (mirror != null)
                {
                    direction = mirror.Reflect(direction, hit.normal);
                    startPosition = hit.point;
                }
                else
                {
                    // ���̳� ��ֹ��� �ε����� ����
                    break;
                }
            }
            else
            {
                // �ƹ��͵� ���� ������ �ִ� �Ÿ����� ����
                lightPath.Add(startPosition + direction * (maxDistance - currentLightDistance));
                break;
            }
        }

        if (!stillHittingTarget && isContactingTarget && !isClear)
        {
            Debug.Log("�ʱ�ȭ ����");
            contactStartTime = 0f;
            isContactingTarget = false;
        }

        // ���� ������ ������Ʈ
        lineRenderer.positionCount = lightPath.Count;
        lineRenderer.SetPositions(lightPath.ToArray());
    }

    // ��� ���� �� ó��
    void OnLightHitTarget()
    {
        if (!isClear)
        {
            isClear = true;
            Debug.Log("���� ��ǥ ������ �����߽��ϴ�! ��� ����!");
        }
    }
}
