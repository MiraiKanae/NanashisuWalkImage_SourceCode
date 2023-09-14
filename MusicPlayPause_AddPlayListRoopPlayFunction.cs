using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Change_Play_Stop : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public System.Action onClickCallback; //ボタン処理用のコールバック

    [SerializeField] AudioSource audio_source; //AudioSauceを格納する変数
    [SerializeField] float audiovolume; //音楽のボリュームを決める変数

    [SerializeField] Slider seekbar; //音楽の再生状況が確認できるスライダーを格納する変数
    [SerializeField] TextMeshProUGUI music_now_time; //音楽の現在の再生時間を表示する変数

    [SerializeField] GameObject change_object; //再生と一時停止を行うオブジェクトを格納する変数
    [SerializeField] Image change_image; //再生と一時停止を交互に表示できるようにするための変数
    [SerializeField] Sprite play_image; //再生ボタンのイメージを格納する変数
    [SerializeField] Sprite stop_image; //一時停止ボタンのイメージを格納する変数

    public float playTime; //現在の再生時間を取得する変数
    public float fullTime; //音楽の終了時間を取得する変数


    private bool playstart; //音楽を最初にスタートさせたか一時停止からスタートさせたかを判断させる変数

    [SerializeField] Image change_image_rs; //ループの表示を変えられる変数
    [SerializeField] Sprite roop_image; //プレイリストループのイメージをを格納する変数
    [SerializeField] Sprite shuffle_image; //プレイリストシャッフルループのイメージを格納する変数
    GameObject musicItem; //音楽の情報を持つオブジェクトを格納する変数
    SelectMusicItem selectmusicitem; //SelectMusicItemクラスの取得
    PointerEventData pointer; //PointerEventDataクラスの取得
    public int roop_cps; //現在のループ位置を把握するための変数
    public List<GameObject> musiclist; //ループに渡す用の音楽リスト
    [SerializeField] GameObject content; //プレイリストを作成するための親オブジェクトを格納する変数
    int childCount; //子オブジェクトの個数をカウントする変数
    GameObject contentItem; //子オブジェクトを格納する変数
    public List<int> shuffleList; //プレイリストシャッフルループ用のリスト



    public void Start()
    {
        //音楽のボリュームを0,1表示にして最大にする
        audiovolume = Math.Clamp(audiovolume, 0.0f, 1.0f);
        audiovolume = 1.0f;

        //始めはPlayで音楽を再生できるように初期化
        playstart = false;

        //音楽の終了時間と現在時間の初期化
        fullTime = 0.0f;
        playTime = 0.0f;

        //プレイリストの作成(初期化)
        childCount = content.transform.childCount;
        Debug.Log(childCount);
        musiclist = new List<GameObject>();
        shuffleList = new List<int>();
        for (int n = 0; n < childCount; n++)
        {
            contentItem = content.transform.GetChild(n).gameObject;
            musiclist.Add(contentItem);
            Debug.Log(musiclist[n]);
            shuffleList.Add(n);
        }

        //ループ再生の位置を初期化
        roop_cps = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //現在の再生時間を取得
        playTime = audio_source.time;

        //1秒ごとに現在の再生時間の表示を修正
        InvokeRepeating("NowTime", 0, 1);

        //現在の再生時間が0秒でない場合はスライダーの位置に反映
        if (playTime != 0.0f)
        {
            seekbar.value = playTime / fullTime;
        }

        //Debug.Log("playTime_" + audio_source.time);

        //現在の再生時間が終了時間と一緒になった場合、かつ、音楽がセットされている場合は
        //再生・一時停止ボタンの表示を再生にして次の音楽を呼び出す
        if(playTime == fullTime && audio_source.clip != null)
        {
            change_image.sprite = play_image;

            MusicEnd();
        }
    }

    //音楽の再生時間の表示をmm:ssになるように調整
    public void NowTime()
    {
        int playnowtime = ((int)playTime % 60);
        if(playnowtime < 10)
        {
            music_now_time.text = "0" + ((int)playTime / 60) + ":0" + ((int)playTime % 60);
        }
        else if(playnowtime >= 10)
        {
            music_now_time.text = "0" + ((int)playTime / 60) + ":" + ((int)playTime % 60);
        }

    }

    //ボタンを1回押したときの処理
    public void OnPointerClick(PointerEventData eventData)
    {
        onClickCallback?.Invoke(); //クリックした時のコールバック

        //再生・一時停止ボタンの表示が一時停止になっていたとき(つまり、音楽再生中の時)
        //ボタンの表示を再生のイメージにして音楽を一時停止する
        if(change_image.sprite == stop_image)
        {
            change_image.sprite = play_image;
            audio_source.Pause();
            Debug.Log("Pause");
        }

        //再生・一時停止ボタンの表示が再生になっていたとき(つまり、一時停止中の時)
        //ボタンの表示を一時停止のイメージにして音楽を再生する
        else if (change_image.sprite == play_image)
        {
            change_image.sprite = stop_image;
            audio_source.time = seekbar.value * fullTime; //スライダーの位置から音楽を再生し直す
            Debug.Log("audio_source_Time_"+ audio_source.time);

            //初めて再生した場合はPlayを使用し、以降はUnPauseを使用して音楽を再生する
            if (playstart == false)
            {
                playstart = true;
                audio_source.Play();
                Debug.Log("playstart");
            }
            else if (playstart == true)
            {
                audio_source.UnPause();
                Debug.Log("UnPause");
            }

        }
    }
    //機能：ボタンが押される
    public void OnPointerDown(PointerEventData eventData)
    {
        change_image.color = new Color32(200, 200, 200, 255);
    }
    // ボタンが離される
    public void OnPointerUp(PointerEventData eventData)
    {
        change_image.color = new Color32(255, 255, 255, 255);
    }

    //スライダーが音楽の再生時刻に合わせて動くように調整
    //使用者の任意の位置にも動かせる
    public void Seek_ChangeValue()
    {
        //Debug.Log("seekbar.value_" + seekbar.value);
        audio_source.time = seekbar.value * fullTime;
    }

    //音楽が1曲分狩猟した後の処理
    public void MusicEnd()
    {
        //プレイリストループ再生の場合、次の音楽を再生
        if (change_image_rs.sprite == roop_image)
        {
            roop_cps += 1;
            Debug.Log(roop_cps);
            if (roop_cps == musiclist.Count)
            {
                roop_cps = 0;
            }
        }

        //プレイリストシャッフルループの場合、
        //shuffleListから現在のプレイリストに対応する数字をランダムに算出し、
        //その後、音楽を再生
        //重複なしランダムを使用
        else if (change_image_rs.sprite == shuffle_image)
        {
            shuffleList.Remove(roop_cps); //再生終了した音楽に対応する数字をshuffleListから削除
            Debug.Log(shuffleList.Count);
            roop_cps = shuffleList[UnityEngine.Random.Range(0, shuffleList.Count)]; //shuffleListに残った数字からランダムに数字を1つ選ぶ
                                                                                    //shuffleListの配列の要素番号をランダムにすることで
                                                                                    //要素番号=要素の内容という構造を作ることなくランダムな取り出しが可能になっている
                                                                                    
            Debug.Log(roop_cps);
            shuffleList.Remove(roop_cps); //これから再生する音楽に対応する数字をshuffleListから削除
            Debug.Log("Remove");
            
            if (shuffleList.Count == 0)//shuffleListの中身が空になった場合
            {
                for (int n = 0; n < musiclist.Count; n++)//プレイリストと同じ数になるまでshuffleListの中身を増やす
                {
                    shuffleList.Add(n);
                }
                shuffleList.Remove(roop_cps); //これから再生する音楽に対応する数字をshuffleListから削除
                Debug.Log("Remove_for");
            }
        }
        musicItem = musiclist[roop_cps]; //選ばれた音楽を再生
        Debug.Log("musicItem");
        selectmusicitem = musicItem.GetComponent<SelectMusicItem>(); //musicItemクラスを取得
        Debug.Log("getcomponent");
        selectmusicitem.OnPointerClick(pointer); //musicItemクラスにある音楽再生機能を強制使用
        Debug.Log("onpointer");
    }

    //プレイリストの1つ前の音楽を再生する機能(プレイリストの1つ前の音楽を再生するボタンから呼び出す用に記載)
    //内容は音楽終了時とほぼ同じ
    //理由：再生リストとプレイリストシャッフルループ用リストを管理しているのはこのクラスであるため
    public void MusicBack()
    {
        if (change_image_rs.sprite == roop_image)
        {
            if (roop_cps == 0)
            {
                roop_cps = musiclist.Count;
            }
            roop_cps -= 1;
            Debug.Log(roop_cps);
        }
        else if (change_image_rs.sprite == shuffle_image)
        {
            Debug.Log(shuffleList.Count);
            roop_cps = shuffleList[UnityEngine.Random.Range(0, shuffleList.Count)];
            Debug.Log(roop_cps);
            shuffleList.Remove(roop_cps);
            Debug.Log("Remove");
            if (shuffleList.Count == 0)
            {
                for (int n = 0; n < musiclist.Count; n++)
                {
                    shuffleList.Add(n);
                }
                shuffleList.Remove(roop_cps);
                Debug.Log("Remove_for");
            }
        }
        musicItem = musiclist[roop_cps];
        Debug.Log("musicItem");
        selectmusicitem = musicItem.GetComponent<SelectMusicItem>();
        Debug.Log("getcomponent");
        selectmusicitem.OnPointerClick(pointer);
        Debug.Log("onpointer");
    }

}
