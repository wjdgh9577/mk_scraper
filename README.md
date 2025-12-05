# MK Digital News Article Image Scraper

This project provides a script that **downloads every article image from the Maeil Business Newspaper digital edition** for a given publication date and section. The script queries the article list JSON API and saves each article image as a JPEG file.

---

## ✨ Features

| Feature | Description |
|---------|-------------|
| Python / pip checks | Verifies the runtime environment automatically |
| Auto-install `requests` | Installs the required dependency if missing |
| Auto-create `configuration.json` | Generates defaults based on today’s date and section `A` |
| Fetch article list JSON | Calls `/new/ajax/getPaperArticleInfo.php` |
| Download article images | Calls `/new/loadArticle.php` per article |
| Auto-create output folder | Saves to `./articles/YYYYMMDD_SECTION` |

---

## 📁 Project Structure

```
📦 project root
├─ 📄 run.bat
├─ 📄 configuration.json        ← auto-created on first run (if missing)
├─ 📄 README.md
│
├─ 📂 source
│  └─ 📄 scrape_images.py
│
└─ 📂 articles                  ← auto-created
   └─ 📂 YYYYMMDD_SECTION       (example: 20251205_A)
      ├─ 📄 1_TitleOne.jpg
      ├─ 📄 2_TitleTwo.jpg
      └─ ...
```

---

## ⚙️ How to Run

1. **Install Python**  
   Python 3.10+ is recommended. Make sure it is added to your PATH (`Add Python to PATH` during installation).

2. **Start the script**  
   Run `run.bat` (double-click or run from Command Prompt).

3. **Configuration file**  
   On first launch, if `configuration.json` does not exist it will be created automatically with today’s date and section `A`. Update the file to scrape a different date or section and rerun the script.  

   ```json
   {
       "date": {
           "year": 2025,
           "month": 12,
           "day": 5
       },
       "section": "A"
   }
   ```

---

## 🔌 API Endpoints

### Article list (JSON)

```
https://digital.mk.co.kr/new/ajax/getPaperArticleInfo.php
?service_type=M0&type=text&year={YYYY}&month={MM}&day={DD}&section={S}&TM=D1
```

### Article image

```
https://digital.mk.co.kr/new/loadArticle.php
?MKC=M0&SC={S}&YC={YYYY}&MC={MM}&DC={DD}&NC={no}&EC={ec}&OC=2&PV=N
```

The `no` and `ec` values used above come from the article list JSON response.

---

## 📌 Additional Notes

- Each downloaded image is the cropped article image from the digital newspaper PDF.
- Filenames follow `index_title.jpg`; characters that are invalid on Windows are removed automatically.
- If the output folder for the date/section already exists, the script stops immediately to avoid duplicate downloads.
