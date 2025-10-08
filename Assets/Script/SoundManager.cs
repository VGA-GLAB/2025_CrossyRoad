using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
    }

    public static SoundManager instance;

    [Header("çƒê∂ÇµÇΩÇ¢âπÇ∆ñºëO")]
    [SerializeField] Sound[] sounds;

    private AudioSource audioSourceSE;
    private AudioSource audioSourceBGM;

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
        audioSourceSE = GetComponent<AudioSource>();
        audioSourceBGM = GetComponent<AudioSource>();
    }

    public void PlaySE(string clipName)
    {
        foreach(var sound in sounds)
        {
            if(sound.name == clipName)
            {
                audioSourceSE.PlayOneShot(sound.clip);
            }
        }
    }

    public void PlayBGM(string clipName)
    {
        foreach (var sound in sounds)
        {
            if (sound.name == clipName)
            {
                audioSourceBGM.clip = sound.clip;
            }
        }
    }

    public void StopBGM(string clipName)
    {
        audioSourceBGM.clip = null;
    }
}
