//ペルチェ素子の制御用の変数などの設定

//PID制御のライブラリ
#include <PID_v1.h>

//PID制御関連の変数、初期設定
double Setpoint_h, Input_h, Output_h; //PID制御の目標値、入力、出力
double Setpoint_c, Input_c, Output_c; //PID制御の目標値、入力、出力
double Input;

//Define the aggressive and conservative Tuning Parameters
double aggKp=150, aggKi=10, aggKd=1;
double consKp=50, consKi=0.1, consKd=0.8;

//Specify the links and initial tuning parameters
PID myPID_h(&Input_h, &Output_h, &Setpoint_h, consKp, consKi, consKd, DIRECT);
PID myPID_c(&Input_c, &Output_c, &Setpoint_c, consKp, consKi, consKd, DIRECT);


//IC温度センサ、ペルチェ制御関連の変数とか
const int sensorPin_1 = A0;
const int sensorPin_2 = A1; 
double Vout_1;  //出力電圧(Vout1)
double Vout_2;  //出力電圧(Vout2)
double R_1;  //サーミスタ抵抗値(R1)
double R_2;  //サーミスタ抵抗値(R2)
double B_1;  //補正係数(B1)
double B_2;  //補正係数(B2)

float T_1;
float T_2;

//double PE; //初期誤差

int Peltier_in1 = 13;  //Arduino デジタル信号入力
int Peltier_in2 = 12;  //Arduino デジタル信号入力
int PWM_output = 11;  //PWM制御 (アナログ入力)



//以下、エアポンプの制御用の変数などの設定

//エアポンプのPWM制御(スイッチング用)のピン
const int pump_output1 = 3;
const int pump_output2 = 5;
const int pump_output3 = 6;
const int pump_output4 = 9;

//Pythonからの入力(目標値)読み込み
int inputchar;
int mode;


void setup() {
  //ペルチェ素子制御関連の初期設定
  //温度センサからの入力を初期化
  pinMode(sensorPin_1, INPUT);
  pinMode(sensorPin_2, INPUT);
  Serial.begin(9600);
  Vout_1 = analogRead(sensorPin1) * 5.0 / 1024.0; //出力電圧(Vout1)を測定
  Vout_2 = analogRead(sensorPin2) * 5.0 / 1024.0; //出力電圧(Vout2)を測定
  R_1 = (5.0 * 4.7) / Vout_1 - 4.7; //サーミスタ抵抗値(R1)を計算
  R_2 = (5.0 * 4.7) / Vout_2 - 4.7; //サーミスタ抵抗値(R2)を計算
  B_1 = 3452.9 * pow(R_1,-0.012329); //補正係数(B1)を計算
  B_2 = 3452.9 * pow(R_2,-0.012329); //補正係数(B2)を計算
  T_1 = B_1 / log(R_1*exp(B_1/(25+273.15))/10)-273.15; //サーミスタ温度(T1)を計算
  T_2 = B_2 / log(R_2*exp(B_2/(25+273.15))/10)-273.15; //サーミスタ温度(T2)を計算
  //PE = (double)(T_2 - T_1); //1が室温、2が制御側

  Serial.print("Initialized  Temperature_1(ºC): ");
  Serial.print(T_1);
  Serial.print("  Temperature_2(ºC): ");
  Serial.println(T_2);

  //ペルチェ素子への出力の初期化
  pinMode(Peltier_in1, OUTPUT);
  pinMode(Peltier_in2, OUTPUT);

  
  //エアポンプ制御関連の初期設定
  pinMode(pump_output1, OUTPUT);
  pinMode(pump_output2, OUTPUT);
  pinMode(pump_output3, OUTPUT);
  pinMode(pump_output4, OUTPUT);
  digitalWrite(pump_output1, LOW);
  digitalWrite(pump_output2, LOW);
  digitalWrite(pump_output3, LOW);
  digitalWrite(pump_output4, LOW);

  //モードの初期化
  mode = 0;
}


void loop() {
  Vout_1 = analogRead(sensorPin1) * 5.0 / 1024.0; //出力電圧(Vout1)を測定
  Vout_2 = analogRead(sensorPin2) * 5.0 / 1024.0; //出力電圧(Vout2)を測定
  R_1 = (5.0 * 4.7) / Vout_1 - 4.7; //サーミスタ抵抗値(R1)を計算
  R_2 = (5.0 * 4.7) / Vout_2 - 4.7; //サーミスタ抵抗値(R2)を計算
  B_1 = 3452.9 * pow(R_1,-0.012329); //補正係数(B1)を計算
  B_2 = 3452.9 * pow(R_2,-0.012329); //補正係数(B2)を計算
  T_1 = B_1 / log(R_1*exp(B_1/(25+273.15))/10)-273.15; //サーミスタ温度(T1)を計算
  T_2 = B_2 / log(R_2*exp(B_2/(25+273.15))/10)-273.15; //サーミスタ温度(T2)を計算

  inputchar = Serial.read(); //シリアル通信で送信された値を読み取る
  //入力があった時
  if(inputchar!=-1){
    switch(inputchar){
    
      //エアポンプの制御
      
      //エアポンプNo.1をONにする
      case '1':
        digitalWrite(pump_output1, HIGH);
        digitalWrite(pump_output2, LOW);
        digitalWrite(pump_output3, LOW);
        digitalWrite(pump_output4, LOW);
        Serial.println("Air Pump NO.1 : ON");
        break;
      //エアポンプNo.2をONにする
      case '2':
        digitalWrite(pump_output1, LOW);
        digitalWrite(pump_output2, HIGH);
        digitalWrite(pump_output3, LOW);
        digitalWrite(pump_output4, LOW);
        Serial.println("Air Pump NO.2 : ON");
        break;
      //エアポンプNo.3をONにする
      case '3':
        digitalWrite(pump_output1, LOW);
        digitalWrite(pump_output2, LOW);
        digitalWrite(pump_output3, HIGH);
        digitalWrite(pump_output4, LOW);
        Serial.println("Air Pump NO.3 : ON");
        break;
      //エアポンプNo.4をONにする
      case '4':
        digitalWrite(pump_output1, LOW);
        digitalWrite(pump_output2, LOW);
        digitalWrite(pump_output3, LOW);
        digitalWrite(pump_output4, HIGH);
        Serial.println("Air Pump NO.4 : ON");
        break;
      //全てのエアポンプをOFFにする
      case '0':
        digitalWrite(pump_output1, LOW);
        digitalWrite(pump_output2, LOW);
        digitalWrite(pump_output3, LOW);
        digitalWrite(pump_output4, LOW);
        Serial.println("All OFF");
        break;


      //ペルチェ素子のPID制御

      //室温と同じ時
      case 'r':
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
          Serial.println("room temperature");
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
          Serial.println("room temperature");
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
        //turn the PID on
        myPID_h.SetMode(AUTOMATIC);
        Serial.println("+2℃ from room temperature");
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
        //turn the PID on
        myPID_c.SetMode(AUTOMATIC);
        Serial.println("-2℃ from room temperature");
        break;
    }
  }
  else{
    if (mode == 1) {
      Input_h = (double)(T_2 - T_1);
      double gap = abs(Setpoint_h - Input_h); //distance away from setpoint
      digitalWrite(Peltier_in1, LOW);
      digitalWrite(Peltier_in2, HIGH);
      if (gap < 0.3) {
        //we're close to setpoint, use conservative tuning parameters
        myPID_h.SetTunings(consKp, consKi, consKd);
      }
      else
      {
         //we're far from setpoint, use aggressive tuning parameters
         myPID_h.SetTunings(aggKp, aggKi, aggKd);
      }
    
      myPID_h.Compute();
      analogWrite(PWM_output, Output_h);

      Serial.print("Temperature_1(ºC): ");
      Serial.print(T_1);
      Serial.print("  Temperature_2(ºC): ");
      Serial.print(T_2);
      Serial.print("  Mode : ");
      Serial.print(mode);
      Serial.print("  Current Gap : ");
      Serial.print(Setpoint_h - Input_h);
      Serial.print("  Output(V) : ");
      Serial.println(6 * Output_h / 255);
    }

    else if (mode == 2) {
      Input_c = -1 * (double)(T_2 - T_1);
      double gap = abs(Setpoint_c - Input_c); //distance away from setpoint
      digitalWrite(Peltier_in1, HIGH); //負
      digitalWrite(Peltier_in2, LOW);
      if (gap < 0.3) {
        //we're close to setpoint, use conservative tuning parameters
        myPID_c.SetTunings(consKp, consKi, consKd);
      }
      else
      {
         //we're far from setpoint, use aggressive tuning parameters
         myPID_c.SetTunings(aggKp, aggKi, aggKd);
      }
    
      myPID_c.Compute();
      analogWrite(PWM_output, Output_c);

      Serial.print("Temperature_1(ºC): ");
      Serial.print(T_1);
      Serial.print("  Temperature_2(ºC): ");
      Serial.print(T_2);
      Serial.print("  Mode : ");
      Serial.print(mode);
      Serial.print("  Current Gap : ");
      Serial.print(Setpoint_c - Input_c);
      Serial.print("  Output(V) : ");
      Serial.println(6 * Output_c / 255);
    }
  }
}
