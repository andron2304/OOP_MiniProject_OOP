# Iteration 2 — Implementation Plan

## Use Cases to Implement

1. Add and Play Track
   - User adds a `Track` to the playlist and requests playback. `PlayerService` enqueues the track and begins playback using the active `IPlaybackStrategy`.

2. Save and Load State
   - Persist playlist contents, current index, and selected playback strategy to an `IDataStore<PlaylistState>` and restore them on load.

3. Playback Strategy Management
   - Select and switch playback strategies at runtime (e.g., Normal, Shuffle); changes affect subsequent playback behavior immediately.

## Points of Extension

- Add strategies by implementing `IPlaybackStrategy` (e.g., `RepeatOneStrategy`, `RepeatAllStrategy`).
- Add persistence backends implementing `IDataStore<T>` (e.g., `JsonDataStore<T>`, `SqlDataStore<T>`).  
- Extend `PlaylistState` with analytics (play counts, last-played timestamps) without changing core playback logic.

## Technical Risks

- Corrupt or partial saves: mitigate with atomic writes (write temp file then rename) and schema versioning.
- Race conditions between playback and save/load: mitigate with short-lived locks and copying state snapshots for persistence.
- Non-deterministic shuffle: mitigate by injecting RNG or seed into `ShuffleStrategy` to allow deterministic tests.
- Migration of persisted formats: mitigate by storing a `version` field in `PlaylistState` and implementing upgrade paths.
