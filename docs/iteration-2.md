# Iteration 2 — Business Rules and Tests

## Business Rules Implemented

1. Track Uniqueness: each `Track` is identified by an `Id`; adding a track with an existing `Id` is ignored or updates metadata.

2. Play Only Existing Tracks: `Play()` will only start or continue if the current index points to a track in the playlist; playing an empty playlist is a no-op.

3. Persisted State Completeness: `Save()` must persist playlist order, current index, and active strategy name so `Load()` fully restores playback state.

4. Strategy Isolation: playback ordering rules are encapsulated in `IPlaybackStrategy` implementations; `PlayerService` delegates next-track selection.

5. Deterministic Shuffle for Tests: `ShuffleStrategy` supports injection of a seed or RNG to produce repeatable sequences in tests.

## Why Use the Strategy Pattern for Playback

- Swap implementations at runtime without changing `PlayerService`.  
- Keeps playback-selection logic isolated and testable (unit-test each strategy independently).  
- Open/Closed: add new strategies (repeat, weighted shuffle) without modifying existing code.  
- Enables dependency injection and deterministic behavior for tests (inject seeded RNG into `ShuffleStrategy`).

## Integration Tests to Cover in Lab 36

- Save/Load round-trip: create a playlist, set strategy, play a few tracks, save, then load and assert playlist, index, and strategy restored.
- Strategy behavior end-to-end: verify `NormalStrategy` plays in order and `ShuffleStrategy` covers all tracks without repeats (with deterministic seed).
- Persistence backend contract: `JsonDataStore<T>` correctly writes and reads `PlaylistState`, including schema versioning.
- Concurrency scenario: saving during playback does not alter the in-memory playback sequence or cause exceptions.
- Switching strategies mid-playback: confirm subsequent track selection follows the new strategy and that restored state maintains the new strategy.
