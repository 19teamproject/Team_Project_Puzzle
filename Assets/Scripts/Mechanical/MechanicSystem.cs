using DG.Tweening; // DOTween: 트윈(부드러운 애니메이션) 라이브러리
using UnityEngine;

namespace HPhysic
{
    // 다양한 메커니즘 타입
    public enum MechanicType
    {
        Cable,       // 케이블 두 개 연결
        Button,      // 버튼 누르기
        Pressure,    // 압력판 (밟으면 작동)
        CableBoth,   // 양쪽 모두 연결
        CallButton   // 버튼을 눌러 다른 버튼 호출
    }

    // 환경 오브젝트 중 하나 (EnvironmentObject를 상속)
    public class MechanicSystem : EnvironmentObject
    {
        // 연결해야 할 케이블들
        [SerializeField] private PhysicCable redCable;
        [SerializeField] private PhysicCable blueCable;

        [SerializeField] private MechanicType type; // 어떤 타입의 메커니즘인가
        [SerializeField] private Vector3 targetPos; // 움직일 목표 위치
        [SerializeField] private float duration;    // 이동에 걸리는 시간
        [SerializeField] private GameObject movingObj; // 움직일 오브젝트
        [SerializeField] private MechanicSystem callBtn; // 호출할 버튼 시스템

        private Vector3 startPos; // 처음 위치 저장
        private bool isActive = false; // 현재 활성화 상태

        [SerializeField] private AudioClip[] clips; // 사운드 클립

        private void Awake()
        {
            startPos = movingObj.transform.position; // 시작할 때 위치 저장
        }

        private void Update()
        {
            // 매 프레임 체크: 케이블 기반 메커니즘만
            if (type == MechanicType.Cable)
                CableSystem();
            else if (type == MechanicType.CableBoth)
                CableBothSystem();
        }

        // 상호작용할 때 UI로 표시할 문구
        public new string GetInteractPrompt()
        {
            string str = $"<font=\"GmarketSansMedium SDF\" material=\"GmarketSansMedium SDF Glow Blue\">" +
                $"{data.displayName}</font> - {data.description}";

            return str;
        }

        // 아웃라인 색상 변경
        public override void SetOutline(bool show)
        {
            if (outline != null)
                outline.color = show ? 0 : 1;
        }

        // 실제 상호작용했을 때
        public override bool OnInteract()
        {
            if (type == MechanicType.Button)
                ButtonSystem();
            else if (type == MechanicType.CallButton)
                callBtn.ButtonSystem(); // 다른 버튼의 버튼시스템 호출

            return false;
        }

        // 버튼 메커니즘: 눌렀을 때 이동
        public void ButtonSystem()
        {
            if (movingObj != null)
            {
                isActive = !isActive; // 토글

                movingObj.transform.DOKill(); // 기존 트윈 중지
                if (isActive)
                    movingObj.transform.DOMove(startPos + targetPos, duration); // 목표로 이동
                else
                    movingObj.transform.DOMove(startPos, duration); // 처음 위치로 이동

                SoundManager.Instance.PlayClip(clips[0]); // 사운드 재생
            }
        }

        // 압력판 메커니즘: 올라가면 이동
        public void PressureSystem()
        {
            if (movingObj != null)
            {
                movingObj.transform.DOKill();
                if (isActive)
                    movingObj.transform.DOMove(startPos + targetPos, duration);
                else
                    movingObj.transform.DOMove(startPos, duration);

                SoundManager.Instance.PlayClip(clips[0]);
            }
        }

        // 케이블 연결 시스템
        public void CableSystem()
        {
            if (redCable != null && blueCable != null)
            {
                // 둘 다 올바르게 연결되었으면
                if (redCable.StartConnector.IsConnectedRight && blueCable.StartConnector.IsConnectedRight && !isActive)
                {
                    isActive = true;
                    movingObj.transform.DOKill();
                    movingObj.transform.DOMove(startPos + targetPos, duration);
                    SoundManager.Instance.PlayClip(clips[0]);
                }
                // 연결 해제되면 되돌리기
                else if ((!redCable.StartConnector.IsConnectedRight || !blueCable.StartConnector.IsConnectedRight) && isActive)
                {
                    isActive = false;
                    movingObj.transform.DOKill();
                    movingObj.transform.DOMove(startPos, duration);
                    SoundManager.Instance.PlayClip(clips[0]);
                }
            }
        }

        // 양쪽 모두 완전 연결 시스템
        public void CableBothSystem()
        {
            if (redCable != null && blueCable != null)
            {
                if (redCable.IsAllConnectedRight && blueCable.IsAllConnectedRight && !isActive)
                {
                    isActive = true;
                    movingObj.transform.DOKill();
                    movingObj.transform.DOMove(startPos + targetPos, duration);
                    SoundManager.Instance.PlayClip(clips[0]);
                }
                else if ((!redCable.IsAllConnectedRight || !blueCable.IsAllConnectedRight) && isActive)
                {
                    isActive = false;
                    movingObj.transform.DOKill();
                    movingObj.transform.DOMove(startPos, duration);
                    SoundManager.Instance.PlayClip(clips[0]);
                }
            }
        }

        // 압력판: 물체가 올라올 때
        private void OnCollisionEnter(Collision collision)
        {
            isActive = true;
            PressureSystem();
        }

        // 압력판: 물체가 내려갈 때
        private void OnCollisionExit(Collision collision)
        {
            isActive = false;
            PressureSystem();
        }
    }
}