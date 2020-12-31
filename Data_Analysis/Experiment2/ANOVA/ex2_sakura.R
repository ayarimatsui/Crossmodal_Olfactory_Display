library(ggplot2)
library(ggsci)
library(tidyverse)

data_rating <- read.csv("ex2_sakura.csv", header=TRUE)
data_rating$鼻部皮膚温度 <- factor(data_rating$温度, levels = c('基準温度', '基準温度+2℃', '基準温度-2℃'))
g <- ggplot() + geom_boxplot(aes(x = 評価軸, y = 評価値, fill = 鼻部皮膚温度), data = data_rating, alpha = 0.7, width = 0.4, position = position_dodge(width = 0.6))
g <- g + scale_x_discrete(limit=c('甘い', '酸っぱい', '苦い', 'アッサリした', '爽快な', 'まとわりつく感じ'))
g <- g + theme_bw(base_family="HiraKakuProN-W3")
g <- g + scale_y_continuous(breaks=seq(0, 6, length=7), limits=c(0,6))
g <- g + scale_fill_manual(values = c("#6B8E23", "#CD5C5C", "#4682B4"))


# 有意差を書き込む
# アスタリスク
g <- g + geom_text(x = 4.1, y = 4.85, label = "*", size = 3) + geom_segment(x = 4, xend = 4, y = 4.8, yend = 4.7, size = 0.2) + geom_segment(x = 4, xend = 4.2, y = 4.8, yend = 4.8, size = 0.2) + geom_segment(x = 4.2, xend = 4.2, y = 4.8, yend = 4.7, size = 0.2)

g <- g + geom_text(x = 4, y = 5.05, label = "***", size = 3) + geom_segment(x = 3.75, xend = 3.75, y = 5, yend = 4.7, size = 0.2) + geom_segment(x = 3.75, xend = 4.25, y = 5, yend = 5, size = 0.2) + geom_segment(x = 4.25, xend = 4.25, y = 5, yend = 4.7, size = 0.2)

# ダガー
g <- g + geom_text(x = 2.1, y = 3.7, label = "†", size = 2.5) + geom_segment(x = 2, xend = 2, y = 3.6, yend = 3.53, size = 0.2) + geom_segment(x = 2, xend = 2.2, y = 3.6, yend = 3.6, size = 0.2) + geom_segment(x = 2.2, xend = 2.2, y = 3.6, yend = 3.53, size = 0.2)

g <- g + geom_text(x = 2, y = 3.9, label = "†", size = 2.5) + geom_segment(x = 1.75, xend = 1.75, y = 3.8, yend = 3.53, size = 0.2) + geom_segment(x = 1.75, xend = 2.25, y = 3.8, yend = 3.8, size = 0.2) + geom_segment(x = 2.25, xend = 2.25, y = 3.8, yend = 3.53, size = 0.2)

g <- g + geom_text(x = 5.1, y = 3.9, label = "†", size = 2.5) + geom_segment(x = 5, xend = 5, y = 3.8, yend = 3.7, size = 0.2) + geom_segment(x = 5, xend = 5.2, y = 3.8, yend = 3.8, size = 0.2) + geom_segment(x = 5.2, xend = 5.2, y = 3.8, yend = 3.7, size = 0.2)

g <- g + geom_text(x = 6, y = 6, label = "* : p<.05", size = 3)
g <- g + geom_text(x = 6, y = 5.8, label = "*** : p<.001", size = 3)
g <- g + geom_text(x = 6, y = 5.6, label = "† : p<.10", size = 3)

# タイトル追加
g <- g + ggtitle("においの感じ方の評価(香料：さくら)") + theme(plot.title = element_text(hjust = 0.5))

# グラフ保存
ggsave("graphs/ex2_sakura.png", width = 12, height = 9, dpi = 120)