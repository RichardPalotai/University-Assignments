N <- 10
deck <- rep(1:N, each = 2)
deck

simulate_once <- function(N, m){
  
  deck <- rep(1:N, each = 2)
  
  remaining <- deck[-sample(1:length(deck), m)]
  
  pairs_left <- sum(table(remaining) == 2)
  
  return(pairs_left)
}

N <- 10
m <- 6
runs <- 10000

results <- replicate(runs, simulate_once(N, m))

E_theory <- N * choose(2*N - 2, m) / choose(2*N, m)

mean(results)
E_theory