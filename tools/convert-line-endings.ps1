param(
    [Parameter(Mandatory=$true)]
    [string]$FilePath
)

try {
    # Read file content
    $content = [System.IO.File]::ReadAllText($FilePath)
    
    # Replace CRLF with LF
    $content = $content.Replace("`r`n", "`n")
    
    # Write back using UTF8 encoding without BOM
    $utf8NoBom = New-Object System.Text.UTF8Encoding $false
    [System.IO.File]::WriteAllText($FilePath, $content, $utf8NoBom)
    
    Write-Host "Successfully converted line endings for: $FilePath"
}
catch {
    Write-Error "Error processing file: $_"
    exit 1
}