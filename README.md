# Roccet

Work in progress 3D model rocket visualizer program.

Developed in Unity 2021.1.13f1.

## TODO
- [ ] Configuration UI
  - [ ] Data log file import
    - [ ] Standardized log file format
    - [ ] Ability to omit certain fields (like TVC data, orientation, inertial acceleration) and have some of them computed by the visualizer
    - [ ] Field remapper for non-standardized log files
  - [ ] Camera angle selection
  - [ ] Time scale control
- [ ] Video file export
- [ ] Better motor exhaust VFX
- [ ] Better scenery
  - [ ] Launchpad
  - [ ] Seemingly infinite terrain
  - Grass, clouds, other props etc.
- [ ] Parachutes
- [ ] Multiple rockets
  - For [REDACTED] vis stuff.
- [ ] Clean up the Unity project

### Stretch goals
- [ ] Timeline
  - [ ] Mark various events (burnout, apogee, time scale transitions) and state transitions (like for Rigidbody state)
- [ ] Scriptable rockets
  - A standardized abstract/"frontend" API where CSV data would just be one of the "backends"
  - Could even support scripting at runtime through C# CompilerServices or a different scripting language
- [ ] Real life map data for scenery
- [ ] Custom model import
  - [ ] .obj with texture/material data (.mtl)
  - [ ] .fbx with bundled texture/material data
- [ ] Binding data log fields to transform properties of bodies in a model
  - Basically a generalization of TVC actuation which could be used with fins and other actuating surfaces.

## Credits
- Rocket and Motor model by [ZegesMenden](https://github.com/zegesmenden)
