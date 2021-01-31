# Fairy

A source generator that generates C# code based on Sitecore Content Serialization (SCS) `.yml` files.

![publish to nuget](https://github.com/hermanussen/Fairy/workflows/publish%20to%20nuget/badge.svg) [![Nuget](https://img.shields.io/nuget/v/Fairy)](https://www.nuget.org/packages/Fairy/) [![Nuget](https://img.shields.io/nuget/dt/Fairy?label=nuget%20downloads)](https://www.nuget.org/packages/Fairy/) [![Twitter URL](https://img.shields.io/twitter/url?style=social&url=https%3A%2F%2Ftwitter.com%2Fknifecore%2F)](https://twitter.com/knifecore)

> :warning: This package is highly experimental! It has not been tested very well.

## Installation

1. Install the NuGet package in your project. Run the following command in the NuGet package manager console
```
Install-Package Fairy
```
2. Ensure the SCS `.yml` files that you want to generate code for are added as `AdditionalFiles` in your `.csproj` file. E.g.:
```xml
<ItemGroup>
  <AdditionalFiles Include="../serialization/**/*.yml" />
</ItemGroup>
```
3. You can now use the generated classes in your code. Reference the following namespace to get to them.
```csharp
using Templates;
```

## Use your own Scriban template instead of the default one

If you have specific needs for the generated code, you can easily create a [Scriban](https://github.com/scriban/scriban) template to replace the default one. All you have to do is:
1. Create a file named `FairyTemplate.sbntxt` and place it in the root of your project.
2. Ensure that the file is included in the `AdditionalFiles` for your project (the same way that you include your json files).
3. Copy the contents of the [default template](Fairy/FairyTemplate.sbntxt), paste them in the file and save. Or find a template you like under [the templates that are used for testing](Fairy.Tests/Templates/)
4. Change the template in any way you want, and you should observe the changes when you build your project.

## More

It was based on ideas and code from:
- [Leprechaun](https://github.com/blipson89/Leprechaun) - code generation tool for Sitecore
- [Helixbase](https://github.com/muso31/Helixbase) - used mainly for testing
- [RainbowDataAnalyzer](https://github.com/hermanussen/RainbowDataAnalyzer) (my own project)
- [JsonByExampleGenerator](https://github.com/hermanussen/JsonByExampleGenerator/) (my own project)