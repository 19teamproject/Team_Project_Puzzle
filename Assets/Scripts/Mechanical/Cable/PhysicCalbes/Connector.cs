using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using cakeslice;

namespace HPhysic
{
    // Rigidbody 없으면 자동 추가
    [RequireComponent(typeof(Rigidbody))]
    public class Connector : MonoBehaviour
    {
        // 플러그인지 콘센트인지 구분
        public enum ConType { Plug, Outlet }
        public enum CableColor { White, Red, Green, Yellow, Blue, Cyan, Magenta }

        [field: Header("Settings")]

        [field: SerializeField] public ConType ConnectionType { get; private set; } = ConType.Plug;
        [field: SerializeField, OnValueChanged(nameof(UpdateConnectorColor))] public CableColor ConnectionColor { get; private set; } = CableColor.White;
        // OnValueChanged : 인스펙터에서 값 변경될때 특정 메서드 자동 실행

        [SerializeField] private bool makeConnectionKinematic = false;
        private bool _wasConnectionKinematic;

        [SerializeField] private bool hideInteractableWhenIsConnected = false;
        [SerializeField] private bool allowConnectDifrentCollor = false;

        // 연결한 오브젝트의 Connector
        [field: SerializeField] public Connector ConnectedTo { get; private set; }

        
        // 세팅해야할 오브젝트
        [Header("Object to set")]
        [SerializeField, Required] private Transform connectionPoint;
        [SerializeField] private MeshRenderer collorRenderer;
        [SerializeField] private ParticleSystem sparksParticle;
        [SerializeField] private Outline outline;


        private FixedJoint _fixedJoint;
        public Rigidbody rb { get; private set; }

        public Vector3 ConnectionPosition => connectionPoint ? connectionPoint.position : transform.position;
        public Quaternion ConnectionRotation => connectionPoint ? connectionPoint.rotation : transform.rotation;
        public Quaternion RotationOffset => connectionPoint ? connectionPoint.localRotation : Quaternion.Euler(Vector3.zero);
        public Vector3 ConnectedOutOffset => connectionPoint ? connectionPoint.right : transform.right;

        public bool IsConnected => ConnectedTo != null;
        public bool IsConnectedRight => IsConnected && ConnectionColor == ConnectedTo.ConnectionColor;


        [SerializeField] private AudioClip[] clips;


        private void Awake()
        {
            rb = gameObject.GetComponent<Rigidbody>();
        }

        private void Start()
        {
            UpdateConnectorColor();

            // 이미 연결되어 있다면
            if (ConnectedTo != null)
            {
                // 연결된 상태로 세팅
                Connector t = ConnectedTo;
                ConnectedTo = null;
                Connect(t);
            }
        }

        private void OnDisable() => Disconnect();

        // 연결할 곳에 연결한다
        public void SetAsConnectedTo(Connector secondConnector)
        {
            ConnectedTo = secondConnector;
            _wasConnectionKinematic = secondConnector.rb.isKinematic;
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

            // 이미 연결되어 있다면 연결 해제
            if (IsConnected)
                Disconnect(secondConnector);

            // 연결할 오브젝트의 회전값을 설정
            secondConnector.transform.rotation = ConnectionRotation * secondConnector.RotationOffset;
            secondConnector.transform.position = ConnectionPosition - (secondConnector.ConnectionPosition - secondConnector.transform.position);

            _fixedJoint = gameObject.AddComponent<FixedJoint>();
            _fixedJoint.connectedBody = secondConnector.rb;

            secondConnector.SetAsConnectedTo(this);
            _wasConnectionKinematic = secondConnector.rb.isKinematic;
            if (makeConnectionKinematic)
                secondConnector.rb.isKinematic = true;
            ConnectedTo = secondConnector;

            // 잘못된 연결에서 발생하는 스파크
            if (incorrectSparksC == null && sparksParticle && IsConnected && !IsConnectedRight)
            {
                SoundManager.PlayClip(clips[0]);
                incorrectSparksC = IncorrectSparks();
                StartCoroutine(incorrectSparksC);
            }
            else
            {
                SoundManager.PlayClip(clips[1]);
            }

            UpdateInteractableWhenIsConnected();
        }
        
        // 연결 해제하기
        public void Disconnect(Connector onlyThis = null)
        {
            if (ConnectedTo == null || onlyThis != null && onlyThis != ConnectedTo)
                return;

            Destroy(_fixedJoint);

            // 재귀를 사용하지 않는 것이 중요하다
            Connector toDisconect = ConnectedTo;
            ConnectedTo = null;
            if (makeConnectionKinematic)
                toDisconect.rb.isKinematic = _wasConnectionKinematic;
            toDisconect.Disconnect(this);

            if (sparksParticle)
            {
                sparksParticle.Stop();
                sparksParticle.Clear();
            }

            UpdateInteractableWhenIsConnected();
        }

        private void UpdateInteractableWhenIsConnected()
        {
            if (hideInteractableWhenIsConnected)
            {
                if (TryGetComponent(out Collider collider))
                    collider.enabled = !IsConnected;
            }
        }

        // 스파크 반복적으로 재생
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

        // Connector 색상 설정
        private void UpdateConnectorColor()
        {
            if (collorRenderer == null)
                return;

            Color color = MaterialColor(ConnectionColor);
            // Renderer 에 적용된 Material 의 속성 개별적으로 변경
            MaterialPropertyBlock probs = new();
            collorRenderer.GetPropertyBlock(probs);
            probs.SetColor("_Color", color);
            collorRenderer.SetPropertyBlock(probs);
        }

        // 종류에 맞게 색상 반환
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

        // 연결할 수 있는지 확인
        public bool CanConnect(Connector secondConnector) =>
            this != secondConnector
            && !this.IsConnected && !secondConnector.IsConnected
            && this.ConnectionType != secondConnector.ConnectionType
            && (this.allowConnectDifrentCollor || secondConnector.allowConnectDifrentCollor || this.ConnectionColor == secondConnector.ConnectionColor);

        public void SetOutline(bool show)
        {
            if (outline != null)
            {
                outline.color = show ? 0 : 1;
            }
        }
    }
}