# 実験2
# グレープフルーツ、アップル、チェリー、マスカット、ラズベリー、さくらの6種類の香料をランダムに並び替えてcsvファイルに保存

import random
import csv

N = 6 #被験者数

for n in range(1, N + 1):
    aroma_list = ['grapefruit', 'apple', 'cherry', 'white grapes', 'raspberry', 'sakura']
    random.shuffle(aroma_list)
    with open('csvFiles/order_subject' + str(n) + '.csv', 'w') as f:
        writer = csv.writer(f)
        writer.writerow(aroma_list)