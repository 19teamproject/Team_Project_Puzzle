using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using HInteractions;
using NaughtyAttributes;
using System;
using HPlayer;
using StarterAssets;

public class Interaction : MonoBehaviour, IObjectHolder
{
    // 바라보고 있는 오브젝트
    [Header("Select")]
    [SerializeField, Required] private Transform playerCamera;
    [SerializeField] private float selectRange = 10f;
    [SerializeField] private LayerMask selectLayer;
    [field: SerializeField, ReadOnly] public Interactable SelectedObject { get; private set; } = null;

    // 들고 있는 오브젝트
    [Header("Hold")]
    [SerializeField, Required] private Transform handTransform;
    [SerializeField, Min(1)] private float holdingForce = 0.5f;
    [SerializeField] private int heldObjectLayer;
    [SerializeField][Range(0f, 90f)] private float heldClamXRotation = 45f;
    [field: SerializeField, ReadOnly] public Liftable HeldObject { get; private set; } = null;

    // 상호작용 중인지
    [field: Header("Input")]
    [field: SerializeField, ReadOnly] public bool Interacting { get; private set; } = false;

    public event Action OnSelect;           // 커서 활성화
    public event Action OnDeselect;         // 커서 비활성화

    public event Action OnInteractionStart; // 상호작용
    public event Action OnInteractionEnd;

    [Header("Raycast")]
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float maxCheckDistance;
    [SerializeField] private float checkRate = 0.05f;
    private float lastCheckTime;

    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private TextMeshProUGUI promptTextPlus;

    private GameObject curInteractGameObject;
    private IInteractable curInteractable;
    private RectTransform promptTextRect;
    private CanvasGroup promptTextCanvas;
    private Camera cam;
    private Cube curCube;
    private RotateObject selectedRotatableObject;
    private bool isRotating = false;
    public float CheckDistanceBonus { get; set; }

    private void Start()
    {
        cam = Camera.main;

        promptTextRect = promptText.GetComponent<RectTransform>();
        promptTextCanvas = promptText.GetComponent<CanvasGroup>();

        promptTextRect.anchoredPosition = new(0f, 25f);
        promptTextCanvas.alpha = 0f;
    }

    private void OnEnable()
    {
        OnInteractionStart += ChangeHeldObject;

        //PlayerController.OnPlayerEnterPortal += CheckHeldObjectOnTeleport;
    }
    private void OnDisable()
    {
        OnInteractionStart -= ChangeHeldObject;

        //PlayerController.OnPlayerEnterPortal -= CheckHeldObjectOnTeleport;
    }

    private void Update()
    {
        // 입력
        UpdateInput();

        // 바라보는 오브젝트 업데이트
        UpdateSelectedObject();

        if (HeldObject)
            UpdateHeldObjectPosition();

        if (Time.time - lastCheckTime > checkRate)
        {
            lastCheckTime = Time.time;

            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

            if (Physics.Raycast(ray, out RaycastHit hit, maxCheckDistance + CheckDistanceBonus, layerMask))
            {
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    if (curInteractGameObject != null)
                    {
                        curInteractGameObject.GetComponent<IInteractable>().SetOutline(false);
                    }

                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
                    Debug.Log($"[확인] curInteractable 타입: {curInteractable.GetType().Name}");
                    curInteractable.SetOutline(true);
                    AnimatePromptText(true);
                    SetPromptText();
                }
            }
            else
            {
                if (curInteractGameObject == null && curInteractable == null) return;

                curInteractable.SetOutline(false);
                AnimatePromptText(false);

                curInteractGameObject = null;
                curInteractable = null;
            }
        }
        RotateInput();
    }

    #region -input-

    private void UpdateInput()
    {
        // 마우스 좌클릭하면 상호작용 시작
        bool interacting = Input.GetMouseButton(0);
        if (interacting != Interacting)
        {
            Interacting = interacting;
            if (interacting)
                OnInteractionStart?.Invoke();
            else
                OnInteractionEnd?.Invoke();
        }
    }

    #endregion

    #region -selected object-

    // 바라보는 오브젝트 업데이트
    private void UpdateSelectedObject()
    {
        Interactable foundInteractable = null;
        // 원형 레이로 주변 오브젝트 감지
        if (Physics.SphereCast(playerCamera.position, 0.2f, playerCamera.forward, out RaycastHit hit, selectRange, selectLayer))
            foundInteractable = hit.collider.GetComponent<Interactable>();

        // 바라보는 오브젝트가 감지된 오브젝트와 같다면 돌아가기
        if (SelectedObject == foundInteractable)
            return;

        if (SelectedObject)
        {
            SelectedObject.Deselect();
            OnDeselect?.Invoke();
        }

        SelectedObject = foundInteractable;

        if (foundInteractable && foundInteractable.enabled)
        {
            foundInteractable.Select();
            OnSelect?.Invoke();
        }
    }

    #endregion

    #region -held object-

    // 들고있는 오브젝트 위치 업데이트
    private void UpdateHeldObjectPosition()
    {
        HeldObject.rb.velocity = (handTransform.position - HeldObject.transform.position) * holdingForce;

        Vector3 handRot = handTransform.rotation.eulerAngles;
        if (handRot.x > 180f)
            handRot.x -= 360f;
        handRot.x = Mathf.Clamp(handRot.x, -heldClamXRotation, heldClamXRotation);
        HeldObject.transform.rotation = Quaternion.Euler(handRot + HeldObject.LiftDirectionOffset);
    }
    // 들고있는 오브젝트 바꾸기
    private void ChangeHeldObject()
    {
        if (HeldObject)
            DropObject(HeldObject);
        else if (SelectedObject is Liftable liftable)
            PickUpObject(liftable);
    }
    // 오브젝트 들기
    private void PickUpObject(Liftable obj)
    {
        if (obj == null)
        {
            Debug.LogWarning($"{nameof(PlayerInteractions)}: Attempted to pick up null object!");
            return;
        }

        HeldObject = obj;
        obj.PickUp(this, heldObjectLayer);
    }
    // 오브젝트 놓기
    private void DropObject(Liftable obj)
    {
        if (obj == null)
        {
            Debug.LogWarning($"{nameof(PlayerInteractions)}: Attempted to drop null object!");
            return;
        }

        HeldObject = null;
        obj.Drop();
    }

    // 들고있는 오브젝트가 있다면 놓기
    private void CheckHeldObjectOnTeleport()
    {
        if (HeldObject != null)
            DropObject(HeldObject);
    }

    #endregion

    private void SetPromptText()
    {
        promptText.text = curInteractable.GetInteractPrompt();
        promptTextPlus.text = curInteractable.GetInteractPrompt();
    }

    public void OnInteract(InputValue value)
    {
        if (value.isPressed && curInteractable != null)
        {
            if (curCube != null) Debug.Log($"{curCube}");
            if (!curInteractable.OnInteract()) return;

            CheckRotatable();

            curInteractGameObject = null;
            curInteractable = null;
            AnimatePromptText(false);
        }
    }

    public void CheckRotatable()
    {
        if (selectedRotatableObject != null && selectedRotatableObject != curInteractGameObject.GetComponent<RotateObject>())
        {
            isRotating = false;  // 기존 선택 해제
        }

        selectedRotatableObject = curInteractGameObject.GetComponent<RotateObject>();
        

        if (selectedRotatableObject != null)
        {
            isRotating = !isRotating; // 토글 방식으로 설정 (켜기/끄기)
            Debug.Log($"회전 가능 상태: {isRotating} ({curInteractGameObject.name})");
        }
    }

    public void RotateInput()
    {
        if (isRotating && selectedRotatableObject != null)
        {
            float rotationInput = 0f;

            if (Mouse.current.leftButton.isPressed)
            {
                rotationInput = -1f; // 왼쪽 회전
            }
            else if (Mouse.current.rightButton.isPressed)
            {
                rotationInput = 1f; // 오른쪽 회전
            }

            if (rotationInput != 0f) // 입력이 있을 때만 회전
            {
                selectedRotatableObject.Rotate(rotationInput);
            }
        }
    }

    private void AnimatePromptText(bool show)
    {
        float alpha = show ? 1f : 0f;
        float yPos = show ? 50f : 25f;

        promptTextCanvas.DOFade(alpha, 0.5f)
            // .From(1f - alpha)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);

        promptTextRect.DOAnchorPos(new Vector2(0f, yPos), 0.5f)
            // .From(new Vector2(0f, 75f - yPos))
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);
    }
}