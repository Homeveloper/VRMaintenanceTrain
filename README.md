# Equipment Maintenance Trainer

A short Unity 6 maintenance scenario for PC and XR. Turn off the power, fit a replacement part into the matching socket, then hold the tool in the work zone for two seconds. Actions out of order do not advance the scenario and show a hint. The result screen has restart and main-menu buttons.

## Open and run

- Unity version: **6000.5.9f1**.
- Open `Assets/Scenes/MainMenu.unity` and enter Play Mode.
- In `Window > Asset Management > Addressables > Groups > Play Mode Script`, select **Use Asset Database (fastest)** for editor testing. `Workshop` is loaded using the address `Scenes/Workshop`.
- For a Windows build, use **Tools > Build Windows Trainer**. This builds Addressables first and writes the player to `Builds/Windows/`. You can also build Addressables manually and then build the Windows player from Build Profiles.

## Desktop controls

1. Click the power lever.
2. Click the green replacement part, then the green socket. The red socket is incorrect. Right-click to put down a held part or tool.
3. Click the tool, aim at the yellow work zone, and hold the left mouse button for two seconds.

`Esc` returns to the main menu. The result screen also offers **Restart** and **Main Menu**.

## Project structure

- `Assets/Scenes` contains the main menu and workshop.
- `Assets/Scripts/Core` contains the ordered scenario rules.
- `Assets/Scripts/Interactables` handles the lever, part, sockets, and tool.
- `Assets/Scripts/App` handles desktop input, runtime mode, and scene loading.
- `Assets/Scripts/UI` updates hints, progress, and the result screen.
- `Assets/Editor/BuildWindows.cs` is an optional one-click Windows build command.

The project uses OpenXR, the Input System, XR Interaction Toolkit, and Addressables. PC mode and the editor's XR Interaction Simulator have been used for testing. A physical VR headset has not been tested.
