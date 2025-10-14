using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIEffect : MonoBehaviour
{
    [Header("�^�C�g��")]
    [SerializeField] GameObject title;
    [Header("�^�C�g���w�i")]
    [SerializeField] Image titleImage;
    [Header("�e�ʒu")]
    [SerializeField] Vector3 titlePositonFirst;
    [SerializeField] Vector3 titlePositionMiddle;
    [SerializeField] Vector3 titlePositionFinal;
    [Header("���o����")]
    [SerializeField] float titleDuration;

    [Header("���g���C�{�^��")]
    [SerializeField] RectTransform retryButtonRectTransform;
    [Header("�I���ʒu")]
    [SerializeField] Vector2 retryButtonPosition;
    [Header("���o����")]
    [SerializeField] float retryButtonDuration;

    public void TitleFadeIn()
    {
        title.transform.DOMove(titlePositionMiddle, titleDuration).SetEase(Ease.InOutSine);
        titleImage.DOFade(1, titleDuration);
    }

    public void TitleFadeOut()
    {
        titleImage.DOFade(0, titleDuration);
        title.transform.DOMove(titlePositionFinal, titleDuration).SetEase(Ease.InOutSine).
            OnComplete(() =>
            {
                GameManager.instance.inGameUIManager.TitleUI.SetActive(false);
                title.transform.position = titlePositonFirst;
            });
    }

    public void ButtonAppearanceAnimation()
    {
        retryButtonRectTransform.DOAnchorPos(retryButtonPosition, retryButtonDuration).SetEase(Ease.InOutSine);
    }
}
