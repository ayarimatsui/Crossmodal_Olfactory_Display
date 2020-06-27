//シリアル通信でPythonの入力から目標値を取得
#include <PID_v1.h>

//Define Variables we'll be connecting to
//PID制御関連の変数、初期設定
double Setpoint_h, Input_h, Output_h; //PID制御の目標値、入力、出力
double Setpoint_c, Input_c, Output_c; //PID制御の目標値、入力、出力

//Define the aggressive and conservative Tuning Parameters
double aggKp=150, aggKi=10, aggKd=1;
double consKp=50, consKi=0.1, consKd=0.8;

//Specify the links and initial tuning parameters
PID myPID_h(&Input_h, &Output_h, &Setpoint_h, consKp, consKi, consKd, DIRECT);
PID myPID_c(&Input_c, &Output_c, &Setpoint_c, consKp, consKi, consKd, DIRECT);


//IC温度センサ、ペルチェ制御関連の変数とか
const int sensorPin_1 = A0;
const int sensorPin_2 = A1; 
float sensorValue_1;
float voltageOut_1;
float sensorValue_2;
float voltageOut_2;

float temperatureC_1;
float temperatureC_2;

//double PE; //初期誤差

int Peltier_in1 = 2;  //Arduino デジタル信号入力
int Peltier_in2 = 4;  //Arduino デジタル信号入力
int PWM_output = 3;  //PWM制御 (アナログ入力)

//Pythonからの入力(目標値)読み込み
int inputchar;
int mode;

//PWM制御(スイッチング用)のピン
const int fan_output = 11;


void setup()
{
  //温度センサからの入力を初期化
  pinMode(sensorPin_1, INPUT);
  pinMode(sensorPin_2, INPUT);
  Serial.begin(9600);
  sensorValue_1 = analogRead(sensorPin_1);
  sensorValue_2 = analogRead(sensorPin_2);
  voltageOut_1 = (sensorValue_1 * 5000) / 1024;
  voltageOut_2 = (sensorValue_2 * 5000) / 1024;
  // calculate temperature for LM335
  temperatureC_1 = voltageOut_1 / 10 - 273;
  temperatureC_2 = voltageOut_2 / 10 - 273;
  //PE = (double)(temperatureC_2 - temperatureC_1); //1が室温、2が制御側

  Serial.print("Initialized  Temperature_1(ºC): ");
  Serial.print(temperatureC_1);
  Serial.print("  Temperature_2(ºC): ");
  Serial.println(temperatureC_2);

  //ペルチェ制御の初期化
  pinMode(Peltier_in1, OUTPUT);
  pinMode(Peltier_in2, OUTPUT);

  //ファンの設定
  pinMode(fan_output, OUTPUT);
  digitalWrite(fan_output, LOW);

  
  //PID制御の初期化
  //initialize the variables we're linked to
  //Input = (double)(temperatureC_2 - temperatureC_1);
  //Setpoint = 0;

  mode = 0;
}

void loop()
{
  sensorValue_1 = analogRead(sensorPin_1);
  sensorValue_2 = analogRead(sensorPin_2);
  voltageOut_1 = (sensorValue_1 * 5000) / 1024;
  voltageOut_2 = (sensorValue_2 * 5000) / 1024;
  // calculate temperature for LM335
  temperatureC_1 = voltageOut_1 / 10 - 273;
  temperatureC_2 = voltageOut_2 / 10 - 273;

  inputchar = Serial.read(); //シリアル通信で送信された値を読み取る
  //入力があった時
  if(inputchar!=-1){
    switch(inputchar){
      case 'h':
        Input_h = (double)(temperatureC_2 - temperatureC_1);
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
        break;

      case 'c':
        Input_c = -1 * (double)(temperatureC_2 - temperatureC_1);
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
        //ファンをon
        digitalWrite(fan_output, HIGH);
        break;
    }
  }
  else{
    if (mode == 1) {
      Input_h = (double)(temperatureC_2 - temperatureC_1);
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
      Serial.print(temperatureC_1);
      Serial.print("  Temperature_2(ºC): ");
      Serial.print(temperatureC_2);
      Serial.print("  Mode : ");
      Serial.print(mode);
      Serial.print("  Current Gap : ");
      Serial.print(Setpoint_h - Input_h);
      Serial.print("  Output(V) : ");
      Serial.println(6 * Output_h / 255);

      //目標値に近づいてきたら、ファンの回転を止める
      if (abs(Setpoint_h - Input_h) < 2) {
        digitalWrite(fan_output, LOW);
      }
    }

    else if (mode == 2) {
      Input_c = -1 * (double)(temperatureC_2 - temperatureC_1);
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
      Serial.print(temperatureC_1);
      Serial.print("  Temperature_2(ºC): ");
      Serial.print(temperatureC_2);
      Serial.print("  Mode : ");
      Serial.print(mode);
      Serial.print("  Current Gap : ");
      Serial.print(Setpoint_c - Input_c);
      Serial.print("  Output(V) : ");
      Serial.println(6 * Output_c / 255);
    }
  }
}
