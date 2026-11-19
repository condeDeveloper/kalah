/// Kalah com m covas por lado e n sementes por cova.
///
/// O tabuleiro tem as m covas de cada jogador e um armazem para cada um. Na sua vez
/// voce pega todas as sementes de uma cova sua e semeia uma por cova, no sentido
/// anti-horario: as suas covas, o SEU armazem, as covas do outro — e pula o armazem
/// do outro.
///
///   - Se a ultima semente cai no seu armazem, voce joga de novo.
///   - Se a ultima cai numa cova SUA que estava vazia, e a cova da frente tem
///     sementes, voce captura as duas para o seu armazem.
///   - Quando um dos lados fica sem semente nenhuma, o jogo acaba, e cada um leva
///     para o armazem o que sobrou do seu lado.
///
/// Ganha quem tiver mais no armazem.
///
/// O truque do motor: o que importa daqui para a frente e so o que ainda esta nas
/// covas. O que ja esta nos armazens nao muda mais. Entao o valor de uma posicao e
/// "quanto eu ainda vou ganhar a mais que o outro", e a memoria guarda so as covas.
module Conde.Kalah.Nucleo

open System
open System.Collections.Generic

/// As covas, do ponto de vista de quem vai jogar: 0..m-1 sao as suas (da esquerda
/// para a direita, ate o seu armazem), m..2m-1 sao as do outro (na ordem em que a
/// semeadura passa por elas). A cova da frente da sua cova i e a 2m-1-i.
type Covas = int[]

let inicial m n : Covas = Array.create (2 * m) n

/// O que uma jogada faz: as covas depois dela (ainda do ponto de vista de quem
/// jogou), quantas sementes foram para o armazem de quem jogou, e se ele joga de novo.
[<Struct>]
type Lance =
    { Depois: Covas
      ProArmazem: int
      JogaDeNovo: bool }

/// Semeia a cova i. A volta tem 2m + 1 casas: as 2m covas e o armazem de quem joga
/// (posicao m na volta). As voltas inteiras sao contadas de uma vez, por divisao.
let semeia (c: Covas) (i: int) : Lance =
    let m = c.Length / 2
    let volta = 2 * m + 1
    let s = c.[i]
    if s = 0 then invalidArg "i" "cova vazia"
    let d = Array.copy c
    d.[i] <- 0
    let voltas = s / volta
    let resto = s % volta
    // Na volta, a casa k e: k < m a cova k; k = m o armazem; k > m a cova k - 1.
    let mutable armazem = voltas
    for k in 0 .. 2 * m - 1 do
        d.[k] <- d.[k] + voltas
    for passo in 1 .. resto do
        let k = (i + passo) % volta
        if k = m then armazem <- armazem + 1
        elif k < m then d.[k] <- d.[k] + 1
        else d.[k - 1] <- d.[k - 1] + 1
    let ultima = (i + s) % volta
    if ultima = m then
        { Depois = d; ProArmazem = armazem; JogaDeNovo = true }
    elif ultima < m && d.[ultima] = 1 && d.[2 * m - 1 - ultima] > 0 then
        let frente = 2 * m - 1 - ultima
        let ganho = 1 + d.[frente]
        d.[ultima] <- 0
        d.[frente] <- 0
        { Depois = d; ProArmazem = armazem + ganho; JogaDeNovo = false }
    else
        { Depois = d; ProArmazem = armazem; JogaDeNovo = false }

/// Gira o tabuleiro para o ponto de vista do outro.
let gira (c: Covas) : Covas =
    let m = c.Length / 2
    Array.append c.[m..] c.[.. m - 1]

/// Se um lado acabou, quanto o fim da a quem jogou (as suas sobras menos as do outro).
let fim (c: Covas) : int option =
    let m = c.Length / 2
    let minhas = Array.sum c.[.. m - 1]
    let deles = Array.sum c.[m..]
    if minhas = 0 || deles = 0 then Some(minhas - deles) else None

/// As covas numa chave de 128 bits (7 bits por cova: ate 127 sementes por cova).
let chave (c: Covas) : UInt128 =
    let mutable k = UInt128.Zero
    for v in c do
        k <- (k <<< 7) ||| UInt128.op_Implicit (uint64 v)
    k

/// O motor: o placar exato daqui para a frente, para quem vai jogar.
type Motor(m: int) =
    do
        if m < 1 || m > 9 then invalidArg "m" "de 1 a 9 covas por lado"
    let memoria = Dictionary<UInt128, int>()

    member _.Posicoes = memoria.Count

    member this.Valor(c: Covas) : int =
        let k = chave c
        match memoria.TryGetValue k with
        | true, v -> v
        | _ ->
            let mutable melhor = Int32.MinValue
            for i in 0 .. m - 1 do
                if c.[i] > 0 then
                    let l = semeia c i
                    let v =
                        match fim l.Depois with
                        | Some sobra -> l.ProArmazem + sobra
                        | None when l.JogaDeNovo -> l.ProArmazem + this.Valor l.Depois
                        | None -> l.ProArmazem - this.Valor(gira l.Depois)
                    if v > melhor then melhor <- v
            memoria.[k] <- melhor
            melhor

    /// O placar de cada primeira jogada (None para cova vazia).
    member this.Primeiras(c: Covas) : int option[] =
        [| for i in 0 .. m - 1 ->
               if c.[i] = 0 then None
               else
                   let l = semeia c i
                   Some(
                       match fim l.Depois with
                       | Some sobra -> l.ProArmazem + sobra
                       | None when l.JogaDeNovo -> l.ProArmazem + this.Valor l.Depois
                       | None -> l.ProArmazem - this.Valor(gira l.Depois)
                   ) |]

/// Resolve Kalah(m, n): (placar final de quem comeca menos o do outro, posicoes guardadas).
let resolve m n =
    let motor = Motor m
    let v = motor.Valor(inicial m n)
    v, motor.Posicoes
