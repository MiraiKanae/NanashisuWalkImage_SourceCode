package com.example;

import android.app.Activity;
import android.app.AlarmManager;
import android.app.IntentService;
import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.graphics.Color;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.os.Environment;
import android.os.IBinder;
import android.util.Log;

import androidx.annotation.Nullable;
import androidx.core.app.JobIntentService;

import com.example.unitylibrary.R;
import com.unity3d.player.UnityPlayer;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.util.Calendar;

public class StepCounterService extends Service {

    private Context context;
    private SensorManager mSensorManager;
    private Sensor mStepConterSensor;
    private String sensorValue_str;
    private String date_s;
    private boolean flag_switch;


    //今回はバインドしない
    @Nullable
    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    //サービスクラスの初期化
    @Override //Bundle savedInstanceState
    public void onCreate() {
        super.onCreate();

        //いつでも処理できるようにUnityじゃないContextを取得
        context = getApplicationContext();
        Log.d("AlarmManager", "Context_s");

        //センサーマネージャを取得
        mSensorManager = (SensorManager) getSystemService(SENSOR_SERVICE);

        //センサマネージャから TYPE_STEP_COUNTER についての情報を取得する
        mStepConterSensor = mSensorManager.getDefaultSensor(Sensor.TYPE_STEP_COUNTER);
        flag_switch = false;
    }

    //サービスの内容開始
    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {

        Log.d("AlarmManager", "Start_s");

        //2周回す用の条件分岐
        //1周目はデータ取得用の条件分岐
        if (flag_switch == false)
        {
            date_s = setDate();
            sensorValue_str = setCount();
            Log.d("AlarmManager", "getParams_s_" + date_s + "_" + sensorValue_str);
        }

        //2周目は記録とアラームの設定用の条件分岐
        else if (flag_switch == true)
        {
            saveText(context, date_s, sensorValue_str);
            Log.d("AlarmManager", "Finished_WriteOpe_s");
        }

        //削除不可の通知作成、これによって歩数を取得しつつアラーム機能も使える
        int requestCode = intent.getIntExtra("REQUEST_CODE", 0);

        String channelId = "default";
        String title = context.getString(R.string.app_name);

        PendingIntent pendingIntent_not =
                PendingIntent.getActivity(context, requestCode,
                        intent, PendingIntent.FLAG_UPDATE_CURRENT);

        // ForegroundにするためNotificationが必要、Contextを設定
        NotificationManager notificationManager
                = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);

        NotificationChannel channel = new NotificationChannel(
                channelId, title, NotificationManager.IMPORTANCE_DEFAULT);
        channel.setDescription("Silent Notification");
        // 通知音を消さないと毎回通知音が出てしまう、この辺りの設定はcleanにしてから変更
        channel.setSound(null, null);
        // 通知ランプを消す、常にある通知であるため、ONにしていると常にランプが光ってしまう
        channel.enableLights(false);
        channel.setLightColor(Color.BLUE);
        // 通知バイブレーション無し、常にある通知であるため、ONにしていると常にバイブレーションが起こってしてしまう
        channel.enableVibration(false);

        Log.d("AlarmManager", "Finished_setNotification_s");

        if (notificationManager != null) {
            notificationManager.createNotificationChannel(channel);
            Notification notification = new Notification.Builder(context, channelId)
                    .setContentTitle(title)
                    // android標準アイコンから
                    .setSmallIcon(android.R.drawable.btn_star)
                    .setContentText("Alarm Counter")
                    .setAutoCancel(true)
                    .setContentIntent(pendingIntent_not)
                    .setWhen(System.currentTimeMillis())
                    .build();

            // startForegroundで通知起動
            startForeground(1001, notification);
            Log.d("AlarmManager", "Finished_Notification_s");
        }

        if(flag_switch == false)
        {
            // 毎回Alarmを設定する
            setNextAlarmService(context);
            Log.d("AlarmManager", "Finished_setNextAlarmService_s");
        }
        else if (flag_switch == true)
        {
            setSecondAlarmService(context);
            Log.d("AlarmManager", "Finished_setSecondAlarmService_s");
        }

        //Log.d("AlarmManager",intent.getStringExtra("CHECK_CODE"));

        //サービスの再起動が起きないようにSTART_NOT_STICKYを設定
        return Service.START_NOT_STICKY;
    }

    //現在の日付を取得する関数
    public String setDate() {
        Calendar calendar_r = Calendar.getInstance();
        int yyyy = calendar_r.get(Calendar.YEAR);
        int mm = calendar_r.get(Calendar.MONTH) + 1;
        int dd = calendar_r.get(Calendar.DAY_OF_MONTH);
        String yyyy_str = Integer.toString(yyyy);
        String date;

        String mm_str = Integer.toString(mm);
        if (mm_str.length() == 1) {
            mm_str = "0" + mm_str;
        }

        String dd_str = Integer.toString(dd);
        if (dd_str.length() == 1) {
            dd_str = "0" + dd_str;
        }

        date = yyyy_str + mm_str + dd_str;
        Log.d("AlarmManager", "DATE_" + date);

        return date;
    }

    //歩数を取得する関数
    public String setCount() {
        Log.d("AlarmManager", "setCount_Start");
        mSensorManager.registerListener(new SensorEventListener() {
            @Override
            public void onSensorChanged(SensorEvent event) {
                float[] sensorvalue_fltarr = event.values;
                int sensorValue_int = (int)sensorvalue_fltarr[0];
                sensorValue_str = String.valueOf(sensorValue_int);
                Log.d("AlarmManager", "sensorValue_s" + sensorValue_str);
            }

            @Override
            public void onAccuracyChanged(Sensor sensor, int accuracy) {
            }
        }, mStepConterSensor, SensorManager.SENSOR_DELAY_UI);


        Log.d("AlarmManager", "getWalkCount_" + sensorValue_str);

        return sensorValue_str;
    }

    //日付と歩数をファイルに書き込む関数
    public void saveText(Context context, String date, String walkcount_str) {

        String text = date + " " + walkcount_str + "\n";
        String fileName = "WalkCountLog.txt";
        String filepath = String.valueOf(context.getExternalFilesDir(Environment.DIRECTORY_DOCUMENTS));

        File writefile = new File(filepath + "/" + fileName);

        String state = Environment.getExternalStorageState();
        if (Environment.MEDIA_MOUNTED.equals(state)) {
            try {
                FileOutputStream fos = new FileOutputStream(writefile, true);
                OutputStreamWriter writer = new OutputStreamWriter(fos, "UTF-8");
                PrintWriter pw = new PrintWriter(writer);
                pw.append(text);
                pw.close();
                writer.close();
                fos.close();

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

    // 次のアラームの設定
    private void setNextAlarmService(Context context) {
        long startMillis;
        Calendar calendar = Calendar.getInstance();

        //MILLISECONDまで設定することで狙ったタイミングでファイルの更新ができるようになる
        //詳細に設定しないとその間常に更新作業をしてしまう
        //例えば23：59だけの場合、その1分間常にサービスの更新機能が起動し続けてしまう
        //サービスの更新の速さはミリ秒レベルの速さなので、アラームを設定し続けてしまうし、ファイルにデータを記録し続けてしまう
        calendar.set(Calendar.HOUR_OF_DAY, 23);
        calendar.set(Calendar.MINUTE, 59);
        calendar.set(Calendar.SECOND, 58);
        calendar.set(Calendar.MILLISECOND, 50);

        //現在の時間がアラームをセットしたい日時より前の自国の場合、1日分の時間を追加
        if(calendar.getTimeInMillis() < System.currentTimeMillis()){
            calendar.add(Calendar.DAY_OF_YEAR, 1);
        }
        Log.d("AlarmManager", "Finished_calendarTime_s_" + Long.toString(calendar.getTimeInMillis()));

        //アラームの日付設定する
        startMillis=calendar.getTimeInMillis();
        Log.d("AlarmManager", "Finished_delayTime_s_" + Long.toString(startMillis));

        //IntentとPendingIntentを作成し、アラームマネージャーの用意する
        Intent intent = new Intent(context, StepCounterService.class);
        PendingIntent pendingIntent
                = PendingIntent.getService(context, 1001, intent, PendingIntent.FLAG_UPDATE_CURRENT);
        AlarmManager alarmManager = (AlarmManager) context.getSystemService(Context.ALARM_SERVICE);

        //intent.putExtra("CHECK_CODE", "Service");

        //アラームを設定
        alarmManager.setExactAndAllowWhileIdle(AlarmManager.RTC, startMillis, pendingIntent);
        Log.d("AlarmManager", "Finished_firstAlarmSet_s");

        flag_switch = true;
        //サービスの機能をもう一度起動させる
        context.startForegroundService(intent);
        Log.d("AlarmManager", "Finished_secondAlarmStart_s");
    }

    //2周目用の関数
    //フラグをfalseに戻して次のサービス開始時にファイルの記録ができるようにする
    private void setSecondAlarmService(Context context) {

        flag_switch = false;
        Log.d("AlarmManager", "Finished_FlagSwitch_s");
    }
}
