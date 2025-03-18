using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ObstacleCube : MonoBehaviour
{
    [Header("장애물 정보")]
    [SerializeField] private float disappearTime = 3f; // 사라지기까지의 걸리는 시간 변수
    [SerializeField] private float respawnTime = 3f; // 사라졌다가 Respawn되는 변수
    public TextMeshProUGUI warningText; //경고 메세지
    [SerializeField] private PostProcessVolume DamageIndicator;

    [Header("오디오 정보")]
    [SerializeField] private AudioClip clip;

    private bool isPlayOnCube = false; // 플레이어가 해당 큐브에 닿았을 때의 bool 변수
    private BoxCollider boxCollider;
    private CapsuleCollider capsuleCollider;
    private Coroutine disappearCoroutine;
    private MeshRenderer meshRenderer;
    private float targetWeight;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (boxCollider == null && capsuleCollider == null)
            capsuleCollider.isTrigger = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            SoundManager.Instance.PlayClip(clip);
            warningText.gameObject.SetActive(true);
            if(DamageIndicator != null )
            {
                DOTween.To(() => DamageIndicator.weight, x => DamageIndicator.weight = x, 1f, 1f); 
            }
            isPlayOnCube = true;

            if(disappearCoroutine == null)
            {
                disappearCoroutine = StartCoroutine(DisapperCountDown()); // 큐브 사라지는 코루틴 실행
            }
        }
    }
    private IEnumerator DisapperCountDown()
    {
        yield return new WaitForSeconds(disappearTime); //사라질때까지 걸리는 변수로 대기

        if(isPlayOnCube)
        {
            capsuleCollider.enabled = false;
            boxCollider.enabled = false;
            meshRenderer.enabled = false;
            warningText.gameObject.SetActive(false);
            DOTween.To(() => DamageIndicator.weight, x => DamageIndicator.weight = x, 0f, 1f);

            yield return new WaitForSeconds(respawnTime); // 일정 시간 후 다시 등장하도록 설정

            boxCollider.enabled = true;
            capsuleCollider.enabled = true;
            meshRenderer.enabled = true;
        }

        disappearCoroutine = null;  // 코루틴이 끝나면 초기화 
    }
}
