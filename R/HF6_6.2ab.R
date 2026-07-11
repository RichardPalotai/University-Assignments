zk2 <- c(3, 5, 11, 8, 10, 12, 6, 5, 10, 14)
kk2 <- c(2, 2, 2, 0, 2, 1, 1, 0, 1, 2)
pk3 <- c(1, 3, 5, 3, 2, 0, 0, 0, 1, 3)

adat <- data.frame(zk2, kk2, pk3)

boxplot(adat,
        main = "Boxplot of a)",
        xlab = "Variables",
        ylab = "Values")

# Következtetés:
# 2zk : IQR nagy => adatok szóródása nagy;
#.      |L-U| nagy => az értékek között nagy az ingadozás meccsről meccsre (többihez képest);
#.      Szélsőséges értékek mindkét irányban vannak;
#.      |Me-Q3|<|Me-Q1| => Az adatok szóródása nagyobb Medián és Q1 között IQR-ben;
# 2kk : IQR kicsi => adatok szóródása kicsi;
#.      |L-U| kicsi => az értékek között kicsi az ingadozás meccsről meccsre (többihez képest);
#.      Szélsőséges érték L felé fordult elő egy-két 0 pontos dobás;
#.      |Me-Q3|=|Me-Q1| => Az adatok szóródása kiegyensúlyozott;
# 3pk : IQR közepes => adatok szóródása közepes a többi boxplothoz képest;
#.      |L-U| közepes => az értékek között közepes (többihez képest);
#.      Szélsőséges értékek U felé fordultak elő 4-5 pontos dobásokkal;
#.      |Me-Q3|=|Me-Q1| => Az adatok szóródása kiegyensúlyozott;
# OVERALL: A játék során a legváltozatosabb 2zk értékei voltak, és 2kk és 3pk értékei kiegyensúlyozottak kisebb kilengésekkel

ido <- c(18, 27, 29, 31, 40, 30, 23, 22, 35, 36)
pt  <- c(10, 9, 27, 24, 21, 20, 11, 12, 21, 24)
zk2 <- c(3, 5, 11, 8, 10, 12, 6, 5, 10, 14)

adat <- data.frame(ido, pt, zk2)

boxplot(adat,
        main = "Boxplot of b)",
        xlab = "Variables",
        ylab = "Values")

# Következtetés:
# Idő : IQR nagy => adatok szóródása nagy;
#.      |L-U| nagy => az értékek között nagy az ingadozás meccsről meccsre (többihez képest);
#.      Szélsőséges értékek mindkét irányban vannak;
#.      |Me-Q3|<|Me-Q1| => Az adatok szóródása nagyobb Medián és Q1 között IQR-ben;
# Pt : IQR nagy => adatok szóródása nagy;
#.      |L-U| közepes => az értékek között közepes az ingadozás meccsről meccsre (többihez képest);
#.      Szélsőséges értékek mindkét irányban vannak;
#.      |Me-Q3|<|Me-Q1| => Az adatok szóródása nagyobb Medián és Q1 között IQR-ben;
# 2zk : IQR kicsi => adatok szóródása kicsi a többi boxplothoz képest;
#.      |L-U| kicsi => az értékek között kicsi (többihez képest);
#.      Szélsőséges értékek mindkét irányban vannak;
#.      |Me-Q3|<|Me-Q1| => Az adatok szóródása nagyobb Medián és Q1 között IQR-ben;
# OVERALL: A játék során a pályán töltött idő közel kiegyensúlyozott volt voltak szélsőséges értékek mindkét irányban, pt zömében 20 pont alatt volt mindkét irányban kilengésekkel, 2zk zömében 10 pont alatt volt mindkét irányba kilengésekkel.
