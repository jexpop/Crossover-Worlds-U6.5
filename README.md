# Crossover-Worlds-U6.5

Proyecto Unity **6000.6.5f1 (Unity 6.5)** con pipeline URP.

## Requisitos

- Unity `6000.6.5f1` (ver `ProjectSettings/ProjectVersion.txt`)
- Git + Git LFS (recomendado para binarios grandes)

## Assets excluidos del repo (importante)

Por tamano y licencia, las siguientes carpetas estan ignoradas en `.gitignore` y **NO se suben a GitHub**:

```
/Assets/Third Party/
/Assets/RealToon/
```

Al clonar el repo esas carpetas no existiran y veras materiales rosas / errores de scripts faltantes hasta que las restaures.

### 1. `Assets/Third Party/` (~2.9 GB)

Contiene assets de pago / externos. Reimportalos desde el Asset Store / Package Manager / tus backups en estas rutas exactas:

- `Assets/Third Party/AssetInventory`
- `Assets/Third Party/AssetInventory 1`
- `Assets/Third Party/GPUInstancerPro`
- `Assets/Third Party/MapMagic`
- `Assets/Third Party/NatureManufacture Assets` (incluye WorldStreamer + packs `HD RP 17 Unity 6` / `URP 17 Unity 6`)
- `Assets/Third Party/Point-Grass-Renderer`

### 2. `Assets/RealToon/` (~93 MB)

- `Assets/RealToon/` (RealToon shader: Editor, Shaders, Resources, Tools, Shader Packages, Examples)

Importa RealToon desde el Asset Store en esa misma ruta.

## Como restaurar el proyecto tras clonar

1. Clona el repo:
   ```bash
   git clone <url> Crossover-Worlds-U6.5
   ```
2. Abre el proyecto con Unity `6000.6.5f1`. Deja que genere `Library/`.
3. Importa los assets listados arriba en sus rutas exactas.
4. Verifica que no haya errores en Console y que las escenas en `Assets/Scenes/` abran correctamente.

## Notas de versionado

- `.gitignore` basado en el template oficial de GitHub para Unity (`github/gitignore/Unity.gitignore`).
- Solo se anaden al final las exclusiones de `Third Party` y `RealToon` (+ sus `.meta`).
- `Library/`, `Temp/`, `Obj/`, `Logs/`, `Builds/`, `UserSettings/`, `MemoryCaptures/`, `Recordings/`, `*.csproj`, `*.sln`, `.vs/` estan ignorados por defecto.
- `*.unitypackage` esta ignorado por defecto en el template oficial.
- Se versionan: `Assets/` (excepto lo excluido), `ProjectSettings/`, `Packages/manifest.json` y `Packages/packages-lock.json`.
