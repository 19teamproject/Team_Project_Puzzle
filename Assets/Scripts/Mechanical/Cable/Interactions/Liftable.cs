using System.Collections.Generic;
using UnityEngine;

namespace HInteractions
{
    // Rigidbody 없으면 자동으로 추가
    [RequireComponent(typeof(Rigidbody))]
    public class Liftable : Interactable
    {
        // 들고 있는 상태인지
        [field: SerializeField] public bool IsLift { get; private set; } = false;
        [field: SerializeField] public Vector3 LiftDirectionOffset { get; private set; } = Vector3.zero;

        public Rigidbody rb { get; protected set; }
        public IObjectHolder ObjectHolder { get; protected set; }

        // 오브젝트와 레이어를 한쌍으로 담음
        private readonly List<(GameObject obj, int defaultLayer)> _defaultLayers = new();

        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody>();
        }

        // 잡아들기
        public virtual void PickUp(IObjectHolder holder, int layer)
        {
            // 무언가 들고있다면 돌아가기
            if (IsLift)
                return;

            // 바라보고 있는 오브젝트 정보
            ObjectHolder = holder;

            // 레이어 저장
            _defaultLayers.Clear();
            foreach (Collider col in gameObject.GetComponentsInChildren<Collider>())
                _defaultLayers.Add((col.gameObject, col.gameObject.layer));

            // 세팅
            rb.useGravity = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate; // 물체 움직임을 부드럽게
            foreach ((GameObject obj, int defaultLayer) item in _defaultLayers)
                item.obj.layer = layer; // 해당 오브젝트의 레이어를 설정

            IsLift = true;
        }

        // 놓기
        public virtual void Drop()
        {
            // 아무것도 들고있지 않다면 돌아가기
            if (!IsLift)
                return;

            // 바라보고 있는 오브젝트 정보
            ObjectHolder = null;

            // 세팅
            rb.useGravity = true;
            rb.interpolation = RigidbodyInterpolation.None;
            foreach ((GameObject obj, int defaultLayer) item in _defaultLayers)
                item.obj.layer = item.defaultLayer;

            IsLift = false;
        }
    }
}