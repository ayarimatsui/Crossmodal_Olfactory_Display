# 香料のクラスタリング

import pandas as pd
import matplotlib
import matplotlib.pyplot as plt
from scipy.cluster.hierarchy import linkage, dendrogram, fcluster
from matplotlib.font_manager import FontProperties
fp = FontProperties(fname=r'ipaexg.ttf')

# データ読み込み
df = pd.read_csv("aroma_features.csv",index_col=0)
 
linkage_result = linkage(df, method='ward', metric='euclidean')  # ward法，ユークリッド距離を使用
plt.figure(num=None, figsize=(12, 8), dpi=120, facecolor='w', edgecolor='k')
dendrogram(linkage_result, labels=df.index)

plt.title("香料のクラスター分析結果を示したデンドログラム", fontproperties=fp, fontsize=14)


for l in plt.gca().get_xticklabels(): 
    l.set_fontproperties(fp)

plt.show()