import serial   #モジュール名はpyserialだが, importする際はserialである

def main():
    speed = 9600; #シリアル通信のデータ転送レート
    port = '/dev/cu.usbmodem143101' #Arduinoを接続してるシリアルポート
	   #'/dev/cu.usbmodem144101' #Arduinoを接続してるシリアルポート
    with serial.Serial(port,speed,timeout=1) as ser:
        while True:
            flag=bytes(input(),'utf-8')

            #シリアル通信で文字を送信する際は, byte文字列に変換する
            #input()する際の文字列はutf-8

            ser.write(flag)

            #シリアル通信:送信

            if(flag==bytes('a','utf-8')):
                break;
        ser.close()

if __name__ == "__main__":
    main()
