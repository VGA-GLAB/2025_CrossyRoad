using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundData
    {
        public string name;
        public AudioClip clip;
    }

    public static SoundManager instance;

    [Header("�Đ����������Ɩ��O")]
    [SerializeField] private SoundData[] sounds;

    [Header("SE�p")]
    [SerializeField] private AudioSource audioSourceSE;
    [Header("BGM�p")]
    [SerializeField] private AudioSource audioSourceBGM;


    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(audioSourceSE != null)
        {
            audioSourceSE = GetComponent<AudioSource>();
        }
        if (audioSourceBGM != null)
        {
            audioSourceBGM = GetComponent<AudioSource>();
        }
    }

    public void PlaySE(string clipName) //SE���Đ�����
    {
        //�Đ��������T�E���h�̖��O�ƃ��X�g�ɂ���T�E���h�̖��O����v������Đ�����
        foreach(var sound in sounds)
        {
            if(sound.name == clipName)
            {
                audioSourceSE.PlayOneShot(sound.clip);
            }
        }
    }

    public void PlayBGM(string clipName) //BGM���Đ�����
    {
        foreach (var sound in sounds)
        {
            if (sound.name == clipName)
            {
                audioSourceBGM.clip = sound.clip;
            }
        }
    }

    public void StopBGM() //���ݗ���Ă���BGM���~�߂�
    {
        audioSourceBGM.clip = null;
    }
}
