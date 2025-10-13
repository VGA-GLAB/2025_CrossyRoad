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

    [Header("再生したい音と名前")]
    [SerializeField] private SoundData[] sounds;

    [Header("SE用")]
    [SerializeField] private AudioSource audioSourceSE;
    [Header("BGM用")]
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

    public void PlaySE(string clipName) //SEを再生する
    {
        //再生したいサウンドの名前とリストにあるサウンドの名前が一致したら再生する
        foreach(var sound in sounds)
        {
            if(sound.name == clipName)
            {
                audioSourceSE.PlayOneShot(sound.clip);
            }
        }
    }

    public void PlayBGM(string clipName) //BGMを再生する
    {
        foreach (var sound in sounds)
        {
            if (sound.name == clipName)
            {
                audioSourceBGM.clip = sound.clip;
            }
        }
    }

    public void StopBGM() //現在流れているBGMを止める
    {
        audioSourceBGM.clip = null;
    }
}
