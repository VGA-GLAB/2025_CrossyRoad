using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIEffect : MonoBehaviour
{
    [Header("タイトル")]
    [SerializeField] GameObject title;
    [Header("タイトル背景")]
    [SerializeField] Image titleImage;
    [Header("各位置")]
    [SerializeField] Vector3 titlePositonFirst;
    [SerializeField] Vector3 titlePositionMiddle;
    [SerializeField] Vector3 titlePositionFinal;
    [Header("演出時間")]
    [SerializeField] float titleDuration;

    [Header("リトライボタン")]
    [SerializeField] RectTransform retryButtonRectTransform;
    [Header("終了位置")]
    [SerializeField] Vector2 retryButtonPosition;
    [Header("演出時間")]
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
