using DG.Tweening;
using UnityEngine;

namespace HPhysic
{
    public class DoorSystem : MechanicSystem
    {
        public MechanicSystem mechanicSystem { get; }

        [SerializeField] private PhysicCable redCable;      // 케이블일 경우
        [SerializeField] private PhysicCable blueCable;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Update()
        {
            CableSystem();
        }

        private void CableSystem()
        {
            if (redCable != null && blueCable != null)
            {
                if (redCable.IsAllConnectedRight && blueCable.IsAllConnectedRight && !isActive)
                {
                    isActive = true;
                    transform.DOKill();
                    transform.DOMove(startPos + targetPos, duration);
                }
                else if (!redCable.IsAllConnectedRight || !blueCable.IsAllConnectedRight)
                {
                    isActive = false;
                    transform.DOKill();
                    transform.DOMove(startPos, duration);
                }
            }
        }
    }
}