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

    [Header("再生サウンド")]
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

    /// <summary>
    /// SE再生
    /// </summary>
    /// <param name="clipName">鳴らしたいSEの名前</param>
    public void PlaySE(string clipName)
    {
        //一致したSEを再生する
        foreach(var sound in sounds)
        {
            if(sound.name == clipName)
            {
                audioSourceSE.PlayOneShot(sound.clip);
            }
        }
    }

    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="clipName">鳴らしたいBGMの名前</param>
    public void PlayBGM(string clipName)
    {
        foreach (var sound in sounds)
        {
            if (sound.name == clipName)
            {
                audioSourceBGM.clip = sound.clip;
                audioSourceBGM.Play();
                return;
            }
        }
    }

    /// <summary>
    /// BGMを止める
    /// </summary>
    public void StopBGM()
    {
        audioSourceBGM.clip = null;
        audioSourceBGM.Stop();
    }
}
