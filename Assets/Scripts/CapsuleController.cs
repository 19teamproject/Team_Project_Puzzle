using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CapsuleController : MonoBehaviour
{
    public GameObject player;

    [Header("Ray")]
    public float rayDistance = 10f; // Ray가 닿는 거리
    public LayerMask layerMask; // Player가 확인 할 수 있는 Ray

    private TeleportEffect teleportEffect;
    private CubeInteraction cubeInteraction;
    private float scaleAmount = 25f; //한칸당 25f씩

    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            Ray ray = new Ray(transform.position, transform.forward); // Ray를 캐릭터의 포지션으로부터
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                Debug.Log($"Ray가 맞은 오브젝트 : {hit.collider.gameObject.name}");
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("XCube"))
                {
                    cubeInteraction.ChangeScale(hit.collider.gameObject, Vector3.right, scaleAmount);
                }
                else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("YCube"))
                {
                    cubeInteraction.ChangeScale(hit.collider.gameObject, Vector3.up, scaleAmount);
                }
            }
        }
    }
    public void OnTriggerEnter(Collider other) //닿았을 때 실행되어야하므로 Ray가 아닌 Trigger로
    {
        if(other.CompareTag("TeleportCube"))
        {
            TeleportEffect teleport = other.GetComponent<TeleportEffect>();

            if (teleport != null)
            {
                teleport.StartTeleport(player);
            }
        }

    }
}
