# Unity Context System

A flexible context management system for Unity that provides a structured way to handle application state and data flow through context stacks.

## Features

- **Stack-based Context Management**: Push and pop contexts to manage different application states
- **Data Sharing**: Share data between contexts through `IContextData` interfaces
- **Context Lifecycle**: Built-in lifecycle methods (`Start`, `Update`, `Dispose`)
- **Observable Updates**: Integrates with R3 for reactive update loops
- **Type-safe Data Access**: Generic methods for retrieving context data with compile-time safety

## Installation

Add this package to your Unity project using the Package Manager with Git URL:
1. Open Window > Package Manager
2. Click the + button and select "Add package from git URL..."
3. Enter the following URL: `https://github.com/lourenco-pedro/UnityServiceSystem.git`
4. Click "Add" and Unity will download and install the package

## Usage

### 1. Define Context Data

Create data classes that implement `IContextData` to share information between contexts:

```csharp
using ContextSystem;

public class PlayerData : IContextData
{
    public int Health { get; set; }
    public string PlayerName { get; set; }
}

public class GameSettings : IContextData
{
    public float Volume { get; set; }
    public bool FullScreen { get; set; }
}
```

### 2. Create a Context

Extend `BaseContext` to create your own contexts:

```csharp
using ContextSystem;
using UnityEngine;

public class GameplayContext : BaseContext
{
    public override void Start(ContextArgs args)
    {
        Debug.Log("Gameplay context started");
        
        // Access shared data from parent contexts
        args.UseContextData<GameSettings>(settings =>
        {
            Debug.Log($"Volume: {settings.Volume}");
        });
    }

    public override void Update(ContextArgs args)
    {
        // Called every frame
        args.UseContextData<PlayerData>(player =>
        {
            // Update player logic
        });
    }

    public override void Dispose()
    {
        Debug.Log("Gameplay context disposed");
        // Clean up resources
    }
}
```

### 3. Initialize the Context Manager

Initialize the context manager at application startup:

```csharp
using ContextSystem;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    void Start()
    {
        ContextManager.Tick();
        
        // Push the initial context
        var playerData = new PlayerData 
        { 
            Health = 100, 
            PlayerName = "Player1" 
        };
        
        ContextManager.PushContext<GameplayContext>(playerData);
    }

    void OnDestroy()
    {
        ContextManager.Terminate();
    }
}
```

### 4. Managing Context Stack

#### Push a Context
Add a new context to the stack:
```csharp
ContextManager.PushContext<MenuContext>();
```

Push with initial data:
```csharp
var data = new PlayerData { Health = 100 };
ContextManager.PushContext<GameplayContext>(data);
```

#### Pop a Context
Remove the current context from the stack:
```csharp
ContextManager.PopContext();
```

#### Switch Context
Replace the current context with a new one:
```csharp
ContextManager.SwitchContext<GameOverContext>();
```

### 5. Accessing Context Data

Use `ContextArgs` to access data in your context methods:

```csharp
public override void Update(ContextArgs args)
{
    // Access single data type
    args.UseContextData<PlayerData>(player =>
    {
        Debug.Log($"Player health: {player.Health}");
    });
    
    // Access multiple data types
    args.UseContextData<PlayerData, GameSettings>((player, settings) =>
    {
        Debug.Log($"{player.PlayerName} playing at volume {settings.Volume}");
    });
}
```

### 6. Register Additional Context Data

You can register additional data after context creation:

```csharp
public override void Start(ContextArgs args)
{
    var enemyData = new EnemyData();
    RegisterContexData(enemyData);
}
```

## API Reference

### ContextManager

Static manager class that controls the context stack.

- `Tick()` - Initializes the update loop
- `Terminate()` - Stops the update loop
- `PushContext<T>(IContextData data = null)` - Adds a new context to the stack
- `PopContext()` - Removes the top context from the stack
- `SwitchContext<T>(IContextData data = null)` - Replaces the current context
- `CurrentContext` - Returns the name of the current context (read-only)
- `Initialized` - Returns whether the manager is initialized (read-only)

### BaseContext

Abstract base class for all contexts.

- `Start(ContextArgs args)` - Called when the context is pushed onto the stack
- `Update(ContextArgs args)` - Called every frame
- `Dispose()` - Called when the context is removed from the stack
- `RegisterContexData(params IContextData[] data)` - Register data to be shared with child contexts
- `GetContextDatas()` - Returns all registered context data

### ContextArgs

Structure passed to context lifecycle methods containing shared data.

- `UseContextData<T>(Action<T> onFound)` - Execute an action if the data type is found
- `UseContextData<T1, T2>(Action<T1, T2> onFound)` - Execute an action if both data types are found
- `Count` - Returns the number of available context data objects

## Example: Menu System

```csharp
public class MainMenuContext : BaseContext
{
    public override void Start(ContextArgs args)
    {
        Debug.Log("Main Menu opened");
    }

    public override void Update(ContextArgs args)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Switch to gameplay
            ContextManager.SwitchContext<GameplayContext>(new PlayerData());
        }
    }

    public override void Dispose()
    {
        Debug.Log("Main Menu closed");
    }
}
```

## Dependencies

- R3 (Reactive Extensions for Unity)
- Unity 2021.3 or later

## License

[Your License Here]
