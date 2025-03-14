using System.Collections;
using UnityEngine;
using HInteractions;

namespace HPhysic
{
    // Connector 없으면 자동으로 추가
    [RequireComponent(typeof(Connector))]
    // Start나 End에 들어갈 스크립트
    public class PhysicCableCon : Liftable
    {
        private Connector _connector;

        protected override void Awake()
        {
            base.Awake();

            _connector = gameObject.GetComponent<Connector>();
        }

        // 잡아들기
        public override void PickUp(IObjectHolder holder, int layer)
        {
            base.PickUp(holder, layer);

            if (_connector.ConnectedTo)
                _connector.Disconnect();
        }

        // 놓기
        public override void Drop()
        {
            if (ObjectHolder.SelectedObject && ObjectHolder.SelectedObject.TryGetComponent(out Connector secondConnector))
            {
                if (_connector.CanConnect(secondConnector))
                    secondConnector.Connect(_connector);
                else if (!secondConnector.IsConnected)
                {
                    transform.rotation = secondConnector.ConnectionRotation * _connector.RotationOffset;
                    transform.position = (secondConnector.ConnectionPosition + secondConnector.ConnectedOutOffset * 0.2f) - (_connector.ConnectionPosition - _connector.transform.position);
                }
            }

            base.Drop();
        }
    }
}