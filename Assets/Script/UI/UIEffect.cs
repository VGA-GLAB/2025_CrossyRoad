using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIEffect : MonoBehaviour
{
    [Header("タイトル")]
    [SerializeField] private RectTransform titleRectTransform;
    [Header("タイトル背景")]
    [SerializeField] Image titleImage;
    [Header("各位置")]
    [SerializeField] Vector2 titlePositionFirst;
    [SerializeField] Vector2 titlePositionMiddle;
    [SerializeField] Vector2 titlePositionFinal;
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
        titleRectTransform.anchoredPosition = titlePositionFirst;

        titleRectTransform
            .DOAnchorPos(titlePositionMiddle, titleDuration)
            .SetEase(Ease.InOutSine);

        titleImage.DOFade(1f, titleDuration);
    }

    public void TitleFadeOut()
    {
        titleImage.DOFade(0f, titleDuration);

        titleRectTransform
            .DOAnchorPos(titlePositionFinal, titleDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                GameManager.instance.inGameUIManager.TitleUI.SetActive(false);
                // 次回用に初期ローカル位置へ戻す
                titleRectTransform.anchoredPosition = titlePositionFirst;
            });
    }

    public void ButtonAppearanceAnimation()
    {
        retryButtonRectTransform.DOAnchorPos(retryButtonPosition, retryButtonDuration).SetEase(Ease.InOutSine);
    }
}
