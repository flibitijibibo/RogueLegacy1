Believe it or not, Rogue Legacy uses a shader system that is very very old!

Like all XNA games, RL1 uses the Effects Framework, a forgotten DXSDK format.
XNA had a way to compile these via the Content Project, but you can get the same
binaries by using FXC, the old shader compiler from the DirectX SDK (June 2010).

Whether on Windows or not, install the June 2010 DirectX SDK and `buildEffects`
should do what it needs to do. It's not _exactly_ the same since XNA expects a
specific compression on top of the FX output, but FNA is more flexible, so to
accommodate community changes the SDL3 update for RL1 now uses the "fxb" format
directly, to avoid weird content overrides and collisions.

Enjoy!
