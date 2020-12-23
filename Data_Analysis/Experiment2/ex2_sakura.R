library(ggplot2)
library(ggsci)

data_rating <- read.csv("ex2_sakura.csv", header=TRUE)
data_rating$鼻部皮膚温度 <- factor(data_rating$温度, levels = c('基準温度', '基準温度+2℃', '基準温度-2℃'))
g <- ggplot(data_rating, aes(x = 評価軸, y = 評価値, fill = 鼻部皮膚温度))
g <- g + geom_bar(width = 0.7, position = position_dodge(width = 0.7), stat = "identity")
g <- g + scale_x_discrete(limit=c('甘い', '酸っぱい', '苦い', 'アッサリした', '爽快な', 'まとわりつく感じ'))
g <- g + theme_bw(base_family="HiraKakuProN-W3")
g <- g + scale_y_continuous(breaks=seq(0, 6, length=7), limits=c(0,6))
g <- g + scale_fill_manual(values = c("#6B8E23", "#CD5C5C", "#4682B4"))
# 有意差を書き込む
# アスタリスク
g <- g + geom_text(x = 0.88, y = 4.5, label = "***", size = 3) + geom_segment(x = 0.775, xend = 0.775, y = 4.45, yend = 4.35, size = 0.2) + geom_segment(x = 0.775, xend = 1, y = 4.45, yend = 4.45, size = 0.2) + geom_segment(x = 1, xend = 1, y = 4.45, yend = 4.35, size = 0.2)

g <- g + geom_text(x = 6, y = 6, label = "* : p<.05", size = 3)
g <- g + geom_text(x = 6, y = 5.8, label = "*** : p<.001", size = 3)
g <- g + geom_text(x = 6, y = 5.6, label = "† : p<.10", size = 3)

# タイトル追加
g <- g + ggtitle("においの感じ方の評価(香料：さくら)") + theme(plot.title = element_text(hjust = 0.5))

plot(g)