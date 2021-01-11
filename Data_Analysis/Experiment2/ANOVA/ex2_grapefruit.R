library(ggplot2)
library(ggsci)
library(tidyverse)

data_rating <- read.csv("ex2_grapefruit.csv", header=TRUE)
data_rating$鼻部皮膚温度 <- factor(data_rating$温度, levels = c('基準温度', '基準温度+2℃', '基準温度-2℃'))
g <- ggplot(data = data_rating, aes(x = 評価軸, y = 評価値)) + geom_boxplot(aes(fill = 鼻部皮膚温度), alpha = 0.7, width = 0.4, position = position_dodge(width = 0.6))
g <- g + scale_x_discrete(limit=c('甘い', '酸っぱい', '苦い', 'アッサリした', '爽快な', 'まとわりつく感じ'))
g <- g + theme_bw(base_family="HiraKakuProN-W3")
g <- g + scale_y_continuous(breaks=seq(0, 6, length=7), limits=c(0,6.3))
g <- g + scale_fill_manual(values = c("#6B8E23", "#CD5C5C", "#4682B4"))


# 有意差を書き込む
# アスタリスク
g <- g + geom_text(x = 4.1, y = 5.2, label = "*", size = 5, color='red') + geom_segment(x = 4, xend = 4, y = 5.17, yend = 5.14, size = 0.2, color='red') + geom_segment(x = 4, xend = 4.2, y = 5.17, yend = 5.17, size = 0.2, color='red') + geom_segment(x = 4.2, xend = 4.2, y = 5.17, yend = 5.14, size = 0.2, color='red')

# タイトル追加
g <- g + ggtitle("においの感じ方の評価(香料：グレープフルーツ)") + theme(plot.title = element_text(hjust = 0.5))

# グラフ保存
ggsave("graphs/ex2_grapefruit.png", width = 12, height = 9, dpi = 120)