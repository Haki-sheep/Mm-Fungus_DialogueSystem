[CmdletBinding()]
param(
    [string] $Command = 'help',
    [string] $ProjectPath,
    [string] $Id,
    [string] $File,
    [string] $Revision = '',
    [int] $TimeoutSeconds = 30,
    [switch] $Help
)

$ErrorActionPreference = 'Stop'
$CommandList = @('capabilities', 'list', 'read', 'validate', 'import', 'delete', 'audit', 'status')

function Write-JsonResult {
    param([object] $Value)
    [Console]::Out.WriteLine((ConvertTo-Json -InputObject $Value -Depth 100 -Compress))
}

try {
    if ($Help -or $Command -eq 'help') {
        Write-JsonResult @{
            protocolVersion = 1
            usage = 'pwsh -File Assets/AmanitaWebStory/CLI/story.ps1 <command> [-ProjectPath <project>] [-Id <id>] [-File <json>] [-Revision <revision>] [-TimeoutSeconds 30]'
            commands = @{
                capabilities = '返回八类剧情卡片的 JSON Schema 与当前资源目录'
                list = '列出 Unity 已回填剧情'
                read = '-Id 剧情标识 返回源数据和 revision'
                validate = '-File UTF-8 剧情 JSON 只校验'
                import = '-File UTF-8 剧情 JSON 新建时不填 Revision 更新时必须提供 read 返回的 Revision'
                delete = '-Id 剧情标识 -Revision 当前版本 将剧情移入回收目录'
                audit = '-Id 剧情标识 只读检查剧情 SO 的角色 立绘 音频缺失引用与依赖'
                status = '读取连接的工程与 Editor 状态 不返回令牌'
            }
            prerequisite = '目标 Unity Editor 中启用 Tools/MmFungus/打开网页剧情编辑器'
            schemaFile = (Join-Path $PSScriptRoot 'story.schema.json')
        }
        exit 0
    }
    if ($Command -notin $CommandList) { throw "未知命令 $Command" }
    if ($TimeoutSeconds -lt 1 -or $TimeoutSeconds -gt 300) { throw 'TimeoutSeconds 必须为 1 到 300' }
    if ($Command -in @('read', 'delete', 'audit') -and $Id -notmatch '^[A-Za-z][A-Za-z0-9_]{0,63}$') { throw '请提供有效的 -Id' }
    if ($Command -in @('validate', 'import') -and [string]::IsNullOrWhiteSpace($File)) { throw '请提供 -File 剧情 JSON 路径' }
    if ($Command -eq 'delete' -and [string]::IsNullOrWhiteSpace($Revision)) { throw '删除必须提供 -Revision 请先 read 获取当前版本' }
    if ($PSBoundParameters.ContainsKey('Id') -and $Command -notin @('read', 'delete', 'audit')) { throw '-Id 仅用于 read delete 和 audit' }
    if ($PSBoundParameters.ContainsKey('File') -and $Command -notin @('validate', 'import')) { throw '-File 仅用于 validate 和 import' }
    if ($PSBoundParameters.ContainsKey('Revision') -and $Command -notin @('import', 'delete')) { throw '-Revision 仅用于 import 和 delete' }
    if ([string]::IsNullOrWhiteSpace($ProjectPath)) {
        $ProjectPath = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
    }
    $ResolvedProject = (Resolve-Path -LiteralPath $ProjectPath).Path.TrimEnd([char[]]'\/')
    if (-not (Test-Path -LiteralPath (Join-Path $ResolvedProject 'ProjectSettings/ProjectVersion.txt') -PathType Leaf)) { throw "不是 Unity 工程 $ResolvedProject" }
    $EndpointPath = Join-Path $ResolvedProject 'Library/AmanitaWebStory/endpoint.json'
    if (-not (Test-Path -LiteralPath $EndpointPath -PathType Leaf)) { throw 'CLI 服务未启动 请在目标 Unity Editor 打开网页剧情编辑器' }
    $Endpoint = Get-Content -LiteralPath $EndpointPath -Raw -Encoding UTF8 | ConvertFrom-Json
    if ($Endpoint.protocolVersion -ne 1) { throw 'CLI 协议版本不兼容' }
    $EndpointProject = [IO.Path]::GetFullPath([string]$Endpoint.projectPath).TrimEnd([char[]]'\/')
    $PathComparison = if ([IO.Path]::DirectorySeparatorChar -eq '\') { [StringComparison]::OrdinalIgnoreCase } else { [StringComparison]::Ordinal }
    if (-not [string]::Equals($ResolvedProject, $EndpointProject, $PathComparison)) { throw 'CLI 连接文件属于其他工程' }
    $OriginUri = [uri]$Endpoint.origin
    if ($OriginUri.Scheme -ne 'http' -or $OriginUri.Host -ne '127.0.0.1' -or $OriginUri.AbsolutePath -ne '/' -or $OriginUri.UserInfo -or $OriginUri.Query -or $OriginUri.Fragment) { throw 'CLI 仅允许连接本机 127.0.0.1 HTTP 服务' }
    if ([string]::IsNullOrWhiteSpace($Endpoint.token)) { throw 'CLI 连接令牌缺失 请重新启动网页剧情服务' }
    $Route = switch ($Command) {
        'read' { '/api/story?id=' + [uri]::EscapeDataString($Id) }
        'validate' { '/api/validate' }
        'import' { '/api/import' }
        'delete' { '/api/delete' }
        'audit' { '/api/cli/audit?id=' + [uri]::EscapeDataString($Id) }
        default { '/api/cli/' + $Command }
    }
    $Request = @{
        Uri = $Endpoint.origin.TrimEnd('/') + $Route
        Method = 'GET'
        Headers = @{ Authorization = 'Bearer ' + $Endpoint.token }
        TimeoutSec = $TimeoutSeconds
        MaximumRedirection = 0
        ErrorAction = 'Stop'
    }
    if ($Command -in @('validate', 'import')) {
        $DocumentText = Get-Content -LiteralPath $File -Raw -Encoding UTF8
        $Document = ConvertFrom-Json -InputObject $DocumentText
        $Request.Method = 'POST'
        $Body = if ($Command -eq 'import') { ConvertTo-Json -InputObject @{ document = $Document; revision = $Revision } -Depth 100 -Compress } else { $DocumentText }
        $Request.Body = [Text.Encoding]::UTF8.GetBytes($Body)
        $Request.ContentType = 'application/json; charset=utf-8'
    }
    if ($Command -eq 'delete') {
        $Request.Method = 'POST'
        $Request.Body = [Text.Encoding]::UTF8.GetBytes((ConvertTo-Json -InputObject @{ id = $Id; revision = $Revision } -Compress))
        $Request.ContentType = 'application/json; charset=utf-8'
    }
    $Result = Invoke-RestMethod @Request
    if ($Command -eq 'list') {
        $StoryList = if ($null -eq $Result) { @() } else { @($Result) }
        Write-JsonResult -Value @($StoryList)
    }
    else { Write-JsonResult $Result }
    if ($Command -eq 'audit' -and $Result.valid -eq $false) { exit 1 }
    exit 0
}
catch {
    $FailureMessage = if ($_.ErrorDetails.Message) { $_.ErrorDetails.Message } else { $_.Exception.Message }
    [Console]::Error.WriteLine((ConvertTo-Json -InputObject @{ error = $FailureMessage; command = $Command } -Compress))
    exit 1
}
