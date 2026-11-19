/// As tabelas do README. Calculadas aqui, e em lugar nenhum mais.
module Conde.Kalah.Medidas

open System.Collections.Concurrent
open Conde.Kalah.Nucleo

let private guardadas = ConcurrentDictionary<string, obj>()

let private umaVez<'T> (chave: string) (f: unit -> 'T) : 'T =
    guardadas.GetOrAdd(chave, fun _ -> box (f ())) :?> 'T

/// (placar de quem comeca menos o do outro, posicoes guardadas) de Kalah(m, n).
let resolvido m n : int * int = umaVez (sprintf "k:%d,%d" m n) (fun () -> resolve m n)

/// Os casos leves (todos em menos de 3 s): m de 1 a 3 com n de 1 a 6, e m = 4 com n de 1 a 3.
let leves =
    [ for m in 1 .. 3 do for n in 1 .. 6 -> m, n ] @ [ for n in 1 .. 3 -> 4, n ]

/// O placar de cada primeira jogada de Kalah(m, n), cova por cova.
let primeiras m n : int[] =
    umaVez (sprintf "p:%d,%d" m n) (fun () ->
        let motor = Motor m
        motor.Primeiras(inicial m n) |> Array.map Option.get)

/// Os juizes: o Kalah literal (semente a semente, tabuleiro de verdade, placar ate o
/// fim) contra o motor, e as sementes que nunca somem.
let juizes () =
    umaVez "juizes" (fun () ->
        let casos = [ for m in 1 .. 4 do for n in 1 .. 6 do if m * n <= 9 then yield m, n ]
        let discordam =
            casos |> List.filter (fun (m, n) -> fst (resolvido m n) <> Juiz.placar m n)
        let sementes = [ for m, n in [ (2, 4); (3, 3); (4, 2) ] -> Juiz.confereSementes m n ]
        casos.Length, discordam, sementes)
