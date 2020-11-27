# 実験1-0
# 評価の信頼性を確かめるための実験
import random

pair_list = [[1, 1], [2, 2], [3, 3], [4, 4], [1, 2], [1, 3], [1, 4], [2, 3], [2, 4], [3, 4]]
random.shuffle(pair_list)

for pairs in pair_list:
    random.shuffle(pairs)
    print(pairs)