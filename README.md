# Rogue Legacy 1 Source Code

This is the source code for Rogue Legacy - more specifically, it is _only_ the
code, and only the FNA build. The assets and contentproj are _not_ included.

## License

Rogue Legacy's source code is released under a specialized, non-commercial-use
license. See
[LICENSE.md](https://github.com/flibitijibibo/RogueLegacy1/blob/main/LICENSE.md)
for details.

## Build Environment

The build environment for Rogue Legacy matches the one recommended by the FNA
project:

https://fna-xna.github.io/docs/1%3A-Setting-Up-FNA/

## Build Instructions

First, download this repository, FNA, and the native FNA libraries:

```
git clone --recursive https://github.com/FNA-XNA/FNA.git
git clone --recursive https://github.com/flibitijibibo/RogueLegacy1.git
curl -O https://fna.flibitijibibo.com/archive/fnalibs3.tar.bz2
tar xvfj fnalibs3.tar.bz2 --one-top-level
```

From here you should be able to `dotnet build RogueLegacy.sln`. The output
should be at `RogueCastle/bin/x64/Debug/net40/`.

Lastly, you'll need to copy a few files to the output folder manually:

- Copy the Content folder from your personal copy of the game
- Depending on your OS/architecture, copy the appropriate native libraries from fnalibs (for example, on Windows you would copy `fnalibs/x64/*.dll` next to RogueLegacy.exe)

The game should now start!

## Level Editor

The original level editor used to make Rogue Legacy can be found
[here](https://github.com/flibitijibibo/RogueCastleEditor). Note, however, that
the editor is no longer compatible with the latest version of the game!
