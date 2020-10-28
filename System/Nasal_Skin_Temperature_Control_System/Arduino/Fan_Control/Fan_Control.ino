//PWM制御(スイッチング用)のピン
const int fan_output = 10;

//Pythonからの入力(目標値)読み込み
int inputchar;

void setup() {
  pinMode(fan_output, OUTPUT);
  Serial.begin(9600);
  digitalWrite(fan_output, LOW);
}

void loop() {
  inputchar = Serial.read(); //シリアル通信で送信された値を読み取る
  //入力があった時
  if(inputchar!=-1){
    switch(inputchar){
      case '1':
        digitalWrite(fan_output, HIGH);
        break;

      case '0':
        digitalWrite(fan_output, LOW);
        break;
    }
  }
}
