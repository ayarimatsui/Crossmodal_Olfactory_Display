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

df_w = pd.DataFrame(columns=['sweet', 'sour', 'bitter', 'light', 'fresh', 'sticky'])  # 温めた時の変化用
df_c = pd.DataFrame(columns=['sweet', 'sour', 'bitter', 'light', 'fresh', 'sticky'])  # 冷やした時の変化用
df_n_and_w = pd.DataFrame(columns=['sweet', 'sour', 'bitter', 'light', 'fresh', 'sticky'])  # 基準温度，基準温度+2℃のデータを保存
df_n_and_c = pd.DataFrame(columns=['sweet', 'sour', 'bitter', 'light', 'fresh', 'sticky'])  # 基準温度，基準温度-2℃のデータを保存

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
        plt.annotate(k, xy=(x, y), fontproperties=fp, fontsize=6)

plt.grid()
plt.xlabel("PC1(鼻部を温めた時)", fontproperties=fp, fontsize=10)
plt.ylabel("PC2(鼻部を温めた時)", fontproperties=fp, fontsize=10)
plt.title("基準温度・基準温度+2℃の条件下でのデータを主成分分析した結果", fontproperties=fp, fontsize=14)
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
        plt.annotate(k, xy=(x, y), fontproperties=fp, fontsize=6)

plt.grid()
plt.xlabel("PC1(鼻部を冷やした時)", fontproperties=fp, fontsize=10)
plt.ylabel("PC2(鼻部を冷やした時)", fontproperties=fp, fontsize=10)
plt.title("基準温度・基準温度-2℃の条件下でのデータを主成分分析した結果", fontproperties=fp, fontsize=14)
plt.show()
