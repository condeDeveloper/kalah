/// O medidor. Roda as tabelas e cospe os numeros que vao para o README.
///
///   dotnet run --project ferramentas/Medidor/Medidor.fsproj -c Release              os casos leves e os juizes
///   dotnet run --project ferramentas/Medidor/Medidor.fsproj -c Release -- longo     Kalah(4,4), (5,1), (5,2) e (6,1)
open Conde.Kalah.Medidas

let linha (m, n) =
    let sw = System.Diagnostics.Stopwatch.StartNew()
    let v, p = resolvido m n
    let quem = if v > 0 then "quem comeca ganha" elif v < 0 then "o segundo ganha" else "empate"
    printfn "   Kalah(%d,%d): %+d  %-18s  %d posicoes  (%.1f s)" m n v quem p sw.Elapsed.TotalSeconds

let leve () =
    printfn "\n== Kalah(m, n): o placar de quem comeca menos o do outro =="
    for c in leves do linha c
    printfn "\n== as primeiras jogadas, cova por cova (da esquerda para o armazem) =="
    for m, n in [ (3, 3); (3, 6); (4, 3) ] do
        printfn "   Kalah(%d,%d): %A" m n (primeiras m n)
    let n, discordam, sementes = juizes ()
    printfn "\n== os juizes =="
    printfn "   o Kalah literal contra o motor: %d casos, discordam %A" n discordam
    for (vistos, erros), (m, n) in List.zip sementes [ (2, 4); (3, 3); (4, 2) ] do
        printfn "   sementes em Kalah(%d,%d): %d tabuleiros, %d com semente a mais ou a menos" m n vistos erros

let longo () =
    printfn "\n== os casos caros =="
    for c in [ (4, 4); (5, 1); (5, 2); (6, 1) ] do linha c
    printfn "   primeiras de Kalah(4,4): %A" (primeiras 4 4)

[<EntryPoint>]
let main argv =
    match argv with
    | [||] -> leve (); 0
    | [| "longo" |] -> longo (); 0
    | _ -> eprintfn "nao conheco %A. conheco: longo" argv; 1
