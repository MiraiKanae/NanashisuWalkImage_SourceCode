using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FilterSortingApply : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public System.Action onClickCallback; //ボタン処理用のコールバック

    public string tabname; //タブの名前を取得
    BoolandChangeColor boolandChangeColor; //BoolandChangeColorクラスを取得
    [SerializeField] GameObject filterview_artist; //アーティストフィルターを格納する変数
    [SerializeField] GameObject filterview_album; //アルバムフィルターを格納する変数
    [SerializeField] GameObject filterview_playlist; //プレイリストフィルターを格納する変数
    [SerializeField] GameObject content; //親オブジェクトを格納する変数
    SelectMusicItem selectmusicItem_musicinfo; //SelectMusicItemクラスを取得


    int childCount; //子オブジェクトの数を格納する変数
    GameObject filterItem; //各フィルターを格納する変数

    [SerializeField] List<string> filterlist; //フィルターする要素を格納する変数
    [SerializeField] List<GameObject> nonmusiclist; //表示しない音楽をまとめるリスト
    int roop; //ループ用編素
    [SerializeField] GameObject playstopbutton; //再生・一時停止ボタンを格納する変数
    Change_Play_Stop change_Play_Stop; //Change_Play_Stopクラスの取得
    string filter_string; //表示するためのフィルター名を格納する変数
    [SerializeField] TextMeshProUGUI filter_text; //どのフィルターが適応されているか表示するための変数
    [SerializeField] GameObject filter; //フィルター画面本体

    private void Start()
    {
        filterlist = new List<string>(); //配列変数の初期化
        nonmusiclist = new List<GameObject>(); //配列変数の初期化
        filter_string = "フィルター:"; //適応されているフィルター表示するための変数の初期化
        roop = 0; //ループ回数の初期化
        change_Play_Stop = playstopbutton.GetComponent<Change_Play_Stop>(); //playstopbuttonオブジェクトのChange_Play_Stopクラスを参照
    }

    //ボタンが押された時
    public void OnPointerClick(PointerEventData eventData)
    {
        filterlist = new List<string>(); //配列変数の初期化
        change_Play_Stop.musiclist = new List<GameObject>(); //表示する音楽の配列変数の初期化
        nonmusiclist = new List<GameObject>(); //表示しない音楽の配列変数の初期化
        change_Play_Stop.shuffleList = new List<int>(); //シャッフル音楽リストの初期化
        filter_string = "フィルター:"; //適応されているフィルター表示するための変数の初期化
        roop = 0; //ループ回数の初期化
        change_Play_Stop.roop_cps = 0; //change_Play_Stopクラスの音楽リストループ用の変数の初期化

        //適応させるフィルターを作成
        //3種類のフィルターの掛け方があるため、混ざらないようにオブジェクト名で条件分岐
        if (tabname == "FilterView_TAB_1")
        {
            // 子オブジェクトの数を取得
            childCount = filterview_artist.transform.childCount;

            // 子オブジェクトを順に取得する
            for (int i = 0; i < childCount; i++)
            {
                //子オブジェクトの順番で取得。最初が0で二番目が1となる。つまり↓は最初の子オブジェクト
                filterItem = filterview_artist.transform.GetChild(i).gameObject;
                boolandChangeColor = filterItem.GetComponent<BoolandChangeColor>();
                //Debug.Log(boolandChangeColor.name);
                if (boolandChangeColor.filter_onoff == true)
                {
                    Debug.Log(filterItem.name);
                    filterlist.Add(filterItem.name);
                }
            }
        }
        else if (tabname == "FilterView_TAB_2")
        {
            // 子オブジェクトの数を取得
            childCount = filterview_album.transform.childCount;

            // 子オブジェクトを順に取得する
            for (int i = 0; i < childCount; i++)
            {
                //子オブジェクトの順番で取得。最初が0で二番目が1となる。つまり↓は最初の子オブジェクト
                filterItem = filterview_album.transform.GetChild(i).gameObject;
                boolandChangeColor = filterItem.GetComponent<BoolandChangeColor>();
                //Debug.Log(boolandChangeColor.name);
                if (boolandChangeColor.filter_onoff == true)
                {
                    Debug.Log(filterItem.name);
                    filterlist.Add(filterItem.name);
                }
            }
        }
        else if (tabname == "FilterView_TAB_3")
        {
            // 子オブジェクトの数を取得
            childCount = filterview_playlist.transform.childCount;

            // 子オブジェクトを順に取得する
            for (int i = 0; i < childCount; i++)
            {
                //子オブジェクトの順番で取得。最初が0で二番目が1となる。つまり↓は最初の子オブジェクト
                filterItem = filterview_playlist.transform.GetChild(i).gameObject;
                boolandChangeColor = filterItem.GetComponent<BoolandChangeColor>();
                //Debug.Log(boolandChangeColor.name);
                if (boolandChangeColor.filter_onoff == true)
                {
                    filterlist.Add(filterItem.name);
                }
            }
        }


        //Debug.Log(filterlist[0]);

        //フィルター適応させるループ
        //content内の音楽オブジェクト取得
        
        //子オブジェクトの数を取得
        childCount = content.transform.childCount;

        if (filterlist.Count != 0)
        {
            // 子オブジェクトを順に取得するループ
            for (int i = 0; i < childCount; i++)
            {
                //子オブジェクトの順番で取得。最初が0で二番目が1となる。つまり↓は最初の子オブジェクト
                filterItem = content.transform.GetChild(i).gameObject;
                selectmusicItem_musicinfo = filterItem.GetComponent<SelectMusicItem>();

                for (int n = 0; n < filterlist.Count; n++)
                {
                    //構造体のアーティスト名でフィルター
                    if (selectmusicItem_musicinfo.mi.ArtistName == filterlist[n])
                    {
                        //Debug.Log(filterItem.transform.localPosition);
                        filterItem.transform.localPosition = new Vector2(400.0f, -50f - roop * 100); //フィルターに掛けられた順にオブジェクトの場所を修正
                        Debug.Log(filterItem.transform.localPosition);
                        change_Play_Stop.musiclist.Add(filterItem); //再生リストに情報を渡す
                        nonmusiclist.Remove(filterItem); //非表示音楽リストから削除
                        change_Play_Stop.shuffleList.Add(n); //シャッフル音楽リストに参照番号を追加
                        filterItem.SetActive(true); //非表示にされている可能性があるので表示させる
                        roop += 1;
                        break;
                    }

                    //構造体のアルバム名1でフィルター
                    else if (selectmusicItem_musicinfo.mi.AlbumName_1 == filterlist[n])
                    {
                        filterItem.transform.localPosition = new Vector2(400.0f, -50f - roop * 100);
                        change_Play_Stop.musiclist.Add(filterItem);
                        nonmusiclist.Remove(filterItem);
                        Debug.Log(filterItem.transform.localPosition);
                        change_Play_Stop.shuffleList.Add(n);
                        filterItem.SetActive(true);
                        roop += 1;
                        break;
                    }

                    //構造体のアルバム名2でフィルター
                    else if (selectmusicItem_musicinfo.mi.AlbumName_2 == filterlist[n])
                    {
                        filterItem.transform.localPosition = new Vector2(400.0f, -50f - roop * 100);
                        change_Play_Stop.musiclist.Add(filterItem);
                        nonmusiclist.Remove(filterItem);
                        Debug.Log(filterItem.transform.localPosition);
                        change_Play_Stop.shuffleList.Add(n);
                        filterItem.SetActive(true);
                        roop += 1;
                        break;
                    }

                    //構造体のアルバム名3でフィルター
                    else if (selectmusicItem_musicinfo.mi.AlbumName_3 == filterlist[n])
                    {
                        filterItem.transform.localPosition = new Vector2(400.0f, -50f - roop * 100);
                        change_Play_Stop.musiclist.Add(filterItem);
                        nonmusiclist.Remove(filterItem);
                        Debug.Log(filterItem.transform.localPosition);
                        change_Play_Stop.shuffleList.Add(n);
                        filterItem.SetActive(true);
                        roop += 1;
                        break;
                    }

                    //全てに当てはまらなかった場合は非表示音楽リストに追加
                    if (selectmusicItem_musicinfo.mi.ArtistName != filterlist[n] && selectmusicItem_musicinfo.mi.AlbumName_1 != filterlist[n]
                        && selectmusicItem_musicinfo.mi.AlbumName_2 != filterlist[n] && selectmusicItem_musicinfo.mi.AlbumName_3 != filterlist[n])
                    {
                        if(nonmusiclist.Contains(filterItem) == false)
                        {
                            nonmusiclist.Add(filterItem);
                            //Debug.Log(filterItem.name);
                            //Debug.Log(selectmusicItem_musicinfo.mi.ArtistName);
                            //Debug.Log(selectmusicItem_musicinfo.mi.AlbumName_1);
                            //Debug.Log(selectmusicItem_musicinfo.mi.AlbumName_2);
                            //Debug.Log(selectmusicItem_musicinfo.mi.AlbumName_3);
                        }
                    }
                    Debug.Log(filterItem.name+ "_" + filterlist[n]);
                }
            }

            //非表示音楽リスト内の音楽を非表示にする
            childCount = content.transform.childCount;

            // 子オブジェクトを順に取得する
            for (int m = 0; m < childCount; m++)
            {
                //子オブジェクトの順番で取得。最初が0で二番目が1となる。つまり↓は最初の子オブジェクト
                filterItem = content.transform.GetChild(m).gameObject;

                for (int k = 0; k < nonmusiclist.Count; k++)
                {
                    if (filterItem == nonmusiclist[k])
                    {
                        filterItem.SetActive(false);
                    }

                }
            }

            //適応されているフィルター名を接続
            for (int name = 0; name < filterlist.Count; name++)
            {
                filter_string = filter_string + filterlist[name] + ",";
            }

        }

        //フィルターリセット時、全ての音楽を初期の配置に戻して表示させる
        else if (filterlist.Count == 0)
        {
            childCount = content.transform.childCount;
            Debug.Log(childCount);

            for (int n = 0; n < childCount; n++)
            {
                filterItem = content.transform.GetChild(n).gameObject;
                filterItem.transform.localPosition = new Vector2(400.0f, -50f - n * 100);
                filterItem.SetActive(true);
                change_Play_Stop.musiclist.Add(filterItem); //音楽再生リストに追加
                change_Play_Stop.shuffleList.Add(n); //シャッフル音楽リストに追加(リセット)
            }

            filter_string = "フィルター:なし"; //適応されているフィルターがない場合の表示

        }



        filter_text.text = filter_string; //適応されているフィルターの表示

        filter.SetActive(false); //フィルター画面の非表示

    }
    //機能：ボタンが押される
    public void OnPointerDown(PointerEventData eventData)
    {

    }
    // ボタンが離される
    public void OnPointerUp(PointerEventData eventData)
    {

    }
}
