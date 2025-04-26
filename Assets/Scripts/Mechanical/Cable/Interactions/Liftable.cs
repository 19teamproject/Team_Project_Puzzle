using System.Collections.Generic;
using UnityEngine;

namespace HInteractions
{
    // Rigidbody가 반드시 필요함 (없으면 자동 추가)
    [RequireComponent(typeof(Rigidbody))]
    public class Liftable : GrabbableInteractable
    {
        // 들 때 방향 보정 값 (예: 살짝 앞쪽으로 들어올리기)
        [field: SerializeField] public Vector3 LiftDirectionOffset { get; private set; } = Vector3.zero;

        public Rigidbody rb { get; protected set; } // 자신의 Rigidbody

        // (오브젝트, 원래 레이어) 쌍을 저장하는 리스트
        private readonly List<(GameObject obj, int defaultLayer)> defaultLayers = new();

        // 초기화
        protected override void Awake()
        {
            base.Awake(); // Interactable 초기화도 호출
            rb = GetComponent<Rigidbody>(); // Rigidbody 가져오기
        }

        // 오브젝트를 집어든다
        public override void PickUp(IObjectHolder holder, int layer)
        {
            if (IsHeld) return; // 이미 들고 있으면 무시

            Holder = holder; // 나를 들고 있는 주체 설정

            defaultLayers.Clear(); // 레이어 백업 초기화
            foreach (Collider col in gameObject.GetComponentsInChildren<Collider>())
                defaultLayers.Add((col.gameObject, col.gameObject.layer)); // 현재 레이어 저장

            // 들었을 때 세팅
            rb.useGravity = false;                        // 중력 비활성화
            rb.interpolation = RigidbodyInterpolation.Interpolate; // 물리 움직임 부드럽게

            foreach (var item in defaultLayers)
                item.obj.layer = layer;                   // 모두 지정 레이어로 변경 (예: PlayerHand 레이어)

            IsHeld = true; // 들림 상태로 전환
        }

        // 오브젝트를 내려놓는다
        public override void Drop()
        {
            if (!IsHeld) return; // 들고 있지 않으면 무시

            Holder = null; // 들고 있는 주체 정보 제거

            // 내려놓을 때 세팅
            rb.useGravity = true;                         // 중력 다시 활성화
            rb.interpolation = RigidbodyInterpolation.None; // 기본 설정으로 복귀

            foreach (var item in defaultLayers)
                item.obj.layer = item.defaultLayer;        // 원래 레이어로 복원

            IsHeld = false; // 들림 상태 해제
        }
    }
}