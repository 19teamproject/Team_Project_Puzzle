using UnityEngine;
using HInteractions;

namespace HPhysic
{
    // Connector가 필수 (없으면 자동 추가됨)
    [RequireComponent(typeof(Connector))]
    // 케이블의 Start나 End 부분에 붙는 스크립트
    public class PhysicCableCon : Liftable
    {
        private Connector connector; // 자신의 Connector 참조

        protected override void Awake()
        {
            base.Awake(); // Liftable 초기화
            connector = gameObject.GetComponent<Connector>(); // Connector 컴포넌트 가져오기
        }

        // 들 때 (PickUp)
        public override void PickUp(IObjectHolder holder, int layer)
        {
            base.PickUp(holder, layer); // 기본 PickUp 처리 (Liftable)

            if (connector.ConnectedTo)
                connector.Disconnect(); // 들 때 이미 연결되어 있으면 해제
        }

        // 놓을 때 (Drop)
        public override void Drop()
        {
            if (Holder.SelectedObject && Holder.SelectedObject.TryGetComponent(out Connector secondConnector))
            {
                // 놓으려고 할 때, 다른 Connector를 바라보고 있다면
                if (connector.CanConnect(secondConnector))
                {
                    // 연결할 수 있으면 바로 연결
                    secondConnector.Connect(connector);
                }
                else if (!secondConnector.IsConnected)
                {
                    // 연결은 안되지만, 빈 상태라면 위치만 적절히 맞춰줌
                    transform.rotation = secondConnector.ConnectionRotation * connector.RotationOffset;
                    transform.position = (secondConnector.ConnectionPosition + secondConnector.ConnectedOutOffset * 0.2f)
                                         - (connector.ConnectionPosition - connector.transform.position);
                }
            }

            base.Drop(); // 기본 Drop 처리 (Liftable)
        }
    }
}
