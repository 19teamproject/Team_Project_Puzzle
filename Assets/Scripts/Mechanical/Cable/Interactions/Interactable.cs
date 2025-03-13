using UnityEngine;
using NaughtyAttributes;

namespace HInteractions
{
    // 해당 클래스가 게임 오브젝트에 한 번만 추가되도록
    [DisallowMultipleComponent]
    public class Interactable : MonoBehaviour
    {
        // 에임 표시할지
        [field: SerializeField] public bool ShowPointerOnInterract { get; private set; } = true;

        // 바라보고 있는지
        [field: SerializeField, ReadOnly] public bool IsSelected { get; private set; }

        protected virtual void Awake()
        {
            Deselect();
        }

        public virtual void Select()
        {
            IsSelected = true;
        }

        public virtual void Deselect()
        {
            IsSelected = false;
        }
    }
}