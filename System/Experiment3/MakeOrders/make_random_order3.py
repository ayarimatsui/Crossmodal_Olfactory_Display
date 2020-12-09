# 実験3
# グレープフルーツ、アップル、チェリー、マスカット、ラズベリー、さくらの6種類の香料をランダムに並び替えてcsvファイルに保存

import random
import csv

N = 15 #被験者数

for n in range(N // 3):
    aroma_list = ['grapefruit', 'apple', 'cherry', 'white grapes', 'raspberry', 'sakura']
    random.shuffle(aroma_list)
    for i in range(3):
        with open('csvFiles/order_subject' + str(n * 3 + i + 1) + '.csv', 'w') as f:
            writer = csv.writer(f)
            writer.writerow([1, aroma_list[i * 2]])
            writer.writerow([2, aroma_list[i * 2 + 1]])