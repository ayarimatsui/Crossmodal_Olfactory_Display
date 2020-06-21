int Peltier_in1 = 0;  //Arduino デジタル信号入力
int Peltier_in2 = 1;  //Arduino デジタル信号入力
int PWM_output = 3;  //PWM制御 (アナログ入力)

void setup() {
  pinMode(Peltier_in1, OUTPUT);
  pinMode(Peltier_in2, OUTPUT);
}

void loop() {
 digitalWrite(Peltier_in1, HIGH); //正
 int duty = (int)(0.25 * 255);
 analogWrite(PWM_output, duty); //PWM設定値:0～255　255=6V(外部電源)
 delay(10000);
 digitalWrite(Peltier_in1,LOW); //無負荷
 analogWrite(PWM_output, 0);
 delay(1000);

 digitalWrite(Peltier_in2, HIGH); //負
 analogWrite(PWM_output, duty);
 delay(10000);
 digitalWrite(Peltier_in2,LOW); //無負荷
 analogWrite(PWM_output,0);
 delay(1000);
}
