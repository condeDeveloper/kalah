module Conde.Kalah.JuizTestes

open Xunit
open Conde.Kalah

[<Fact>]
let ``o Kalah literal concorda com o motor em todo caso de ate 9 sementes por lado`` () =
    for m in 1 .. 4 do
        for n in 1 .. 6 do
            if m * n <= 9 then
                Assert.True((fst (Nucleo.resolve m n) = Juiz.placar m n), sprintf "Kalah(%d,%d)" m n)

[<Fact>]
let ``nenhuma semente aparece ou some em Kalah(3,3)`` () =
    let vistos, erros = Juiz.confereSementes 3 3
    Assert.Equal(27076, vistos)
    Assert.Equal(0, erros)

[<Fact>]
let ``a jogada literal: cair no armazem repete a vez`` () =
    let t = Juiz.tabuleiroInicial 3 3
    let depois, proximo = Juiz.joga 3 t Juiz.Sul 0
    Assert.Equal(Some Juiz.Sul, proximo)
    Assert.Equal<int[]>([| 0; 4; 4; 1; 3; 3; 3; 0 |], depois)

[<Fact>]
let ``com 1 cova e 3 sementes quem comeca leva as 6: a volta exata cai na cova vazia e captura`` () =
    Assert.Equal(6, Juiz.placar 1 3)
