using DG.Tweening;
using UnityEngine;

namespace HPhysic
{
    public enum MechanicType
    {
        Cable,
        Button,
        Pressure,
        CableBoth,
        CallButton
    }

    public class MechanicSystem : EnvironmentObject
    {
        [SerializeField] private PhysicCable redCable;
        [SerializeField] private PhysicCable blueCable;
        [SerializeField] private MechanicType type;
        [SerializeField] private Vector3 targetPos;         // 목표 위치
        [SerializeField] private float duration;            // 이동 시간
        [SerializeField] private GameObject movingObj;
        [SerializeField] private MechanicSystem callBtn;
        private Vector3 startPos;
        private bool isActive = false;

        [SerializeField] private AudioClip[] clips;

        private void Awake()
        {
            startPos = movingObj.transform.position;
        }

        private void Update()
        {
            if (type == MechanicType.Cable)
                CableSystem();
            else if (type == MechanicType.CableBoth)
                CableBothSystem();
        }

        public new string GetInteractPrompt()
        {
            string str = $"<font=\"GmarketSansMedium SDF\" material=\"GmarketSansMedium SDF Glow Blue\">" +
                $"{data.displayName}</font> - {data.description}";

            return str;
        }

        public override void SetOutline(bool show)
        {
            if (outline != null)
            {
                outline.color = show ? 0 : 1;
            }
        }

        public override bool OnInteract()
        {
            if (type == MechanicType.Button)
                ButtonSystem();
            else if (type == MechanicType.CallButton)
                callBtn.ButtonSystem();

            return false;
        }

        public void ButtonSystem()
        {
            if (movingObj != null)
            {
                isActive = !isActive;

                if (isActive)
                {
                    movingObj.transform.DOKill();
                    movingObj.transform.DOMove(startPos + targetPos, duration);
                    SoundManager.PlayClip(clips[0]);
                }
                else
                {
                    movingObj.transform.DOKill();
                    movingObj.transform.DOMove(startPos, duration);
                    SoundManager.PlayClip(clips[0]);
                }
            }
        }

        public void PressureSystem()
        {
            if (movingObj != null)
            {
                if (isActive)
                {
                    movingObj.transform.DOKill();
                    movingObj.transform.DOMove(startPos + targetPos, duration);
                    SoundManager.PlayClip(clips[0]);
                }
                else
                {
                    movingObj.transform.DOKill();
                    movingObj.transform.DOMove(startPos, duration);
                    SoundManager.PlayClip(clips[0]);
                }
            }
        }

        public void CableSystem()
        {
            if (redCable != null && blueCable != null)
            {
                if (redCable.StartConnector.IsConnectedRight && blueCable.StartConnector.IsConnectedRight && !isActive)
                {
                    isActive = true;
                    movingObj.transform.DOKill();
                    movingObj.transform.DOMove(startPos + targetPos, duration);
                    SoundManager.PlayClip(clips[0]);
                }
                else if ((!redCable.StartConnector.IsConnectedRight || !blueCable.StartConnector.IsConnectedRight) && isActive)
                {
                    isActive = false;
                    movingObj.transform.DOKill();
                    movingObj.transform.DOMove(startPos, duration);
                    SoundManager.PlayClip(clips[0]);
                }
            }
        }

        public void CableBothSystem()
        {
            if (redCable != null && blueCable != null)
            {
                if (redCable.IsAllConnectedRight && blueCable.IsAllConnectedRight && !isActive)
                {
                    isActive = true;
                    movingObj.transform.DOKill();
                    movingObj.transform.DOMove(startPos + targetPos, duration);
                    SoundManager.PlayClip(clips[0]);
                }
                else if ((!redCable.IsAllConnectedRight || !blueCable.IsAllConnectedRight) && isActive)
                {
                    isActive = false;
                    movingObj.transform.DOKill();
                    movingObj.transform.DOMove(startPos, duration);
                    SoundManager.PlayClip(clips[0]);
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            isActive = true;
            PressureSystem();
        }

        private void OnCollisionExit(Collision collision)
        {
            isActive = false;
            PressureSystem();
        }
    }
}