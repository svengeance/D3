# D3 MVP Roadmap

## Project Intent

D3 is a first Unity game project inspired by Momentum Missile Mayhem: a physics-heavy 2D defense game where the player launches energy projectiles to stop waves of enemies from reaching or damaging the base.

This roadmap is intentionally high level. Use it as durable context, then reacquire focused repo and Unity scene context before implementing each milestone.

## MVP Gameplay Loop

1. User enters the main menu.
2. User selects a level.
3. User enters the game scene.
4. Enemies spawn in a set number of waves.
5. User controls an energy slingshot/cannon/base with health.
6. Enemies can cause loss by damaging the base or by leaking past defenses.
7. User earns experience for each defeated wave.
8. User can open a paused upgrade menu during the level.
9. User spends experience to unlock new projectiles or upgrade existing ones.
10. Projectile availability increases with level progression.
11. User wins when all waves are complete.
12. User loses when base damage or leaked enemy limits are exceeded.

## Chosen Direction

- Build style: vertical slice first.
- MVP length: short arcade levels, roughly 3-5 minutes.
- First level structure: roughly 3-5 waves.
- Maps: a few hand-authored map shapes, not procedural generation.
- Progression: tiny version of both in-run XP spending and persistent projectile unlocks.
- Upgrade menu: paused gameplay while open.
- Controls: support mouse, touch, and gamepad.
- Gamepad aiming: native analog aim rather than cursor simulation.
- Learning priority: gameplay systems before polish.

## Implementation Milestones

### 1. Core Level Loop

Create one complete playable level loop before broadening content.

- Start level.
- Run waves.
- Track base health.
- Track leaked enemies separately from base damage.
- Detect win and loss.
- Restart or return to menu.

Future context prompt:

> Read the roadmap, then inspect the current Game scene, bootstrapper, player/base scripts, enemy despawner, and any manager scripts. Plan or implement the smallest core level loop.

### 2. Wave Spawning

Replace ad hoc spawning with simple wave data and a wave runner.

- Spawn enemies from wave definitions.
- Track spawned/alive enemies.
- Complete a wave when all enemies for that wave are gone.
- Award wave XP.
- Scale difficulty by level using simple counts, enemy types, spawn timing, or health.

Future context prompt:

> Read the roadmap, then inspect enemy spawning, enemy data, scenes, prefabs, and current ScriptableObject patterns. Design or implement the first wave runner.

### 3. XP and Projectile Upgrades

Use the existing projectile effect architecture as the foundation for upgrades.

- Define projectile identity and unlock requirements.
- Track in-run XP.
- Add a paused upgrade menu.
- Support a tiny set of upgrades such as damage, push force, bounce strength, or explosion unlock.
- Keep the first version data-light and easy to tune.

Future context prompt:

> Read the roadmap, then inspect projectile effects, projectile prefabs, PlayerManager, and UI setup. Plan or implement the smallest XP and projectile upgrade loop.

### 4. Menu, Level Select, and Save Data

Add enough shell around the game to feel like a real loop.

- Main menu.
- Level select.
- Results screen.
- Lightweight persistent save data for unlocked levels, persistent XP, projectile unlocks, and projectile upgrade levels.

Future context prompt:

> Read the roadmap, then inspect build settings, scenes, UI packages, and any existing menu assets. Plan or implement the minimal menu and save flow.

### 5. Map Variants

After the first level loop works, create a few hand-authored layouts.

Candidate maps:

- Baseline rectangle.
- Bottleneck or hourglass shape.
- Obstacle/island map that changes ricochets and enemy movement.

Each map should define:

- Player/base position.
- Enemy spawn zones.
- Leak or damage zones.
- Terrain colliders.
- Camera bounds/framing.

Future context prompt:

> Read the roadmap, then inspect current scenes, terrain layers, colliders, camera setup, and enemy movement. Plan or implement the next hand-authored map variant.

### 6. Input Abstraction

Move gameplay toward an aim/charge/release abstraction so mouse, touch, and gamepad can share core behavior.

- Mouse and touch use drag direction and distance.
- Gamepad uses analog direction and charge/release buttons.
- Projectile spawning should depend on a launch intent, not directly on mouse-only pointer state.

Future context prompt:

> Read the roadmap, then inspect InputManager, D3Input actions, ProjectileInputSpawnerController, and DragIndicatorController. Plan or implement input abstraction without breaking mouse drag.

## Open Design Questions

- Should enemy damage happen through collision, passing a boundary, explicit attacks, or a mix?
- Should projectiles be limited by cooldown, ammo, energy cost, or only launch timing?
- Which upgrades should be temporary per-run versus persistent?
- Should levels become a campaign ladder, mission select, or arcade progression?
- How much should the remake stay faithful to Momentum Missile Mayhem versus becoming its own game?
- What map shapes produce interesting projectile bounces and enemy movement without overwhelming a first MVP?

## Working Rule

Do not try to one-shot the whole game plan in one context. For each milestone:

1. Read this roadmap.
2. Inspect the relevant scripts and Unity scene state.
3. Make a small, decision-complete plan.
4. Implement only that slice.
5. Verify in Unity when possible.
6. Update this roadmap if the design changes.
