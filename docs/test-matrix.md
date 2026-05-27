# Test Matrix for MusicPlayer_OOP Lab 36

| Use Case | Unit Test | Integration Test |
|---|---|---|
| Create playlist and add tracks | `Playlist_AddTrack_AppendsTrack` | `PlaylistService_CanCreateAndSavePlaylist` |
| Remove track from playlist | `Playlist_RemoveExistingTrack_RemovesTrack` | `PlaylistService_RemoveTrackPersistsChanges` |
| Validate track metadata | `Track_InvalidTrack_ThrowsInvalidTrackException` | `PlaylistService_RejectsInvalidTrackDuringAdd` |
| Persist playlist data | `PlaylistRepository_Save_CallsRepository` | `Repository_InMemory_SaveAndLoad_ReturnsSamePlaylist` |
| Handle storage failure | `PlayerService_Save_ThrowsStorageExceptionOnRepositoryFailure` | `Repository_InMemory_Save_ThrowsStorageExceptionOnFailure` |
| Prevent invalid playlist operations | `Playlist_RemoveMissingTrack_ThrowsArgumentException` | `PlaylistService_InvalidRemoveRequest_ReturnsFailure` |
