using cakeslice; // 외부 라이브러리: 아웃라인 효과용
using UnityEngine;

namespace HInteractions
{
    // 전기박스(ElectricBox) — 들고 이동할 수 있는 상호작용 오브젝트
    public class ElectricBox : GrabbableInteractable
    {
        [SerializeField] private Vector3 offset;   // 플레이어 위치로부터 들 때의 오프셋
        [SerializeField] private Outline outline;  // 아웃라인 컴포넌트
        private Rigidbody rb;                      // Rigidbody 컴포넌트

        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody>(); // Rigidbody를 미리 캐싱
        }

        private void Update()
        {
            if (!IsHeld) return; // 들고 있지 않으면 아무것도 하지 않음

            // 들고 있을 때 플레이어 앞쪽에 위치를 고정시킴
            Vector3 holdPos = CharacterManager.Instance.Player.transform.position + offset;
            transform.position = holdPos;
        }

        public override void PickUp(IObjectHolder holder, int layer)
        {
            if (IsHeld) return;

            Holder = holder;
            IsHeld = true;
            rb.useGravity = false;
        }

        public override void Drop()
        {
            if (!IsHeld) return;

            Holder = null;
            IsHeld = false;
            rb.useGravity = true;
        }

        // 인터랙트할 때 보여줄 문구 (현재는 비워둠)
        public override string GetInteractPrompt() => "";

        // 아웃라인 표시 제어
        public override void SetOutline(bool show)
        {
            if (outline != null)
                // 들고 있지 않을 때만 아웃라인 활성화
                outline.color = show && !IsHeld ? 0 : 1;
        }
    }
}