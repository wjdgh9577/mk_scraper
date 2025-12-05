import os
import sys
import re
import html
import json
import requests
from datetime import date
from urllib.parse import urljoin
from typing import List, Dict, Any

# =========================
# Load configuration.json
# =========================
def load_config():
    config_path = os.path.join(os.getcwd(), "configuration.json")
    config = None

    if not os.path.exists(config_path):
        today = date.today()
        config = {
            "date": {
                "year": today.year,
                "month": today.month,
                "day": today.day,
            },
            "section": "A",
        }
        print("[INFO] configuration.json not found. Creating file with default values.")

        try:
            with open(config_path, "w", encoding="utf-8") as f:
                json.dump(config, f, ensure_ascii=False, indent=4)
        except Exception as e:
            print(f"[WARN] Failed to create configuration.json automatically: {e}")
            print("[WARN] Continuing with in-memory defaults despite the write failure.")

    if config is None:
        try:
            with open(config_path, "r", encoding="utf-8") as f:
                config = json.load(f)
        except Exception as e:
            print(f"[ERROR] Failed to read configuration.json: {e}")
            sys.exit(1)

    try:
        year = config["date"]["year"]
        month = config["date"]["month"]
        day = config["date"]["day"]
        section = config["section"].upper()
    except Exception:
        print("[ERROR] Invalid configuration.json structure.")
        sys.exit(1)

    return year, month, day, section


# =========================
# Date / section settings
# =========================
YEAR, MONTH, DAY, SECTION = load_config()

DATE_STR: str = f"{YEAR}{MONTH:02d}{DAY:02d}"
DATE_SECTION_DIR: str = f"{DATE_STR}_{SECTION}"

BASE_DIR: str = os.getcwd()

ARTICLES_ROOT: str = os.path.join(BASE_DIR, "articles")
os.makedirs(ARTICLES_ROOT, exist_ok=True)

SAVE_PATH: str = os.path.join(ARTICLES_ROOT, DATE_SECTION_DIR)

BASE_URL = "https://digital.mk.co.kr"
ARTICLE_LIST_PATH = "/new/ajax/getPaperArticleInfo.php"
ARTICLE_IMAGE_PATH = "/new/loadArticle.php"

DEFAULT_HEADERS = {
    "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64)"
}

# =========================
# Sanitize file name (title) helper
# =========================
def sanitize_title(title: str, max_len: int = 80) -> str:
    if not title:
        return "untitled"

    title = html.unescape(title)
    title = title.strip()
    title = re.sub(r"\s+", " ", title)

    invalid_chars = r'\\/:*?"<>|\''
    for ch in invalid_chars:
        title = title.replace(ch, "")

    if len(title) > max_len:
        title = title[:max_len].rstrip()

    title = title.rstrip(".")
    return title or "untitled"


# =========================
# Fetch article metadata JSON
# =========================
def fetch_paper_article_info(
    year: int,
    month: int,
    day: int,
    section: str,
) -> List[Dict[str, Any]]:
    url = urljoin(BASE_URL, ARTICLE_LIST_PATH)

    params = {
        "service_type": "M0",
        "type": "text",
        "year": year,
        "month": month,
        "day": day,
        "section": section,
        "TM": "D1",
    }

    resp = requests.get(url, params=params, headers=DEFAULT_HEADERS, timeout=10)
    resp.raise_for_status()
    data = resp.json()

    if isinstance(data, list):
        return data
    elif isinstance(data, dict):
        for key in ("list", "articles", "data"):
            if key in data and isinstance(data[key], list):
                return data[key]
        raise ValueError(f"Unexpected JSON structure: {list(data.keys())}")
    else:
        raise ValueError(f"Unexpected JSON type: {type(data)}")


# =========================
# Download article image
# =========================
def download_article_image(
    yc: int,
    mc: int,
    dc: int,
    nc: str,
    ec: str,
    index: int,
    title: str,
    save_dir: str,
    section: str,
) -> str:
    url = urljoin(BASE_URL, ARTICLE_IMAGE_PATH)

    params = {
        "MKC": "M0",
        "SC": section,
        "YC": str(yc),
        "MC": f"{mc:02d}",
        "DC": f"{dc:02d}",
        "NC": str(nc),
        "EC": str(ec),
        "OC": "2",
        "PV": "N",
    }

    resp = requests.get(url, params=params, headers=DEFAULT_HEADERS, timeout=20)
    resp.raise_for_status()

    filename = f"{index}_{title}.jpg"
    full_path = os.path.join(save_dir, filename)

    with open(full_path, "wb") as f:
        f.write(resp.content)

    return full_path


# =========================
# Main
# =========================
def main() -> None:
    # Exit early when folder already exists
    if os.path.exists(SAVE_PATH):
        print(f"[INFO] Target date/section folder already exists: {SAVE_PATH}")
        print("[INFO] Exiting program.")
        return

    os.makedirs(SAVE_PATH, exist_ok=False)

    print(f"[INFO] Date: {DATE_STR}, Section: {SECTION}, Save path: {SAVE_PATH}")

    try:
        articles = fetch_paper_article_info(YEAR, MONTH, DAY, SECTION)
    except Exception as e:
        print(f"[ERROR] Failed to fetch article list: {e}")
        return

    print(f"[INFO] Number of articles: {len(articles)}")

    index = 1
    for article in articles:
        nc = article.get("no") or article.get("NC")
        ec = article.get("ec") or article.get("EC")
        title = article.get("title") or article.get("subject") or ""

        if not nc or not ec:
            print(f"[SKIP] Missing no/ec → {article}")
            continue

        try:
            safe_title = sanitize_title(title)
            path = download_article_image(
                yc=YEAR, mc=MONTH, dc=DAY,
                nc=str(nc), ec=str(ec),
                index=index,
                title=safe_title,
                save_dir=SAVE_PATH,
                section=SECTION,
            )
            print(f"[OK] Download succeeded: {safe_title}")
            index += 1
        except Exception as e:
            print(f"[ERROR] NC={nc}, EC={ec}, TITLE={title}, {e}")


if __name__ == "__main__":
    main()
