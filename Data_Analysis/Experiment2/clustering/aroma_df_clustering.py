# 鼻部皮膚温度制御によるにおいの変化の仕方によって，香料をクラスタリング
# 温めたとき／冷やしたとき

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

df_w = pd.DataFrame(columns=['甘い', '酸っぱい', '苦い', 'アッサリした', '爽やかな', 'まとわりつく感じ'])  # 温めた時の変化用
df_c = pd.DataFrame(columns=['甘い', '酸っぱい', '苦い', 'アッサリした', '爽やかな', 'まとわりつく感じ'])  # 冷やした時の変化用
df_n_and_w = pd.DataFrame(columns=['甘い', '酸っぱい', '苦い', 'アッサリした', '爽やかな', 'まとわりつく感じ'])  # 基準温度，基準温度+2℃のデータを保存
df_n_and_c = pd.DataFrame(columns=['甘い', '酸っぱい', '苦い', 'アッサリした', '爽やかな', 'まとわりつく感じ'])  # 基準温度，基準温度-2℃のデータを保存

name_list = ['1アップル', '2チェリー', '3グレープフルーツ', '4ラズベリー', '5さくら', '6マスカット']
for i in range(len(df) // 3):
    tmp_se_w = pd.Series((df.values[3 * i + 1] - df.values[3 * i]) / np.linalg.norm(df.values[3 * i + 1] - df.values[3 * i]), index=df_w.columns, name=name_list[i]) # 温めた時の変位ベクトルを正規化して方向ベクトルにして追加
    tmp_se_c = pd.Series((df.values[3 * i + 2] - df.values[3 * i]) / np.linalg.norm(df.values[3 * i + 2] - df.values[3 * i]), index=df_c.columns, name=name_list[i]) # 冷やした時の変位ベクトルを正規化して方向ベクトルにして追加
    df_w = df_w.append(tmp_se_w)
    df_c = df_c.append(tmp_se_c)
    tmp_se_n_and_w = pd.Series(df.values[3 * i], index=df_n_and_w.columns, name=name_list[i]+'基準温度') # 基準温度のデータを追加
    tmp_se_n_and_c = pd.Series(df.values[3 * i], index=df_n_and_w.columns, name=name_list[i]+'基準温度') # 基準温度のデータを追加
    df_n_and_w = df_n_and_w.append(tmp_se_n_and_w)
    df_n_and_c = df_n_and_c.append(tmp_se_n_and_c)
    tmp_se_n_and_w = pd.Series(df.values[3 * i + 1], index=df_n_and_w.columns, name=name_list[i]+'+2℃') # 基準温度+2℃のデータを追加
    tmp_se_n_and_c = pd.Series(df.values[3 * i + 2], index=df_n_and_c.columns, name=name_list[i]+'-2℃') # 基準温度+2℃のデータを追加
    df_n_and_w = df_n_and_w.append(tmp_se_n_and_w)
    df_n_and_c = df_n_and_c.append(tmp_se_n_and_c)


# 鼻部を温めた時の変化の仕方でクラスタリング
# デンドログラム作成
linkage_result_w = linkage(df_w, method='ward', metric='euclidean')  # ward法，ユークリッド距離を使用
plt.figure(num=None, figsize=(12, 8), dpi=120, facecolor='w', edgecolor='k')
dendrogram_fig_w = dendrogram(linkage_result_w, labels=df_w.index, leaf_font_size=10)

plt.title("鼻部を温めた時の変化の仕方で香料をクラスター分析した結果を示したデンドログラム", fontproperties=fp, fontsize=14)
for l in plt.gca().get_xticklabels(): 
    l.set_fontproperties(fp)
    l.set_fontsize(10)
plt.show()

# 基準温度条件，基準温度+2℃のデータを主成分分析して二次元にマッピング
# 色分けはデンドログラムに合わせる

# 色の設定うんうん
colors_w = ['none'] * len(df_w)
for xs, c in zip(dendrogram_fig_w['icoord'], dendrogram_fig_w['color_list']):
    for xi in xs:
        if xi % 10 == 5:
            colors_w[(int(xi)-5) // 10] = c

colors_w_dic = {}
for condition, c_cluster in zip(dendrogram_fig_w["ivl"], colors_w):
    colors_w_dic[condition] = c_cluster

new_colors_w_dic = sorted(colors_w_dic.items())
c_list_w = []
for k, v in new_colors_w_dic:
    c_list_w.append(v)
    c_list_w.append(v) #ここでカラーリストを二倍しておく

print(c_list_w)

# PCA
pca = PCA()
pca.fit(df_n_and_w) #基準温度，基準温度+2℃のデータをマッピング
# データを主成分空間に写像
feature = pca.transform(df_n_and_w)
#print(feature)
plt.figure(figsize=(8, 6))
plt.scatter(feature[:, 0], feature[:, 1], color=c_list_w, s=60, alpha=0.8)


for (x,y,k,p_c) in zip(feature[:, 0], feature[:, 1], df_n_and_w.index, c_list_w):
        plt.plot(x,y,'o', c=p_c)
        plt.annotate(k, xy=(x, y), fontproperties=fp, fontsize=7)

plt.grid()
plt.xlabel("PC1(基準温度・基準温度+2℃)", fontproperties=fp, fontsize=10)
plt.ylabel("PC2(基準温度・基準温度+2℃)", fontproperties=fp, fontsize=10)
plt.title("基準温度・基準温度+2℃の条件下でのデータを主成分分析した結果", fontproperties=fp, fontsize=14)
plt.show()

#主成分の寄与率を調べる
print(pd.DataFrame(pca.explained_variance_ratio_, index=["PC{}".format(x + 1) for x in range(len(df_n_and_w.columns))]))
'''
PC1  0.552552
PC2  0.226145
PC3  0.114498
PC4  0.062364
PC5  0.040359
PC6  0.004083
'''

# 累積寄与率を図示する
import matplotlib.ticker as ticker
plt.figure(figsize=(7, 6))
plt.gca().get_xaxis().set_major_locator(ticker.MaxNLocator(integer=True))
plt.plot([0] + list( np.cumsum(pca.explained_variance_ratio_)), "-o")
plt.xlabel("主成分", fontproperties=fp, fontsize=10)
plt.ylabel("累積寄与率", fontproperties=fp, fontsize=10)
plt.title("主成分の累積寄与率 (基準温度・基準温度+2℃)", fontproperties=fp, fontsize=12)
plt.grid()
plt.show()

# PCA の固有ベクトル
print(pd.DataFrame(pca.components_, columns=df_n_and_w.columns, index=["PC{}".format(x + 1) for x in range(len(df_n_and_w.columns))]))
'''
PC1  0.193163  0.049984 -0.582593  0.355697  0.545643 -0.443314
PC2  0.022118  0.578970 -0.594409 -0.369686 -0.132992  0.395763
PC3 -0.608198  0.629095  0.216765  0.359651 -0.038481 -0.237737
PC4  0.520657  0.451702  0.409894 -0.410499  0.148111 -0.407949
PC5  0.142824  0.169575  0.303746  0.253428  0.616194  0.643944
PC6 -0.548463 -0.183699 -0.001317 -0.612150  0.530541 -0.096120
'''

# 第一主成分と第二主成分における観測変数の寄与度をプロットする
plt.figure(figsize=(7, 6))
for x, y, name in zip(pca.components_[0], pca.components_[1], df_n_and_w.columns):
    plt.text(x, y, name, fontproperties=fp, fontsize=9)
plt.scatter(pca.components_[0], pca.components_[1], alpha=0.8)
plt.grid()
plt.xlabel("PC1(基準温度・基準温度+2℃の条件下)", fontproperties=fp, fontsize=10)
plt.ylabel("PC2(基準温度・基準温度+2℃の条件下)", fontproperties=fp, fontsize=10)
plt.title("第一主成分と第二主成分における観測変数の寄与度 (基準温度・基準温度+2℃)", fontproperties=fp, fontsize=12)
plt.show()


# 鼻部を冷やした時の変化の仕方でクラスタリング
linkage_result_c = linkage(df_c, method='ward', metric='euclidean')  # ward法，ユークリッド距離を使用
plt.figure(num=None, figsize=(12, 8), dpi=120, facecolor='w', edgecolor='k')
dendrogram_fig_c = dendrogram(linkage_result_c, labels=df_c.index, leaf_font_size=10)

plt.title("鼻部を冷やした時の変化の仕方で香料をクラスター分析した結果を示したデンドログラム", fontproperties=fp, fontsize=14)
for l in plt.gca().get_xticklabels(): 
    l.set_fontproperties(fp)
    l.set_fontsize(10)
plt.show()

# 基準温度条件，基準温度-2℃のデータを主成分分析して二次元にマッピング
# 色分けはデンドログラムに合わせる

# 色の設定うんうん
colors_c = ['none'] * len(df_w)
for xs, c in zip(dendrogram_fig_c['icoord'], dendrogram_fig_c['color_list']):
    for xi in xs:
        if xi % 10 == 5:
            colors_c[(int(xi)-5) // 10] = c

colors_c_dic = {}
for condition, c_cluster in zip(dendrogram_fig_c["ivl"], colors_c):
    colors_c_dic[condition] = c_cluster

new_colors_c_dic = sorted(colors_c_dic.items())
c_list_c = []
for k, v in new_colors_c_dic:
    c_list_c.append(v)
    c_list_c.append(v) #ここでカラーリストを二倍しておく

print(c_list_c)

# PCA
pca = PCA()
pca.fit(df_n_and_c) #基準温度，基準温度+2℃のデータをマッピング
# データを主成分空間に写像
feature = pca.transform(df_n_and_c)
#print(feature)
plt.figure(figsize=(8, 6))
plt.scatter(feature[:, 0], feature[:, 1], color=c_list_c, s=60, alpha=0.8)


for (x,y,k,p_c) in zip(feature[:, 0], feature[:, 1], df_n_and_c.index, c_list_c):
        plt.plot(x,y,'o', c=p_c)
        plt.annotate(k, xy=(x, y), fontproperties=fp, fontsize=7)

plt.grid()
plt.xlabel("PC1(基準温度・基準温度-2℃)", fontproperties=fp, fontsize=10)
plt.ylabel("PC2(基準温度・基準温度-2℃)", fontproperties=fp, fontsize=10)
plt.title("基準温度・基準温度-2℃の条件下でのデータを主成分分析した結果", fontproperties=fp, fontsize=14)
plt.show()

#主成分の寄与率を調べる
print(pd.DataFrame(pca.explained_variance_ratio_, index=["PC{}".format(x + 1) for x in range(len(df_n_and_c.columns))]))
'''
PC1  0.674430
PC2  0.154048
PC3  0.086960
PC4  0.050463
PC5  0.024904
PC6  0.009195
'''

# 累積寄与率を図示する
import matplotlib.ticker as ticker
plt.figure(figsize=(7, 6))
plt.gca().get_xaxis().set_major_locator(ticker.MaxNLocator(integer=True))
plt.plot([0] + list( np.cumsum(pca.explained_variance_ratio_)), "-o")
plt.xlabel("主成分", fontproperties=fp, fontsize=10)
plt.ylabel("累積寄与率", fontproperties=fp, fontsize=10)
plt.title("基準温度・基準温度-2℃の条件下での主成分の累積寄与率 (基準温度・基準温度-2℃)", fontproperties=fp, fontsize=12)
plt.grid()
plt.show()

# PCA の固有ベクトル
print(pd.DataFrame(pca.components_, columns=df_n_and_c.columns, index=["PC{}".format(x + 1) for x in range(len(df_n_and_c.columns))]))
'''
PC1  0.220188  0.138813 -0.592025  0.428226  0.535149 -0.334653
PC2 -0.526158 -0.267625  0.303148  0.747563  0.027224  0.006635
PC3 -0.073902 -0.356463 -0.612097  0.069519 -0.189442  0.672375
PC4  0.479320 -0.830296  0.147793 -0.019493  0.048953 -0.237152
PC5  0.488250  0.197210  0.398670  0.233379  0.380003  0.604081
PC6 -0.448417 -0.231857  0.046531 -0.445077  0.728139  0.121325
'''

# 第一主成分と第二主成分における観測変数の寄与度をプロットする
plt.figure(figsize=(7, 6))
for x, y, name in zip(pca.components_[0], pca.components_[1], df_n_and_c.columns):
    plt.text(x, y, name, fontproperties=fp, fontsize=9)
plt.scatter(pca.components_[0], pca.components_[1], alpha=0.8)
plt.grid()
plt.xlabel("PC1(基準温度・基準温度-2℃)", fontproperties=fp, fontsize=10)
plt.ylabel("PC2(基準温度・基準温度-2℃)", fontproperties=fp, fontsize=10)
plt.title("第一主成分と第二主成分における観測変数の寄与度 (基準温度・基準温度-2℃)", fontproperties=fp, fontsize=12)
plt.show()