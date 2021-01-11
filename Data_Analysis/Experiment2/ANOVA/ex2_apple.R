library(ggplot2)
library(ggsci)
library(tidyverse)
library(ggsignif)
library(reshape2)


data_rating <- read.csv("ex2_apple.csv", header=TRUE)
data_rating$鼻部皮膚温度 <- factor(data_rating$温度, levels = c('基準温度', '基準温度+2℃', '基準温度-2℃'))
g <- ggplot(data = data_rating, aes(x = 評価軸, y = 評価値)) + geom_boxplot(aes(fill = 鼻部皮膚温度), alpha = 0.7, width = 0.4, position = position_dodge(width = 0.6))

g <- g + scale_x_discrete(limit=c('甘い', '酸っぱい', '苦い', 'アッサリした', '爽快な', 'まとわりつく感じ'))
g <- g + theme_bw(base_family="HiraKakuProN-W3")
g <- g + scale_y_continuous(breaks=seq(0, 6, length=7), limits=c(0,6.3))
g <- g + scale_fill_manual(values = c("#6B8E23", "#CD5C5C", "#4682B4"))



# 有意差を書き込む
# アスタリスク
g <- g + geom_text(x = 4, y = 6.22, label = "*", size = 5, color='red') + geom_segment(x = 3.8, xend = 3.8, y = 6.19, yend = 6.16, size = 0.2, color='red') + geom_segment(x = 3.8, xend = 4.2, y = 6.19, yend = 6.19, size = 0.2, color='red') + geom_segment(x = 4.2, xend = 4.2, y = 6.19, yend = 6.16, size = 0.2, color='red')

g <- g + geom_text(x = 4.1, y = 6.1, label = "*", size = 5, color='red') + geom_segment(x = 4, xend = 4, y = 6.07, yend = 6.04, size = 0.2, color='red') + geom_segment(x = 4, xend = 4.2, y = 6.07, yend = 6.07, size = 0.2, color='red') + geom_segment(x = 4.2, xend = 4.2, y = 6.07, yend = 6.04, size = 0.2, color='red')

#g <- g + geom_text(x = 6, y = 6, label = "* : p<.05", size=3)

# タイトル追加
g <- g + ggtitle("においの感じ方の評価(香料：アップル)") + theme(plot.title = element_text(hjust = 0.5))


# グラフ保存
ggsave("graphs/ex2_apple.png", width = 12, height = 9, dpi = 120)