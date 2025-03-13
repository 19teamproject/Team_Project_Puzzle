using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceObject : MonoBehaviour
{
    private Renderer iceRenderer; // ���� ������ Renderer
    public Rigidbody rigid;
    private BoxCollider boxCollider;

    private bool isMelting = false;  // ������ ��� ������ ����

    public float minSize = 0.1f; // �ּ� ũ�� ����
    private Vector3 currentScale;
    private float elapsedTime = 0f; // ������ �ҿ� ���� �� ����� �ð�
    public float meltingDuration = 5f;  // ��� �� �ɸ��� �� �ð�

    public void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        currentScale = transform.localScale;
        iceRenderer = GetComponent<Renderer>();
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire") && !isMelting)
        {
            StartCoroutine(MeltIce());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            isMelting = false;
        }
    }

    private IEnumerator MeltIce()
    {
        rigid.isKinematic = true;  // ������Ʈ�� ���� �������� �ʰ� ����
        isMelting = true;

        while (elapsedTime < meltingDuration && isMelting)  // duration ���� ����
        {
            float progress = elapsedTime / meltingDuration;

            currentScale = Vector3.Lerp(currentScale, Vector3.one * minSize, Time.deltaTime / (meltingDuration - elapsedTime));
            transform.localScale = currentScale;

            boxCollider.size = Vector3.Max(boxCollider.size, new Vector3(0.3f, 0.3f, 0.3f));
            // boxCollider ũ�Ⱑ �ʹ� �پ��� Fire�� Trigger�� �� �Ͼ�ϴ�. ���� ũ�� ����.

            Color newColor = iceRenderer.material.color;
            newColor.a = Mathf.Lerp(1f, 0f, progress);
            iceRenderer.material.color = newColor;

            elapsedTime += Time.deltaTime;
            yield return null;

        }

        if (transform.localScale.x <= minSize)
        {
            Destroy(transform.parent.gameObject);
        }

    }

}


