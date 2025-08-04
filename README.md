# PlayPerfect XO 

Lets just call it XO on steroids... Lack of sleep included...
Ithc: https://raspik.itch.io/perfectplay-xo

## Features 

- **Advanced Scoring System** - Time-based scoring with performance bonuses
- **Persistent Save/Load** - Automatic game state management
- **Dependency Injection** - Clean architecture using Zenject
- **Object Pooling** - Optimized cell rendering system
- **Async Operations** - Smooth gameplay using UniTask

## Architecture Overview 

```mermaid
flowchart TD
    %% Entry Point
    A[Game Start] --> B[GameManager Initialize]
    
    %% Core Systems
    B --> C[AssetLoader]
    B --> D[ScoringSystem]
    B --> E[GameSaveManager]
    B --> F[GameUIManager]
    
    %% Game Flow
    F --> G[New Game Request]
    G --> H[GameManager.LoadNewGameAsync]
    H --> I[GameData Reset]
    H --> J[ScoringSystem.StartNewGame]
    
    %% Player Turn
    J --> K[Player Turn]
    K --> L[GameUIManager Handles Input]
    L --> M[GameManager.HandlePlayerMove]
    M --> N[GameData.MakeMove]
    M --> O[ScoringSystem.AddPlayerTurnTime]
    
    %% Game State Check
    N --> P{Game Over?}
    P -->|No| Q[Computer Turn]
    P -->|Yes| R[Calculate Final Score]
    
    %% Computer Turn
    Q --> S[ComputerPlayer.GetMoveAsync]
    S --> T[GameManager.HandleComputerTurn]
    T --> U[GameData.MakeMove]
    U --> P
    
    %% Score Calculation
    R --> V[ScoringSystem.CalculateGameScore]
    V --> W[ScoringSystem.CompleteGame]
    W --> X[Store CurrentGameScore]
    W --> Y[Save ScoreData]
    
    %% Events & UI Updates
    O --> Z[OnCellChanged Event]
    Y --> AA[OnScoreCalculated Event]
    AA --> BB[UI Updates]
    
    %% Save System
    M --> CC[GameSaveManager.SaveGame]
    T --> CC
    
    %% Object Pooling
    Z --> DD[CellPool Management]
    DD --> EE[Cell View Updates]
    
    %% Styling
    classDef manager fill:#e1f5fe
    classDef system fill:#f3e5f5
    classDef data fill:#e8f5e8
    classDef ui fill:#fff3e0
    classDef event fill:#ffebee
    
    class B,H,M,T manager
    class D,E,C,J,V,W system
    class I,N,U,X,Y data
    class F,L,BB,EE ui
    class Z,AA event
```

## Core Components 

### GameManager
- **Central coordinator** for game flow and state management
- Handles player/computer turns and game lifecycle
- Integrates with all major systems through dependency injection
- Manages async operations for smooth gameplay

### ScoringSystem
- **Time-based scoring** with performance bonuses
- Tracks `CurrentGameScore` for the active round
- Persistent score data storage using PlayerPrefs
- Calculates final scores based on game outcome and turn efficiency

### GameData
- **Game state representation** with 3x3 board
- Cell state management (Empty, X, O)
- Move validation and win condition detection
- Serializable for save/load functionality

### ComputerPlayer
- **AI opponent** with strategic move calculation
- Async move generation for responsive gameplay
- Difficulty scaling based on game progression

### GameSaveManager
- **Automatic save/load** functionality
- JSON-based serialization
- Auto-save on each move for seamless resume

## Game States 

```mermaid
stateDiagram-v2
    [*] --> Playing
    Playing --> PlayerWin : Player completes line
    Playing --> ComputerWin : Computer completes line
    Playing --> Draw : Board full, no winner
    
    PlayerWin --> [*] : Game Complete
    ComputerWin --> [*] : Game Complete
    Draw --> [*] : Game Complete
    
    Playing --> Playing : Valid move made
```

## Scoring System 

- **Win Score**: 1000+ points (with time bonuses)
- **Draw Score**: 500+ points (with time bonuses)
- **Loss Score**: 0 points
- **Time Bonus**: Faster moves = higher scores

## Technical Stack 

- **Unity 2022.3+**
- **Zenject** - Dependency Injection
- **UniTask** - Async/Await operations
- **TextMeshPro** - UI text rendering
- **Addressable Assets** - Resource management

## Project Structure 

```
Assets/
├── Code/
│   ├── Game/           # Core game logic
│   │   ├── Data/       # Game data structures
│   │   ├── Views/      # UI components
│   │   └── Interfaces/ # Contracts
│   ├── Services/       # External services
│   └── Installers/     # Dependency injection setup
├── Art/                # Visual assets
├── Prefabs/           # Game object prefabs
└── Scenes/            # Unity scenes
```
