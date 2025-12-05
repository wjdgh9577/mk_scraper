@echo off

echo *********************************
echo * Python / pip / 라이브러리 체크 *
echo *********************************
echo.

REM 1. Python 설치 확인
python --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Python 이 설치되어 있지 않거나 PATH 에 등록되어 있지 않습니다.
    echo Python 3.x 를 설치한 뒤 다시 실행해 주세요.
    echo.
    pause
    exit /b 1
)

REM 2. pip 설치 확인
pip --version >nul 2>&1
if errorlevel 1 (
    echo [WARN] pip 가 설치되어 있지 않거나 PATH 에 등록되어 있지 않습니다.
    echo [INFO] python -m ensurepip --upgrade 를 사용해 설치를 시도합니다.
    echo.

    python -m ensurepip --upgrade
    if errorlevel 1 (
        echo [ERROR] pip 설치에 실패했습니다.
        echo [ERROR] "python -m pip install --upgrade pip" 등을 사용해 수동으로 설정해 주세요.
        echo.
        pause
        exit /b 1
    )

    echo [INFO] pip 설치/업데이트 완료. 버전 확인 중...
)

pip --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] pip 가 설치되어 있지 않거나 PATH 에 등록되어 있지 않습니다.
    echo Python 설치 시 "Add Python to PATH" 옵션을 체크했는지 확인하거나 pip 를 별도로 설치해 주세요.
    echo.
    pause
    exit /b 1
)

REM 3. 필수 라이브러리(requests) 설치 확인
echo *********************************
echo * 필수 라이브러리 설치 (requests) *
echo *********************************
echo.
pip install requests

echo.
echo *********************************
echo * configuration.json 확인        *
echo *********************************
echo.

REM 4. configuration.json 존재 여부 확인
if not exist configuration.json (
    echo [INFO] configuration.json 이 없어 새로 생성합니다.

	>configuration.json echo {
	>>configuration.json echo     "date": {
	>>configuration.json echo         "year": 2025,
	>>configuration.json echo         "month": 12,
	>>configuration.json echo         "day": 3
	>>configuration.json echo     },
	>>configuration.json echo     "section": "A"
	>>configuration.json echo }
	
	echo [INFO] configuration.json 을 생성했습니다.
	echo [INFO] 설정값을 업데이트 후 다시 실행해주세요.
	echo.
	
    pause
    exit /b 1
) else (
    echo [INFO] configuration.json 이 이미 존재합니다. 그대로 사용합니다.
)

echo.
echo *********************************
echo *  Python 스크립트 실행         *
echo *********************************
echo.

REM 5. 소스코드 실행
python source/scrape_images.py

echo.
pause
