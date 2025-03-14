using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using cakeslice;

public class Interaction : MonoBehaviour
{
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

    private void Update()
    {
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