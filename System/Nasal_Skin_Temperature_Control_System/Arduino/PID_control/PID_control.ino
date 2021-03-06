//温度を上げたい時
#include <PID_v1.h>

//Define Variables we'll be connecting to
//PID制御関連の変数、初期設定
double Setpoint, Input, Output; //PID制御の目標値、入力、出力

//Define the aggressive and conservative Tuning Parameters
double aggKp=100, aggKi=1, aggKd=1;
double consKp=50, consKi=0.1, consKd=0.8;

//Specify the links and initial tuning parameters
PID myPID(&Input, &Output, &Setpoint, consKp, consKi, consKd, DIRECT);


//IC温度センサ、ペルチェ制御関連の変数とか
int sensorPin_1 = A0;
int sensorPin_2 = A1; 
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

  //PID制御の初期化
  //initialize the variables we're linked to
  Input = (double)(temperatureC_2 - temperatureC_1);
  Setpoint = 2;

  //turn the PID on
  myPID.SetMode(AUTOMATIC);
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
  
  Input = (double)(temperatureC_2 - temperatureC_1);

  double gap = abs(Setpoint - Input); //distance away from setpoint

  //digitalWrite(Peltier_in1, HIGH); //正
  //digitalWrite(Peltier_in2, LOW);
  
  if (gap < 0.3) {
    //we're close to setpoint, use conservative tuning parameters
    myPID.SetTunings(consKp, consKi, consKd);
  }
  else
  {
     //we're far from setpoint, use aggressive tuning parameters
     myPID.SetTunings(aggKp, aggKi, aggKd);
  }

  myPID.Compute();
  digitalWrite(Peltier_in1, LOW);
  digitalWrite(Peltier_in2, HIGH);
  analogWrite(PWM_output, Output);
  
  Serial.print("Temperature_1(ºC): ");
  Serial.print(temperatureC_1);
  Serial.print("  Temperature_2(ºC): ");
  Serial.print(temperatureC_2);
  Serial.print("  Current Gap : ");
  Serial.print(Setpoint - Input);
  Serial.print("  Output(V) : ");
  Serial.println(6 * Output / 255);
}
