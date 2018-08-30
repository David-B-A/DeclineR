MejorARIMA <- function(produccion, d)
{

matrizAic <- matrix(nrow = 9, ncol = 9)
p=1
maxQ=9
q=1
  while(q <= maxQ)
  {
    a <- arima(produccion, order = c(p,d,q))
    matrizAic[p,q] <- a$aic
    q = q + 1
  }
p=2
q=1
while(q <= maxQ)
{
  a <- arima(produccion, order = c(p,d,q))
  matrizAic[p,q] <- a$aic
  q = q + 1
}
p=3
q=1
while(q <= maxQ)
{
  a <- arima(produccion, order = c(p,d,q))
  matrizAic[p,q] <- a$aic
  q = q + 1
}
p=4
q=1
while(q <= maxQ)
{
  a <- arima(produccion, order = c(p,d,q))
  matrizAic[p,q] <- a$aic
  q = q + 1
}
p=5
q=1
while(q <= maxQ)
{
  a <- arima(produccion, order = c(p,d,q))
  matrizAic[p,q] <- a$aic
  q = q + 1
}
p=6
q=1
while(q <= maxQ)
{
  a <- arima(produccion, order = c(p,d,q))
  matrizAic[p,q] <- a$aic
  q = q + 1
}
p=7
q=1
while(q <= maxQ)
{
  a <- arima(produccion, order = c(p,d,q))
  matrizAic[p,q] <- a$aic
  q = q + 1
}
p=8
q=1
while(q <= maxQ)
{
  a <- arima(produccion, order = c(p,d,q))
  matrizAic[p,q] <- a$aic
  q = q + 1
}
p=9
q=1
while(q <= maxQ)
{
  a <- arima(produccion, order = c(p,d,q))
  matrizAic[p,q] <- a$aic
  q = q + 1
}

mejorAic <- matrizAic[1,1]

p=1
q=2
pMejorAic <- p
qMejorAic <- q
i <- 1
iMejorAic <- i
while(i <= 81)
{
    if(matrizAic[i] < mejorAic)
    {
      mejorAic <-matrizAic[i]
      iMejorAic <- i
    }
  i =i+1
}
pMejorAic = (iMejorAic - 1) %% 9 +1
qMejorAic = (iMejorAic - 1) %/% 9 +1


mejorModelo <- arima(produccion, order = c(pMejorAic,d,qMejorAic))
devolver <- list("modelo" = mejorModelo, "p" = pMejorAic, "q" = qMejorAic, "matriz" = matrizAic)
return(devolver)
}
