# 実験2
# 全試行(630試行)を6人にランダムに振り分け、その順番をcsvファイルに書き出すプログラム
import random
import csv
import itertools

N = 12   # トータルの被験者数 (6の倍数である必要あり)

for i in range(N // 6):
    # Order生成
    # あとで実験1の結果より変更
    selected_Aromas = [1, 2, 3, 4, 5, 6]
    AromaCondition = []
    for aroma in selected_Aromas:
        AromaCondition.append(str(aroma) + "h")
        AromaCondition.append(str(aroma) + "c")
    for j in range(24):
        AromaCondition.append(str(j + 1) + "r")
    C = list(itertools.combinations(AromaCondition, 2))  # 組み合わせのリストを生成
    random.shuffle(C)  # ランダムに並び替え
    # 6分割してcsvファイルに保存
    total_trials = len(C)
    one_sixth = total_trials // 6
    for k in range(6):
        A = []
        B = []
        for l in range(k * one_sixth, (k + 1) * one_sixth):
            A.append(C[l][0])
            B.append(C[l][1])
        with open("orders/Experiment2_subject" + str(i * 6 + k + 1) + "_order.csv", 'w') as f:
            writer = csv.writer(f)
            writer.writerow(A)
            writer.writerow(B)
