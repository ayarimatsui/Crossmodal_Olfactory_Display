//Arduinoの制御
//初期の鼻部皮膚温度を基準として制御

//PID制御のライブラリ]
#include <PID_v1.h>
#include <math.h>

//変数設定
//PID制御関連の変数、初期設定
double Setpoint_h, Input_h, Output_h; //PID制御の目標値、入力、出力
double Setpoint_c, Input_c, Output_c; //PID制御の目標値、入力、出力
double Input;

// 温めるか冷やすかによって、PIDパラメータも別に設定しておく
//Define the aggressive and conservative Tuning Parameters
// 温める時 (kp_h=40, kp_c= 60もあり)
double aggKp_h = 50, aggKi_h = 0.5, aggKd_h = 1;
double consKp_h = 20, consKi_h = 0.8, consKd_h = 0.25;
// 冷やす時
double aggKp_c = 80, aggKi_c = 0.5, aggKd_c = 1;
double consKp_c = 40, consKi_c = 0.8, consKd_c = 0.25;

//Specify the links and initial tuning parameters
PID myPID_h(&Input_h, &Output_h, &Setpoint_h, consKp_h, consKi_h, consKd_h, DIRECT);
PID myPID_c(&Input_c, &Output_c, &Setpoint_c, consKp_c, consKi_c, consKd_c, DIRECT);


//IC温度センサ、ペルチェ制御関連の変数とかM
const int sensorPin_1 = A0;
const int sensorPin_2 = A1;

float T_r; //サーミスタ1の温度(室温)
float T_1; //サーミスタ2の初期温度(初期の鼻部皮膚温度)
float T_2; //サーミスタ2の温度(板の温度)

int Peltier_in1 = 13;  //Arduino デジタル信号入力
int Peltier_in2 = 12;  //Arduino デジタル信号入力
int PWM_output = 11;  //PWM制御 (アナログ入力)

//モードを表す変数
int mode;
char command;
int count_to_off = 0;

//シーン切り替えしてOKかどうか
bool OK = false;

//DCファンのPWM制御(スイッチング)のピン
const int fan_output = 10;

//エアポンプのPWM制御(スイッチング用)のピン
const int aroma_output1 = 3; //匂い呈示用1
const int aroma_output2 = 5; //匂い呈示用2
const int aroma_output3 = 6; //匂い呈示用3
const int air_output = 9; //空気噴出用
const int duty = (int)(0.8 * 255); //duty比はテスト


float calcTemperature(int sensorPin)
{
  double Vout, R, B;
  float T;

  Vout = analogRead(sensorPin) * 5.0 / 1024.0; //出力電圧を計算
  R = (5.0 * 4.7) / Vout - 4.7; //サーミスタ抵抗値を計算
  B = 3452.9 * pow(R, -0.012329); //補正係数(B1)を計算
  T = B / log(R * exp(B / (25 + 273.15)) / 10) - 273.15; //サーミスタ温度を計算

  return T;
}

void setup()
{
  //ペルチェ素子制御関連の初期設定
  //温度センサからの入力を初期化
  pinMode(sensorPin_1, INPUT);
  pinMode(sensorPin_2, INPUT);
  Serial.begin(9600);
  T_1 = calcTemperature(sensorPin_1); //サーミスタ温度(T1)を計算
  T_2 = calcTemperature(sensorPin_2); //サーミスタ温度(T2)を計算

  //ペルチェ素子への出力の初期化
  pinMode(Peltier_in1, OUTPUT);
  pinMode(Peltier_in2, OUTPUT);

  //ファンの設定
  pinMode(fan_output, OUTPUT);
  digitalWrite(fan_output, LOW);

  //エアポンプ制御関連の初期設定
  pinMode(aroma_output1, OUTPUT);
  pinMode(aroma_output2, OUTPUT);
  pinMode(aroma_output3, OUTPUT);
  pinMode(air_output, OUTPUT);
  digitalWrite(aroma_output1, LOW);
  digitalWrite(aroma_output2, LOW);
  digitalWrite(aroma_output3, LOW);
  digitalWrite(air_output, LOW);

  //モードの初期化
  mode = 0;
}

//UnityからArduinoへのデータ送信
void sendOK()
{
  //シリアル送信
  Serial.print(1);
  Serial.print(",");
  Serial.println("");
}

void sendNG()
{
  //シリアル送信
  Serial.print(0);
  Serial.print(",");
  Serial.println("");
}

void sendTemp()
{
  T_r = calcTemperature(sensorPin_1); //サーミスタ1の温度(T_r 室温)を計算
  T_1 = calcTemperature(sensorPin_2); //サーミスタ2の温度(T1 初期の鼻部皮膚温度)を計算
  //シリアル送信
  Serial.print(T_r);
  Serial.print(",");
  Serial.print(T_1);
  Serial.print(",");
  Serial.println("");
}

void Receive()
{
  T_2 = calcTemperature(sensorPin_2); //サーミスタ温度(T2)を計算

  if (Serial.available()) {
    char inputchar = Serial.read();

    //Unityから送られてきた文字によって動作を変える
    switch (inputchar) {
      
      //エアポンプ制御
      //エアポンプ1(匂い呈示用1)をONにする
      case '1':
        analogWrite(aroma_output1, duty);
        digitalWrite(aroma_output2, LOW);
        digitalWrite(aroma_output3, LOW);
        digitalWrite(air_output, LOW);
        break;
      //エアポンプ2(匂い呈示用2)をONにする
      case '2':
        digitalWrite(aroma_output1, LOW);
        analogWrite(aroma_output2, duty);
        digitalWrite(aroma_output3, LOW);
        digitalWrite(air_output, LOW);
        break;
      //エアポンプ3(匂い呈示用3)をONにする
      case '3':
        digitalWrite(aroma_output1, LOW);
        digitalWrite(aroma_output2, LOW);
        analogWrite(aroma_output3, duty);
        digitalWrite(air_output, LOW);
        break;
      //エアポンプ4(空気噴出用)をONにする
      case '4':
        digitalWrite(aroma_output1, LOW);
        digitalWrite(aroma_output2, LOW);
        digitalWrite(aroma_output3, LOW);
        analogWrite(air_output, duty);
        break;
      //全てのエアポンプをOFFにする
      case '0':
        digitalWrite(aroma_output1, LOW);
        digitalWrite(aroma_output2, LOW);
        digitalWrite(aroma_output3, LOW);
        digitalWrite(air_output, LOW);
        break;

      //以下、温度制御
      //室温、初期の鼻部皮膚温度をUnityに送信
      case 's':
        mode = 3;
        break;

      //ペルチェ素子のPID制御
      //ペルチェ素子にかかる電圧を0にする
      case 'o':
        digitalWrite(Peltier_in1, LOW);
        digitalWrite(Peltier_in2, LOW);
        analogWrite(PWM_output, 0);
        //ファンをOFF
        digitalWrite(fan_output, LOW);
        mode = 0;
        command = 'o';
        break;

      //室温と同じ時
      case 'r':
        command = 'r';
        Input = (double)(T_2 - T_1);
        if (Input >= 0) {
          Input_c = -1 * (double)(T_2 - T_1);
          Setpoint_c = 0;
          myPID_h.SetMode(MANUAL);
          digitalWrite(Peltier_in1, LOW);
          digitalWrite(Peltier_in2, LOW);
          analogWrite(PWM_output, 0);
          delay(1000);
          digitalWrite(Peltier_in1, HIGH); //負
          digitalWrite(Peltier_in2, LOW);
          mode = 2;
          //turn the PID on
          myPID_c.SetMode(AUTOMATIC);
          //ファンをon
          digitalWrite(fan_output, HIGH);
          //Serial.println("room temperature"); //テスト
          OK = false;
          sendNG();
          break;
        }
        else {
          Input_h = (double)(T_2 - T_1);
          Setpoint_h = 0;
          myPID_c.SetMode(MANUAL);
          digitalWrite(Peltier_in1, LOW);
          digitalWrite(Peltier_in2, LOW);
          analogWrite(PWM_output, 0);
          delay(1000);
          digitalWrite(Peltier_in1, LOW);
          digitalWrite(Peltier_in2, HIGH);
          mode = 1;
          //turn the PID on
          myPID_h.SetMode(AUTOMATIC);
          //Serial.println("room temperature"); //テスト
          OK = false;
          sendNG();
          break;
        }
      //室温+2度の時
      case 'h':
        Input_h = (double)(T_2 - T_1);
        Setpoint_h = 2;
        myPID_c.SetMode(MANUAL);
        digitalWrite(Peltier_in1, LOW);
        digitalWrite(Peltier_in2, LOW);
        analogWrite(PWM_output, 0);
        delay(1000);
        digitalWrite(Peltier_in1, LOW);
        digitalWrite(Peltier_in2, HIGH);
        mode = 1;
        command = 'h';
        //turn the PID on
        myPID_h.SetMode(AUTOMATIC);
        //Serial.println("+2℃ from room temperature");  //テスト
        OK = false;
        sendNG();
        break;
      //室温-2度の時
      case 'c':
        Input_c = -1 * (double)(T_2 - T_1);
        Setpoint_c = -1 * -2;
        myPID_h.SetMode(MANUAL);
        digitalWrite(Peltier_in1, LOW);
        digitalWrite(Peltier_in2, LOW);
        analogWrite(PWM_output, 0);
        delay(1000);
        digitalWrite(Peltier_in1, HIGH); //負
        digitalWrite(Peltier_in2, LOW);
        mode = 2;
        command = 'c';
        //turn the PID on
        myPID_c.SetMode(AUTOMATIC);
        //ファンをon
        digitalWrite(fan_output, HIGH);
        //Serial.println("-2℃ from room temperature"); //テスト
        OK = false;
        sendNG();
        break;
    }
  }
}

void sendData()
{
  if (mode == 1) {
    Input_h = (double)(T_2 - T_1);
    double gap = abs(Setpoint_h - Input_h); //distance away from setpoint
    digitalWrite(Peltier_in1, LOW);
    digitalWrite(Peltier_in2, HIGH);

    if (gap < 1.5) {
      //we're close to setpoint, use conservative tuning parameters
      myPID_h.SetTunings(consKp_h, consKi_h, consKd_h);
    }
    else
    {
      //we're far from setpoint, use aggressive tuning parameters
      myPID_h.SetTunings(aggKp_h, aggKi_h, aggKd_h);
    }

    if (command == 'r' && gap < 0.3) {
      mode = 0;
    }

    myPID_h.Compute();
    analogWrite(PWM_output, Output_h);

    //目標値に近づいてきたら、ファンの回転を止める
    if (abs(Setpoint_h - Input_h) < 2) {
      digitalWrite(fan_output, LOW);
    }

    //目標値に到達したら、UnityにOKサインを送信する
    if (OK == false && (Setpoint_h - Input_h) < 0.15) {
      sendOK();
      OK = true;
    }

    //Unityからの指示が「室温(r)」の場合は、目標値に近くなったら電圧を0にする
    if (command == 'r' && gap < 1.4) {
      count_to_off += 1;
      if (count_to_off >= 3) {
        digitalWrite(Peltier_in1, LOW);
        digitalWrite(Peltier_in2, LOW);
        analogWrite(PWM_output, 0);
        //ファンをOFF
        digitalWrite(fan_output, LOW);
        mode = 0;
        count_to_off = 0;
      }
    }
  }

  else if (mode == 2) {
    Input_c = -1 * (double)(T_2 - T_1);
    double gap = abs(Setpoint_c - Input_c); //distance away from setpoint
    digitalWrite(Peltier_in1, HIGH);
    digitalWrite(Peltier_in2, LOW);

    if (gap < 1) {
      //we're close to setpoint, use conservative tuning parameters
      myPID_c.SetTunings(consKp_c, consKi_c, consKd_c);
    }
    else
    {
      //we're far from setpoint, use aggressive tuning parameters
      myPID_c.SetTunings(aggKp_c, aggKi_c, aggKd_c);
    }

    myPID_c.Compute();
    analogWrite(PWM_output, Output_c);

    //目標値に到達したら、UnityにOKサインを送信する
    if (OK == false && (Setpoint_c - Input_c) < 0.15) {
      sendOK();
      OK = true;
    }

    //Unityからの指示が「室温(r)」の場合は、目標値に近くなったら電圧を0にする
    if (command == 'r' && gap < 1.2) {
      count_to_off += 1;
      if (count_to_off >= 3) {
        digitalWrite(Peltier_in1, LOW);
        digitalWrite(Peltier_in2, LOW);
        analogWrite(PWM_output, 0);
        //ファンをOFF
        digitalWrite(fan_output, LOW);
        mode = 0;
        count_to_off = 0;
      }
    }
  }

  else if (mode == 0 && command == 'r') {
    double gap = abs(T_2 - T_1);

    //目標値に到達したら、UnityにOKサインを送信する
    if (OK == false && gap < 0.15) {
      sendOK();
      OK = true;
    }
  }

  else if (mode == 3){
    sendTemp();
  }
}

void loop()
{
  Receive();
  sendData();
}
