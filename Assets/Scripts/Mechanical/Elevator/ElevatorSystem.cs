using DG.Tweening;
using UnityEngine;

namespace HPhysic
{
    public class ElevatorSystem : EnvironmentObject, IMechanicHolder, IInteractable
    {
        [SerializeField] private Vector3 targetPos;       // 목표 위치
        [SerializeField] private float duration;          // 이동 시간
        [SerializeField] private GameObject Elevator;       // 엘리베이터일 경우
        private Vector3 startPos;
        private bool isActive = false;

        private void Awake()
        {
            startPos = Elevator.transform.position;
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
            Debug.Log("오류!");
            MechanicSystem();

            return false;
        }

        public void MechanicSystem()
        {
            if (Elevator != null)
            {
                isActive = !isActive;

                if (isActive)
                {
                    Elevator.transform.DOKill();
                    Elevator.transform.DOMove(startPos + targetPos, duration);
                }
                else
                {
                    Elevator.transform.DOKill();
                    Elevator.transform.DOMove(startPos, duration);
                }
            }
        }
    }
}