using UnityEngine;
using cakeslice;
using StarterAssets;
using DG.Tweening;
using NaughtyAttributes;

public class Cube : MonoBehaviour, IInteractable
{
    [Header("Data")]
    [SerializeField] private CubeData data;
    [SerializeField] private Outline outline;
    [SerializeField] private AudioClip[] audioClips;
    [Range(0, 1)]
    [SerializeField] private float audioVolume;

    [Space(10f)]
    [Header("Teleport Cube Only")]
    [SerializeField] private GameObject target;
    [SerializeField] private Vector3 offset = Vector3.up;

    private GameObject player;
    private bool isTrigger;

    private void Start()
    {
        player = CharacterManager.Instance.Player.gameObject;
    }

    public string GetInteractPrompt()
    {
        string str = $"<font=\"GmarketSansMedium SDF\" material=\"GmarketSansMedium SDF Glow Blue\">{data.displayName}</font> - {data.description}";
        return str;
    }

    public virtual void SetOutline(bool show)
    {
        if (outline != null) outline.color = show ? 0 : 1;
    }


#region Interact

    public bool OnInteract()
    {
        switch (data.type)
        {
            case CubeType.Scale:
                Scale(); break;

            case CubeType.Teleport:
                Teleport(); break;

            case CubeType.Jump:
                if (isTrigger) Jump(); break;
        }

        return false;
    }

    // 크기 조절
    private void Scale()
    {
        if (audioClips.Length > 0)
        {
            var index = Random.Range(0, audioClips.Length);
            AudioSource.PlayClipAtPoint(audioClips[index], transform.position, audioVolume);
        }

        if (TryGetComponent(out CubeBoneScaler scaler))
        {
            scaler.ChangeScale(data.scaleDir);
        }
        else
        {
            Debug.LogWarning($"{gameObject}: CubeBoneScaler 스크립트가 없습니다.");
        }
    }

    // 텔레포트
    private void Teleport()
    {
        if (audioClips.Length > 0)
        {
            var index = Random.Range(0, audioClips.Length);
            AudioSource.PlayClipAtPoint(audioClips[index], transform.position, audioVolume);
        }
        
        if (target != null) //이동할 큐브가 존재한다면
        {
            CharacterController controller = player.GetComponent<CharacterController>();

            controller.enabled = false;
            player.transform.position = target.transform.position + offset; //큐브타겟 목표지점의 위에 텔레포트하여 충돌 방지
            controller.enabled = true;

            Debug.Log($"플레이어 이동 완료: {gameObject} => {target}");
        }
        else
        {
            Debug.LogWarning($"{gameObject}: 연결된 큐브가 없습니다.");
        }
    }

    // 점프
    private void Jump()
    {
        if (audioClips.Length > 0)
        {
            var index = Random.Range(0, audioClips.Length);
            AudioSource.PlayClipAtPoint(audioClips[index], transform.position, audioVolume);
        }
        
        if (player.TryGetComponent(out ThirdPersonController thirdPersonController))
        {
            DOVirtual.DelayedCall(0.1f, () =>
            {
                thirdPersonController.AddJumpForce(data.jumpForce);
            });
        }
    }

#endregion


#region Trigger

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTrigger = true;
        }
    }

    private void OTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTrigger = false;
        }
    }

#endregion
}
