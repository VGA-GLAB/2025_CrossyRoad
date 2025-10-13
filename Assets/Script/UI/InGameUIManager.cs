using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    [Header("�^�C�g��UI")]
    [SerializeField] private GameObject titleUI;
    [Header("�C���Q�[��UI")]
    [SerializeField] private GameObject inGameUI;
    [Header("���U���gUI")]
    [SerializeField] private GameObject resultUI;

    public GameObject TitleUI => titleUI;
    public GameObject InGameUI => inGameUI;
    public GameObject ResultUI => resultUI;

}
