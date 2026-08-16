# unity-project-beta-3D

Unity 6 stealth game prototype with automated EditMode tests and GitHub Actions infrastructure.

## Local Tests

Tests live in `Assets/_3DStealthGame/Tests/EditMode`.

Run the EditMode suite locally from the repository root:

```powershell
unity -batchmode -projectPath . -runTests -testPlatform EditMode -testResults TestResults/EditMode.xml -enableCodeCoverage -coverageResultsPath CodeCoverage -coverageOptions "generateAdditionalMetrics;generateHtmlReport;generateBadgeReport;assemblyFilters:+Assembly-CSharp" -quit
```

The project uses Unity `6000.4.7f1`, the Unity Test Framework package, and the Unity Code Coverage package pinned in `Packages/manifest.json`.

## GitHub Actions

The `CI` workflow runs on pull requests and pushes to `main`.

### Unit Tests

`Unit Tests` runs Unity EditMode tests through `game-ci/unity-test-runner@v4`, caches the Unity `Library` folder, uploads test artifacts, and generates coverage output for `Assembly-CSharp`.

This job needs Unity activation secrets configured in the repository:

- `UNITY_LICENSE` for a Unity Personal license file
- `UNITY_EMAIL`
- `UNITY_PASSWORD`

For a paid Unity license, set `UNITY_SERIAL` with `UNITY_EMAIL` and `UNITY_PASSWORD` instead of `UNITY_LICENSE`. The workflow includes a preflight check so a missing `UNITY_LICENSE` or `UNITY_SERIAL` fails with a direct setup message before `game-ci/unity-test-runner` starts Unity.

### Code Scanning: Quality

`Code Scanning / Quality` validates Unity package JSON files and checks that EditMode test scripts keep matching Unity `.meta` files.

### Code Scanning: Security

`Code Scanning / Security` runs GitHub CodeQL for C# with `build-mode: none`, which avoids depending on Unity-generated build output. CodeQL results appear in GitHub Code Scanning when the repository has Code Scanning available. Public repositories generally get this for free; private repository availability can depend on the GitHub plan or Advanced Security settings.

### Dependency Automation

Dependabot is configured for weekly updates to GitHub Actions. GitHub Dependabot does not currently list Unity package manifests as a supported ecosystem, so Unity package updates still need to be reviewed through Unity Package Manager or direct manifest changes.
