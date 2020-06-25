const int sensorPin_1 = A0;
const int sensorPin_2 = A1; 
float sensorValue_1;
float voltageOut_1;
float sensorValue_2;
float voltageOut_2;

float temperatureC_1;
float temperatureF_1;
float temperatureC_2;
float temperatureF_2;

// uncomment if using LM335
float temperatureK_1;
float temperatureK_2;

void setup() {
  pinMode(sensorPin_1, INPUT);
  pinMode(sensorPin_2, INPUT);
  Serial.begin(9600);
}

void loop() {
  sensorValue_1 = analogRead(sensorPin_1);
  sensorValue_2 = analogRead(sensorPin_2);
  voltageOut_1 = (sensorValue_1 * 5000) / 1024;
  voltageOut_2 = (sensorValue_2 * 5000) / 1024;
  

  // calculate temperature for LM335
  temperatureK_1 = voltageOut_1 / 10;
  temperatureC_1 = temperatureK_1 - 273;
  temperatureF_1 = (temperatureC_1 * 1.8) + 32;
  temperatureK_2 = voltageOut_2 / 10;
  temperatureC_2 = temperatureK_2 - 273;
  temperatureF_2 = (temperatureC_2 * 1.8) + 32;

  // calculate temperature for LM34
  //temperatureF = voltageOut / 10;
  //temperatureC = (temperatureF - 32.0)*(5.0/9.0);

  Serial.print("Temperature_1(ºC): ");
  Serial.print(temperatureC_1);
  Serial.print("  Temperature_2(ºC): ");
  Serial.println(temperatureC_2);
  delay(1000);
}
