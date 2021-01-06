# 香料を主成分分析してマッピング
# クラスタリングが対応したデンドログラムも出力する

import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
import sklearn
from sklearn.decomposition import PCA #主成分分析器
from matplotlib.font_manager import FontProperties
from scipy.cluster.hierarchy import linkage, dendrogram, fcluster
fp = FontProperties(fname=r'ipaexg.ttf')

# データ読み込み
df = pd.read_csv("aroma_features2.csv",index_col=0)
# データ標準化
#dfs = df.iloc[:, 1:].apply(lambda x: (x-x.mean())/x.std(), axis=0)
 
linkage_result = linkage(df, method='ward', metric='euclidean')  # ward法，ユークリッド距離を使用
dendrogram_fig = dendrogram(linkage_result, labels=df.index, leaf_font_size=6, color_threshold=2)

plt.show()

colors = ['none'] * len(df)
for xs, c in zip(dendrogram_fig['icoord'], dendrogram_fig['color_list']):
    for xi in xs:
        if xi % 10 == 5:
            colors[(int(xi)-5) // 10] = c

colors_dic = {}
for condition, c_cluster in zip(dendrogram_fig["ivl"], colors):
    colors_dic[condition] = c_cluster

new_colors_dic = sorted(colors_dic.items())
c_list = []
for k, v in new_colors_dic:
    c_list.append(v)

print(c_list)
pca = PCA()
pca.fit(df)
# データを主成分空間に写像
feature = pca.transform(df)
#print(feature)
plt.figure(figsize=(6, 6))
plt.scatter(feature[:, 0], feature[:, 1], color=c_list, s=60, alpha=0.8)


for (x,y,k,p_c) in zip(feature[:, 0], feature[:, 1], df.index, c_list):
        plt.plot(x,y,'o', c=p_c)
        plt.annotate(k, xy=(x, y))

plt.grid()
plt.xlabel("PC1")
plt.ylabel("PC2")
plt.show()