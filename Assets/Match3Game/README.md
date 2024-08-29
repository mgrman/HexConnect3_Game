# Description

Match3Game folder contains the main codebase.

`Grid` serves as the starting point. It stores the values for the hex grid and initializes each cell of the hex grid.

The configuration is handled in the Unity Scenes and prefabs. See Application folder for examples.

To improve: Use Zenject or similar library to allow Dependency injection. This would remove a lot of unnecessary tedious configuration in the Unity Scene.