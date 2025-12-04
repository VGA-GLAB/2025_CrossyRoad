using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ClickUI : MonoBehaviour, IPointerClickHandler
{
    [Header("�k��")]
    [SerializeField] private Vector2 changeScale;
    [Header("���̃T�C�Y")]
    [SerializeField] private Vector2 originalScale;
    [Header("���o����")]
    [SerializeField] private float scaleDuration;

    private bool isClicked; //�N���b�N�ς݂�

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //���ɉ������ꍇ�́A�N���b�N�o���Ȃ��悤�ɂ���
        if (isClicked) return;
        isClicked = true;

        this.gameObject.transform.DOScale(changeScale, scaleDuration)
            .OnComplete(() =>
            {
                this.gameObject.transform.DOScale(originalScale, scaleDuration).
                OnComplete(Click);
            });
        SoundManager.instance.PlaySE("クリック");
    }

    void Click()
    {
        GameManager.instance.ChangeGameState(GameState.Title);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
