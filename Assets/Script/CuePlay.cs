using System.Collections;
using UnityEngine;
using CriWare;

public class CuePlay : MonoBehaviour
{
    public static CuePlay instance;
    
    private GameObject player;

    [Header("鳴らす距離")] 
    [SerializeField] private float minDistance;
    
    private CriAtomExPlayer criAtomExPlayer;
    private CriAtomExPlayer bgmCriAtomExPlayer;
    private CriAtomEx.CueInfo[] cueInfos;
    private CriAtomExAcb criAtomExAcb;
    

    //再生したサウンドを保持しておく
    //private Dictionary<int, CriAtomSource> sounds = new Dictionary<int, CriAtomSource>();
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            
            //生成
            criAtomExPlayer = new CriAtomExPlayer();
            bgmCriAtomExPlayer = new CriAtomExPlayer();
        }
        else
        {
            Destroy(gameObject);
        }
        
        player =  GameObject.FindGameObjectWithTag("Player");
    }

    IEnumerator Start()
    {
        //キューシートファイルのロード待ち
        while (CriAtom.CueSheetsAreLoading) {
            yield return null;
        }
        
        //Cueの情報を取得
        criAtomExAcb = CriAtom.GetAcb("CueSheet_0");
        cueInfos = criAtomExAcb.GetCueInfoList();
        
        //タイトルのBGMを再生する
        PlayBGM("BGM_Title");
    }

    /// <summary>
    /// SEを再生する
    /// </summary>
    /// <param name="name">CueSheetの名前</param>
    public void PlaySE(string name)
    {
        for (int i = 0; i < cueInfos.Length; i++)
        {
            //インデックスが一致したら音を鳴らす
            if (cueInfos[i].name == name)
            {
                criAtomExPlayer.SetCue(criAtomExAcb, cueInfos[i].name);
                criAtomExPlayer.Start();
                break;
            }
        }
    }
    
    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="clipName">鳴らしたいBGMの名前</param>
    public void PlayBGM(string clipName)
    {
        for (int i = 0; i < cueInfos.Length; i++)
        {
            //インデックスが一致したら音を鳴らす
            if (cueInfos[i].name == clipName)
            {
                //bgmCriAtomExPlayer.Stop();
                bgmCriAtomExPlayer.SetCue(criAtomExAcb, cueInfos[i].name);
                bgmCriAtomExPlayer.Loop(true);
                bgmCriAtomExPlayer.Start();
                Debug.Log("Trying to play BGM: " + clipName);
                break;
            }
        }
    }

    /// <summary>
    /// BGMを止める
    /// </summary>
    public void StopBGM()
    {
        bgmCriAtomExPlayer.Loop(false);
        bgmCriAtomExPlayer.Stop();
    }

    
    //ここから下はＣＲＩの実装
    //なぜか、ロボットのＳＥが再生されない
    //ＣＲＩは一部だけに使用する

    /// <summary>
    /// 障害物のSEを再生する
    /// </summary>
    /// <param name="name">CueSheetの名前</param>
    public void ObstaclePlaySE(string name, GameObject obstacle)
    {
        if (obstacle == null)
        {
            Debug.Log(obstacle.name);
            return;
        }
        
        //障害物とプレイヤーの距離が鳴らす範囲外なら処理をしない
        var distance =  Vector3.Distance(obstacle.transform.position, player.transform.position);
        if(distance > minDistance) return;
        
        foreach (var cue in cueInfos)
        {
            //名前と一致したら
            if (cue.name == name)
            {
                var id = obstacle.transform.GetInstanceID();
                
                /*
                //プレイヤーを生成し、SEを再生する
                var cri = new CriAtomExPlayer();
                cri.SetCue(criAtomExAcb, cue.name);
                cri.Loop(true);
                cri.Start();

                //保持しておく
                sounds[id] = cri;
                break;
                */
            }
        }
    }

    /// <summary>
    /// 障害物のSEを停止する
    /// </summary>
    public void ObstacleStopSE(GameObject obstacle)
    {
        /*
        //IDがなかったら処理をしないようにする
        if(!sounds.ContainsKey(obstacle.transform.GetInstanceID())) return;
        
        var id =  obstacle.transform.GetInstanceID();
        sounds[id].Stop();
        
        //playerを削除する
        Destroy(sounds[id]);
        
        //削除
        sounds.Remove(id);
        */
    }

    /// <summary>
    /// プレイヤーと障害物の距離を判定する
    /// </summary>
    /// <param name="obstacle">障害物</param>
    /// <returns>true：範囲内　false：範囲外</returns>
    public bool ObstacleDistance(GameObject obstacle)
    {
        var distance = Vector3.Distance(obstacle.transform.position, player.transform.position);
        if (distance < minDistance) return true;
        
        return false;
    }
}
