using cakeslice;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
    [Header("Lever 정보")]
    [SerializeField] protected EnvironmentData data;
    [SerializeField] protected Outline outline;

    [Header("Cube 정보")]
    [SerializeField] private Transform cubeTransform; //이동할 큐브의 Transform
    [SerializeField] private Vector3 targetPosition; //큐브를 이동시키고 싶은 목표 위치   
    [SerializeField] private float cubeMovsSpeed = 2f; //큐브의 이동속도
    
    private Cube cube;
    private Animator animator;

    private bool isPull = false;
    private bool isMove = false;

    [SerializeField] private AudioClip[] clips;

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator가 할당되지 않았습니다! GameObject에 Animator 컴포넌트가 있는지 확인하세요.");
        }
        cube = cubeTransform.GetComponent<Cube>();
    }
    public string GetInteractPrompt()
    {
        string str = $"<font=\"GmarketSansMedium SDF\" material=\"GmarketSansMedium SDF Glow Blue\">{data.displayName}</font> - {data.description}";
        return str;
    }

    public bool OnInteract()
    {
        if (isMove) return false; //이미 큐브가 움직이고 있다면 상호작용 불가

        if(cube != null)
        {
           // SoundManager.PlayClip(clips[0]);
            isPull = !isPull;
            animator.SetBool("IsPull", isPull);
            Debug.Log("애니메이션 시작");

            isMove = true;
            StartCoroutine(WaitForAnimation());// 애니메이션이 실행될 때까지 기다렸다가 이후 큐브 이동

            return true;
        }
        Debug.Log("Cube가 연결되지 않았습니다.");
        return false;
    }
    private IEnumerator WaitForAnimation()
    {
        Debug.Log("레버를 당겼습니다. 애니메이션 시작");
        float aniLength = animator.GetCurrentAnimatorStateInfo(0).length; // 애니메이션의 이벤트를 활용하여 애니메이션의 정보를 가져오는 메서드
        Debug.Log($"애니메이션 길이 {aniLength}초");

        if (aniLength <= 0f)
        {
            Debug.Log("애니메이션이 없음.");
            yield break;
        }
        yield return new WaitForSeconds(aniLength); //애니메이션이 끝날때까지 대기
        Debug.Log("애니메이션 종료 큐브 이동 시작");
        

        StartCoroutine(MoveCube());// 애니메이션이 끝이나면 큐브가 이동되도록
    }
    public virtual void SetOutline(bool show)
    {
        if(outline == null)
        {
            Debug.Log("Outline 컴포넌트가 없습니다.");
        }
        if (outline != null) outline.color = show ? 0 : 1;
    }
    private IEnumerator MoveCube()
    {
        Debug.Log("큐브 이동 시작됨");

        if(cube == null)
        {
            Debug.Log("큐브가 할당되지 않음");
        }
        //while (cube.transform.position.z >= targetPosition.z)
        //{
        //    // Debug.Log($"{startPosition}, {targetPosition},{cubeMovsSpeed * Time.deltaTime}");
        //    cube.transform.position = Vector3.MoveTowards(cubeTransform.position, targetPosition, cubeMovsSpeed*Time.deltaTime);
        //    // 시간단위로 원하는 목표지점까지 Lerp가 아닌 MoveTowards()를 통해 일정 속도로 이동
        //    // 속도 조절에 용이
        //    Debug.Log(cube.transform.position);
        //    yield return null; // 한 프레임을 대기
        //}
        //cube.transform.DOMove(cube.transform.position + targetPosition, cubeMovsSpeed);
        cube.GetComponent<Rigidbody>().AddForce(-Vector3.forward * (isPull ? 3f : -3f), ForceMode.Impulse);
        //SoundManager.PlayClip(clips[1]);
        yield return new WaitForSeconds(5f);
        Debug.Log("큐비 이동 완료!");
        isMove = false; //큐브가 모두 움직였다면 다시 상호작용이 가능하도록 세팅
        //animator.SetBool("IsPull", false);
    }
}
