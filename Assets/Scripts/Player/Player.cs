using System;
using UnityEngine;
using StarterAssets;

public class Player : MonoBehaviour
{
    public Action AddItem;

    [SerializeField] private StarterAssetsInputs controller;
    [SerializeField] private PlayerCondition condition;
    [SerializeField] private Interaction interaction;
    [SerializeField] private Transform dropPosition;
    [SerializeField] private Equipment equipment;

    public StarterAssetsInputs Controller => controller;
    public PlayerCondition Condition => condition;
    public Interaction Interaction => interaction;
    public Transform DropPosition => dropPosition;
    public Equipment Equipment => equipment;
    public ItemData ItemData { get; set; }

    private void Awake()
    {
        CharacterManager.Instance.Player = this;
    }
}