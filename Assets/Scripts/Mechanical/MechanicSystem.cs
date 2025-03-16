
using UnityEngine;

namespace HPhysic
{
    public class MechanicSystem : MonoBehaviour
    {
        [SerializeField] protected Vector3 targetPos;         // 목표 위치
        [SerializeField] protected float duration;            // 이동 시간
        protected Vector3 startPos;
        protected bool isActive = false;

        protected virtual void Awake()
        {
            startPos = transform.position;
        }
    }
}