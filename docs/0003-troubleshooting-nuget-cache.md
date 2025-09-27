# Troubleshooting NuGet cache cleanup errors on Windows

When running `dotnet nuget locals all --clear` on Windows you might see errors such as:

```
error: No se pudo eliminar "C:\Users\Usuario\.nuget\packages\microsoft.build.tasks.git\8.0.0\tools\core\Microsoft.Build.Tasks.Git.dll".
```

These errors typically mean that some process still holds a file handle on a DLL inside the cache. Follow the steps below to release the locks and retry the cleanup.

## 1. Close Visual Studio, Rider, and other IDEs

Any open IDE or command prompt that previously built the solution may keep MSBuild or NuGet assemblies loaded. Close them before retrying the cleanup command.

## 2. Stop dotnet build servers

The .NET SDK caches MSBuild nodes and other helpers across sessions. Shut them down with:

```powershell
dotnet build-server shutdown
```

## 3. Kill lingering MSBuild or dotnet processes

If the cache still cannot be cleared, look for running processes that reference the locked DLLs:

```powershell
tasklist | findstr /I "MSBuild dotnet"  # review processes
```

End the suspicious processes from Task Manager or run:

```powershell
taskkill /IM MSBuild.exe /F
taskkill /IM dotnet.exe /F
```

Be sure to stop only processes related to your development session.

## 4. Retry the cleanup with elevated permissions

Some files might require administrator privileges to delete. Open a new PowerShell window **as Administrator** and run:

```powershell
dotnet nuget locals all --clear
```

## 5. Manually delete leftover folders (last resort)

If the command still fails, delete the offending folders manually. Use File Explorer or PowerShell:

```powershell
Remove-Item -Recurse -Force "$env:USERPROFILE\.nuget\packages\microsoft.build.tasks.git"
Remove-Item -Recurse -Force "$env:USERPROFILE\.nuget\packages\microsoft.sourcelink.common"
Remove-Item -Recurse -Force "$env:USERPROFILE\.nuget\packages\microsoft.sourcelink.github"
```

After removing the locked folders, rerun `dotnet nuget locals all --clear` to ensure the cache is fully cleared.
