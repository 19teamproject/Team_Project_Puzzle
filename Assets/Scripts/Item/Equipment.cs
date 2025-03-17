using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class Equipment : MonoBehaviour
{
    [SerializeField] private Transform equipParent;

    private StarterAssetsInputs controller;
    private PlayerCondition condition;

    public Equip CurEquip { get; set; }

    [SerializeField]
    private EquipItemToHand equipItemToHand; // 손에 들린 Item의 세부 위치를 조정해주는 스크립트

    void Start()
    {
        controller = CharacterManager.Instance.Player.Controller;
        condition = CharacterManager.Instance.Player.Condition;
        equipItemToHand = GetComponent<EquipItemToHand>();
    }

    public void EquipNew(ItemData data)
    {
        UnEquip();
        CurEquip = Instantiate(data.equipPrefab, equipParent).GetComponent<Equip>();
        if (data.displayName == "횃불")
        {
            equipItemToHand.EquipTorchToHand();
        }
    }

    public void UnEquip()
    {
        if (CurEquip != null)
        {
            Destroy(CurEquip.gameObject);
            CurEquip = null;
        }
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && CurEquip != null && controller.cursorInputForLook)
        {
            CurEquip.OnAttackInput();
        }
    }
}