![License: MIT](https://img.shields.io/badge/License-MIT-yellow)
![GitHub stars](https://img.shields.io/github/stars/owmacohe/penchant)
![GitHub downloads](https://img.shields.io/github/downloads/owmacohe/penchant/total)

![Penchant logo](Media/Logo/Penchant_Logo_200.png)

# Penchant

*pen • chant*

> 1. a strong or habitual inclination.
> 2. a tendency to do something.
> 3. a taste or liking for a person or thing.

## Overview

**Penchant** is a simple and effective random number generation plugin. Random number generation is integral to video game development, from item drops, to terrain generation, to procedural soundscapes. Sometimes, consistency of random numbers is necessary either for testing, balance, or reproduction purposes. **Penchant** aims to provide a simple solution to both generate and reliably reproduce seeded random values.

## Installation

1. Install the latest release from the [GitHub repository](https://github.com/Owmacohe/Penchant/releases), unzip it, and place the folder into your Unity project's `Packages` folder.
2. Return to Unity, and the package should automatically be recognized and visible in the **Package Manager**.
3. A sample scene can be found at: `Penchant/Example/Example.unity`.
4. Opening this scene may prompt you to install **Text Mesh Pro**. Simply click on **Import TMP Essentials** to do so.

## Usage

1. To begin generating random numbers, simple create a new `SeededRandom` object with your desired seed, and get the object's `RandomValue`. Your seed can be any string.
2. You may wish to create multiple `SeededRandom`s if you have multiple systems in your game *(e.g. a random enemy spawn system and a random item reward system)*
3. The `SeededRandom` class comes with other methods for generating useful random object types *(e.g. `RandomRange`, `RandomVector3` `RandomRotation`)*
4. If at any time you want to reset a `SeededRandom`, so that it starts generating numbers from the beginning again, simple use `SeededRandom.Reset()`.
5. Lastly, to preview seeds and their distributions, use the `Penchant Visualizer`, which can be accessed in the menu bar via `Penchant/Visualizer`.

**Example:**
```C#
var enemySpawnSeededRandom = new SeededRandom("enemies");
var itemDropSeededRandom = new SeededRandom("items");

for (int i = 0; i < enemyCount; i++)
{
    EnemyController enemy = Instantiate(
        enemyPrefab,
        enemySpawnSeededRandom.RandomVector3 * arenaSize,
        enemySpawnSeededRandom.RandomQuaternion);
    
    enemy.itemDrop = itemDropSeededRandom.RandomEntry(itemDrops);
}
```