# Roccet

Work in progress 3D model rocket visualizer program.

## TODO
- [ ] Configuration UI
  - [ ] Model import
    - [ ] .obj with texture/material data (.mtl)
    - [ ] .fbx with bundled texture/material data
  - [ ] Data log file import
    - [ ] Standardized log file format
    - [ ] Ability to omit certain fields (like TVC data, orientation, inertial acceleration) and have some of them computed by the visualizer
    - [ ] Field remapper for non-standardized log files
  - [ ] Camera angle selection
  - [ ] Time scale control
- [ ] Video file export
- [ ] Better motor exhaust VFX
- [ ] Better scenery
  - Real life map data?
  - Grass, clouds, other props etc.
  - Launchpad
- [ ] Parachutes
- [ ] Clean up the Unity project

### Stretch goals
- [ ] Timeline?
  - [ ] Mark various events (burnout, apogee, time scale transitions) and state transitions (like for Rigidbody state)
- [ ] Multiple rockets
- [ ] Scriptable rockets
  - A standardized abstract/"frontend" API where CSV data would just be one of the "backends"
  - Could even support scripting at runtime through C# CompilerServices or a different scripting language

## Credits
- Rocket and Motor model by [ZegesMenden](https://github.com/zegesmenden)