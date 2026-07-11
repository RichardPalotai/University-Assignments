sampA <- c(7.64, 7.43, 5.82, 7.02, 7.36, 7.38, 6.69, 7.18, 6.36, 5.99, 7.24, 6.11, 6.68, 7.69, 6.96, 6.9, 6.44, 7.00, 6.83, 6.2, 6.13, 7.28, 7.22, 6.8, 6.26, 6.87, 6.29, 6.97, 6.73, 6.7);
sampB <- c(8.29, 7.19, 7.27, 7.2, 7.01, 7.54, 8.13, 7.66, 7.57, 7.23, 6.74, 8.3, 6.86, 7.23, 6.6, 7.69, 7.6, 7.6, 6.99, 7.12);
mean(sampA)
mean(sampB)

n <- length(sampA)
xavg <- mean(sampA)
sigma1 <- 0.4

m <- length(sampB)
yavg <- mean(sampB)
sigma2 <- 0.6

alpha <- 0.05

T <- ((xavg-yavg)/sqrt((sigma1^2/n)+(sigma2^2/m)))

if (abs(T) > qnorm(1 - alpha/2)) {
  print("H0-t elutasítjuk, tehát szignifikáns különbség")
} else {
  print("H0-t elfogadjuk, tehát nincs szignifikáns különbség")
}

p_value <- 2*(1 - pnorm(abs(T)))

if (p_value < alpha) {
  print("H0-t elutasítjuk, tehát szignifikáns különbség (megerősítés)")
} else {
  print("H0-t elfogadjuk, tehát nincs szignifikáns különbség (megerősítés)")
}
