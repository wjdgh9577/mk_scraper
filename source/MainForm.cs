using System.Drawing;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MkScraper.WinForms;

public partial class MainForm : Form
{
    private const string BaseUrl = "https://digital.mk.co.kr";
    private const string ArticleListPath = "/new/ajax/getPaperArticleInfo.php";
    private const string ArticleImagePath = "/new/loadArticle.php";
    private const string InvalidTitleChars = "\\/:*?\"<>|'";

    private static readonly Regex WhitespaceRegex = new("\\s+", RegexOptions.Compiled);
    private readonly HttpClient _httpClient;

    public MainForm()
    {
        InitializeComponent();

        _httpClient = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.All
        });
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

        datePicker.Value = DateTime.Today;
        comboSection.SelectedIndex = 0;
        txtOutputRoot.Text = LoadSavedOutputRoot() ?? GetDefaultOutputRoot();
    }

    private void btnDownload_Click(object sender, EventArgs e)
    {
        _ = RunScrapeAsync();
    }

    private void btnBrowse_Click(object sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Select the download location (an articles folder will be created inside).",
            UseDescriptionForTitle = true,
            SelectedPath = Directory.Exists(txtOutputRoot.Text)
                ? txtOutputRoot.Text
                : GetDefaultOutputRoot()
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            txtOutputRoot.Text = dialog.SelectedPath;
            SaveOutputRoot(dialog.SelectedPath);
        }
    }

    private async Task RunScrapeAsync()
    {
        btnDownload.Enabled = false;

        try
        {
            var targetDate = datePicker.Value.Date;
            var section = (comboSection.SelectedItem as string ?? string.Empty).Trim().ToUpperInvariant();
            var userRoot = txtOutputRoot.Text.Trim();

            if (string.IsNullOrWhiteSpace(section))
            {
                Log("[WARN] Section is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(userRoot))
            {
                Log("[WARN] Please select a download location.");
                return;
            }

            SaveOutputRoot(userRoot);

            string baseDir;
            try
            {
                baseDir = EnsureArticlesRoot(userRoot);
            }
            catch (Exception ex)
            {
                Log($"[ERROR] Failed to prepare download folder: {ex.Message}");
                return;
            }

            var dateStr = targetDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            var folderName = $"{dateStr}_{section}";
            var destinationDir = Path.Combine(baseDir, folderName);

            if (Directory.Exists(destinationDir))
            {
                Log($"[INFO] Target folder already exists: {destinationDir}");
                return;
            }

            Directory.CreateDirectory(destinationDir);
            Log($"[INFO] Date: {dateStr}, Section: {section}");
            Log($"[INFO] Save path: {destinationDir}");

            var downloadedFiles = 0;
            try
            {
                List<ArticleInfo> articles;
                try
                {
                    articles = await FetchArticleListAsync(targetDate, section);
                }
                catch (Exception ex)
                {
                    Log($"[ERROR] Failed to fetch article list: {ex.Message}");
                    return;
                }

                Log($"[INFO] Number of articles: {articles.Count}");

                var index = 1;
                foreach (var article in articles)
                {
                    if (string.IsNullOrWhiteSpace(article.Nc) || string.IsNullOrWhiteSpace(article.Ec))
                    {
                        Log($"[SKIP] Missing NC/EC codes → {article.Title}");
                        continue;
                    }

                    try
                    {
                        var safeTitle = SanitizeTitle(article.Title);
                        await DownloadArticleImageAsync(targetDate, section, article.Nc, article.Ec, index, safeTitle, destinationDir);
                        Log($"[OK] Download succeeded: {safeTitle}");
                        downloadedFiles++;
                        index++;
                    }
                    catch (Exception ex)
                    {
                        Log($"[ERROR] NC={article.Nc}, EC={article.Ec}, TITLE={article.Title}, {ex.Message}");
                    }
                }

                if (downloadedFiles > 0)
                {
                    Log($"[INFO] Completed download. Files saved: {downloadedFiles}");
                }
            }
            finally
            {
                if (downloadedFiles == 0 && Directory.Exists(destinationDir))
                {
                    try
                    {
                        Directory.Delete(destinationDir, true);
                        Log($"[INFO] No files downloaded. Removed folder: {destinationDir}");
                    }
                    catch (Exception ex)
                    {
                        Log($"[WARN] Failed to remove empty folder: {ex.Message}");
                    }
                }
            }
        }
        finally
        {
            btnDownload.Enabled = true;
        }
    }

    private async Task<List<ArticleInfo>> FetchArticleListAsync(DateTime targetDate, string section)
    {
        var parameters = new Dictionary<string, string>
        {
            ["service_type"] = "M0",
            ["type"] = "text",
            ["year"] = targetDate.Year.ToString(CultureInfo.InvariantCulture),
            ["month"] = targetDate.Month.ToString("D2", CultureInfo.InvariantCulture),
            ["day"] = targetDate.Day.ToString("D2", CultureInfo.InvariantCulture),
            ["section"] = section,
            ["TM"] = "D1"
        };

        var requestUri = $"{BaseUrl}{ArticleListPath}?{BuildQueryString(parameters)}";
        using var response = await _httpClient.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);

        var root = doc.RootElement;
        var articles = new List<ArticleInfo>();

        foreach (var element in EnumerateArticleNodes(root))
        {
            var nc = ReadPropertyValue(element, "no", "NC");
            var ec = ReadPropertyValue(element, "ec", "EC");
            var title = ReadPropertyValue(element, "title", "subject", "TITLE") ?? string.Empty;

            if (string.IsNullOrWhiteSpace(nc) || string.IsNullOrWhiteSpace(ec))
            {
                continue;
            }

            articles.Add(new ArticleInfo(nc, ec, title));
        }

        return articles;
    }

    private async Task DownloadArticleImageAsync(
        DateTime targetDate,
        string section,
        string nc,
        string ec,
        int index,
        string title,
        string destinationDir)
    {
        var parameters = new Dictionary<string, string>
        {
            ["MKC"] = "M0",
            ["SC"] = section,
            ["YC"] = targetDate.Year.ToString(CultureInfo.InvariantCulture),
            ["MC"] = targetDate.Month.ToString("D2", CultureInfo.InvariantCulture),
            ["DC"] = targetDate.Day.ToString("D2", CultureInfo.InvariantCulture),
            ["NC"] = nc,
            ["EC"] = ec,
            ["OC"] = "2",
            ["PV"] = "N"
        };

        var requestUri = $"{BaseUrl}{ArticleImagePath}?{BuildQueryString(parameters)}";
        using var response = await _httpClient.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();

        var fileName = $"{index}_{title}.jpg";
        var fullPath = Path.Combine(destinationDir, fileName);

        await using var fileStream = File.Create(fullPath);
        await response.Content.CopyToAsync(fileStream);
    }

    private static IEnumerable<JsonElement> EnumerateArticleNodes(JsonElement root)
    {
        if (root.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in root.EnumerateArray())
            {
                yield return item;
            }
            yield break;
        }

        if (root.ValueKind == JsonValueKind.Object)
        {
            foreach (var key in new[] { "list", "articles", "data" })
            {
                if (root.TryGetProperty(key, out var child) && child.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in child.EnumerateArray())
                    {
                        yield return item;
                    }
                    yield break;
                }
            }
        }

        throw new InvalidOperationException($"Unexpected JSON structure: {root.ValueKind}");
    }

    private static string? ReadPropertyValue(JsonElement element, params string[] propertyNames)
    {
        foreach (var property in element.EnumerateObject())
        {
            foreach (var candidate in propertyNames)
            {
                if (string.Equals(property.Name, candidate, StringComparison.OrdinalIgnoreCase))
                {
                    return property.Value.ValueKind switch
                    {
                        JsonValueKind.String => property.Value.GetString(),
                        JsonValueKind.Number => property.Value.GetRawText(),
                        _ => null
                    };
                }
            }
        }

        return null;
    }

    private static string SanitizeTitle(string? title)
    {
        var decoded = WebUtility.HtmlDecode(title ?? string.Empty).Trim();
        decoded = WhitespaceRegex.Replace(decoded, " ");

        foreach (var ch in InvalidTitleChars)
        {
            decoded = decoded.Replace(ch.ToString(), string.Empty);
        }

        if (decoded.Length > 80)
        {
            decoded = decoded[..80].TrimEnd();
        }

        decoded = decoded.Trim().TrimEnd('.');
        return string.IsNullOrWhiteSpace(decoded) ? "untitled" : decoded;
    }

    private static string BuildQueryString(Dictionary<string, string> parameters)
    {
        return string.Join("&", parameters.Select(kvp =>
            $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
    }

    private static string EnsureArticlesRoot(string userRoot)
    {
        var articlesRoot = Path.Combine(userRoot, "articles");
        Directory.CreateDirectory(articlesRoot);
        return articlesRoot;
    }

    private static string GetDefaultOutputRoot()
    {
        var executableDir = AppContext.BaseDirectory;
        var parent = Directory.GetParent(executableDir)?.FullName;
        return string.IsNullOrWhiteSpace(parent) ? executableDir : parent;
    }

    private static string? LoadSavedOutputRoot()
    {
        try
        {
            var filePath = GetSettingsFilePath();
            if (!File.Exists(filePath))
            {
                return null;
            }

            using var doc = JsonDocument.Parse(File.ReadAllText(filePath));
            if (doc.RootElement.TryGetProperty("outputRoot", out var rootProp))
            {
                var value = rootProp.GetString();
                return string.IsNullOrWhiteSpace(value) ? null : value;
            }
        }
        catch
        {
            // Ignore corrupt settings files
        }

        return null;
    }

    private static void SaveOutputRoot(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        try
        {
            var filePath = GetSettingsFilePath();
            var directory = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var payload = JsonSerializer.Serialize(new { outputRoot = path });
            File.WriteAllText(filePath, payload);
        }
        catch
        {
            // Ignore persistence errors
        }
    }

    private static string GetSettingsFilePath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var baseDir = string.IsNullOrWhiteSpace(appData)
            ? AppContext.BaseDirectory
            : Path.Combine(appData, "MkScraper");

        return Path.Combine(baseDir, "settings.json");
    }

    private void Log(string message)
    {
        if (txtLog.InvokeRequired)
        {
            txtLog.Invoke(new Action(() => Log(message)));
            return;
        }

        var line = $"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}";
        txtLog.SelectionStart = txtLog.TextLength;
        txtLog.SelectionLength = 0;
        txtLog.SelectionColor = GetLogColor(message);
        txtLog.AppendText(line);
        txtLog.SelectionColor = txtLog.ForeColor;
        txtLog.ScrollToCaret();
    }

    private static Color GetLogColor(string message)
    {
        if (message.Contains("[ERROR]", StringComparison.OrdinalIgnoreCase))
        {
            return Color.Red;
        }

        if (message.Contains("[WARN]", StringComparison.OrdinalIgnoreCase))
        {
            return Color.Yellow;
        }

        if (message.Contains("[OK]", StringComparison.OrdinalIgnoreCase))
        {
            return Color.Green;
        }

        return Color.Black;
    }

    private sealed record ArticleInfo(string Nc, string Ec, string Title);
}
