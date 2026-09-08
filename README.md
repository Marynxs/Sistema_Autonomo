# Magic Trick — Autonomous Player System (Sistema Autônomo)

A Windows Forms desktop client for **Magic Trick**, a trick-taking / betting card game (in the style of games such as Hearts or *Copas*), featuring a fully **autonomous AI bot player** that can play an entire match on its own — reading game state, deciding what to bet, and choosing which card to play, turn after turn, with no human input.

This was built as a university systems project (BCC 2024) around a game server library provided as a black-box DLL, with the client (this repo) responsible for the UI, game-state polling, and — the core deliverable — the **decision-making engine for the automated player**.

## What it does

- Renders a full graphical card table (WinForms), including player hands, played cards, bets, scores, and turn/round status, for **2 or 4 players**.
- Connects to a match server (`MagicTrickServer.dll`) that manages match creation, joining, turns, and scoring; the client polls the server for game state (whose turn it is, what's been played, current hands, scores) and translates it into the UI.
- Includes a lobby flow: create a match, join an existing match, and start play.
- Drives an **autonomous bot** that plays a full seat at the table automatically:
  - Decides how to open a round, how to respond to the first card played, and how to play on later turns.
  - Places bets based on its current hand and how many tricks it has already won.
  - Tracks which of its own cards have already been played or bet so it never tries to reuse them.
- Handles card art, hand layout/rotation per seat, and live UI refresh as the match state changes.

## How the AI bot works

The bot's decision-making is built with the **Strategy design pattern** — each distinct moment in a turn is its own swappable strategy class, so the logic for "how do I play" changes depending on context rather than living in one giant conditional block:

| Strategy | Used when |
|---|---|
| `ComecarRodada` | The bot must lead a brand-new round (no cards played yet) |
| `ComecarPrimeiraJogada` / `ComecarTurno` | The bot opens a fresh turn within the round |
| `ResponderPrimeiroTurno` | The bot must respond to the very first card played in the round |
| `ResponderJogada` / `ResponderTurno` | The bot must follow suit / respond to a card already on the table |

A shared `Estrategia` (Strategy context) class gives every strategy access to hand analysis helpers — e.g. counting remaining cards of a given suit, and computing which of the bot's cards have already been played or wagered — so decisions are based on actual remaining hand state, not just the current trick.

The `Bot` class orchestrates all of this on a timer loop: each tick it checks whose turn it is, figures out which phase of the round it's in, delegates to the matching strategy, and — once the betting round is reached — runs its own betting heuristic (biased toward cards it hasn't used yet, and adjusted by how many tricks it's already won that turn).

## Architecture

- **Client / server split**: gameplay rules, match state, scoring, and turn validation live entirely in the external `MagicTrickServer` library (provided as a compiled `.dll` with XML doc comments); this project is the *client*, responsible for UI and for the autonomous agent's decisions. All state is read via a small set of server queries (list players, check whose turn it is, view hands, view plays/bets) and mutated via two commands (`Jogar` – play a card, `Apostar` – place a bet).
- **Polling-based sync**: the client has no persistent socket/event stream from the server; it polls on a `System.Windows.Forms.Timer` and re-renders any part of the table that changed (hands, played cards, bets, scores, turn indicator).
- **String-protocol parsing**: the server returns delimited plaintext (comma/newline separated) for things like current hands, turn state, and play history; a small parsing layer (`GerenciadorStrings`) centralizes turning that into typed data and surfaces server-side errors as UI dialogs.
- **Rendering layer**: `RenderizadorCartas` computes per-seat card positions (including layout differences for 2p vs 4p tables and seat rotation) and swaps in face, back, or "already played" card art as state changes.
- **Entity layer**: `Jogador` (player), `Carta`/`Cartas` (card / hand), `Partida` (a running match, owns rendering + state-sync), and `InicializadorPartida` (match bootstrap: builds seats, deals hands, wires up the bot).

## Tech stack

- **C# / .NET Framework 4.8**
- **Windows Forms (WinForms)** for the desktop UI
- **Strategy design pattern** for the AI's turn-by-turn decision logic
- Consumes a pre-built **game-server class library** (`MagicTrickServer.dll`) for all authoritative game rules and state
- Visual Studio solution (`.sln`) / MSBuild project structure

## Project structure

```
PI/
├── Entidades/
│   ├── Bot.cs                    # Autonomous player: turn loop + betting logic
│   ├── Partida.cs                # Live match: state sync + card/board rendering
│   ├── InicializadorPartida.cs   # Match bootstrap: seats, dealing, bot wiring
│   ├── RenderizadorCartas.cs     # Card layout & rendering per seat
│   ├── Jogador.cs / Carta.cs / Cartas.cs   # Player / card / hand models
│   ├── Estrategias/              # Strategy pattern: one class per decision context
│   └── Configuracoes/            # Table layout config + server response parsing
├── FormularioMenu / FormularioEntrada / FormularioCriarPartida / FormularioPartida
│                                  # Lobby (menu, join, create) and match screens
├── Cards/                        # Card face/back artwork
└── MagicTrickServer.dll          # External game-rules server library
```

## Notes

- This is a course/academic project built against a fixed external server API, so the server library itself is not part of this repo's logic — the interesting engineering here is the client-side state sync and the bot's decision strategies.
- UI strings, class, and method names are in Portuguese (the original development language); this README describes the project in English for a broader audience.
