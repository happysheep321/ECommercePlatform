# EF Core Migration Management Script
# Usage: .\ef-migrations.ps1 [service] [action] [parameters]

param(
    [Parameter(Mandatory=$true)]
    [string]$Service,
    
    [Parameter(Mandatory=$true)]
    [ValidateSet("add", "update", "remove", "script", "list")]
    [string]$Action,
    
    [Parameter(Mandatory=$false)]
    [string]$MigrationName,
    
    [Parameter(Mandatory=$false)]
    [string]$OutputPath
)

# Service Configuration
$Services = @{
    "identity" = @{
        Path = "services/identity/Ecommerce.Identity.API"
        StartupPath = "services/identity/Ecommerce.Identity.API"
    }
    "product" = @{
        Path = "services/product/ECommerce.Product.API"
        StartupPath = "services/product/ECommerce.Product.API"
    }
    "cart" = @{
        Path = "services/cart/ECommerce.Cart.API"
        StartupPath = "services/cart/ECommerce.Cart.API"
    }
    "order" = @{
        Path = "services/order/ECommerce.Order.API"
        StartupPath = "services/order/ECommerce.Order.API"
    }
    "payment" = @{
        Path = "services/payment/ECommerce.Payment.API"
        StartupPath = "services/payment/ECommerce.Payment.API"
    }
    "inventory" = @{
        Path = "services/inventory/ECommerce.Inventory.API"
        StartupPath = "services/inventory/ECommerce.Inventory.API"
    }
}

# Check if service exists
if (-not $Services.ContainsKey($Service.ToLower())) {
    Write-Host "Error: Unsupported service '$Service'" -ForegroundColor Red
    Write-Host "Supported services: $($Services.Keys -join ', ')" -ForegroundColor Yellow
    exit 1
}

$ServiceConfig = $Services[$Service.ToLower()]
$ProjectPath = $ServiceConfig.Path
$StartupPath = $ServiceConfig.StartupPath

# Check if project path exists
if (-not (Test-Path $ProjectPath)) {
    Write-Host "Error: Project path does not exist '$ProjectPath'" -ForegroundColor Red
    exit 1
}

Write-Host "Service: $Service" -ForegroundColor Green
Write-Host "Project Path: $ProjectPath" -ForegroundColor Green
Write-Host "Action: $Action" -ForegroundColor Green

# Build command
$Command = "dotnet ef"

switch ($Action) {
    "add" {
        if (-not $MigrationName) {
            Write-Host "Error: Migration name is required for add action" -ForegroundColor Red
            Write-Host "Usage: .\ef-migrations.ps1 $Service add -MigrationName 'MigrationName'" -ForegroundColor Yellow
            exit 1
        }
        $Command += " migrations add `"$MigrationName`" --project `"$ProjectPath`" --startup-project `"$StartupPath`""
    }
    "update" {
        $Command += " database update --project `"$ProjectPath`" --startup-project `"$StartupPath`""
    }
    "remove" {
        $Command += " migrations remove --project `"$ProjectPath`" --startup-project `"$StartupPath`""
    }
    "script" {
        $ScriptCommand = " migrations script --project `"$ProjectPath`" --startup-project `"$StartupPath`""
        if ($OutputPath) {
            $ScriptCommand += " --output `"$OutputPath`""
        }
        $Command += $ScriptCommand
    }
    "list" {
        $Command += " migrations list --project `"$ProjectPath`" --startup-project `"$StartupPath`""
    }
}

Write-Host "Executing command: $Command" -ForegroundColor Cyan
Write-Host ""

# Execute command
try {
    Invoke-Expression $Command
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "Operation completed successfully!" -ForegroundColor Green
    } else {
        Write-Host ""
        Write-Host "Operation failed with exit code: $LASTEXITCODE" -ForegroundColor Red
        exit $LASTEXITCODE
    }
} catch {
    Write-Host "Error occurred while executing command: $_" -ForegroundColor Red
    exit 1
}
