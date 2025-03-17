using UnityEngine;
using cakeslice;
using StarterAssets;
using NaughtyAttributes;

public class Cube : MonoBehaviour, IInteractable
{
    [Header("Data")]
    [SerializeField] protected CubeData data;
    [SerializeField] protected Outline outline;

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
        // if (!isTrigger) return false;

        switch (data.type)
        {
            case CubeType.Scale:
                Scale(); break;

            case CubeType.Teleport:
                Teleport(); break;

            case CubeType.Jump:
                Jump(); break;
        }

        return false;
    }

    // 크기 조절
    public void Scale()
    {
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
        if (player.TryGetComponent(out ThirdPersonController thirdPersonController))
        {
            thirdPersonController.AddJumpForce(data.jumpForce);
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
