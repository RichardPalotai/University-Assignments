probV <- c(177, 160 ,151, 189, 154, 169)
probU <- c(188, 144, 165, 175, 138, 190)

n <- 1000
m <- 1000

alpha <- 0.05
r <- 6

T_1 <- n*m
T_2 <- 0
for(i in 1:6){
  T_2 <- T_2+((probV[i]/1000)-(probU[i]/1000))^2/(probV[i]+probU[i])
}

T_1*T_2

qchisq(1-alpha, df=r-1)