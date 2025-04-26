using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HInteractions
{
    // 모든 '잡을 수 있는 상호작용 오브젝트'의 기본
    public abstract class GrabbableInteractable : Interactable
    {
        public bool IsHeld { get; protected set; }
        public IObjectHolder Holder { get; protected set; }

        // 오브젝트를 잡는다
        public abstract void PickUp(IObjectHolder holder, int layer);

        // 오브젝트를 놓는다
        public abstract void Drop();
    }
}