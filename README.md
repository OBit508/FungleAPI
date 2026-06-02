> This mod is not affiliated with or endorsed by Innersloth LLC. Portions of the materials contained herein are property of Innersloth LLC. © Innersloth LLC.

# Fungle API

Fungle API is a modding framework for Among Us, heavily inspired by both [Mira API](https://github.com/All-Of-Us-Mods/MiraAPI) and [Reactor](https://github.com/nuclearpowered/reactor). It provides a wide range of tools and systems designed to simplify mod development and extend the game's capabilities.

## Features

- Custom role creation
- Custom team creation
- Custom game options
- Custom lobby settings
- Custom game modes
- Custom game over screens
- Custom buttons
- Custom RPCs
- Custom cosmetics
- Simplified localization using StringNames
- Easy asset loading
- Extensive utility extensions to speed up development

## Getting Started

1. Add the Fungle API DLL to your project's references.

2. Add the following dependency attribute to your main plugin class:

   ```csharp
   [BepInDependency(FungleApiPlugin.ModId)]
   ```

3. Implement the `IFungleBasePlugin` interface in your plugin class.

4. Do **not** use BepInEx's standard `Load()` method for your mod initialization. Instead, use one or both of the initialization callbacks provided by Fungle API:

   ```csharp
   public void AlmostLoaded()
   {
       // Called when your mod has been registered by Fungle API.
   }
   ```

   `AlmostLoaded()` is invoked after your mod has been registered by Fungle API, but before the API has fully completed its loading process. This stage is similar to BepInEx's `Load()` method, meaning some systems may not yet be fully initialized. While it can be useful for early initialization, using API features at this point may lead to unexpected issues.

   ```csharp
   public void FullyLoaded()
   {
       // Called when Fungle API has completely finished loading your mod.
   }
   ```

   `FullyLoaded()` is called after Fungle API has completely finished loading your mod and all API systems, registrations, and dependencies are fully initialized and ready for use.

   Both callbacks can be implemented if needed. However, it is **strongly recommended** to perform all initialization inside `FullyLoaded()`, as it is the safest and most reliable point in the loading process.

## Reactor Compatibility

Although Fungle API is heavily inspired by Mira API and Reactor, it does not require Reactor to function.

For developers who wish to take advantage of both frameworks, Fungle API includes Reactor compatibility support, allowing them to be used together seamlessly. Combining both APIs is recommended when you want access to a broader set of features and development tools.

## Credits

Cosmetic system based on [CorsacCosmetics](https://github.com/XtraCube/CorsacCosmetics)
Role tab [Mira API](https://github.com/All-Of-Us-Mods/MiraAPI)
