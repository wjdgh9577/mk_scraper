# MK Digital News Article Image Scraper (WinForms)

WinForms desktop app that **downloads every article image for a chosen date and section** of the Maeil Business Newspaper digital edition. The app queries MK’s public JSON endpoints, then stores each JPEG inside `articles/YYYYMMDD_SECTION`.

---

## ✨ Highlights

| Feature | Details |
|---------|---------|
| Date & section picker | Calendar control plus drop-down for editions (A, B, …) |
| Custom output root | Pick any folder; the app builds `articles/<date>_<section>` inside it |
| Duplicate guard | Stops immediately if the destination folder already exists |
| Clean filenames | Strips invalid characters and caps overly long article titles |
| Color-coded log | `[OK]`, `[WARN]`, `[ERROR]` entries streamed live while scraping |
| Path persistence | Remembers the last output root in `%AppData%\MkScraper\settings.json` |

---

## 📁 Project Layout

```
mk_scraper/
├─ README.md
└─ source/
   ├─ MkScraper.WinForms.sln
   ├─ MkScraper.WinForms.csproj
   ├─ Program.cs
   ├─ MainForm.cs
   └─ MainForm.Designer.cs
```

Downloaded images go under the chosen root: `articles/YYYYMMDD_SECTION`. Example: `D:\Downloads\articles\20231205_A`.

---

## ⚙️ Requirements

- Windows 10+
- [.NET 8 SDK](https://dotnet.microsoft.com/download) (includes the runtime and CLI tooling)

---

## 🚀 Getting Started

1. Clone the repo and run from the root:

   ```bash
   dotnet run --project source/MkScraper.WinForms
   ```

   Prefer Visual Studio? Open `source/MkScraper.WinForms.sln` and press `F5`.

2. In the app:
   - Pick the publication date (defaults to today).
   - Choose a section from the drop-down.
   - Browse to a download root; the app creates `articles` inside it.
   - Hit **Download**. Images are saved as `index_title.jpg` in chronological order.

3. Use the log pane to track progress.  
   `[OK]` = success, `[WARN]` = input/validation issue, `[ERROR]` = request or file failure.

---

## 🔌 MK API Usage

- Article list: `https://digital.mk.co.kr/new/ajax/getPaperArticleInfo.php`
- Article image: `https://digital.mk.co.kr/new/loadArticle.php`

Each request passes the target date, section, and the `NC`/`EC` codes obtained from the list response.

---

## 📝 Notes

- Failed downloads do not stop the batch; only the successful files remain.
- If zero files are saved the app removes the empty folder automatically.
- Any upstream API changes at MK can break scraping—check the log output if that happens.
