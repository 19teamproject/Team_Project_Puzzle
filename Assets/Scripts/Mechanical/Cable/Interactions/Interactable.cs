using UnityEngine;
using NaughtyAttributes;

namespace HInteractions
{
    [DisallowMultipleComponent]
    public class Interactable : MonoBehaviour
    {
        // 상호작용 가능한 오브젝트를 보고있으면 에임 표시
        [field: SerializeField] public bool ShowPointerOnInterract { get; private set; } = true;

        // 
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