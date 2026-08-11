# AGENTS.md

## Project
This is a small Flappy Bird-style clone made in Unity using the Universal 2D template.

The goal is to build the game incrementally while keeping the project simple, understandable, and easy to debug.

## Development approach
Work one small step at a time.

Do not generate the entire game at once.

For each task:

1. Inspect the existing project before changing anything.
2. Explain briefly what you intend to change.
3. Implement only the current requested feature.
4. Keep changes small and focused.
5. Tell me what I need to do manually in the Unity Editor, such as:
   - creating GameObjects
   - adding components
   - assigning Inspector references
   - creating prefabs
   - changing Rigidbody2D or Collider2D settings
6. Tell me exactly how to test the change.
7. Stop after the feature is working before moving to the next system.

Do not automatically continue to the next feature.

## Flappy Bird structure
Build the game roughly in this order:

1. Basic scene and camera
2. Bird GameObject
3. Rigidbody2D gravity
4. Flap/jump input
5. Ground and collision
6. Pipe pair
7. Pipe movement
8. Pipe spawning with randomized gap height
9. Pipe cleanup/despawning
10. Bird collision and game over
11. Score trigger between pipes
12. Score UI
13. Restart flow
14. Start screen
15. Audio and visual polish

Do not implement later systems early unless they are required for the current step.

## Code style
Use simple Unity C#.

Prefer small, focused MonoBehaviour scripts over large manager classes.

Use descriptive names such as:

- BirdController
- PipeMover
- PipeSpawner
- GameManager
- ScoreTrigger

Keep scripts under:

`Assets/Scripts/`

Avoid unnecessary abstractions, frameworks, dependency injection, complex design patterns, or premature optimization.

Use Unity's standard 2D systems such as:

- Rigidbody2D
- Collider2D
- Trigger colliders
- Prefabs
- Inspector references

Prefer `[SerializeField] private` fields instead of unnecessary public fields.

## Error handling and debugging
Never blindly rewrite large sections of working code to fix one error.

When an error occurs:

1. Read the full Unity Console error.
2. Identify the script and line causing it.
3. Explain the likely cause.
4. Make the smallest reasonable fix.
5. Check whether the fix could break existing behavior.
6. Give me a specific test to verify the bug is actually fixed.

For runtime bugs, check Unity setup as well as code.

Common things to verify include:

- missing Inspector references
- missing components
- incorrect Rigidbody2D settings
- incorrect Collider2D settings
- trigger settings
- object layers
- prefab configuration
- script attachment
- object hierarchy
- tags
- scene references

Do not assume every problem requires new code.

## Safety rules
Do not delete scenes, prefabs, assets, or existing working scripts unless explicitly necessary.

Do not make broad project-wide changes for a small feature.

Do not modify Unity-generated folders such as:

- Library
- Temp
- Logs
- obj

Avoid editing project settings unless the current task specifically requires it.

## Communication
Keep explanations concise but specific.

When giving Unity Editor instructions, use exact GameObject, component, field, and menu names whenever possible.

If something depends on the current project state, inspect the project rather than guessing.

The priority is:

working game → understandable code → easy debugging → polish.