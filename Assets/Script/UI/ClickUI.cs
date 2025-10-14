using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ClickUI : MonoBehaviour, IPointerClickHandler
{
    [Header("縮尺")]
    [SerializeField] private Vector2 changeScale;
    [Header("元のサイズ")]
    [SerializeField] private Vector2 originalScale;
    [Header("演出時間")]
    [SerializeField] private float scaleDuration;

    private bool isClicked; //クリック済みか

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //既に押した場合は、クリック出来ないようにする
        if (isClicked) return;
        isClicked = true;

        this.gameObject.transform.DOScale(changeScale, scaleDuration)
            .OnComplete(() =>
            {
                this.gameObject.transform.DOScale(originalScale, scaleDuration).
                OnComplete(Click);
            });
    }

    void Click()
    {
        GameManager.instance.ChangeGameState(GameState.Title);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
