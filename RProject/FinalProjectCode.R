#---------------------------------------------------------------------------------------#
# Wen Bo Yu - 100579309
# Daniel MacCormick - 100580519
# Game Analytics Final Project

# Our data has 2 independent variables (Group and NotificationType)
# It has 1 dependent variable (AvgErrorValue)
# Our study is repeated measures but participants are split into multiple groups
# The groups are counterbalanced using a 4x4 Latin Square
# Our within subject variable is NotificationType
# Our between subject variable is the Group
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


# 2b - Within subject factors should consist of at least two related groups
# Our within subject factor is the NotificationType
# NotificationType can be NONE, ICON, ANIMATION, or COLOUR which is at least two
# Therefore, this assumption PASSES


# 2c - Between subject factors should consist of at least two related groups
# Our between subject factor is the Group
# Group can be A, B, C, or D which is at least two
# Therefore, this assumption PASSES


# 2d - No significant outliers in any group of within subject factors or between subject factors
# We can visualize the outliers by creating a box and whisker plot
if (!require(ggpubr)) install.packages(("ggpubr"))
library(ggpubr)
print(ggplot(participantData, aes(x = ParticipantGroup, y = AvgErrorValue, fill = NotificationType)) + geom_boxplot() + ggtitle("Comparing User Error Between Groups and Notification Types"))
print(ggplot(participantData, aes(x = NotificationType, y = AvgErrorValue, fill = ParticipantGroup)) + geom_boxplot() + ggtitle("Comparing User Error Between Groups and Notification Types"))
# None of the box and whisker plots show any outliers so this assumption PASSES


# 2e - DV in each combination of related groups is approximately normally distributed
# We can check normality using the Shapiro-Wilk test
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
# Results: 
# Group A~Animation is not normally distributed. However, it has a p-value of 0.02 which is reasonably close to being normally distributed
# All the others are confirmed normally distributed by the Shapiro-Wilk tests
# This assumption PASSES, outside of the one p-value. After conferring with the prof, it is confirmed to be PASSED


# 2f - There needs to be homogeneity of variance for each combination
# We can check this with the Levene's test
if (!require(car)) install.packages("car")
library(car)
print(leveneTest(AvgErrorValue~ParticipantGroup, data = participantData))
print(leveneTest(AvgErrorValue~ParticipantGroup * NotificationType, data = participantData))
# Results:
# Test 1 -> p-value = 0.9454, so this PASSES
# Test 2 -> p-value = 0.9895, so this PASSES
# Since both Levene's tests have pssed, this assumption PASSES


# 2g - Sphericity must be equal
# We can confirm this with ezANOVA which is run below
# NotificationType -> p-value = 0.18
# ParticipantGroup:NotificationType -> p-value = 0.18
# Sphericity was equal and was not violated and so this assumption PASSES



#--- Step 3: Run the Mixed ANOVA Test ---#
if (!require(ez)) install.packages("ez")
library(ez)
print(ezANOVA(data = participantData, dv = .(AvgErrorValue), wid = ParticipantID, within = NotificationType, between = ParticipantGroup, detailed = T, type = 3))



#--- Step 4: Additional Visualizations ---#
print(ggplot(participantData, aes(x = NotificationType, y = AvgErrorValue)) + geom_boxplot() + ggtitle("Comparing Average Error By Notification Type"))