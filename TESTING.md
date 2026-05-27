# Running Tests and Viewing Coverage

## Run Unit Tests
1. Open a terminal at the repository root.
2. Run:

```powershell
dotnet test ./tests/MusicPlayer.Tests/MusicPlayer.Tests.csproj
```

## Run All Projects Tests

```powershell
dotnet test MusicPlayer.sln
```

## View Coverage Locally
1. Install a coverage tool if needed, for example `coverlet` or `dotnet-reportgenerator`.
2. Run coverage with Coverlet:

```powershell
dotnet test ./tests/MusicPlayer.Tests/MusicPlayer.Tests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

3. Generate an HTML report if `reportgenerator` is available:

```powershell
dotnet tool install --global dotnet-reportgenerator-globaltool
reportgenerator -reports:./tests/MusicPlayer.Tests/coverage.opencover.xml -targetdir:./tests/MusicPlayer.Tests/CoverageReport
```

## Recommended Checks
- Verify that the `CoverageReport/index.htm` file opens in the browser.
- Confirm unit tests cover edge cases and error paths for the domain model and repository.
