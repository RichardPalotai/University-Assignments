cars

model <- lm(dist ~ speed, data = cars)
summary(model)

plot(cars$speed, cars$dist)
abline(model)

y <- cars$dist
x <- cars$speed
avgY <- mean(cars$dist)
avgX <- mean(cars$speed)
n <- length(cars$speed)

b_1 <- 0
b_2 <- 0
for(i in 1:n){
  b_1 <- b_1+(x[i]-avgX)*(y[i]-avgY)
  b_2 <- b_2+(x[i]-avgX)^2
}

estB <-  b_1 / b_2

estA <- avgY-b*avgX

estY_i <- estA + estB*x
estE_i <- y - estY_i
estE_i