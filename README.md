# MK Digital News Article Image Scraper

이 프로젝트는 **매일 발행되는 매일경제 디지털신문의 각 기사 이미지를 자동으로 다운로드**하는 스크립트입니다.  
기사 목록(JSON API)을 조회하고, 개별 기사 이미지를 JPEG 형식으로 저장합니다.

---

## ✨ 기능 구성

| 기능 | 설명 |
|------|------|
| Python 환경 및 pip 검사 | 실행 환경 자동 점검 |
| requests 라이브러리 자동 설치 | 없는 경우 자동 설치 진행 |
| configuration.json 자동 생성 | 날짜/섹션 기본 템플릿 생성 |
| 기사 목록 JSON 조회 | `/new/ajax/getPaperArticleInfo.php` 호출 |
| 기사 이미지 다운로드 | `/new/loadArticle.php` 호출 |
| 출력 디렉토리 자동 생성 | `./articles/YYYYMMDD_Section` |

---

## 📁 실행 구조

```
📦 프로젝트 루트
├─ 📄 run.bat
├─ 📄 configuration.json        ← 최초 실행 시 자동 생성
├─ 📄 README.md
│
├─ 📂 source
│  └─ 📄 scrape_images.py
│
└─ 📂 articles                  ← 자동 생성
   └─ 📂 YYYYMMDD_SECTION       (예: 20251205_A)
      ├─ 📄 1_기사제목1.jpg
      ├─ 📄 2_기사제목2.jpg
      └─ ...
```

---

## ⚙️ 실행 방법

### 1. 파이썬 설치 필수
Python 3.10 이상 권장  
PATH 등록 필요 (`Add Python to PATH` 체크)

### 2. 실행

run.bat

### 3. 첫 실행 시
`configuration.json` 이 없으면 아래 예시로 자동 생성됩니다:

```json
{
    "date": {
        "year": (YEAR),
        "month": (MONTH),
        "day": (DAY)
    },
    "section": "A"
}
```

## 🔌 다운로드 API 구조

### 기사 목록(JSON)

```
https://digital.mk.co.kr/new/ajax/getPaperArticleInfo.php
?service_type=M0&type=text&year={YYYY}&month={MM}&day={DD}&section={S}&TM=D1
```

### 기사 이미지

```
https://digital.mk.co.kr/new/loadArticle.php
?MKC=M0&SC=S&YC=YYYY&MC=MM&DC=DD&NC={no}&EC={ec}&OC=2&PV=N
```

no, ec 값은 JSON 목록에서 추출합니다.

## 📌 참고 사항

- 기사별 이미지는 디지털 신문 PDF의 기사 발췌 이미지 형태입니다.

- 저장 파일명은 번호_기사제목.jpg입니다.

- 파일명에 사용할 수 없는 문자는 자동 제거됩니다.

- 이미 동일 날짜/섹션 폴더가 존재하면 프로그램은 종료합니다.