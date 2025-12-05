@echo off

echo *********************************
echo * Checking Python / pip / libs  *
echo *********************************
echo.

REM 1. Verify Python installation
python --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Python is not installed or not on PATH.
    echo Please install Python 3.x and run this script again.
    echo.
    pause
    exit /b 1
)

REM 2. Verify pip installation
pip --version >nul 2>&1
if errorlevel 1 (
    echo [WARN] pip is not installed or not on PATH.
    echo [INFO] Attempting installation via "python -m ensurepip --upgrade".
    echo.

    python -m ensurepip --upgrade
    if errorlevel 1 (
        echo [ERROR] Failed to install pip automatically.
        echo [ERROR] Please run "python -m pip install --upgrade pip" manually.
        echo.
        pause
        exit /b 1
    )

    echo [INFO] pip installation/update complete. Checking version...
)

pip --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] pip is not installed or not on PATH.
    echo Please ensure "Add Python to PATH" was selected or install pip separately.
    echo.
    pause
    exit /b 1
)

REM 3. Ensure required library (requests) is installed
echo *********************************
echo * Installing required library   *
echo *********************************
echo.
pip install requests

echo.
echo *********************************
echo * Running Python script         *
echo *********************************
echo.

REM 4. Run source code
python source/scrape_images.py

echo.
pause
