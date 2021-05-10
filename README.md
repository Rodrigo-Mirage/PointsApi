# PointsApi

Essa solução está semaprada em duas aplicações, e é iniciada por meio de docker composer e pode ser iniciado com uma das duas formas:

> docker composer up
> docker-composer up

para fins de teste em localhost, a parte da api pode ser iniciada com

> dotnet run

## Swagger
A Api possui acesso ao swagger e necessita de autenticação para algumas funções, para utilizar faça o login e inclua "Bearer " antes do token apertando o botão Authorize no canto superior direito.

o swagger pode ser acessado em:

https://localhost:5001/swagger/index.html

## Postman

Segue uma sequencia de requests para a API, apontadas para o localhost:5001
https://www.getpostman.com/collections/4238587a2a38e34829d6
