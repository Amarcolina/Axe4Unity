# Axe-4-Unity

[Give it a try in your Browser!](https://amarcolina.github.io/Axe4Unity/)

Axe-4-Unity is a compiler and execution environment to run Ti83/84+ Calculator Axe programs within the Unity game engine.  Import your .8xp source files into Unity, and they get automatically compiled into an executable asset. The included execution environment can run these executables, allowing you to experience legacy Axe games and applications.  Notably, Axe-4-Unity does NOT emulate the calculator operating system, and so it does NOT require a ROM dump to function.  It operates directly on the Axe source, and provides its own execution environment separate from the calculator.

The [Axe Parser project](https://www.ticalc.org/archives/files/fileinfo/456/45659.html) allowed users to write Axe programs directly on their graphing calculators using the built-in TiBasic editor, and the Axe Application would then compile these programs into native assembly programs.  Even though the users used the built-in TiBasic editor, the syntax and rules of Axe programs were totally custom.  Axe unlocked a new way to build games and applications that was previously only available to those developing using native assembly on a computer.  With Axe-4-Unity, you can experience these games and applications again, and package them how you see fit with Unity's rich platform compatibility.

## Examples / Media

<img width="288" height="192" alt="BallPhysics" src="https://github.com/user-attachments/assets/c0a3c317-df51-49b3-9c00-3a8a8aae7aa3" />
<img width="288" height="192" alt="CUBE" src="https://github.com/user-attachments/assets/34e04f1e-1d64-4653-b6fb-c215d5147eec" />
<img width="288" height="192" alt="Zedd" src="https://github.com/user-attachments/assets/c2562636-ae79-4bde-86e7-13f37017d275" />
<img width="288" height="192" alt="Tag" src="https://github.com/user-attachments/assets/0b6b4875-8e01-44f8-89cb-e6a709c33294" />
<img width="288" height="192" alt="PortalPrelude" src="https://github.com/user-attachments/assets/26f86541-6c96-4461-b112-f0380615baae" />
<img width="288" height="192" alt="Racer" src="https://github.com/user-attachments/assets/79d48c59-b844-409f-b5a3-6efb0bcc260a" />
<img width="288" height="192" alt="RPG" src="https://github.com/user-attachments/assets/e7a636e9-b8b9-48e9-8ce9-db2a7bb36c52" />
<img width="288" height="192" alt="Starship" src="https://github.com/user-attachments/assets/e719687a-4f2c-49ae-876f-de87bef15263" />

-------

<img width="1117" height="750" alt="Debugger" src="https://github.com/user-attachments/assets/ee862c09-ff4b-4f33-b527-fd22af979fe0" />

-------

<img width="571" height="505" alt="Inspector_AxeRunner" src="https://github.com/user-attachments/assets/12d184b1-8007-49b4-8c73-7d2a336f7655" />
<img width="571" height="341" alt="Inspector_ScreenAndKeyboard" src="https://github.com/user-attachments/assets/d6cf9fdb-4c33-4291-89be-ba4476f86c95" />

## Features

 - **Axe4Unity Compiler**.
    - Compiles .8xp axe source files into a data asset that can be run within Unity
    - Unity scriptable import integration, just drag-n-drop your .8xp files into Unity to Compile!
    - Handles multi-file includes
    - Handles picture file includes
 - **AppVar Importer**
    - Drag-n-drop your AppVar files into Unity to turn them into data assets
    - Useful if your Axe programs require data files to run
 - **Axe Virtual Machine**
    - C# machine that runs compiled Axe source code
    - Mostly Unity-agnostic so you can integrate as you see fit
    - Includes very basic file system with RAM/Archive
    - Easy to integrate for any custom use-case, from running Unit tests, to simulating a whole game
    - State can be copied and restored for easy rewind / save-states
 - **Unity Integration**
    - A fully-featured integration of the AxeVirtualMachine to allow drag-n-drop experience
    - Integrates with Unity update cycle for frame-based simulation
    - Integrates with Unity rendering for easy visualization of what's happening
    - Integrates with Unity input system for easy keyboard or UI control
 - **Axe Virtual Screen**
    - Part of the Unity Integration, visualizes the display buffer using Unity rendering
    - Supports 100% flicker-free greyscale
    - Supports basic simulation of LCD response time for more accurate-feeling experience
 - **Debugger**
    - Allows you to debug your Axe programs in real-time as they run in Unity
    - View the lines of the currently-running program, including the specific Operation and call stack
    - Add Breakpoints to stop execution when a specific line is reached
    - View the details and metadata of the current Operation
    - View variable information, in hex, signed, and unsigned values
    - View custom expressions you can type-in
    - Execute custom expressions you can type-in
    - Visualize the intermediate state of the display buffers, front, back, and combined
    - Watch specific variables or memory locations, stepping until they are changed
 - **Comprehensive Unit Testing**
    - Lots of coverage for Axe features to ensure stability
    - Specific coverage for handling nuanced Axe optimizations, like constant folding or HL value fall-through

## Platform Support
There is no platform-specific code anywhere in the codebase, so Axe-4-Unity should run on any operating system that Unity supports, and export to any target platform Unity supports.  Notably, Axe-4-Unity does not involve any threaded operations, and so web export is also possible, to allow exporting your Axe applications into a web environment.

## Getting Started
You can either clone this repository directly, copy the package by hand into your project, or include the package in your own Unity project by using a git package reference from the Unity package manager.  After the package is installed into your Unity project, .8XP and .8XV files should automatically start importing as ProgramAsset and AppVarAsset files.

**NOTE:** Not much work has been spent ensuring that the compiler rejects *incorrect* Axe code, only that it accepts *correct* Axe code.  If you are trying to use Axe4Unity as a regular development environment and writing fresh Axe code, you may need to make extra sure your code is valid Axe, or you may get a weird result that you don't expect instead of an error!

## Command Support
Command support is ongoing, but a large percent of commands are already supported, enough to successfully port many real-world games.  For a detailed command support table, you can take a look at [this spreadsheet](https://docs.google.com/spreadsheets/d/1CxBL67rhhKhXEnQ2w5x26f7lzPvsHPnrVhZ7roYywWQ/edit?usp=drive_link).

Commands fall into a few different categories:
 1) Implemented Fully (Green):  The command is implemented and works exactly as intended.
 2) Implemented Partially (Yellow):  The command is only partially implemented, or has differences that have not been resolved.  Signed division is an example of this.
 3) Implemented As No-Op (Teal):  The command is ported but does nothing.  DiagnosticOn is an example of this.
 4) Not Yet Implemented (White):  The command is not yet implemented, but could be added in the future.  BigEndian support is an example of this.
 5) Not Planned (Red):  The command is not planned to be implemented ever.  Interrupt or LinkPort commands are examples of this.

If you have a specific command you want support for, feel free to make a pull request!  Similarly, if you notice an issue with a command, feel free to open an Issue.

## Motivation and Design

One of the biggest decisions to make when starting this project was deciding on whether or not the system would operate at the Axe level, or the z80 assembly level.  I chose to design a system that operates on the Axe level, operating on Axe source files directly, and basically ignoring z80 entirely.  However, it would also be possible to operate on the z80 level, emulating z80 assembly and operating on compiled binaries instead.  There are some tradeoffs which are interesting, here are the pros and cons that I considered:

**Axe-Centric Benefits**
 + Lower overall effort.  Writing an accurate z80 asm emulator I believe takes more overall code and getting things exactly right.  With an Axe-centric approach, I don't have to emulate the calculator nearly as accurately.  For example, for a z80 centric approach, I would need to properly mock all of the potential OS calls that an Axe program could take, with all of those subtleties.
 + Easier to get partially-working results.  When I was developing the Axe version, I could implement a handful of easy operations and slowly step into the process.  With z80 emulation, a fair amount of z80 would have to be working correctly before I could even start to see results on the screen.
 + Easier to debug.  When something goes wrong, I can inspect Axe code line by line, and see if the results match my expectations.  Doing this for z80 can be much harder, as I'd be reading raw un-commented z80, trying to infer what part of the program I am in, and what it *should* be doing.
 + Easier to test.  Writing unit tests in Axe is super straightforward, just a handful of Axe lines are sufficient to test a feature.
 + Easier to integrate into Unity.  By working in the high-level Axe language, it is much easier for the Unity runner to know what defines a frame, or where to input a keycode, as those concepts are directly present in the Axe language.
 + Easier to write new Axe programs.  Without a z80 emulator and ROM, there is no easy way to write new Axe programs and compile them into a working z80 binary.  Working directly on the Axe language allows new programs to be written and tested without invoking the original z80 compiler.

**Z80-Centric Benefits**
 + Higher accuracy ceiling than Axe-centric.  Using z80 bypasses re-implementing all of the nuances of the Axe language and parser, and so is more able to get bit-accurate results.  The signed-division operator is a good example of this.  There were a lot of bugs involved with the naive C# implementation because it did not properly emulate some of the edge-cases in the z80 implementation.  The same goes for the drawing operations like line or circle, being pixel-accurate can be tricky when the operation is so complex, and using direct z80 emulation allows for a much more natural way to get good accuracy.
 + Able to run binaries without requiring source to be published.  Many Axe programs are published to TiCalc or Forums without source, and the ability to run a direct binary can be a big benefit for preserving these programs.
 + Possibly able to run non-Axe assembly programs as well.  Axe programs are just regular assembly programs, so the only restriction would be what set of OS functions or hardware features need to be fully emulated to support an assembly program.

## How it works
The package is split into a few different conceptual pieces:
 - The Parser
 - The Compiler
 - The Virtual Machine
 - The Unity Integration

The Parser opens the .8XP files and reads the tokenized data into a sequence of token objects.  The parser is what handles source-level operations, like `prgm` includes, or colon line splits.  The output is a simple list of lines, where every line is a simply a list of tokens for that line.

The Compiler traverses this list of tokens output by the Parser and compiles the result into a Program asset.  The Program asset is basically just a list of operation objects (Ops), where each Op represents a single action that can be taken.  Each Op is hand-coded and represents a single action that can be taken, such as drawing a Sprite, adding 2 numbers, storing to a variable, clearing the screen, etc...  The Compiler handles all of the nuance of emitting the right tokens in the right order.  These operations are serialized using Unity serialization just as regular Objects, so there is no need for any encoding.

The virtual machine is then able to execute these compiled programs by stepping through the operations one at a time and executing them in sequence.  The machine contains all of the structure needed to represent the virtual calculator, including memory locations, and mocked parts of the OS, such as the file-system.  The machine also handles execution state, like the call stack, argument stack, and current state of HL.  The state of the machine can also be copied and restored easily, allowing for easy rewind or save-state.

The Unity Integration is then the opinionated integration to the Unity execution environment.  This handles how execution is tied into the Unity update cycle, how frames are rendered, and how input is passed into the virtual machine.  For example, the Runner component makes the decision on when to consider that the Axe program has generated a "Frame" and that it should yield to Unity to allow the Unity application to progress.

## Experimental Native Code Generation

A currently-in-progress feature is the ability to generate 'Native Runners' for specific programs.  This is a generated C# source file that is generated from the Axe program when it is imported.  This file can be compiled by the Unity Burst Compiler, which results in a native binary that executes operations directly instead of stepping through them one at a time.  This can result in huge speed boosts.  The feature is still experimental and has lots of known bugs and has the potential to freeze or crash the editor.  It is also not compatible with the Debugger, which requires the full Virtual Machine environment to work properly.  Give it a try if you are curious, but use at your own risk!
