using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using HInteractions;
using NaughtyAttributes;
using System;
using HPhysic;

public class Interaction : MonoBehaviour, IObjectHolder
{
    // 바라보고 있는 오브젝트
    [Header("Select")]
    [SerializeField, Required] private Transform playerCamera;
    [SerializeField] private float selectRange = 10f;
    [SerializeField] private LayerMask selectLayer;
    [field: SerializeField, ReadOnly] public Interactable SelectedObject { get; private set; } = null;
    [field: SerializeField, ReadOnly] public Connector connector { get; private set; } = null;

    // 들고 있는 오브젝트
    [Header("Hold")]
    [SerializeField, Required] private Transform handTransform;
    [SerializeField] private Transform handTransform2;
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
    private bool firstActivated = false;
    private float rotationInputX = 0f;
    private float rotationInputY = 0f;
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
                int hitLayer = hit.collider.gameObject.layer;
                if (hitLayer == LayerMask.NameToLayer("Default"))
                {
                    if (curInteractGameObject == null && curInteractable == null) return;

                    curInteractable.SetOutline(false);
                    AnimatePromptText(false);

                    curInteractGameObject = null;
                    curInteractable = null;

                    return;
                }
                if (hit.collider.gameObject != curInteractGameObject)
                {
                    if (curInteractGameObject != null)
                    {
                        curInteractGameObject.GetComponent<IInteractable>().SetOutline(false);
                    }

                    curInteractGameObject = hit.collider.gameObject;
                    curInteractable = hit.collider.GetComponent<IInteractable>();
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

        if (selectedRotatableObject != null && selectedRotatableObject.isRotating)
            HandleRotation(rotationInputX, rotationInputY);
    }

    #region -input-

    private void UpdateInput()
    {
        // 마우스 좌클릭하면 상호작용 시작
        bool interacting = Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.E);
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
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, selectRange, selectLayer))
            foundInteractable = hit.collider.GetComponent<Interactable>();

        // 바라보는 오브젝트가 감지된 오브젝트와 같다면 돌아가기
        if (SelectedObject == foundInteractable)
            return;

        if (SelectedObject)
        {
            SelectedObject.Deselect();
            connector.SetOutline(false);
            OnDeselect?.Invoke();
        }

        SelectedObject = foundInteractable;
        if (SelectedObject)
            connector = SelectedObject.GetComponent<Connector>();


        if (foundInteractable && foundInteractable.enabled && foundInteractable != HeldObject)
        {
            foundInteractable.Select();
            connector.SetOutline(true);
            OnSelect?.Invoke();
        }
    }

    #endregion

    #region -held object-

    // 들고있는 오브젝트 위치 업데이트
    private void UpdateHeldObjectPosition()
    {
        Vector3 handRot;
        if (HeldObject.tag == "ElectricBox")
        {
            HeldObject.rb.velocity = (handTransform2.position - HeldObject.transform.position) * holdingForce;
            HeldObject.transform.position = handTransform2.position;
            handRot = handTransform2.rotation.eulerAngles;
        }
        else
        {
            HeldObject.rb.velocity = (handTransform.position - HeldObject.transform.position) * holdingForce;
            handRot = handTransform.rotation.eulerAngles;
        }

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
            Debug.LogWarning($"{nameof(Interaction)}: Attempted to pick up null object!");
            return;
        }

        connector.SetOutline(false);
        HeldObject = obj;
        obj.PickUp(this, heldObjectLayer);
    }
    // 오브젝트 놓기
    private void DropObject(Liftable obj)
    {
        if (obj == null)
        {
            Debug.LogWarning($"{nameof(Interaction)}: Attempted to drop null object!");
            return;
        }

        HeldObject = null;
        obj.Drop();
    }

    #endregion

    private void SetPromptText()
    {
        promptText.text = curInteractable.GetInteractPrompt();
        promptTextPlus.text = curInteractable.GetInteractPrompt();
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && curInteractable != null)
        {
            if (curCube != null) Debug.Log($"{curCube}");
            if (!curInteractable.OnInteract()) return;

            CheckRotatable();

            curInteractGameObject = null;
            curInteractable = null;
            AnimatePromptText(false);
        }
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (selectedRotatableObject.isRotating && context.phase == InputActionPhase.Performed)
        {
            // 방향키 입력만 기록
            Vector2 input = context.ReadValue<Vector2>();

            // 좌우 (X)
            if (input.x != 0f)
            {
                rotationInputX = input.x;
            }
            // 상하 (Y)
            if (input.y != 0f)
            {
                rotationInputY = input.y;
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            rotationInputX = 0f;  // 방향 초기화
            rotationInputY = 0f;  // 방향 초기화
        }
    }

    private void HandleRotation(float rotationInputX, float rotationInputY)
    {
        Vector3 rotationAxis = Vector3.zero;

        // 좌우 회전 처리
        if (rotationInputX != 0f)
        {
            rotationAxis = transform.up;  // Y축 기준 좌우 회전
            selectedRotatableObject.Rotate(rotationInputX * selectedRotatableObject.rotationSpeed * Time.deltaTime, rotationAxis);
        }

        // 상하 회전 처리
        if (rotationInputY != 0f)
        {
            rotationAxis = selectedRotatableObject.transform.right; // X축 기준 상하 회전
            selectedRotatableObject.Rotate(rotationInputY * selectedRotatableObject.rotationSpeed * Time.deltaTime, rotationAxis);
        }

        if (rotationInputX != 0f || rotationInputY != 0f)
        {
            selectedRotatableObject.lastInputTime = Time.time;
        }

        selectedRotatableObject.RotateTimeOutCheck();

        Vector3 eulerAngles = selectedRotatableObject.transform.rotation.eulerAngles;
        eulerAngles.z = 0f;  // Z축 회전 방지
        selectedRotatableObject.transform.rotation = Quaternion.Euler(eulerAngles);
    }

    public void CheckRotatable()
    {
        RotateObject newSelectedObject = curInteractGameObject.GetComponent<RotateObject>();

        if (selectedRotatableObject != null && selectedRotatableObject != newSelectedObject)
        {
            selectedRotatableObject.isRotating = false;
        }

        if(newSelectedObject != null)
        {
            // 이전과 같은 오브젝트를 선택했을 때만 토글
            if (selectedRotatableObject == newSelectedObject)
            {
                selectedRotatableObject.isRotating = !selectedRotatableObject.isRotating;
            }
            else
            {
                // 다른 오브젝트를 선택하면 기본적으로 회전 활성화
                selectedRotatableObject = newSelectedObject;
                selectedRotatableObject.isRotating = true;
            }
        }


        if (selectedRotatableObject != null)
        {
            selectedRotatableObject.lastInputTime = Time.time;
            
            if (!firstActivated && !selectedRotatableObject.IsClear())
            {
                firstActivated = true;
            }

            if (selectedRotatableObject.IsClear())
            {
                firstActivated = false;
            }

            if (!selectedRotatableObject.isRotating)
            {
                selectedRotatableObject.isRotating = true;
            }

            Debug.Log($"회전 가능 상태: {selectedRotatableObject.isRotating} ({curInteractGameObject.name})");
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