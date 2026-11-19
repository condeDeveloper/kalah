module Conde.Kalah.NucleoTestes

open Xunit
open Conde.Kalah.Nucleo

[<Fact>]
let ``a ultima semente no armazem da a vez de novo`` () =
    // 3 covas; a cova 0 com 3 sementes cai em 1, 2 e no armazem.
    let l = semeia [| 3; 1; 1; 1; 1; 1 |] 0
    Assert.True(l.JogaDeNovo)
    Assert.Equal(1, l.ProArmazem)
    Assert.Equal<int[]>([| 0; 2; 2; 1; 1; 1 |], l.Depois)

[<Fact>]
let ``cair na propria cova vazia captura a cova da frente`` () =
    // A cova 0 joga 1 semente na cova 1, que estava vazia; a frente da 1 e a 4 (2m-1-1).
    let l = semeia [| 1; 0; 5; 2; 7; 3 |] 0
    Assert.False(l.JogaDeNovo)
    Assert.Equal(1 + 7, l.ProArmazem)
    Assert.Equal<int[]>([| 0; 0; 5; 2; 0; 3 |], l.Depois)

[<Fact>]
let ``cova vazia com a frente vazia nao captura`` () =
    let l = semeia [| 1; 0; 5; 2; 0; 3 |] 0
    Assert.Equal(0, l.ProArmazem)
    Assert.Equal<int[]>([| 0; 1; 5; 2; 0; 3 |], l.Depois)

[<Fact>]
let ``o armazem do outro e pulado e as voltas inteiras contam`` () =
    // m = 2: a volta tem 5 casas (2 covas, o armazem, 2 covas do outro).
    // 11 sementes na cova 0 = 2 voltas e mais 1: a ultima cai na cova 1.
    let l = semeia [| 11; 0; 0; 0 |] 0
    Assert.Equal(2, l.ProArmazem)
    Assert.Equal<int[]>([| 2; 3; 2; 2 |], l.Depois)
    Assert.Equal(11, Array.sum l.Depois + l.ProArmazem)

[<Fact>]
let ``uma volta exata termina na cova que foi esvaziada, e ela estava vazia: captura`` () =
    // m = 1: volta de 3 casas. 3 sementes na cova 0 = uma volta, a ultima cai nela.
    let l = semeia [| 3; 3 |] 0
    Assert.Equal(1 + 1 + 4, l.ProArmazem)
    Assert.Equal<int[]>([| 0; 0 |], l.Depois)

[<Fact>]
let ``girar duas vezes volta ao comeco`` () =
    let c = [| 1; 2; 3; 4; 5; 6 |]
    Assert.Equal<int[]>([| 4; 5; 6; 1; 2; 3 |], gira c)
    Assert.Equal<int[]>(c, gira (gira c))

[<Fact>]
let ``o fim leva as sobras de cada lado para o dono`` () =
    Assert.Equal(Some -5, fim [| 0; 0; 2; 3 |])
    Assert.Equal(Some 4, fim [| 1; 3; 0; 0 |])
    Assert.Equal(None, fim [| 1; 0; 0; 1 |])

[<Fact>]
let ``a chave separa tabuleiros que somam igual`` () =
    Assert.NotEqual(chave [| 1; 2; 3; 4 |], chave [| 4; 3; 2; 1 |])
    Assert.NotEqual(chave [| 0; 127 |], chave [| 1; 0 |])
