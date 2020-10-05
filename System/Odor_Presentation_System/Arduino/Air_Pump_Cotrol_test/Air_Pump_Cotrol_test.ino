//PWM制御(スイッチング用)のピン
const int pump_output = 3;

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
      case '1':
        digitalWrite(pump_output, HIGH);
        Serial.println("ON");
        break;

      case '0':
        digitalWrite(pump_output, LOW);
        Serial.println("OFF");
        break;
    }
  }
}
