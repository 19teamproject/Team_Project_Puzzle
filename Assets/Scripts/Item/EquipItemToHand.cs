using UnityEngine;

public class EquipItemToHand : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public void EquipTorchToHand()
    {

        GameObject itemToHand = player.Equipment.CurEquip.gameObject;

        // F키로 장착버튼 누르면 Item 소환은 되는데 위치가 이상한 곳에 떠서 미세조정했습니다.

        itemToHand.transform.localPosition = new Vector3(0.2f, 0, 0);
        itemToHand.transform.localRotation = Quaternion.Euler(new Vector3(180, -180, -280));
        itemToHand.transform.localScale = Vector3.one / 2;

        Debug.Log("Torch 장착 완료");

    }
}
