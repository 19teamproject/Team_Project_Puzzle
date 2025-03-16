using DG.Tweening;
using UnityEngine;

namespace HPhysic
{
    public class DoorSystem : MonoBehaviour, IMechanicHolder
    {
        [SerializeField] private PhysicCable redCable;      // 케이블일 경우
        [SerializeField] private PhysicCable blueCable;
        [SerializeField] protected Vector3 targetPos;       // 목표 위치
        [SerializeField] protected float duration;          // 이동 시간
        protected Vector3 startPos;
        protected bool isActive = false;

        protected virtual void Awake()
        {
            startPos = transform.position;
        }

        private void FixedUpdate()
        {
            MechanicSystem();
        }

        public void MechanicSystem()
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