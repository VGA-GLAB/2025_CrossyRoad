using System.Collections.Generic;
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
    
    [Header("鳴らす距離")] 
    [SerializeField] private float minDistance;

    [Header("プレイヤー")]
    [SerializeField] private GameObject player;
    
    private Dictionary<int, AudioSource> obstacleSourceSE  = new Dictionary<int, AudioSource>();
    
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

    /*
    /// <summary>
    /// SE再生
    /// </summary>
    /// <param name="clipName">鳴らしたいSEの名前</param>
    public void PlaySE(string clipName)
    {
        //一致したSEを再生する
        foreach(var sound in sounds)
        {
            if (sound.name == clipName)
            {
                audioSourceSE.PlayOneShot(sound.clip);
            }
        }
    }
    */

    /*
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
    */

    /// <summary>
    /// 障害物用のSE再生
    /// </summary>
    /// <param name="clipName">鳴らすSE</param>
    /// <param name="obstacle">障害物</param>
    public void ObstaclePlaySE(string clipName, GameObject obstacle)
    {
        //障害物とプレイヤーの距離が鳴らす範囲外なら処理をしない
        var distance =  Vector3.Distance(obstacle.transform.position, player.transform.position);
        if(distance > minDistance) return;

        foreach (var sound in sounds)
        {
            if (sound.name == clipName)
            {
                var id = obstacle.transform.GetInstanceID();
                
                //新規AudioSource生成
                var sourceGO = new GameObject($"LoopSource_{id}_{clipName}");
                sourceGO.transform.SetParent(transform);

                var source = sourceGO.AddComponent<AudioSource>();
                source.clip = sound.clip;
                source.loop = true;
                source.playOnAwake = false;
                source.volume = 0.5f;

                obstacleSourceSE[id] = source;
                source.Play();
            }
        }
    }

    /// <summary>
    /// 障害物のSE再生
    /// </summary>
    /// <param name="obstacle">障害物</param>
    public void ObstacleStopSE(GameObject obstacle)
    {
        if(!obstacleSourceSE.ContainsKey(obstacle.transform.GetInstanceID())) return;
        
        var id =  obstacle.transform.GetInstanceID();
        obstacleSourceSE[id].Stop();
        
        //AudioSourceを削除
        Destroy(obstacleSourceSE[id]);
        
        //削除
        obstacleSourceSE.Remove(id);
        
    }

    /// <summary>
    /// プレイヤーとの距離を判定する
    /// </summary>
    /// <param name="obstacle">障害物</param>
    /// <returns>範囲外かを返す</returns>
    public bool PlayerDistance(GameObject obstacle)
    {
        //障害物とプレイヤーの距離が鳴らす範囲外なら処理をしない
        var distance =  Vector3.Distance(obstacle.transform.position, player.transform.position);
        if(distance > minDistance) return true;
        
        return false;
    }
}
