using UnityEngine;

// 다른 스크립트에서 WaterManager.instance.Melt();를 호출하면 물이 점점 차오르게 됩니다.

public class WaterManager : MonoBehaviour
{

    public static WaterManager instance;
    [SerializeField]
    private GameObject waterObj;
    [SerializeField]
    private int meltingNum;
    [SerializeField]
    private int meltMinNum = 0;  // 물의 수위 초기화

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        waterObj.transform.position = new Vector3(0, 0 - 0.55f);  // 물의 초기 위치
        waterObj.SetActive(false);

    }

    public void Melt()
    {
        meltingNum++;  // Melt()가 호출될 때마다 meltingNum 1씩 증가
        
        if (meltingNum >= meltMinNum)  // MeltingNum이 MelitngMinNum 이상일 때면 물이 올라감
        {
            if (waterObj.transform.position.y < 1.25f)  // 물이 1.5f에 도달하면 더 이상 상승하지 않음
            {
                waterObj.SetActive(true);
                Vector3 currentPos = waterObj.transform.position;
                currentPos.y += 0.051f;  // 물의 수위 증가

                waterObj.transform.position = currentPos;

            }
        }

    }


}
