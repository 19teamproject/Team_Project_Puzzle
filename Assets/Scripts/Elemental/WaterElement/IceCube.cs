using System.Collections;
using UnityEngine;

public class IceCube : MonoBehaviour
{
    private Renderer iceRenderer; // 투명도 조절할 Renderer
    public Rigidbody rigid;
    private BoxCollider boxCollider;

    private bool isMelting = false;  // 얼음이 녹는 중인지 여부

    public float minSize = 0.2f; // 최소 크기 제한
    private Vector3 currentScale;
    private float elapsedTime = 0f; // 얼음이 불에 닿은 후 경과된 시간
    public float meltingDuration = 5f;  // 녹는 데 걸리는 총 시간

    public GameObject waterPuddlePrefab;  // 물 웅덩이 프리팹

    public AudioSource audioSource;
    public AudioClip meltingSound; // 녹는 소리 추가

    public void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        currentScale = transform.localScale;
        iceRenderer = GetComponent<Renderer>();
        boxCollider = GetComponent<BoxCollider>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource가 없습니다! 컴포넌트를 추가하세요.");
            audioSource = gameObject.AddComponent<AudioSource>(); // 자동으로 추가
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire") && !isMelting)
        {
            StartCoroutine(MeltIce());
        }
    }

    private IEnumerator MeltIce()
    {
        
        if (rigid != null) rigid.isKinematic = true;  // 오브젝트가 마구 움직이지 않게 고정

        isMelting = true;

        if (audioSource != null && meltingSound != null)
        {
            Debug.Log("소리 재생 시작");
            audioSource.clip = meltingSound;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            Debug.LogError("AudioSource 또는 meltingSound가 없습니다!");
        }


        while (elapsedTime < meltingDuration && isMelting)  // duration 동안 실행
        {
            float progress = elapsedTime / meltingDuration;

            currentScale = Vector3.Lerp(currentScale, Vector3.one * minSize, Time.deltaTime / (meltingDuration - elapsedTime));
            transform.localScale = currentScale;

            boxCollider.size = Vector3.Max(boxCollider.size, new Vector3(0.3f, 0.3f, 0.3f));
            // boxCollider 크기가 너무 줄어들면 Fire와 Trigger가 안 일어납니다. 따라서 크기 고정.

            Color newColor = iceRenderer.material.color;
            newColor.a = Mathf.Lerp(1f, 0f, progress);
            iceRenderer.material.color = newColor;

            elapsedTime += Time.deltaTime;
            yield return null;

        }

        if (transform.localScale.x <= minSize)  // 얼음이 다 녹으면
        {
            WaterManager.instance.Melt();

            if (audioSource != null)
            {
                audioSource.Stop();
            }

            Destroy(gameObject, 1f);
        }

    }

}