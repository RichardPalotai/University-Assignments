mtcars

model <- lm(mpg ~ wt, data = mtcars)
summary(model)

plot(mtcars$wt, mtcars$mpg)
abline(model)

y <- mtcars$mpg
x <- mtcars$wt
avgY <- mean(mtcars$mpg)
avgX <- mean(mtcars$wt)
n <- length(mtcars$wt)

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

avgY
avgX
n
estB
estA