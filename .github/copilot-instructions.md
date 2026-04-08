# Copilot Instructions for nanoFramework.DependencyInjection

## Project Overview

This repository implements a **Dependency Injection (DI) / Inversion of Control (IoC) container** for [.NET nanoFramework](https://www.nanoframework.net/) — a free, open-source platform that enables C# development for constrained embedded devices (microcontrollers). The library mirrors the API of the official [Microsoft.Extensions.DependencyInjection](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) package as closely as possible, with adaptations for the limitations of nanoFramework.

The NuGet package is published as **`nanoFramework.DependencyInjection`** and is a dependency of [nanoFramework.Hosting](https://github.com/nanoframework/nanoFramework.Hosting).

## Repository Structure

```
/
├── nanoFramework.DependencyInjection/          # Main library project
│   ├── nanoFramework.DependencyInjection.nfproj   # nanoFramework project file (not .csproj)
│   ├── Microsoft/Extensions/DependencyInjection/  # Core DI types (mirrors MS.Extensions.DI namespace)
│   │   ├── ServiceCollection.cs
│   │   ├── ServiceDescriptor.cs
│   │   ├── ServiceLifetime.cs
│   │   ├── ServiceProvider.cs
│   │   ├── ServiceProviderEngine.cs
│   │   ├── ServiceProviderEngineScope.cs
│   │   ├── ServiceProviderOptions.cs
│   │   ├── ActivatorUtilities.cs
│   │   ├── ServiceCollectionServiceExtensions.cs
│   │   ├── ServiceCollectionDescriptorExtensions.cs
│   │   ├── ServiceCollectionContainerBuilderExtensions.cs
│   │   ├── ServiceProviderServiceExtensions.cs
│   │   ├── ServiceScope.cs
│   │   ├── TypeExtensions.cs
│   │   └── (interfaces: IServiceCollection, IServiceScope, IServiceProviderIsService)
│   └── System/                                # nanoFramework-specific polyfills
│       ├── Activator.cs                       # Custom Activator (uses reflection)
│       ├── AggregateException.cs              # Custom AggregateException
│       └── IServiceProvider.cs
├── tests/                                     # Unit tests
│   ├── nanoFramework.DependencyInjection.UnitTests.nfproj
│   ├── nano.runsettings                       # Test run settings (IsRealHardware=False → nanoCLR simulator)
│   ├── Fakes/                                 # Test fakes and stubs
│   ├── DependencyInjectionTests.cs
│   ├── ServiceProviderTests.cs
│   ├── ServiceCollectionTests.cs
│   ├── ServiceCollectionDescriptorExtensionsTests.cs
│   ├── ServiceProviderExtensionsTests.cs
│   ├── ActivatorTests.cs
│   ├── ActivatorUtilitiesTests.cs
│   └── AggregateExceptionTests.cs
├── nanoFramework.DependencyInjection.sln      # Visual Studio solution
├── nanoFramework.DependencyInjection.nuspec   # NuGet package specification
├── azure-pipelines.yml                        # Primary CI/CD (Azure Pipelines)
├── version.json                               # Nerdbank.GitVersioning config (version: 1.1)
├── NuGet.Config                               # NuGet source (nuget.org only)
└── .github/workflows/                         # GitHub Actions (PR checks, dependency updates)
    ├── pr-checks.yml                          # Checks package lock and NuGet versions on PRs
    ├── update-dependencies.yml
    ├── update-dependencies-develop.yml
    └── generate-changelog.yml
```

## Critical Platform Constraints

**.NET nanoFramework is NOT standard .NET.** The code targets an embedded runtime with significant restrictions:

1. **No generics in the public API** — All methods that would use `<T>` in standard .NET use `Type` parameters instead. Example: `serviceCollection.AddSingleton(typeof(IMyService), typeof(MyService))` instead of `AddSingleton<IMyService, MyService>()`.

2. **Limited reflection** — `Type.GetConstructor()`, `Type.GetConstructors()`, `ConstructorInfo.Invoke()` are available but behaviour can differ from full .NET. `GetType()` is available on instances.

3. **No struct support in DI container** — `struct` types cannot be used as service registrations. The constructor for `struct` types fails in `typeof(T).GetConstructor()`. See the commented-out test `ServicesRegisteredWithImplementationTypeForStructSingletonServices` and the issue reference: https://github.com/nanoframework/Home/issues/1085.

4. **No array-typed constructor parameters** — Types that have `byte[]` or other array parameters in their constructors will cause a null exception during activation.

5. **No full BCL** — Only a subset of the .NET base class library is available via `nanoFramework.CoreLibrary` (`mscorlib`). Standard types like `ArrayList` (not `List<T>`), `Hashtable`, etc. are used.

6. **Namespace `System.Collections`** — Use `ArrayList` and `Hashtable` instead of generic collections.

7. **Factory delegates use a custom delegate** — `ImplementationFactoryDelegate` (`delegate object ImplementationFactoryDelegate(IServiceProvider serviceProvider)`) is used instead of `Func<IServiceProvider, object>`.

## Build Environment

> **⚠️ IMPORTANT: The nanoFramework build system is Windows-only.**

The project uses `.nfproj` files (nanoFramework project format), which require:
- **Windows OS** with the nanoFramework Visual Studio extension or MSBuild extensions installed at `$(MSBuildExtensionsPath)\nanoFramework\v1.0\`
- **MSBuild** (via Visual Studio or .NET SDK for Windows)
- The CI/CD pipeline (`azure-pipelines.yml`) uses `windows-latest` runners

**You cannot build or run tests in a Linux/macOS environment** (the standard GitHub Copilot cloud agent environment). Attempting `dotnet build` on `.nfproj` files will fail because the `NFProjectSystem.props`/`.targets` files are not present.

### What you CAN do in the cloud agent environment:
- Read, analyze, and edit all source files
- Update documentation (README.md, CHANGELOG.md, XML doc comments)
- Add/modify test files and source files
- Review and update NuGet package versions in `packages.config`
- Update GitHub Actions workflows in `.github/workflows/`
- Make structural changes that will be validated by CI on Azure Pipelines

### What requires Windows to validate:
- Compiling the project
- Running unit tests (via nanoCLR simulator with `nanoFramework.TestFramework`)
- Verifying NuGet package restore

## Testing

Tests use **nanoFramework.TestFramework** (not MSTest, NUnit, or xUnit). Key points:

- Test classes are decorated with `[TestClass]` and methods with `[TestMethod]` from `nanoFramework.TestFramework`
- Assertions use `Assert.IsType()`, `Assert.AreEqual()`, `Assert.AreNotSame()`, `Assert.IsNull()`, `Assert.IsNotNull()`, `Assert.ThrowsException()`, etc.
- Tests run on the **nanoCLR simulator** (not real hardware) — configured in `tests/nano.runsettings` via `<IsRealHardware>False</IsRealHardware>`
- Test timeout: 120 seconds (`<TestSessionTimeout>120000</TestSessionTimeout>`)
- The Azure Pipelines build passes `unitTestRunsettings: '$(System.DefaultWorkingDirectory)\tests\nano.runsettings'` to the test runner

## Service Lifetimes

The library supports three service lifetimes (same as standard .NET):

| Lifetime | Behavior |
|----------|----------|
| `Singleton` | One instance shared across the entire application lifetime |
| `Transient` | A new instance created every time the service is requested |
| `Scoped` | One instance per scope (created via `serviceProvider.CreateScope()`) |

## Key API Patterns

```csharp
// Registration (no generics — use Type parameters)
var serviceProvider = new ServiceCollection()
    .AddSingleton(typeof(IMyService), typeof(MyServiceImpl))
    .AddTransient(typeof(IAnotherService), typeof(AnotherServiceImpl))
    .AddScoped(typeof(IScopedService), typeof(ScopedServiceImpl))
    .BuildServiceProvider();

// Resolution
var service = (IMyService)serviceProvider.GetService(typeof(IMyService));
var required = (IMyService)serviceProvider.GetRequiredService(typeof(IMyService));
object[] services = serviceProvider.GetServices(typeof(IMyService));

// Scoped resolution
using (var scope = serviceProvider.CreateScope())
{
    var scoped = scope.ServiceProvider.GetService(typeof(IScopedService));
}

// Factory registration
services.AddSingleton(typeof(IMyService), (IServiceProvider sp) => new MyServiceImpl());

// Instance registration
services.AddSingleton(typeof(IMyService), existingInstance);

// ActivatorUtilities (create without registering)
var instance = (MyClass)ActivatorUtilities.CreateInstance(provider, typeof(MyClass), arg1, arg2);

// Validation options
var provider = services.BuildServiceProvider(new ServiceProviderOptions
{
    ValidateOnBuild = true,   // Validate all services can be constructed
    ValidateScopes = true     // Validate scoped services never resolved from root
});
```

## NuGet Package Management

This project uses the **old-style `packages.config`** NuGet format (not `PackageReference`). To update packages:
- Edit `packages.config` in the relevant project directory
- Edit the corresponding `<HintPath>` references in the `.nfproj` file
- The lock file (`packages.lock.json`) is enforced in CI via `<RestoreLockedMode>true</RestoreLockedMode>`

Main dependencies:
- `nanoFramework.CoreLibrary` (v1.17.11) — the nanoFramework mscorlib
- `Nerdbank.GitVersioning` (v3.9.50) — automatic versioning from git history
- `nanoFramework.TestFramework` (v3.0.77) — test framework (tests project only)

## Versioning

Versioning is managed by **Nerdbank.GitVersioning** via `version.json`. The current version base is `1.1`. Releases are tagged `v*` and trigger publish jobs in the Azure Pipelines CI. Preview packages are published from non-release branches.

## Code Style

- File header: `// Copyright (c) .NET Foundation and Contributors\n// See LICENSE file in the project root for full license information.`
- Namespace: Main library uses `Microsoft.Extensions.DependencyInjection` and `System` (polyfills); tests use `nanoFramework.DependencyInjection.UnitTests`
- XML doc comments on all public API members
- `lock (_syncLock)` pattern used in `ServiceCollection` for thread safety
- `.editorconfig` only sets `dotnet_analyzer_diagnostic.severity = silent`
- Assembly is strong-named via `key.snk`

## Known Issues and Workarounds

1. **Struct types in DI**: `struct` types cannot be registered or resolved. This is a nanoFramework limitation (issue [#1085](https://github.com/nanoframework/Home/issues/1085)). The affected test is commented out.

2. **Array constructor parameters**: Types with array-typed constructor parameters (e.g., `byte[]`) will throw a null exception during activation. Document this limitation in any affected code/tests.

3. **`GetImplementationType()` with factory**: When a service is registered only via a factory (`ImplementationFactory`), `GetImplementationType()` returns `null` because nanoFramework lacks generic type argument introspection. See the commented-out code in `ServiceDescriptor.cs`.

4. **`RestoreLockedMode` in CI**: The `packages.lock.json` is enforced in CI. If you update package versions in `packages.config`, the lock file must be regenerated on a Windows machine before the CI will pass.

5. **Building in cloud agent**: As noted above, building `.nfproj` files requires the nanoFramework project system on Windows. Code changes should be validated by the Azure Pipelines CI after merging or creating a PR.

## CI/CD

- **Primary CI**: Azure Pipelines (`azure-pipelines.yml`) — builds, runs tests, publishes NuGet packages
- **PR Checks** (GitHub Actions): `.github/workflows/pr-checks.yml` — verifies `packages.lock.json` consistency and that NuGet packages are up to date
- **Dependency updates**: Automated via `.github/workflows/update-dependencies.yml` and `update-dependencies-develop.yml`
- **Downstream dependents**: `nanoFramework.Hosting` is automatically updated when a new release is published

## Related Resources

- [nanoFramework Home](https://github.com/nanoframework/Home)
- [nanoFramework.Hosting](https://github.com/nanoframework/nanoFramework.Hosting) — uses this library
- [DependencyInjection Samples](https://github.com/nanoframework/Samples/tree/main/samples/DependencyInjection)
- [Contributing Guide](https://github.com/nanoframework/Home/blob/main/CONTRIBUTING.md)
- [Discord Community](https://discord.gg/gCyBu8T)
