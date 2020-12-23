library(ggplot2)
library(ggsci)

data_rating <- read.csv("ex2_raspberry.csv", header=TRUE)
data_rating$鼻部皮膚温度 <- factor(data_rating$温度, levels = c('基準温度', '基準温度+2℃', '基準温度-2℃'))
g <- ggplot(data_rating, aes(x = 評価軸, y = 評価値, fill = 鼻部皮膚温度))
g <- g + geom_bar(width = 0.7, position = position_dodge(width = 0.7), stat = "identity")
g <- g + scale_x_discrete(limit=c('甘い', '酸っぱい', '苦い', 'アッサリした', '爽快な', 'まとわりつく感じ'))
g <- g + theme_bw(base_family="HiraKakuProN-W3")
g <- g + scale_y_continuous(breaks=seq(0, 6, length=7), limits=c(0,6))
g <- g + scale_fill_manual(values = c("#6B8E23", "#CD5C5C", "#4682B4"))
# 有意差を書き込む

# ダガー
g <- g + geom_text(x = 1.875, y = 4.5, label = "†", size = 2.5) + geom_segment(x = 1.775, xend = 1.775, y = 4.4, yend = 4.35, size = 0.2) + geom_segment(x = 1.775, xend = 2, y = 4.4, yend = 4.4, size = 0.2) + geom_segment(x = 2, xend = 2, y = 4.4, yend = 4.35, size = 0.2)

g <- g + geom_text(x = 6, y = 3.9, label = "†", size = 2.5) + geom_segment(x = 5.775, xend = 5.775, y = 3.8, yend = 3.7, size = 0.2) + geom_segment(x = 5.775, xend = 6.225, y = 3.8, yend = 3.8, size = 0.2) + geom_segment(x = 6.225, xend = 6.225, y = 3.8, yend = 3.7, size = 0.2)

g <- g + geom_text(x = 6, y = 6, label = "* : p<.05", size = 3)
g <- g + geom_text(x = 6, y = 5.8, label = "*** : p<.001", size = 3)
g <- g + geom_text(x = 6, y = 5.6, label = "† : p<.10", size = 3)

# タイトル追加
g <- g + ggtitle("においの感じ方の評価(香料：ラズベリー)") + theme(plot.title = element_text(hjust = 0.5))

plot(g)