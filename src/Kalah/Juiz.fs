/// O juiz: outro Kalah, escrito do jeito mais literal possivel.
///
/// Aqui o tabuleiro e o de verdade, com os dois armazens e sem girar nada:
///
///   casas 0..m-1   covas do Sul       casa m      armazem do Sul
///   casas m+1..2m  covas do Norte     casa 2m+1   armazem do Norte
///
/// A semeadura anda UMA semente por vez, casa a casa, pulando o armazem do outro.
/// O placar e o de verdade (armazem do Sul menos o do Norte), contado ate o fim da
/// partida. Nada disso e compartilhado com o motor.
module Conde.Kalah.Juiz

open System.Collections.Generic

type Jogador =
    | Sul
    | Norte

let private armazemDe m quem = if quem = Sul then m else 2 * m + 1
let private covasDe m quem = if quem = Sul then [ 0 .. m - 1 ] else [ m + 1 .. 2 * m ]
let private frente m casa = 2 * m - casa

let tabuleiroInicial m n =
    Array.init (2 * m + 2) (fun k -> if k = m || k = 2 * m + 1 then 0 else n)

/// Joga a cova `casa` (numeracao do tabuleiro de verdade). Devolve o tabuleiro novo e
/// quem joga em seguida, ou None no lugar de quem joga se a partida acabou.
let joga m (t: int[]) (quem: Jogador) (casa: int) =
    let t = Array.copy t
    let mutable mao = t.[casa]
    t.[casa] <- 0
    let mutable pos = casa
    let outroArmazem = armazemDe m (if quem = Sul then Norte else Sul)
    while mao > 0 do
        pos <- (pos + 1) % (2 * m + 2)
        if pos <> outroArmazem then
            t.[pos] <- t.[pos] + 1
            mao <- mao - 1
    let meuArmazem = armazemDe m quem
    let minhas = covasDe m quem
    let deNovo = (pos = meuArmazem)
    if not deNovo && List.contains pos minhas && t.[pos] = 1 && t.[frente m pos] > 0 then
        t.[meuArmazem] <- t.[meuArmazem] + 1 + t.[frente m pos]
        t.[pos] <- 0
        t.[frente m pos] <- 0
    let sul = covasDe m Sul |> List.sumBy (fun k -> t.[k])
    let norte = covasDe m Norte |> List.sumBy (fun k -> t.[k])
    if sul = 0 || norte = 0 then
        for k in covasDe m Sul do
            t.[m] <- t.[m] + t.[k]
            t.[k] <- 0
        for k in covasDe m Norte do
            t.[2 * m + 1] <- t.[2 * m + 1] + t.[k]
            t.[k] <- 0
        t, None
    else
        t, Some(if deNovo then quem else (if quem = Sul then Norte else Sul))

/// Minimax completo do placar final (Sul menos Norte), com o Sul maximizando e o
/// Norte minimizando. Memoria pelo texto do tabuleiro inteiro e de quem joga.
let placar m n =
    let memoria = Dictionary<string, int>()
    let rec valor (t: int[]) (quem: Jogador) =
        let k = (if quem = Sul then "S" else "N") + System.String.Join(",", t)
        match memoria.TryGetValue k with
        | true, v -> v
        | _ ->
            let resultados =
                [ for casa in covasDe m quem do
                      if t.[casa] > 0 then
                          match joga m t quem casa with
                          | fim, None -> yield fim.[m] - fim.[2 * m + 1]
                          | depois, Some proximo -> yield valor depois proximo ]
            let v = if quem = Sul then List.max resultados else List.min resultados
            memoria.[k] <- v
            v
    valor (tabuleiroInicial m n) Sul

/// Toda semente que comeca na partida termina num armazem: confere em todos os
/// tabuleiros alcancaveis de Kalah(m, n). Devolve (tabuleiros visitados, quantos
/// perderam ou ganharam semente).
let confereSementes m n =
    let total = 2 * m * n
    let vistos = HashSet<string>()
    let mutable erros = 0
    let rec anda (t: int[]) quem =
        if vistos.Add((if quem = Sul then "S" else "N") + System.String.Join(",", t)) then
            if Array.sum t <> total then erros <- erros + 1
            for casa in covasDe m quem do
                if t.[casa] > 0 then
                    match joga m t quem casa with
                    | fim, None -> if Array.sum fim <> total then erros <- erros + 1
                    | depois, Some proximo -> anda depois proximo
    anda (tabuleiroInicial m n) Sul
    vistos.Count, erros
