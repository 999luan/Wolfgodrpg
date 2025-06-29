@echo off
echo ========================================
echo    WOLFGOD RPG - DEBUG LOGS
echo ========================================
echo.
echo Escolha uma opção:
echo 1. Ver logs em tempo real
echo 2. Ver todos os logs do mod
echo 3. Ver logs de UI
echo 4. Ver logs de Player
echo 5. Ver logs de Item
echo 6. Ver logs de Debug
echo 7. Limpar logs
echo.
set /p choice="Digite sua escolha (1-7): "

if "%choice%"=="1" (
    echo Monitorando logs em tempo real... (Ctrl+C para parar)
    tail -f "C:\Users\tatal\Documents\My Games\Terraria\tModLoader\Logs\client.log" | grep "WolfGodRPG"
) else if "%choice%"=="2" (
    echo Todos os logs do mod:
    grep "WolfGodRPG" "C:\Users\tatal\Documents\My Games\Terraria\tModLoader\Logs\client.log"
) else if "%choice%"=="3" (
    echo Logs de UI:
    grep "WolfGodRPG.*UI" "C:\Users\tatal\Documents\My Games\Terraria\tModLoader\Logs\client.log"
) else if "%choice%"=="4" (
    echo Logs de Player:
    grep "WolfGodRPG.*Player" "C:\Users\tatal\Documents\My Games\Terraria\tModLoader\Logs\client.log"
) else if "%choice%"=="5" (
    echo Logs de Item:
    grep "WolfGodRPG.*Item" "C:\Users\tatal\Documents\My Games\Terraria\tModLoader\Logs\client.log"
) else if "%choice%"=="6" (
    echo Logs de Debug:
    grep "DebugCheck\|ManualTest" "C:\Users\tatal\Documents\My Games\Terraria\tModLoader\Logs\client.log"
) else if "%choice%"=="7" (
    echo Limpando logs...
    echo "" > "C:\Users\tatal\Documents\My Games\Terraria\tModLoader\Logs\client.log"
    echo Logs limpos!
) else (
    echo Opção inválida!
)

echo.
pause 