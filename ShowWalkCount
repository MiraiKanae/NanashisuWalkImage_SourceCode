using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

//歩数計機能と目標歩数(ドロップダウン)の設定
public class WalkCounter_CS : MonoBehaviour
{
    string readWalkcount_Now; //Androidで取得している現在の総歩数を格納する変数、ファイルに記録した情報を取得するのでString
    string readWalkcount_yesterday; //Androidで取得している昨日までの総歩数を格納する変数、ファイルに記録した情報を取得するのでString
    string readWalkcount_relay; //上2つの加工をする際に仕様、自由に使う

    int readWalkcount_N; //Androidで取得している現在の総歩数を格納する変数、昨日の歩数と引き算するのでint
    int readWalkcount_Y; //Androidで取得している昨日の総歩数を格納する変数、今日の歩数と引き算するのでint
    int readWalkcount_S; //実際に表示される歩数を格納する変数


    private AndroidJavaObject plugin; //StaticではないAndroid Studio側の関数を使用するための変数
    private AndroidJavaClass cls_r; //StaticのAndroid Studio側の関数を使用するための変数

    [SerializeField] TextMeshProUGUI showingWalkcount; //歩数を画面に表示するためのTextMeshPro

    string logbool; //初期化をするかしないか決める変数、ファイルに記録された値で判定するのでString

    float time; //歩数の表示を反映するまでの間隔

    [SerializeField] private TMP_Dropdown today_goal; //現在の目標歩数を決定するドロップダウン
    string setting_relay; //前回アプリを閉じる前に保存したドロップダウンの値を取得する変数

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("LogWrite");

        //Android StudioのgetFileListの呼び出し
        //歩数を記録するファイルの有無を確認し、初期設定の有無を判定する
        using (AndroidJavaClass cls_r = new AndroidJavaClass("com.example.ReadWriter"))
        {
            logbool = cls_r.CallStatic<string>("getFileList");
        }


        //歩数を記録するファイルが無い場合(true)に初期設定を行う
        if (logbool == "true")
        {
            //Android StudioのwriteSettingValueの呼び出し
            //歩数を記録するWalkCountLog.txtと各設定を記録するSettingValue.txtの作成
            using (AndroidJavaClass cls_cf = new AndroidJavaClass("com.example.ReadWriter"))
            {
                cls_cf.CallStatic("createFile", "WalkCountLog.txt");
                cls_cf.CallStatic("createFile", "SettingValue.txt");
            }

            //Android StudioのcreateFileの呼び出し
            //初期設定の記録、ドロップダウンの値など
            using (AndroidJavaObject cls_rr = new AndroidJavaObject("com.example.ReadWriter"))
            {
                Debug.Log("SaveText_True");
                cls_rr.Call("writeSettingValue","0,0");
            }

            Debug.Log("Today_notEqual_Logday");

            //Android StudioのrepeatWriteLogの呼び出し
            //現在の歩数を記録
            using (AndroidJavaObject cls_rt = new AndroidJavaObject("com.example.ReadWriter"))
            {
                Debug.Log("AndroidJavaObject_True");
                cls_rt.Call("repeatWriteLog");
            }
        }

        //AndoroidStudioのreadTextの呼び出し
        //Dropdownの初期化、設定を保存したファイルの1番目の値を指定してドロップダウンに反映させる
        using (AndroidJavaClass cls_rset = new AndroidJavaClass("com.example.ReadWriter"))
        {
            setting_relay = cls_rset.CallStatic<string>("readText", "SettingValue.txt");
        }
        Debug.Log("setting_relay_" + setting_relay);
        Debug.Log("null_check_" + setting_relay.Split(",", StringSplitOptions.RemoveEmptyEntries));
        string[] setarr = setting_relay.Split(",", StringSplitOptions.RemoveEmptyEntries);
        Debug.Log("setting_relay_split" + setarr[0]);
        today_goal.value = int.Parse(setarr[0]);


        //Android StudioのstartSensorListeningの呼び出し
        //歩数計の初期化、ファイルに記録した最後の歩数を取得し現在の歩数と引き算をする
        plugin = new AndroidJavaClass("com.example.UnitySensorPlugin").CallStatic<AndroidJavaObject>("getInstance");
        plugin.Call("startSensorListening", "stepcounter");
        if (plugin != null)
        {
            float[] sensorValue = plugin.Call<float[]>("getSensorValues", "stepcounter");
            if (sensorValue != null)
            {
                Debug.Log("sensorValue:" + string.Join(",", new List<float>(sensorValue).ConvertAll(i => i.ToString()).ToArray()));
                readWalkcount_Now = string.Join(",", new List<float>(sensorValue).ConvertAll(i => i.ToString()).ToArray());
            }
        }
        else
        {
            readWalkcount_Now = "0";
        }

        cls_r = new AndroidJavaClass("com.example.ReadWriter");
        readWalkcount_relay = cls_r.CallStatic<string>("readText", "WalkCountLog.txt");


        Debug.Log("AlarmManager" + readWalkcount_relay);

        readWalkcount_yesterday = readWalkcount_relay.Substring(readWalkcount_relay.IndexOf(" "));
        readWalkcount_yesterday = readWalkcount_yesterday.Trim();

        Debug.Log("AlarmManager" + readWalkcount_yesterday);
        readWalkcount_Y = int.Parse(readWalkcount_yesterday);

        readWalkcount_N = int.Parse(readWalkcount_Now);
        readWalkcount_S = readWalkcount_N - readWalkcount_Y;
        showingWalkcount.text = readWalkcount_S.ToString();
        Debug.Log(readWalkcount_S);
    }


    //3秒ごとに画面に表示されている歩数を更新
    private void Update()
    {
        if (time >= 3f)
        {
            time = 0f;

#if UNITY_ANDROID
            if (plugin != null)
            {
                float[] sensorValue = plugin.Call<float[]>("getSensorValues", "stepcounter");
                if (sensorValue != null)
                {
                    //Debug.Log("sensorValue:" + string.Join(",", new List<float>(sensorValue).ConvertAll(i => i.ToString()).ToArray()));
                    readWalkcount_Now = string.Join(",", new List<float>(sensorValue).ConvertAll(i => i.ToString()).ToArray());
                }
            }
            else
            {
                readWalkcount_Now = "0";
            }
#endif

            readWalkcount_N = int.Parse(readWalkcount_Now);
            readWalkcount_S = readWalkcount_N - readWalkcount_Y;
            //Debug.Log(readWalkcount_S);
            showingWalkcount.text = readWalkcount_S.ToString();

        }
        else if (time < 3f)
        {
            time += Time.deltaTime;
        }

    }

    //Android Studioからterminate呼び出し
    //アプリ終了処理
    void OnApplicationQuit()
    {
#if UNITY_ANDROID
        if (plugin != null)
        {
            plugin.Call("terminate");
            plugin = null;
        }
#endif
    }
}
