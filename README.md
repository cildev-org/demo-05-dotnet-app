# demo-05-dotnet-app

Demo target for Blue-Oryx dependency pull requests in NuGet (fix class A1).

> **This app is vulnerable on purpose.** `main` references `SharpZipLib`
> `1.3.2`, which is affected by CVE-2021-32841 (a tar entry `../uploads-x`
> is written next to the destination folder `/data/uploads`). Do not run it
> anywhere that is reachable from a network you care about.

## What is here

| File | Purpose |
| --- | --- |
| `LabApp.csproj` | The project. One direct `PackageReference` with a single version |
| `Program.cs` | ASP.NET Core app: `POST /extract` unpacks a tar into `/data/uploads` with SharpZipLib's `TarArchive`; `GET /file?name=` says whether a file exists under `/data` |
| `Dockerfile` | .NET 8 SDK stage that restores and publishes, ASP.NET Core runtime stage (both images pinned by digest) |
| `.github/workflows/build.yml` | Builds the image on every pull request; on a push to `main` it also pushes it to `ghcr.io/<repo>:<commit>` |

## Demo flow

1. Blue-Oryx receives a confirmed finding for CVE-2021-32841 on the asset bound
   to this repository.
2. It opens a pull request from a `blue-oryx/<id>` branch that changes one
   thing: the `Version` of the SharpZipLib reference in `LabApp.csproj`, to the
   lowest fixed version (`1.3.3`).
3. The `build` workflow runs on the pull request.
4. A person reviews and merges the pull request. Blue-Oryx never merges.
5. The workflow pushes the image of the merge commit. The operator deploys it
   to the lab target; Blue-Oryx never deploys.
6. The operator confirms the deploy and Blue-Oryx runs the same exploit again.
   Only a blocked re-run makes the task `verified-fixed`.

## Branches

| Branch | Purpose |
| --- | --- |
| `main` | What Blue-Oryx patches |
| `vulnerable-baseline` | The vulnerable state, for resetting `main` between demo runs |
