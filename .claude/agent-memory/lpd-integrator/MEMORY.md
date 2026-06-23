# Aria (Integradora) — Memória do Agente

Mapa de integração do ecossistema LayoutParser (4 repos). Ancorado em arquivos reais.

## Topologia (4 repos, 1 pipeline)
`Sysmiddle → SQL Server (ConnectUS_Macgyver: tbLayout/tbMapper, [ValueContent] = Base64 cripto) → LayoutParserApi (net10.0) → LayoutParserDecrypt.exe (net4.8.1) → Redis → LayoutParserReact (Vite/React)`

## Key Patterns

### Por que o .exe existe
- A API net10.0 não roda `RijndaelManaged` legado em processo de forma compatível → descriptografia out-of-process via child exe. Esta é a razão de ser deste repo.

### Como a API invoca (LayoutParserApi)
- `Services/Database/DecryptionService.cs` (`IDecryptionService.DecryptContent(string)`): escreve cripto em arquivo temp (`Path.GetTempFileName`), roda o `.exe` via `Process`/`ProcessStartInfo` (UseShellExecute=false, CreateNoWindow=true, timeout 30s, ExitCode≠0 → throw), lê o output e apaga os temps.
- `BuildArgs` → `"input" "output" "correlationId" "logDir"`. Também seta env `LAYOUTPARSER_CORRELATION_ID` e `LAYOUTPARSER_LOG_DIR`.
- Caminho do exe: config `LayoutParserDecrypt:Path` (default `C:\inetpub\wwwroot\layoutparser\api\LayoutParserDecrypt.exe`); auto-probe em baseDir, baseDir\tools, ..\LayoutParserDecrypt\bin\{Release,Debug}.

### Redis (StackExchange.Redis, default localhost:6379)
- `Services/Cache/LayoutCacheService.cs` e `MapperCacheService.cs`. Chaves permanentes (sem expiry): `layouts:search:all`, `mappers:search:all`. Outras: `layout:id:{id}` (1h), `mapper:id:{id}`/`mappers:input:{guid}`/`mappers:target:{guid}` (24h).
- Populado no startup (`Program.cs RefreshCacheFromDatabaseAsync`) e via `POST /api/layoutdatabase/refresh-cache`.

### React (LayoutParserReact)
- Vite + React 18 + TS, axios. `src/services/api.ts` resolve baseURL por hostname (→ :5000). `X-Correlation-ID` nasce no browser e flui browser → API → log do .exe.
- `layoutService.hasLayoutsInRedis()` checa se `decryptedContent.length > 0`.

## Gotchas
- **CRITICAL:** só layouts `LayoutType == TextPositional` chegam ao Redis (filtro em `LayoutDatabaseService.cs`); os demais são descartados sem erro.
- **CRITICAL:** se o `.exe` falta, `DecryptionService` loga erro e devolve o conteúdo AINDA criptografado (passthrough silencioso) — parsing XML falha item a item sem erro explícito.
- `LayoutParserApi.csproj` referencia `LayoutParserLib.dll` mas nenhum código usa (referência morta).
- API menciona um `LayoutParserLowCodeRunner.exe` não presente entre estes repos.
