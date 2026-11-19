# Kalah

Kalah resolvido por número de covas e de sementes, em **F#**, sem dependência nenhuma além do xUnit nos testes. Para cada Kalah(m, n) — m covas por lado, n sementes em cada — o motor diz o placar final exato com os dois jogando o melhor possível.

As regras usadas:

- Na sua vez você pega todas as sementes de uma cova sua e semeia uma por casa, no sentido anti-horário: as suas covas, o **seu** armazém, as covas do outro. O armazém do outro é pulado.
- Se a última semente cai no seu armazém, você joga de novo.
- Se a última cai numa cova sua que estava vazia e a cova da frente tem sementes, você captura as duas.
- Quando um dos lados fica sem semente, o jogo acaba e cada um leva para o armazém o que sobrou do seu lado.

---

## Por que F#

O motor é uma função recursiva com memória, e o que ele devolve depende só do que ainda está nas covas: o que já entrou nos armazéns não muda mais. Então o valor de uma posição é "quanto eu ainda ganho a mais que o outro", a memória guarda só as covas, e a jogada que passa a vez vira uma subtração — `ganho - valor (gira depois)`. Em F# isso fica uma expressão `match` de três casos. As covas viram uma chave `UInt128` de 7 bits por cova, que o .NET 8 tem nativo.

---

## Os quatro achados

**1. Em Kalah(2,4) o segundo jogador fica com 13 das 16 sementes.** O placar é −10 para quem começa, e as duas primeiras jogadas perdem (−10 e −12).

**2. Em Kalah(3,6) as três primeiras jogadas perdem.** É o único caso de três covas, de 1 a 6 sementes, em que o segundo ganha: −8, −10 e −12, cova por cova. Com 5 sementes por cova, quem começa ganha por 6.

**3. Em Kalah(4,3) só uma das quatro primeiras jogadas ganha.** A segunda cova dá +6; a primeira perde por 6, a terceira dá +2 e a quarta perde por 2. Em Kalah(4,4) a jogada que ganha muda de lugar: só a primeira cova dá +2, a segunda e a terceira empatam e a quarta perde por 2.

**4. O sinal do placar não segue o número de sementes.** Com uma cova por lado, de 1 a 6 sementes, o placar de quem começa é `0, −2, +6, −2, +2, 0`. Com duas, `+2, −2, −2, −10, +6, +8`. Mais semente não favorece nenhum dos lados.

---

## A tabela

O placar final de quem começa menos o do outro, com os dois jogando o melhor possível, e quantas posições o motor guardou.

| Kalah(m, n) | n = 1 | n = 2 | n = 3 | n = 4 | n = 5 | n = 6 |
|---|---:|---:|---:|---:|---:|---:|
| m = 1 | 0 | −2 | +6 | −2 | +2 | 0 |
| m = 2 | +2 | −2 | −2 | −10 | +6 | +8 |
| m = 3 | 0 | +4 | +2 | +2 | +6 | −8 |
| m = 4 | +2 | +6 | +6 | +2 | | |
| m = 5 | 0 | +2 | | | | |
| m = 6 | +2 | | | | | |

As posições guardadas crescem rápido com as covas: 7.448 em Kalah(3,3), 843.503 em Kalah(4,3), 5.641.244 em Kalah(4,4) (uns 20 s) e 3.390.069 em Kalah(5,2) (uns 14 s). As casas vazias da tabela não foram medidas.

Cada número está preso num teste: os casos leves em `MedidasTestes.fs`, Kalah(4,4), (5,1), (5,2) e (6,1) em `PesadosTestes.fs`.

---

## Os juízes

**O Kalah literal.** O juiz é outro Kalah, escrito do jeito mais direto possível: o tabuleiro de verdade, com os dois armazéns e sem girar nada; a semeadura anda uma semente por vez, casa a casa; o placar é o de verdade, contado até o fim da partida, com o Sul maximizando e o Norte minimizando. Ele não usa a divisão por voltas, nem o placar relativo, nem a memória só das covas. Nos 15 casos com até 9 sementes por lado, **0 discordâncias** com o motor.

**As sementes não somem.** Em todo tabuleiro alcançável de Kalah(2,4), Kalah(3,3) e Kalah(4,2) — 160, 27.076 e 303.299 tabuleiros — a soma das covas e dos armazéns é a do começo, inclusive depois do fim da partida.

---

## O que as medidas me corrigiram

Nada quebrou: o Kalah literal e o motor concordaram de primeira, em todos os casos.

O que ficou de fora foi por memória. Kalah(5,3) passou de 1,7 GB em poucos minutos sem terminar, e eu parei a rodada; por isso a tabela para em Kalah(5,2) e Kalah(6,1).

---

## Como usar

.NET 8 ou mais novo.

```bash
dotnet test testes/Kalah.Testes/Kalah.Testes.fsproj -c Release
dotnet run --project ferramentas/Medidor/Medidor.fsproj -c Release
dotnet run --project ferramentas/Medidor/Medidor.fsproj -c Release -- longo
dotnet fsi exemplos/uso.fsx
```

```fsharp
open Conde.Kalah.Nucleo

let motor = Motor 3
motor.Valor(inicial 3 6)               // -8
motor.Primeiras(inicial 3 6)           // [| Some -8; Some -10; Some -12 |]
```

## Os arquivos

- `src/Kalah/Nucleo.fs` — a semeadura por voltas, o giro, o fim e o motor.
- `src/Kalah/Juiz.fs` — o Kalah literal, semente a semente, e a conferência das sementes.
- `src/Kalah/Medidas.fs` — as tabelas do README, cada uma calculada uma vez só.
- `ferramentas/Medidor/Program.fs` — imprime as tabelas.
- `exemplos/uso.fsx` — o pacote em uso, sem compilar.
- `testes/` — 22 testes, com os números do README presos.

## Licença

MIT.
