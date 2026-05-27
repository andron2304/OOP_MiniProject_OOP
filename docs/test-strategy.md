# Test Strategy for MusicPlayer_OOP Lab 36

## Critical Scenarios
- Loading and playing tracks in a playlist.
- Adding/removing tracks and maintaining playlist order.
- Persisting and retrieving playlist data through repository storage.
- Handling invalid track data and repository failures gracefully.

## Testing Seams
- `PlayerService` boundaries where domain logic calls repository operations.
- `Playlist` behavior when modifying track collections.
- Repository interface (`IRepository<T>`) to swap in-memory test doubles or real persistence implementations.
- Exception handling paths for validation errors and storage failures.

## Mock vs Real Integration
- Unit tests: mock `IRepository<Playlist>` and `IRepository<Track>` to verify domain and service behavior without disk/state dependency.
- Integration tests: use real `InMemoryRepository` and the domain model together to validate end-to-end playlist creation, persistence, and retrieval.
- Reserve full-stack integration for any future file-based or database repository implementations.

## Negative Scenarios to Protect
- `Track` creation with empty title, invalid duration, or null required fields.
- Adding duplicate or malformed tracks to a playlist.
- Removing a non-existent track from a playlist.
- Repository save/load failures, including simulated exceptions from storage.
- Invalid playlist state transitions when operations are disallowed.
