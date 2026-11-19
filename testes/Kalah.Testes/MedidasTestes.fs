/// Os numeros do README, presos.
module Conde.Kalah.MedidasTestes

open Xunit
open Conde.Kalah.Medidas

let esperado =
    [ (1, 1), (0, 1); (1, 2), (-2, 1); (1, 3), (6, 1); (1, 4), (-2, 2); (1, 5), (2, 3); (1, 6), (0, 5)
      (2, 1), (2, 6); (2, 2), (-2, 19); (2, 3), (-2, 92); (2, 4), (-10, 101); (2, 5), (6, 98); (2, 6), (8, 842)
      (3, 1), (0, 57); (3, 2), (4, 1322); (3, 3), (2, 7448); (3, 4), (2, 39796); (3, 5), (6, 125648); (3, 6), (-8, 218997)
      (4, 1), (2, 1106); (4, 2), (6, 66265); (4, 3), (6, 843503) ]

[<Fact>]
let ``a tabela dos casos leves`` () =
    Assert.Equal(esperado.Length, leves.Length)
    for (m, n), (placar, posicoes) in esperado do
        Assert.True((resolvido m n = (placar, posicoes)), sprintf "Kalah(%d,%d) deu %A" m n (resolvido m n))

[<Fact>]
let ``Kalah(2,4): o segundo jogador ganha por 10 de 16 sementes`` () =
    Assert.Equal(-10, fst (resolvido 2 4))

[<Fact>]
let ``Kalah(3,6): todas as tres primeiras jogadas perdem`` () =
    Assert.Equal<int[]>([| -8; -10; -12 |], primeiras 3 6)

[<Fact>]
let ``Kalah(4,3): so uma das quatro primeiras jogadas ganha`` () =
    Assert.Equal<int[]>([| -6; 6; 2; -2 |], primeiras 4 3)

[<Fact>]
let ``Kalah(3,3): as primeiras jogadas`` () =
    Assert.Equal<int[]>([| 2; 2; -4 |], primeiras 3 3)

[<Fact>]
let ``os juizes concordam`` () =
    let n, discordam, sementes = juizes ()
    Assert.Equal(15, n)
    Assert.Empty(discordam)
    Assert.Equal<(int * int) list>([ (160, 0); (27076, 0); (303299, 0) ], sementes)
