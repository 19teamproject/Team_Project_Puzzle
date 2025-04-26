using cakeslice;
using UnityEngine;

public class ElectricBox : MonoBehaviour, IInteractable
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Outline outline;
    private Rigidbody rb;
    private bool isHolding;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!isHolding) return;

        Vector3 holdPos = CharacterManager.Instance.Player.transform.position + offset;
        transform.position = holdPos;
    }

    public string GetInteractPrompt()
    {
        return "";
    }

    public bool OnInteract()
    {
        isHolding = !isHolding;
        rb.useGravity = false;
        return false;
    }

    public void SetOutline(bool show)
    {
        if (outline != null)
        {
            outline.color = show && !isHolding ? 0 : 1;
        }
    }
}
