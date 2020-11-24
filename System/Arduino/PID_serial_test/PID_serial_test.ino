//シリアル通信でPythonの入力から目標値を取得
#include <PID_v1.h>

namespace {
  //PID制御関連の変数、初期設定
  double Setpoint_h, Input_h, Output_h; //PID制御の目標値、入力、出力 (温める場合)
  double Setpoint_c, Input_c, Output_c; //PID制御の目標値、入力、出力 (冷やす場合)
  double Input;
  
  //Define the aggressive and conservative Tuning Parameters
  double aggKp=150, aggKi=10, aggKd=1; //目標値との差が大きい時に使うパラメータ
  double consKp=50, consKi=0.1, consKd=0.8; //目標値との差が小さい時に使うパラメータ
  
  //Specify the links and initial tuning parameters
  //PIDを定義しておく
  PID myPID_h(&Input_h, &Output_h, &Setpoint_h, consKp, consKi, consKd, DIRECT);
  PID myPID_c(&Input_c, &Output_c, &Setpoint_c, consKp, consKi, consKd, DIRECT);
  
  
  //IC温度センサ、ペルチェ制御関連の変数とか
  //センサ1は基準として室温を測る
  //センサ2はペルチェ素子に取り付けて、ペルチェの温度を測る
  const int sensorPin_1 = A0;
  const int sensorPin_2 = A1; 
  float sensorValue_1; //温度センサ1の値
  float sensorValue_2; //温度センサ2の値
  float temperatureC_1; //温度センサ1の温度
  float temperatureC_2; //温度センサ2の温度
  
  
  int Peltier_in1 = 13;  //Arduino デジタル信号入力
  int Peltier_in2 = 12;  //Arduino デジタル信号入力
  int PWM_output = 11;  //PWM制御 (アナログ入力)
  
  //Pythonからの入力(目標値)読み込み
  int inputchar;

  //モード設定(温める時 mode=1, 冷やす時 mode=2)
  int mode;
}

//センサ値から温度を計算して返す関数
float calcTemp(sensorValue)
{
  float voltageOut = (sensorValue * 5000) / 1024;
  float temperatureC = voltageOut / 10 - 273;
  return temperatureC;
}

void setup()
{
  //温度センサからの入力を初期化
  pinMode(sensorPin_1, INPUT);
  pinMode(sensorPin_2, INPUT);

  //シリアル通信開始
  Serial.begin(9600);

  //温度センサの値を読み取って、それぞれの温度を計算する
  sensorValue_1 = analogRead(sensorPin_1);
  sensorValue_2 = analogRead(sensorPin_2);
  temperatureC_1 = calcTemp(sensorValue_1);
  temperatureC_2 = calcTemp(sensorValue_2);

  //最初の温度をシリアルモニタで確認
  Serial.print("Initialized  Temperature_1(ºC): ");
  Serial.print(temperatureC_1);
  Serial.print("  Temperature_2(ºC): ");
  Serial.println(temperatureC_2);

  //ペルチェ制御の初期化
  pinMode(Peltier_in1, OUTPUT);
  pinMode(Peltier_in2, OUTPUT);

  //モードはとりあえず0にしておく(特に何も起こらない)
  mode = 0;
}

void loop()
{
  //温度センサの値を読み取って、それぞれの温度を計算する
  sensorValue_1 = analogRead(sensorPin_1);
  sensorValue_2 = analogRead(sensorPin_2);
  temperatureC_1 = calcTemp(sensorValue_1);
  temperatureC_2 = calcTemp(sensorValue_2);

  inputchar = Serial.read(); //シリアル通信で送信された値を読み取る
  //入力があった時
  if(inputchar!=-1){
    switch(inputchar){

      //文字'r'が送られてきた時(室温にペルチェを制御する)
      case 'r':
      //ペルチェの温度と室温の差を Input と定義
       Input = (double)(temperatureC_2 - temperatureC_1);

        //ペルチェの温度の方が室温よりも高いときは、ペルチェを冷やす必要がある
        if (Input >= 0) {
          Input_c = -1 * (double)(temperatureC_2 - temperatureC_1);  //Inputを逆転させて、PID制御ができるようにする
          Setpoint_c = 0; //目標は、Input_c = 0　とすること
          //使わないPIDの設定
          myPID_h.SetMode(MANUAL);

          //冷やしたいときは、Peltier_in1をHigh, Peltier_in2をLowにする (環境依存)
          digitalWrite(Peltier_in1, HIGH); //負
          digitalWrite(Peltier_in2, LOW);
          analogWrite(PWM_output, 0);
          //冷やしたいので、モードを2に設定しておく
          mode = 2;
          //PID制御をオンにする
          myPID_c.SetMode(AUTOMATIC);
          break;
        }

        //ペルチェの温度の方が室温よりも低いときは、ペルチェを温める必要がある
        else {
          Input_h = (double)(temperatureC_2 - temperatureC_1);
          Setpoint_h = 0; //目標は、Input_h = 0　とすること
          myPID_c.SetMode(MANUAL); //使わないPIDの設定
          
          //温めたいときは、Peltier_in1をLow, Peltier_in2をHighにする (環境依存)
          digitalWrite(Peltier_in1, LOW);
          digitalWrite(Peltier_in2, HIGH);
          analogWrite(PWM_output, 0);
          //温めたいので、モードを1に設定しておく
          mode = 1;
          //PID制御をオンにする
          myPID_h.SetMode(AUTOMATIC);
          break;
        }
        
      //文字'h'が送られてきた時(室温+2℃ にペルチェを制御する)
      case 'h':
        Input_h = (double)(temperatureC_2 - temperatureC_1);
        Setpoint_h = 2; //目標は、Input_h = 2　とすること
        myPID_c.SetMode(MANUAL); //使わないPIDの設定
        digitalWrite(Peltier_in1, LOW);
        digitalWrite(Peltier_in2, HIGH);
        analogWrite(PWM_output, 0);
        mode = 1; //温めたいので、モードを1に設定しておく
        //turn the PID on
        myPID_h.SetMode(AUTOMATIC);
        break;

      //文字'c'が送られてきた時(室温-2℃ にペルチェを制御する)
      case 'c':
        Input_c = -1 * (double)(temperatureC_2 - temperatureC_1);  //Inputを逆転させて、PID制御ができるようにする
        Setpoint_c = -1 * -2;  //目標値も正負逆転させておく　目標は、Input_c = 2　とすること
        myPID_h.SetMode(MANUAL);  //使わないPIDの設定
        digitalWrite(Peltier_in1, HIGH); //負
        digitalWrite(Peltier_in2, LOW);
        analogWrite(PWM_output, 0);
        mode = 2;  //冷やしたいので、モードを2に設定しておく
        //turn the PID on
        myPID_c.SetMode(AUTOMATIC);
        break;
    }
  }

  //特に新しい入力はないとき(前の入力の制御を続けたいとき)
  else{

    //温めているとき
    if (mode == 1) {
      Input_h = (double)(temperatureC_2 - temperatureC_1);
      double gap = abs(Setpoint_h - Input_h); //目標値からの差の絶対値

      //温めたいときは、Peltier_in1をLow, Peltier_in2をHighにする (環境依存)
      digitalWrite(Peltier_in1, LOW);
      digitalWrite(Peltier_in2, HIGH);

      //目標値への差が小さいときは、PIDのパラメータをconsにする
      if (gap < 0.3) {
        //we're close to setpoint, use conservative tuning parameters
        myPID_h.SetTunings(consKp, consKi, consKd);
      }

      //目標値への差が大きいときは、PIDのパラメータをaggに変更する
      else
      {
         //we're far from setpoint, use aggressive tuning parameters
         myPID_h.SetTunings(aggKp, aggKi, aggKd);
      }
    
      myPID_h.Compute(); //PIDの計算 勝手に良い感じの出力を計算して、Output_hに代入してくれる
      analogWrite(PWM_output, Output_h); //計算された出力値をPWMで出力

      //現在の温度や、モード、目標値までの差などをシリアルモニタに表示
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
    }

    //冷やしているとき
    else if (mode == 2) {
      Input_c = -1 * (double)(temperatureC_2 - temperatureC_1);  //Inputを逆転させて、PID制御ができるようにする
      double gap = abs(Setpoint_c - Input_c); //目標値からの差の絶対値

      //冷やしたいときは、Peltier_in1をHigh, Peltier_in2をLowにする (環境依存)
      digitalWrite(Peltier_in1, HIGH); 
      digitalWrite(Peltier_in2, LOW);

      //目標値への差が小さいときは、PIDのパラメータをconsにする
      if (gap < 0.3) {
        //we're close to setpoint, use conservative tuning parameters
        myPID_c.SetTunings(consKp, consKi, consKd);
      }
      //目標値への差が大きいときは、PIDのパラメータをaggに変更する
      else
      {
         //we're far from setpoint, use aggressive tuning parameters
         myPID_c.SetTunings(aggKp, aggKi, aggKd);
      }
    
      myPID_c.Compute(); //PIDの計算　勝手に良い感じの出力を計算して、Output_cに代入してくれる
      analogWrite(PWM_output, Output_c);

      //現在の温度や、モード、目標値までの差などをシリアルモニタに表示
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
