#include <math.h>
double Vout_1;  //出力電圧(Vout1)
double Vout_2;  //出力電圧(Vout2)
double R_1;  //サーミスタ抵抗値(R1)
double R_2;  //サーミスタ抵抗値(R2)
double B_1;  //補正係数(B1)
double B_2;  //補正係数(B2)
double T_1;  //サーミスタ温度(T1)
double T_2;  //サーミスタ温度(T2)
const int sensorPin1 = A0;
const int sensorPin2 = A1;

void setup() {
 Serial.begin(9600);           //9600bpsでシリアルポートを開く
 pinMode(sensorPin1, INPUT);  //A0、A1番ピンを入力用に設定する
 pinMode(sensorPin2, INPUT);
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
 Serial.print(" T1:");Serial.print(T_1);Serial.print(" R1:");Serial.println(R_1); //温度をシリアル出力
 Serial.print(" T2:");Serial.print(T_2);Serial.print(" R2:");Serial.println(R_2); //温度をシリアル出力
 delay(1000);
}
