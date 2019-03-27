#---------------------------------------------------------------------------------------#
# Wen Bo Yu - 100579309
# Daniel MacCormick - 100580519
# Game Analytics Final Project

# Our data has 2 independent variables (Group and NotificationType)
# It has 1 dependent variable (AvgErrorValue)
# Our study is repeated measures but participants are split into multiple groups
# The groups are counterbalanced using a 4x4 Latin Square
# Therefore, the best test for us to perform is the Mixed ANOVA
#---------------------------------------------------------------------------------------#



##--- Step 1 - Load in the Data ---##
# Going to use the 'here' library so the data can be read in using a relative file path
if (!require(here)) install.packages("here")
library(here)

# Read in the data from the CSV
filePath = here("FinalProjectData.csv")
participantData = read.csv(filePath)



#--- Step 2: Verify the Assumptions for Mixed ANOVA ---#
# 2a - DV Is Continuous
# Our DV, AvgErrorValue, is a continous float so this assumption PASSES


# 2b - Two IV consist of at least two related groups
# The first IV is ParticipantGroup, which has 4 possibilities (A,B,C,D) so this PASSES
# The second IV is NotificationType, which has 4 possibilities as well (NONE, ANIMATION, ICON, COLOUR) so this PASSES
# Therefore, this assumption PASSES


# 2c - No significant outliers
# We can visualize the outliers by creating a box and whisker plot
if (!require(ggpubr)) install.packages(("ggpubr"))
library(ggpubr)
print(ggplot(participantData, aes(x = ParticipantGroup, y = AvgErrorValue, fill = NotificationType)) + geom_boxplot() + ggtitle("Comparing User Error Between Groups and Notification Types"))
print(ggplot(participantData, aes(x = NotificationType, y = AvgErrorValue, fill = ParticipantGroup)) + geom_boxplot() + ggtitle("Comparing User Error Between Groups and Notification Types"))
# None of the box and whisker plots show any outliers so this assumption PASSES


# 2d - DV In Each Combination of Related Groups is Approximately Normally Distributed
for (ParticipantGroup in levels(participantData$ParticipantGroup))
{
  for (NotificationType in levels(participantData$NotificationType))
  {
    cat(sprintf("Shapiro-Wilk test for group:%s, notification:%s\n", ParticipantGroup, NotificationType))
    set <-participantData[participantData$ParticipantGroup == ParticipantGroup & participantData$NotificationType== NotificationType,]
    print(set)
    sw<-shapiro.test(set$AvgErrorValue)
    print(sw)
    is_normal<-sw$p.value> 0.05
    
    if (is_normal) print ("Normally distributed") else print ("NOT normally distributed!")
    cat("\n\n")
  }
}
# Results: Group A~Animation are not normally distributed. All the others are


# 2e - No violations of spericity in all combinations of groups
# EzANOVA performs the evaluation of the spericity so we will run that next
if (!require(ez)) install.packages("ez")
library(ez)
ezModel = ezANOVA(data = participantData, dv = AvgErrorValue, wid=ParticipantID, between=.(ParticipantGroup, NotificationType), detailed = T, type = 3)
print(ezModel)

