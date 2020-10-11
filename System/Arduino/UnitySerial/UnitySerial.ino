//UnityとArduinoの連携テスト

namespace {
  int Peltier_in1 = 13;  //Arduino デジタル信号入力
  int Peltier_in2 = 12;  //Arduino デジタル信号入力
  int PWM_output = 11;  //PWM制御 (アナログ入力)
  int duty = (int)(0.4 * 255);
}

void setup()
{
  Serial.begin(9600);
  //ピンの初期化
  pinMode(Peltier_in1, OUTPUT);
  pinMode(Peltier_in2, OUTPUT);
}

//UnityからArduinoへのデータ送信は今回はなし
/*
void readAccelerometer()
{
  //平滑化フィルタ
  x = ratio * prex + (1 – ratio) * analogRead(3);
  y = ratio * prey + (1 – ratio) * analogRead(4);
  prex = x;
  prey = y;

  //センサ値を度に変換
  float rotateX = (x – 277) / 2.48 – 90;
  float rotateY = (y – 277) / 2.48 – 90;

  //シリアル送信
  Serial.print(rotateX);
  Serial.print(",");
  Serial.print(rotateY);
  Serial.println("");
}
*/

void Receive()
{
  if ( Serial.available() ) {
    char mode = Serial.read();

    //Unityから送られてきた文字によって動作を変える
    switch (mode) {
      case 'C' :
        digitalWrite(Peltier_in1, HIGH); //正
        digitalWrite(Peltier_in2, LOW);
        analogWrite(PWM_output, duty);
        break;
      case 'H' :
        digitalWrite(Peltier_in1, LOW); //負
        digitalWrite(Peltier_in2, HIGH);
        analogWrite(PWM_output, duty);
        break;
      case '0' :
        digitalWrite(Peltier_in1, LOW); //OFF
        digitalWrite(Peltier_in2, LOW);
        analogWrite(PWM_output, 0);
        break;
    }
  }
}

void loop()
{
  Receive();
}
