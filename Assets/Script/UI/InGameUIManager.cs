using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    [Header("タイトルUI")]
    [SerializeField] private GameObject titleUI;
    [Header("インゲームUI")]
    [SerializeField] private GameObject inGameUI;
    [Header("リザルトUI")]
    [SerializeField] private GameObject resultUI;

    public GameObject TitleUI => titleUI;
    public GameObject InGameUI => inGameUI;
    public GameObject ResultUI => resultUI;

}
