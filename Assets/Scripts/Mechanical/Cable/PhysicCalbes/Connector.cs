using System.Collections;
using UnityEngine;
using NaughtyAttributes; // Inspector 확장용 Attribute
using cakeslice;         // Outline 효과를 위한 외부 플러그인

namespace HPhysic
{
    // Rigidbody 컴포넌트가 필수. 없으면 자동으로 붙여줌
    [RequireComponent(typeof(Rigidbody))]
    public class Connector : MonoBehaviour
    {
        // 어떤 타입의 커넥터인지 구분
        public enum ConType { Plug, Outlet }  // 플러그 / 콘센트
        public enum CableColor { White, Red, Green, Yellow, Blue, Cyan, Magenta } // 선 색깔

        [field: Header("Settings")]

        // 커넥션 타입 (플러그 or 콘센트)
        [field: SerializeField] public ConType ConnectionType { get; private set; } = ConType.Plug;

        // 연결 색깔 설정. 인스펙터에서 변경 시 색도 갱신
        [field: SerializeField, OnValueChanged(nameof(UpdateConnectorColor))]
        public CableColor ConnectionColor { get; private set; } = CableColor.White;

        [SerializeField] private bool makeConnectionKinematic = false; // 연결 후 물리 정지 여부
        private bool _wasConnectionKinematic; // 원래 상태 저장용

        [SerializeField] private bool hideInteractableWhenIsConnected = false; // 연결되면 상호작용 비활성화 여부
        [SerializeField] private bool allowConnectDifrentCollor = false; // 다른 색상끼리 연결 허용 여부

        [field: SerializeField] public Connector ConnectedTo { get; private set; } // 연결된 상대 Connector

        [Header("Object to set")] // 에디터에서 세팅할 객체들
        [SerializeField, Required] private Transform connectionPoint; // 실제 연결 위치
        [SerializeField] private MeshRenderer collorRenderer;         // 색상 표시용 MeshRenderer
        [SerializeField] private ParticleSystem sparksParticle;       // 스파크 파티클
        [SerializeField] private Outline outline;                     // 아웃라인 표시

        private FixedJoint fixedJoint; // 물리적으로 연결할 때 사용
        public Rigidbody rb { get; private set; } // 자기 자신의 Rigidbody

        // 연결 관련 프로퍼티들
        public Vector3 ConnectionPosition => connectionPoint ? connectionPoint.position : transform.position;
        public Quaternion ConnectionRotation => connectionPoint ? connectionPoint.rotation : transform.rotation;
        public Quaternion RotationOffset => connectionPoint ? connectionPoint.localRotation : Quaternion.Euler(Vector3.zero);
        public Vector3 ConnectedOutOffset => connectionPoint ? connectionPoint.right : transform.right;

        public bool IsConnected => ConnectedTo != null; // 연결 여부
        public bool IsConnectedRight => IsConnected && ConnectionColor == ConnectedTo.ConnectionColor; // 올바른 색상 연결 여부

        [SerializeField] private AudioClip[] clips; // 연결/오류 사운드


        private void Awake()
        {
            rb = gameObject.GetComponent<Rigidbody>(); // Rigidbody 캐싱
        }

        private void Start()
        {
            UpdateConnectorColor(); // 시작할 때 색깔 설정

            // 만약 시작 시 이미 연결된 경우
            if (ConnectedTo != null)
            {
                Connector t = ConnectedTo;
                ConnectedTo = null;
                Connect(t); // 연결 초기화
            }
        }

        private void OnDisable() => Disconnect(); // 비활성화 시 연결 해제

        // 연결 대상 설정
        public void SetAsConnectedTo(Connector secondConnector)
        {
            ConnectedTo = secondConnector;
            _wasConnectionKinematic = secondConnector.rb.isKinematic; // 연결 대상의 원래 상태 저장
            UpdateInteractableWhenIsConnected();
        }

        // 연결하기
        public void Connect(Connector secondConnector)
        {
            // 연결하려는 오브젝트가 없다면 돌아가기
            if (secondConnector == null)
            {
                Debug.LogWarning("Attempt to connect null");
                return;
            }

            if (IsConnected)
                Disconnect(secondConnector); // 이미 연결 중이면 해제

            // 연결 위치 및 회전 정렬
            secondConnector.transform.rotation = ConnectionRotation * secondConnector.RotationOffset;
            secondConnector.transform.position = ConnectionPosition - (secondConnector.ConnectionPosition - secondConnector.transform.position);

            // 물리적으로 붙이기
            fixedJoint = gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = secondConnector.rb;

            secondConnector.SetAsConnectedTo(this);
            _wasConnectionKinematic = secondConnector.rb.isKinematic;
            if (makeConnectionKinematic)
                secondConnector.rb.isKinematic = true;
            ConnectedTo = secondConnector;

            // 잘못된 연결이면 스파크 발생
            if (incorrectSparksC == null && sparksParticle && IsConnected && !IsConnectedRight)
            {
                SoundManager.Instance.PlayClip(clips[0]);
                incorrectSparksC = IncorrectSparks();
                StartCoroutine(incorrectSparksC);
            }
            else
            {
                SoundManager.Instance.PlayClip(clips[1]);
            }

            UpdateInteractableWhenIsConnected();
        }
        
        // 연결 해제
        public void Disconnect(Connector onlyThis = null)
        {
            if (ConnectedTo == null || onlyThis != null && onlyThis != ConnectedTo)
                return;

            Destroy(fixedJoint); // FixedJoint 삭제

            // 연결 해제할 상대 저장 후
            Connector toDisconect = ConnectedTo;
            ConnectedTo = null;

            if (makeConnectionKinematic)
                toDisconect.rb.isKinematic = _wasConnectionKinematic; // 원상복귀

            toDisconect.Disconnect(this); // 재귀 호출 아님
            if (sparksParticle)
            {
                sparksParticle.Stop();
                sparksParticle.Clear();
            }

            UpdateInteractableWhenIsConnected();
        }

        // 연결 상태에 따라 Collider 비활성화
        private void UpdateInteractableWhenIsConnected()
        {
            if (hideInteractableWhenIsConnected)
            {
                if (TryGetComponent(out Collider collider))
                    collider.enabled = !IsConnected;
            }
        }

        // 잘못 연결된 경우 스파크 재생 루프
        private IEnumerator incorrectSparksC;
        private IEnumerator IncorrectSparks()
        {
            while (incorrectSparksC != null && sparksParticle && IsConnected && !IsConnectedRight)
            {
                sparksParticle.Play();

                yield return new WaitForSeconds(Random.Range(0.6f, 0.8f));
            }
            incorrectSparksC = null;
        }

        // 연결 색상 업데이트
        private void UpdateConnectorColor()
        {
            if (collorRenderer == null)
                return;

            Color color = MaterialColor(ConnectionColor);

            MaterialPropertyBlock probs = new();
            collorRenderer.GetPropertyBlock(probs);
            probs.SetColor("_Color", color);
            collorRenderer.SetPropertyBlock(probs);
        }

        // 색깔에 맞는 Unity Color 반환
        private Color MaterialColor(CableColor cableColor) => cableColor switch
        {
            CableColor.White => Color.white,
            CableColor.Red => Color.red,
            CableColor.Green => Color.green,
            CableColor.Yellow => Color.yellow,
            CableColor.Blue => Color.blue,
            CableColor.Cyan => Color.cyan,
            CableColor.Magenta => Color.magenta,
            _ => Color.clear
        };

        // 두 Connector가 연결 가능한지 판단
        public bool CanConnect(Connector secondConnector) =>
            this != secondConnector
            && !this.IsConnected && !secondConnector.IsConnected
            && this.ConnectionType != secondConnector.ConnectionType
            && (this.allowConnectDifrentCollor || secondConnector.allowConnectDifrentCollor || this.ConnectionColor == secondConnector.ConnectionColor);

        // 아웃라인 표시
        public void SetOutline(bool show)
        {
            if (outline != null)
            {
                outline.color = show ? 0 : 1;
            }
        }
    }
}