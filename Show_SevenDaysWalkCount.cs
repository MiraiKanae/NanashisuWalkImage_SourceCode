using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics.Contracts;
using UnityEngine.UI;
using System;

public class GraphBar : MonoBehaviour
{
    private AndroidJavaClass cls_r; //Android StudioのStaticな関数を使用するための変数
    private string oneweekdata; //ファイルに記載された歩数データを一括で取得するための変数

    private string[] data = new string[8]; //8日分の歩数を格納するための配列変数、ファイルから読み込むためString
    private int i; //ループ用の変数


    //グラフ表示用の変数
    [SerializeField] private Image bar_1;
    [SerializeField] private Image bar_2;
    [SerializeField] private Image bar_3;
    [SerializeField] private Image bar_4;
    [SerializeField] private Image bar_5;
    [SerializeField] private Image bar_6;
    [SerializeField] private Image bar_7;

    [SerializeField] private TextMeshProUGUI today_goal; //目標歩数によってグラフのサイズを変更するため目標歩数の現在地を取得するための変数、ドロップダウンから読み込むためString

    private string[] walkcount_relay = new string[8]; //歩数の途中計算を行う際に使用する配列変数
    private int[] walkcount_relay_int = new int[8]; //歩数の途中計算を行う際に使用する配列変数
    private int[] walkcount = new int[8]; //1日の歩数を出す際に引き算をする必要があるので各歩数を格納するための変数

    private int max_walkcount; //目標歩数を数値として使用するための変数

    private float[] ratio = new float[8]; //グラフのサイズを決めるための割合を格納するための変数

    //記録された日付を表示するための変数
    [SerializeField] private TextMeshProUGUI date_1;
    [SerializeField] private TextMeshProUGUI date_2;
    [SerializeField] private TextMeshProUGUI date_3;
    [SerializeField] private TextMeshProUGUI date_4;
    [SerializeField] private TextMeshProUGUI date_5;
    [SerializeField] private TextMeshProUGUI date_6;
    [SerializeField] private TextMeshProUGUI date_7;

    //算出された歩数を表示するための変数
    [SerializeField] private TextMeshProUGUI walk_1;
    [SerializeField] private TextMeshProUGUI walk_2;
    [SerializeField] private TextMeshProUGUI walk_3;
    [SerializeField] private TextMeshProUGUI walk_4;
    [SerializeField] private TextMeshProUGUI walk_5;
    [SerializeField] private TextMeshProUGUI walk_6;
    [SerializeField] private TextMeshProUGUI walk_7;

    [SerializeField] private TextMeshProUGUI oneweekwalk_str; //7日間の総歩数を表示するための変数
    private int oneweekwalk; //7日間の総歩数を表示するための変数

    private string date_relay; //日付をyyyy/mm/dd形式にするための途中計算で使用する変数
    private string mm_str; //日付をyyyy/mm/dd形式にする際の月の値を格納するための変数
    private string dd_str; //日付をyyyy/mm/dd形式にする際の日の値を格納するための変数


    //アプリ開始時の処理
    void Start()
    {
        oneweekwalk = 0;
        Invoke(nameof(GraphBar_Func), 1.0f); //他ソースコードによるデータの取得や初期設定が終わってから処理させたいので若干遅らせている
    }

    //アプリ終了時の処理
    private void OnDestroy()
    {
        // Destroy時に登録したInvokeをすべてキャンセル
        CancelInvoke();
    }

    void GraphBar_Func()
    {
        //Android StudioからreadTextAllの呼び出し
        //ファイルに書かれたテキストデータを一括で取得
        cls_r = new AndroidJavaClass("com.example.ReadWriter");
        oneweekdata = cls_r.CallStatic<string>("readTextAll", "WalkCountLog.txt");
        Debug.Log(oneweekdata);

        //取得したテキストデータを改行(\n)で分けてその場限りの可変配列変数に格納
        string[] arr = oneweekdata.Split("\n", StringSplitOptions.None);
        int arr_length = arr.Length - 1;
        Debug.Log("arr.length_" + (arr.Length).ToString());
        Debug.Log("arr_length_" + arr_length.ToString());


        //取得した歩数データが8個より多ければであればそのまま最後のデータから8個分取得、最初のデータが初期設定用のデータである場合も除くため
        if (arr_length > 8)
        {
            for (i = 0; i < 8; i++)
            {
                data[i] = arr[arr_length - 1 - i];
                Debug.Log(i.ToString() + "_data_main_" + data[i].ToString());
            }
        }
        //取得した歩数データが8個以下であれば取得できた分のデータだけ取得し、残りを19000401で代用する
        else if (arr_length <= 8)
        {
            int n = 0;
            for (i = 7; i >= 8 - arr_length; i--)
            {
                data[i] = arr[n];
                n++;
                Debug.Log(i.ToString() + "_data_19_" + data[i].ToString());
            }
            if (arr_length != 7)
            {
                for (i = 0; i < 8 - arr_length; i++)
                {
                    data[i] = "19000401";
                    Debug.Log(i.ToString() + "_data_19_" + data[i].ToString());
                }
            }
            else if (arr_length == 7)
            {
                data[0] = "19000401";
                Debug.Log(i.ToString() + "_data_19_" + data[0].ToString());

            }

        }

        //最初のデータであり、1番昔の歩数を出すための対象
        //初期設定の際に日付と歩数データが入っているため、日付に1900が入っている場合の条件分岐はなし
        if (data[7].Substring(0, 4) != "1900")
        {
            date_relay = data[7].Substring(4, 4);


            walkcount_relay[7] = data[7].Substring(data[7].IndexOf(" "));
            walkcount_relay[7] = walkcount_relay[7].Trim();
            walkcount_relay_int[0] = int.Parse(walkcount_relay[7]);
        }
        Debug.Log("0_data_" + walkcount[0].ToString());


        //以降、各日の日付と歩数を算出、表示グラフと日付が7個分必要でまとめてセットするやり方が分からなかったので1つずつセット
        if (data[6].Substring(0, 4) != "1900")
        {
            date_relay = data[6].Substring(4, 4);
            mm_str = date_relay.Substring(0, 2);
            dd_str = date_relay.Substring(2, 2);
            date_1.text = mm_str + "/" + dd_str;

            walkcount_relay[6] = data[6].Substring(data[6].IndexOf(" "));
            walkcount_relay[6] = walkcount_relay[6].Trim();
            walkcount_relay_int[1] = int.Parse(walkcount_relay[6]);
            walkcount[1] = walkcount_relay_int[1] - walkcount_relay_int[0];

            walk_1.text = walkcount[1].ToString();
        }
        else if (data[6].Substring(0, 4) == "1900")
        {
            date_1.text = "";
            walkcount_relay[6] = "0";
            walkcount[1] = 0;

            walk_1.text = "";
        }
        Debug.Log("1_data_" + walkcount[1].ToString());

        if (data[5].Substring(0, 4) != "1900")
        {
            date_relay = data[5].Substring(4, 4);
            mm_str = date_relay.Substring(0, 2);
            dd_str = date_relay.Substring(2, 2);
            date_2.text = mm_str + "/" + dd_str;

            walkcount_relay[5] = data[5].Substring(data[5].IndexOf(" "));
            walkcount_relay[5] = walkcount_relay[5].Trim();
            walkcount_relay_int[2] = int.Parse(walkcount_relay[5]);
            walkcount[2] = walkcount_relay_int[2] - walkcount_relay_int[1];

            walk_2.text = walkcount[2].ToString();
        }
        else if (data[5].Substring(0, 4) == "1900")
        {
            date_2.text = "";
            walkcount_relay[5] = "0";
            walkcount[2] = 0;

            walk_2.text = "";
        }
        Debug.Log("2_data_" + walkcount[2].ToString());

        if (data[4].Substring(0, 4) != "1900")
        {
            date_relay = data[4].Substring(4, 4);
            mm_str = date_relay.Substring(0, 2);
            dd_str = date_relay.Substring(2, 2);
            date_3.text = mm_str + "/" + dd_str;

            walkcount_relay[4] = data[4].Substring(data[4].IndexOf(" "));
            walkcount_relay[4] = walkcount_relay[4].Trim();
            walkcount_relay_int[3] = int.Parse(walkcount_relay[4]);
            walkcount[3] = walkcount_relay_int[3] - walkcount_relay_int[2];

            walk_3.text = walkcount[3].ToString();
        }
        else if (data[4].Substring(0, 4) == "1900")
        {
            date_3.text = "";
            walkcount_relay[4] = "0";
            walkcount[3] = 0;

            walk_3.text = "";
        }
        Debug.Log("3_data_" + walkcount[3].ToString());

        if (data[3].Substring(0, 4) != "1900")
        {
            date_relay = data[3].Substring(4, 4);
            mm_str = date_relay.Substring(0, 2);
            dd_str = date_relay.Substring(2, 2);
            date_4.text = mm_str + "/" + dd_str;

            walkcount_relay[3] = data[3].Substring(data[3].IndexOf(" "));
            walkcount_relay[3] = walkcount_relay[3].Trim();
            walkcount_relay_int[4] = int.Parse(walkcount_relay[3]);
            walkcount[4] = walkcount_relay_int[4] - walkcount_relay_int[3];

            walk_4.text = walkcount[4].ToString();
        }
        else if (data[3].Substring(0, 4) == "1900")
        {
            date_4.text = "";
            walkcount_relay[3] = "0";
            walkcount[4] = 0;

            walk_4.text = "";
        }
        Debug.Log("4_data_" + walkcount[4].ToString());

        if (data[2].Substring(0, 4) != "1900")
        {
            date_relay = data[2].Substring(4, 4);
            mm_str = date_relay.Substring(0, 2);
            dd_str = date_relay.Substring(2, 2);
            date_5.text = mm_str + "/" + dd_str;

            walkcount_relay[2] = data[2].Substring(data[2].IndexOf(" "));
            walkcount_relay[2] = walkcount_relay[2].Trim();
            walkcount_relay_int[5] = int.Parse(walkcount_relay[2]);
            walkcount[5] = walkcount_relay_int[5] - walkcount_relay_int[4];

            walk_5.text = walkcount[5].ToString();
        }
        else if (data[2].Substring(0, 4) == "1900")
        {
            date_5.text = "";
            walkcount_relay[2] = "0";
            walkcount[5] = 0;

            walk_5.text = "";
        }
        Debug.Log("5_data_" + walkcount[5].ToString());

        if (data[1].Substring(0, 4) != "1900")
        {
            date_relay = data[1].Substring(4, 4);
            mm_str = date_relay.Substring(0, 2);
            dd_str = date_relay.Substring(2, 2);
            date_6.text = mm_str + "/" + dd_str;

            walkcount_relay[1] = data[1].Substring(data[1].IndexOf(" "));
            walkcount_relay[1] = walkcount_relay[1].Trim();
            walkcount_relay_int[6] = int.Parse(walkcount_relay[1]);
            walkcount[6] = walkcount_relay_int[6] - walkcount_relay_int[5];

            walk_6.text = walkcount[6].ToString();
        }
        else if (data[1].Substring(0, 4) == "1900")
        {
            date_6.text = "";
            walkcount_relay[1] = "0";
            walkcount[6] = 0;
            
            walk_6.text = "";
        }
        Debug.Log("6_data_" + walkcount[6].ToString());

        if (data[0].Substring(0, 4) != "1900")
        {
            date_relay = data[0].Substring(4, 4);
            mm_str = date_relay.Substring(0, 2);
            dd_str = date_relay.Substring(2, 2);
            date_7.text = mm_str + "/" + dd_str;

            walkcount_relay[0] = data[0].Substring(data[0].IndexOf(" "));
            walkcount_relay[0] = walkcount_relay[0].Trim();
            walkcount_relay_int[7] = int.Parse(walkcount_relay[0]);
            walkcount[7] = walkcount_relay_int[7] - walkcount_relay_int[6];

            walk_7.text = walkcount[7].ToString();
        }
        else if (data[0].Substring(0, 4) == "1900")
        {
            date_7.text = "";
            walkcount_relay[0] = "0";
            walkcount[7] = 0;

            walk_7.text = "";
        }
        Debug.Log("7_data_" + walkcount[7].ToString());

        //目標歩数の値を取得
        max_walkcount = int.Parse(today_goal.text);
        Debug.Log(max_walkcount);

        //グラフをの表示割合を算出
        for (i = 1; i < 8; i++)
        {
            ratio[i] = (float)walkcount[i] / (float)max_walkcount;
            Debug.Log(ratio[i].ToString());
        }
        //Debug.Log(walkcount);
        //Debug.Log(subration);

        //グラフの表示割合を適応
        bar_1.fillAmount = ratio[1];
        bar_2.fillAmount = ratio[2];
        bar_3.fillAmount = ratio[3];
        bar_4.fillAmount = ratio[4];
        bar_5.fillAmount = ratio[5];
        bar_6.fillAmount = ratio[6];
        bar_7.fillAmount = ratio[7];
        //Debug.Log(hidebar.sizeDelta.x);
    
    
        //7日分の合計を算出・表示
        for(i=1;i<8;i++)
        {
            oneweekwalk += walkcount[i];
        }

        oneweekwalk_str.text = "7日の合計\n" + oneweekwalk.ToString();


    }
}
