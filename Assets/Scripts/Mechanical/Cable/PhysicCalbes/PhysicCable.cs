using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes; // 인스펙터 강화용 어트리뷰트

namespace HPhysic
{
    // 물리 기반 케이블 구성 스크립트
    public class PhysicCable : MonoBehaviour
    {
        // 케이블의 기본 설정들 (길이, 간격, 크기)
        [Header("Look")]
        [SerializeField, Min(1)] private int numberOfPoints = 3;  // 연결 포인트 수
        [SerializeField, Min(0.01f)] private float space = 0.3f;  // 포인트 간 거리
        [SerializeField, Min(0.01f)] private float size = 0.3f;   // 포인트 크기

        // 케이블 물리 거동 관련 설정
        [Header("Bahaviour")]
        [SerializeField, Min(1f)] private float springForce = 200;        // 스프링 강도
        [SerializeField, Min(1f)] private float brakeLengthMultiplier = 2f; // 끊어질 허용 길이 배수
        [SerializeField, Min(0.1f)] private float minBrakeTime = 1f;       // 끊어지기까지 최소 시간

        private float brakeLength;  // 끊어지기 시작할 길이
        private float timeToBrake = 1f; // 끊어지기까지 남은 시간

        // 필수 설정 오브젝트들
        [Header("Object to set")]
        [SerializeField, Required] private GameObject start; // 케이블 시작점
        [SerializeField, Required] private GameObject end;   // 케이블 끝점
        [SerializeField, Required] private GameObject connector0; // 처음 연결부
        [SerializeField, Required] private GameObject point0;     // 처음 포인트

        // 내부 관리용 리스트들
        private List<Transform> points;       // 포인트(구슬) 리스트
        private List<Transform> connectors;   // 연결부 리스트

        private const string cloneText = "Part"; // 복제 생성물 이름 기본값

        private Connector startConnector;
        private Connector endConnector;

        // 에디터 버튼용: 케이블 포인트 전체 리셋
        [Button("Reset points")]
        private void UpdatePoints()
        {
            if (!start || !end || !point0 || !connector0)
            {
                Debug.LogWarning("설정된 오브젝트가 없습니다!");
                return;
            }

            // 기존 파트 삭제
            int length = transform.childCount;
            for (int i = 0; i < length; i++)
                if (transform.GetChild(i).name.StartsWith(cloneText))
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                    length--; i--;
                }

            // 포인트와 연결부 재구성
            Vector3 lastPos = start.transform.position;
            Rigidbody lastBody = start.GetComponent<Rigidbody>();
            for (int i = 0; i < numberOfPoints; i++)
            {
                GameObject cConnector = i == 0 ? connector0 : CreateNewCon(i);
                GameObject cPoint = i == 0 ? point0 : CreateNewPoint(i);

                Vector3 newPos = CountNewPointPos(lastPos);
                cPoint.transform.position = newPos;
                cPoint.transform.localScale = Vector3.one * size;
                cPoint.transform.rotation = transform.rotation;

                SetSpirng(cPoint.GetComponent<SpringJoint>(), lastBody);
                lastBody = cPoint.GetComponent<Rigidbody>();

                cConnector.transform.position = CountConPos(lastPos, newPos);
                cConnector.transform.localScale = CountSizeOfCon(lastPos, newPos);
                cConnector.transform.rotation = CountRoationOfCon(lastPos, newPos);

                lastPos = newPos;
            }

            // 엔드포인트 설정
            Vector3 endPos = CountNewPointPos(lastPos);
            end.transform.position = endPos;
            SetSpirng(lastBody.gameObject.AddComponent<SpringJoint>(), end.GetComponent<Rigidbody>());

            GameObject endConnector = CreateNewCon(numberOfPoints);
            endConnector.transform.position = CountConPos(lastPos, endPos);
            endConnector.transform.rotation = CountRoationOfCon(lastPos, endPos);

            Vector3 CountNewPointPos(Vector3 pos) => pos + transform.forward * space;
        }

        // 포인트 추가
        [Button("Add point")]
        private void AddPoint()
        {
            Transform lastPrevPoint = GetPoint(numberOfPoints - 1);
            if (lastPrevPoint == null)
            {
                Debug.LogWarning("이전 포인트를 찾지 못했습니다.");
                return;
            }

            Rigidbody endRB = end.GetComponent<Rigidbody>();
            foreach (var spring in lastPrevPoint.GetComponents<SpringJoint>())
                if (spring.connectedBody == endRB)
                    DestroyImmediate(spring);

            GameObject cPoint = CreateNewPoint(numberOfPoints);
            GameObject cConnector = CreateNewCon(numberOfPoints + 1);

            cPoint.transform.position = end.transform.position;
            cPoint.transform.rotation = end.transform.rotation;
            cPoint.transform.localScale = Vector3.one * size;

            SetSpirng(cPoint.GetComponent<SpringJoint>(), lastPrevPoint.GetComponent<Rigidbody>());
            SetSpirng(cPoint.AddComponent<SpringJoint>(), endRB);

            end.transform.position += end.transform.forward * space;

            cConnector.transform.position = CountConPos(cPoint.transform.position, end.transform.position);
            cConnector.transform.localScale = CountSizeOfCon(cPoint.transform.position, end.transform.position);
            cConnector.transform.rotation = CountRoationOfCon(cPoint.transform.position, end.transform.position);

            numberOfPoints++;
        }

        // 포인트 삭제
        [Button("Remove point")]
        private void RemovePoint()
        {
            if (numberOfPoints < 2)
            {
                Debug.LogWarning("1개 이하로는 줄일 수 없습니다.");
                return;
            }

            Transform lastPrevPoint = GetPoint(numberOfPoints - 1);
            Transform lastPrevCon = GetConnector(numberOfPoints);
            Transform lastLastPrevPoint = GetPoint(numberOfPoints - 2);

            Rigidbody endRB = end.GetComponent<Rigidbody>();
            SetSpirng(lastLastPrevPoint.gameObject.AddComponent<SpringJoint>(), endRB);

            end.transform.position = lastPrevPoint.position;
            end.transform.rotation = lastPrevPoint.rotation;

            DestroyImmediate(lastPrevPoint.gameObject);
            DestroyImmediate(lastPrevCon.gameObject);

            numberOfPoints--;
        }

        // 시작할 때 연결 상태 설정
        private void Start()
        {
            startConnector = start.GetComponent<Connector>();
            endConnector = end.GetComponent<Connector>();

            brakeLength = space * numberOfPoints * brakeLengthMultiplier + 2f;

            points = new List<Transform> { start.transform, point0.transform };
            connectors = new List<Transform> { connector0.transform };

            for (int i = 1; i < numberOfPoints; i++)
            {
                connectors.Add(GetConnector(i));
                points.Add(GetPoint(i));
            }
            connectors.Add(GetConnector(numberOfPoints));
            points.Add(end.transform);
        }

        // 매 프레임 케이블 업데이트
        private void Update()
        {
            float cableLength = 0f;
            bool isConnected = startConnector.IsConnected || endConnector.IsConnected;

            int numOfParts = connectors.Count;
            Transform lastPoint = points[0];

            for (int i = 0; i < numOfParts; i++)
            {
                Transform nextPoint = points[i + 1];
                Transform connector = connectors[i];

                connector.position = CountConPos(lastPoint.position, nextPoint.position);
                if (lastPoint.position == nextPoint.position || nextPoint.position == connector.position)
                {
                    connector.localScale = Vector3.zero;
                }
                else
                {
                    connector.rotation = Quaternion.LookRotation(nextPoint.position - connector.position);
                    connector.localScale = CountSizeOfCon(lastPoint.position, nextPoint.position);
                }

                if (isConnected)
                    cableLength += (lastPoint.position - nextPoint.position).magnitude;

                lastPoint = nextPoint;
            }

            // 케이블이 너무 늘어나면 자동으로 끊기
            if (isConnected)
            {
                if (cableLength > brakeLength)
                {
                    timeToBrake -= Time.deltaTime;
                    if (timeToBrake < 0f)
                    {
                        startConnector.Disconnect();
                        endConnector.Disconnect();
                        timeToBrake = minBrakeTime;
                    }
                }
                else
                {
                    timeToBrake = minBrakeTime;
                }
            }
        }

        // 유틸리티 함수들 (포지션/스케일/회전 계산)
        private Vector3 CountConPos(Vector3 start, Vector3 end) => (start + end) / 2f;
        private Vector3 CountSizeOfCon(Vector3 start, Vector3 end) => new Vector3(size, size, (start - end).magnitude / 2f);
        private Quaternion CountRoationOfCon(Vector3 start, Vector3 end) => Quaternion.LookRotation(end - start, Vector3.right);

        private string ConnectorName(int index) => $"{cloneText}_{index}_Conn";
        private string PointName(int index) => $"{cloneText}_{index}_Point";
        private Transform GetConnector(int index) => index > 0 ? transform.Find(ConnectorName(index)) : connector0.transform;
        private Transform GetPoint(int index) => index > 0 ? transform.Find(PointName(index)) : point0.transform;

        public void SetSpirng(SpringJoint spring, Rigidbody connectedBody)
        {
            spring.connectedBody = connectedBody;
            spring.spring = springForce;
            spring.damper = 0.2f;
            spring.autoConfigureConnectedAnchor = false;
            spring.anchor = Vector3.zero;
            spring.connectedAnchor = Vector3.zero;
            spring.minDistance = space;
            spring.maxDistance = space;
        }

        private GameObject CreateNewPoint(int index)
        {
            GameObject temp = Instantiate(point0);
            temp.name = PointName(index);
            temp.transform.parent = transform;
            return temp;
        }

        private GameObject CreateNewCon(int index)
        {
            GameObject temp = Instantiate(connector0);
            temp.name = ConnectorName(index);
            temp.transform.parent = transform;
            return temp;
        }

        // 외부 접근용 프로퍼티
        public Connector StartConnector => startConnector;
        public Connector EndConnector => endConnector;
        public IReadOnlyList<Transform> Points => points;
        public bool IsAllConnectedRight => startConnector.IsConnectedRight && endConnector.IsConnectedRight;
    }
}
