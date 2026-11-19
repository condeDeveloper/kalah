// Exemplo de uso, sem compilar nada:
//
//   dotnet fsi exemplos/uso.fsx
//
// Resolve alguns Kalah pequenos e mostra o placar de cada primeira jogada.
#load "../src/Kalah/Nucleo.fs"

open Conde.Kalah.Nucleo

for m, n in [ (2, 4); (3, 3); (3, 6) ] do
    let motor = Motor m
    let placar = motor.Valor(inicial m n)
    let primeiras = motor.Primeiras(inicial m n) |> Array.map (Option.defaultValue 0)
    printfn "Kalah(%d,%d): %+d para quem comeca; cova por cova: %A" m n placar primeiras
