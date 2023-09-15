package com.example;

import android.app.Activity;
import android.app.AlarmManager;
import android.app.Application;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.os.Environment;
import android.text.TextUtils;
import android.util.Log;

import androidx.work.ListenableWorker;
import androidx.work.OneTimeWorkRequest;
import androidx.work.PeriodicWorkRequest;
import androidx.work.WorkManager;
import androidx.work.WorkerParameters;

import androidx.appcompat.app.AppCompatActivity;
import androidx.constraintlayout.widget.Constraints;

import com.unity3d.player.UnityPlayer;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.Calendar;
import java.util.concurrent.TimeUnit;

import kotlin.Unit;


public class ReadWriter extends Activity {

    //ファイルから1文だけ読み取る関数
    protected static String readText(String filename) {
        String text = "";
        String text_next = "";
        String fileName = filename;
        Context context = UnityPlayer.currentActivity.getApplicationContext();

        //外部ストレージという名の内部ストレージにあるドキュメントディレクトリにアクセス
        String filepath = String.valueOf(context.getExternalFilesDir(Environment.DIRECTORY_DOCUMENTS));

        File readfile = new File(filepath + "/" + fileName);

        String state = Environment.getExternalStorageState();

        //Environment.MEDIA_MOUNTEDで読み書き処理を可能にする
        if (Environment.MEDIA_MOUNTED.equals(state)) {
            //ファイルの読み書きの処理
            try {
                FileInputStream fis = new FileInputStream(readfile);
                BufferedReader reader = new BufferedReader(new InputStreamReader(fis, "UTF-8"));
                while ((text_next = reader.readLine()) != null) {
                    text = text_next + "\n";
                }
            } catch (FileNotFoundException e) {
                text = e.getMessage();
                e.printStackTrace();

            } catch (IOException e) {
                text = e.getMessage();
                e.printStackTrace();
            }
        }
        return text;
    }

    //ファイル内のテキストデータをループで結合して読み取る関数
    protected static String readTextAll(String filename) {
        String text = "";
        String text_next = "";
        String fileName = filename;
        Context context = UnityPlayer.currentActivity.getApplicationContext();
        String filepath = String.valueOf(context.getExternalFilesDir(Environment.DIRECTORY_DOCUMENTS));

        File readfile = new File(filepath + "/" + fileName);

        String state = Environment.getExternalStorageState();
        if (Environment.MEDIA_MOUNTED.equals(state)) {
            //ファイルの読み書きの処理
            try {
                FileInputStream fis = new FileInputStream(readfile);
                BufferedReader reader = new BufferedReader(new InputStreamReader(fis, "UTF-8"));
                while ((text_next = reader.readLine()) != null) {
                    text += text_next + "\n";
                }
            } catch (FileNotFoundException e) {
                text = e.getMessage();
                e.printStackTrace();

            } catch (IOException e) {
                text = e.getMessage();
                e.printStackTrace();
            }
        }

        Log.d("AlarmManager", text);
        return text;
    }

    protected static String readStepcount() {
        String text = "";
        int stepcount;

        UnitySensorPlugin usp = UnitySensorPlugin.getInstance();
        Log.d("AlarmManager", "usp_readStepcount");
        usp.startSensorListening("stepdetector");
        Log.d("AlarmManager", "usp_startSensorListening_readStepcount");
        stepcount = usp.getStepcount("stepdetector");
        text = "get_" + Integer.toString(stepcount);

        return text;
    }

    //1フォルダ内のファイルリストを取得する関数
    public static String getFileList() {
        String file_bool = "false";
        String filepath_str;
        File file;
        Context context = UnityPlayer.currentActivity.getApplicationContext();

        filepath_str = String.valueOf(context.getExternalFilesDir(Environment.DIRECTORY_DOCUMENTS));
        file = new File(filepath_str);
        File fileArray[] = file.listFiles(); //.listFiles()でフォルダ内のファイルを配列に格納

        if (file.exists() == true) {
            Log.d("AlarmManager", String.valueOf(filepath_str));
        } else if (file.exists() == false) {
            Log.d("AlarmManager", "NoPath");
        }

        if (fileArray != null) {
            Log.d("AlarmManager", "FileDirExist");
        } else if (fileArray == null) {
            Log.d("AlarmManager", "NoFileDir");
        }

        Log.d("AlarmManager", Integer.toString(fileArray.length));

        // ファイルの一覧
        if (fileArray.length == 0) {
            file_bool = "true";
            Log.d("AlarmManager", "file_bool_" + file_bool);
        }

        //.getNameでファイル名を取得していく
        else if (fileArray.length > 0) {
            for (int i = 0; i < fileArray.length; i++) {
                Log.d("AlarmManager", fileArray[i].getName());//ファイル,フォルダを表示
                if (fileArray[i].getName() == "WalkCountLog.txt") {
                    file_bool = "true";
                    Log.d("AlarmManager", "file_bool_" + file_bool);
                }
            }
        }

        return file_bool;
    }

    //ファイルを作成する関数
    public static void createFile(String fileName) throws IOException {
        File file;
        Context context = UnityPlayer.currentActivity.getApplicationContext();
        String filepath = String.valueOf(context.getExternalFilesDir(Environment.DIRECTORY_DOCUMENTS));
        //String fileName = "WalkCountLog.txt";

        file = new File(filepath + "/" + fileName);

        //createNewFileメソッドを使用してファイルを作成する
        if (file.createNewFile()) {
            Log.d("AlarmManager", "create_file_Success");
        } else {
            Log.d("AlarmManager", "create_file_Fail");//ファイル,フォルダを表示
        }
    }

    //設定をファイルに書き込む関数
    public void writeSettingValue(String text) {

        Log.d("AlarmManager", "firstSetting");

        Activity activity = UnityPlayer.currentActivity;
        Context context = activity.getApplicationContext();
        String filepath = String.valueOf(context.getExternalFilesDir(Environment.DIRECTORY_DOCUMENTS));
        String fileName = "SettingValue.txt";
        File file;
        File writefile = new File(filepath + "/" + fileName);
        String state = Environment.getExternalStorageState();

        if (Environment.MEDIA_MOUNTED.equals(state)) {
            try {
                FileOutputStream fos = new FileOutputStream(writefile); //ファイル出力ストリーム作成
                OutputStreamWriter writer = new OutputStreamWriter(fos, "UTF-8"); //書き込みストリーム作成
                PrintWriter pw = new PrintWriter(writer); //書き込みモード使用
                pw.println(text); //書き込み
                pw.close(); //書き込みモード終了
                writer.close(); //書き込みストリーム終了
                fos.close(); //ファイル出力ストリーム終了


            } catch (FileNotFoundException e) {
                text = e.getMessage();
                e.printStackTrace();

            } catch (IOException e) {
                text = e.getMessage();
                e.printStackTrace();
            }
            Log.d("AlarmManager", text);
        }
    }

    //サービスクラスを開始するための関数、今回は一定時間ごとにファイルに歩数を記録するサービスクラスの呼び出し
    public void repeatWriteLog() {
        Log.d("AlarmManager", "StartAlarmManager");
        Activity activity = UnityPlayer.currentActivity;
        Context context = activity.getApplicationContext();
        Intent intent = new Intent(context, StepCounterService.class); //Intent作成
        intent.putExtra("REQUEST_CODE", 1001);
        Log.d("AlarmManager", "Init_rw");

        context.startForegroundService(intent); //作成したIntentを使用してフォアグラウンド(今表示されている画面)でサービス開始
    }
}
