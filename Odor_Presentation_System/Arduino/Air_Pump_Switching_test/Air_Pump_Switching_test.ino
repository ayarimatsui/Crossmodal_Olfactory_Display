//PWM制御(スイッチング用)のピン
const int pump_output1 = 3;
const int pump_output2 = 5;
const int pump_output3 = 6;
const int pump_output4 = 9;

//Pythonからの入力(目標値)読み込み
int inputchar;

void setup() {
  pinMode(pump_output, OUTPUT);
  Serial.begin(9600);
  digitalWrite(pump_output, LOW);
}

void loop() {
  inputchar = Serial.read(); //シリアル通信で送信された値を読み取る
  //入力があった時
  if(inputchar!=-1){
    switch(inputchar){
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
    }
  }
}
