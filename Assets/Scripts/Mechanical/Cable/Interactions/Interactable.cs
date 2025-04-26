using UnityEngine;
using NaughtyAttributes; // 인스펙터 기능 강화용 Attribute

namespace HInteractions
{
    // 같은 오브젝트에 중복해서 붙일 수 없게 막는다
    [DisallowMultipleComponent]
    public class Interactable : MonoBehaviour
    {
        // 상호작용 시 포인터(예: UI 표시)를 보여줄지 여부
        [field: SerializeField] public bool ShowPointerOnInterract { get; private set; } = true;

        // 현재 선택(Interaction Focus)된 상태인지
        [field: SerializeField, ReadOnly] public bool IsSelected { get; private set; }

        // 처음 생성될 때 선택 해제된 상태로 초기화
        protected virtual void Awake()
        {
            Deselect();
        }

        // 선택될 때 호출 (예: 마우스 오버, 상호작용 가능 표시)
        public virtual void Select()
        {
            IsSelected = true;
        }

        // 선택 해제될 때 호출 (예: 마우스 벗어남, 다른 상호작용 대체 등)
        public virtual void Deselect()
        {
            IsSelected = false;
        }

        public virtual string GetInteractPrompt()
        {
            return string.Empty;
        }

        public virtual void SetOutline(bool show) { }
    }
}