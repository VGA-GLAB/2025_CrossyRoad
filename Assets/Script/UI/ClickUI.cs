using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ClickUI : MonoBehaviour, IPointerClickHandler
{
    [Header("�k��")]
    [SerializeField] Vector2 changeScale;
    [Header("���̃T�C�Y")]
    [SerializeField] Vector2 originalScale;
    [Header("���o����")]
    [SerializeField] float scaleDuration;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        this.gameObject.transform.DOScale(changeScale, scaleDuration)
            .OnComplete(() =>
            {
                this.gameObject.transform.DOScale(originalScale, scaleDuration).
                OnComplete(Click);
            });
    }

    void Click()
    {
        GameManager.instance.ChangeIGameState(new TitleState());
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
