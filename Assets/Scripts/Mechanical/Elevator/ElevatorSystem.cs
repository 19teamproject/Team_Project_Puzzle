using cakeslice;
using DG.Tweening;
using HInteractions;
using Unity.VisualScripting;
using UnityEngine;

namespace HPhysic
{
    public class ElevatorSystem : MechanicSystem
    {
        [SerializeField] private GameObject Elevator;       // 엘리베이터일 경우

        protected override void Awake()
        {
            startPos = Elevator.transform.position;
        }

        public void ButtonSystem()
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