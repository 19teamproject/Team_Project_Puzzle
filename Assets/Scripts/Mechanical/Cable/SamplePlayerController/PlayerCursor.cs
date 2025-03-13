using System.Collections;
using UnityEngine;
using HInteractions;

namespace HPlayer
{
    // PlayerInteractions 없으면 자동 추가
    [RequireComponent(typeof(PlayerInteractions))]
    public class PlayerCursor : MonoBehaviour
    {
        // 임시 에임
        [SerializeField] private GameObject cursorCanvas;
        [SerializeField, Min(0)] private float minShowDistance;

        private PlayerInteractions playerInteractions;
        private IEnumerator cursorUpdater;

        private void OnEnable()
        {
            playerInteractions = GetComponent<PlayerInteractions>();
            if (playerInteractions == null)
                return;

            playerInteractions.OnSelect += ActiveCursor;
        }
        private void OnDisable()
        {
            if (playerInteractions == null)
                return;

            playerInteractions.OnSelect -= ActiveCursor;

            DesactiveCursor();
        }

        // 커서 활성화
        private void ActiveCursor()
        {
            if (playerInteractions == null)
                return;

            // 바라보는 오브젝트가 Interactable 타입이고, 에임이 표시되어 있다면
            if (playerInteractions.SelectedObject is Interactable interactable && interactable.ShowPointerOnInterract)
            {
                cursorUpdater = UpdateCursor();
                StartCoroutine(cursorUpdater);
            }
        }

        // 커서 비활성화
        private void DesactiveCursor()
        {
            cursorCanvas?.SetActive(false);

            if (cursorUpdater != null)
            {
                StopCoroutine(cursorUpdater);
                cursorUpdater = null;
            }
        }

        // 0.2초마다 커서 업데이트
        private IEnumerator UpdateCursor()
        {
            if (cursorCanvas == null)
                yield break;

            while (playerInteractions.SelectedObject != null)
            {
                float distance = Vector3.Distance(playerInteractions.SelectedObject.transform.position, transform.position);
                cursorCanvas.SetActive(distance >= minShowDistance);

                yield return new WaitForSeconds(0.2f);
            }

            cursorUpdater = null;
            DesactiveCursor();
        }
    }
}