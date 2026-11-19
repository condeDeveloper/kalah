/// Os casos caros: Kalah(4,4) leva uns 20 s e guarda 5,6 milhoes de posicoes.
/// O CI roda num passo separado (filtro `FullyQualifiedName~Pesados`).
module Conde.Kalah.PesadosTestes

open Xunit
open Conde.Kalah.Medidas

[<Fact>]
let ``Kalah(4,4): quem comeca ganha por 2, com 5.641.244 posicoes`` () =
    Assert.Equal((2, 5641244), resolvido 4 4)

[<Fact>]
let ``Kalah(5,2): quem comeca ganha por 2, com 3.390.069 posicoes`` () =
    Assert.Equal((2, 3390069), resolvido 5 2)

[<Fact>]
let ``Kalah(5,1) empata e Kalah(6,1) e de quem comeca por 2`` () =
    Assert.Equal((0, 20005), resolvido 5 1)
    Assert.Equal((2, 347773), resolvido 6 1)

[<Fact>]
let ``Kalah(4,4): so a primeira cova ganha, a segunda e a terceira empatam`` () =
    Assert.Equal<int[]>([| 2; 0; 0; -2 |], primeiras 4 4)
