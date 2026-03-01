# IndicatorEngine

[![Latest Version](https://img.shields.io/github/v/tag/ngqbac/IndicatorEngine)](https://github.com/ngqbac/IndicatorEngine)

**IndicatorEngine** is a lightweight, hierarchical indicator system for Unity.

Built on a fast indicator **tree**, **lazy blueprint hydration** (structure is installed only when touched), and a **host binding** layer that keeps UI automatically in sync.

Designed for modular projects: feature modules declare indicator structure via blueprints, while gameplay updates indicator state through a single service route.


---

## Features

- **Hierarchical `IndicatorId`** via `Child(...)` + `GetHierarchy()`.
- **IndicatorTree**: flag/counter sources, parent aggregation, `ActiveChanged/Removed` events.
- **Blueprint hydration**: register by root, lazy on touch, ancestor fallback for order-independent updates.
- **IndicatorVisual**: host binding + automatic UI sync.
- **Logging (optional)**: pluggable logger abstraction.

---

## Usage
**IndicatorEngine** is designed to be integrated from your project layer. The project is responsible for:
- creating the runtime (`IndicatorTree`, `IndicatorContext`, `IndicatorEngine`)
- wiring blueprint resolution (`BlueprintHooks` → registry)
- registering project blueprints
- binding UI hosts to `IndicatorId`s
- updating indicator state through a single service route

### 1) Initialize
Create the runtime once (e.g., at app start / project bootstrap):

```csharp
var tree = new IndicatorTree(/* optional logger */);

// Provide a resolver so blueprints can request project services.
// This can be backed by DI, a service locator, or a simple dictionary.
Func<Type, object> resolve = t => /* resolve service by type */;

var ctx = new IndicatorContext(tree, resolve /*, optional logger */);
var engine = new IndicatorEngine(tree, ctx /*, optional logger */);

// Use `engine` as your IIndicatorService facade.
IIndicatorService indicators = engine;
```
Wire blueprint lookup + register blueprints
```csharp
// Reset registry at start (recommended)
BlueprintRegistry.Reset();

// Connect the hook to registry
BlueprintHooks.Get(BlueprintRegistry.Get);

// Register blueprints (usually by root ids)
BlueprintRegistry.Register(IndicatorKey.SampleKey, new SampleIndicatorBlueprint());
```

### 2) Basic blueprints usage
```csharp
public class SampleIndicatorBlueprint : AbsIndicatorBlueprint
    {
        protected override bool AbleToCompose(IndicatorContext ctx)
        {
            return SampleModuleUtilities.IsModuleUnlocked(ctx.GetInstance<ConfigManager>(), ctx.GetInstance<UserData>());
        }

        protected override void OnCompose(IndicatorContext ctx)
        {
            ctx.Tree.Reparent(IndicatorKey.SampleFirstChild, IndicatorKey.Sample);
            ctx.Tree.Reparent(IndicatorKey.SampleSecondChild, IndicatorKey.Sample);
            var configType = ctx.GetInstance<ConfigManager>().GetConfig<ModuleConfig>().GetModuleBehaviourType();
            foreach (var type in configType)
            {
                ctx.Tree.Reparent(type.GetModuleBehaviourId(), IndicatorKey.SampleSecondChild);
            }
        }

        protected override void OnRefresh(IndicatorContext ctx)
        {
            var userData = ctx.GetInstance<UserData>();
            var configManager = ctx.GetInstance<ConfigManager>();
            
            ctx.Tree.SetState(IndicatorKey.SampleFirstChild, SampleModuleUtilities.GetState(configManager, userData));
            var configType = configManager.GetConfig<ModuleConfig>().GetModuleBehaviourType();
            foreach (var type in configType)
            {
                var behaviourLogic = type.GetBehaviourLogic();
                ctx.Tree.SetState(type.GetModuleBehaviourId(), behaviourLogic.indicatorState);
            }
        }
    }
```

### 3) Basic API usage
```csharp
// Bind host with an Id
indicators.Bind(host, IndicatorKey.SampleKey);

// Unbind host (host will no longer attach to any Id)
indicators.Unbind(host);

// set node boolean
indicators.SetState(IndicatorKey.SampleKey, true);

// set node counter (active if > 0)
indicators.SetStateCount(IndicatorKey.SampleKey.Child("SampleKeyChild"), 3);

// update node counter
indicators.UpdateStateCount(IndicatorKey.SampleKey.Child("SampleKeyChild"), 1);
indicators.UpdateStateCount(IndicatorKey.SampleKey.Child("SampleKeyChild"), -1);
```
When the tree state changes, the host is updated automatically via the visual binding layer.

### 4) Cleanup / Reset (optional)
Use cleanup/reset when you want to clear runtime caches and bindings at a session boundary, for example:
- leaving the main gameplay scene
- returning to title/login
- restarting a run
```csharp
engine.CleanUp();   // clears visual caches
```
A common project-side hook looks like:
```csharp
SceneManager.sceneUnloaded += scene =>
{
    if (scene.name != "MainScene") return;
    engine.CleanUp();
};
```

---

## Contributing

Pull requests are welcome! Please:
- Follow the project's C# coding style.
- Add appropriate tests and examples.
- Update the documentation if needed.

---

## 📜 License

MIT © [ngqbac](https://github.com/ngqbac)