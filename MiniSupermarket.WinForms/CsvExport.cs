using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    /// <summary>
    /// Xuất dữ liệu ra file CSV. Dùng UTF-8 có BOM để Excel hiển thị đúng tiếng Việt.
    /// </summary>
    public static class CsvExport
    {
        public static bool SaveWithDialog(IWin32Window owner, string defaultFileName, IEnumerable<IEnumerable<string>> rows)
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "CSV (*.csv)|*.csv",
                FileName = defaultFileName,
                Title = "Xuất file CSV",
                AddExtension = true
            };

            if (dialog.ShowDialog(owner) != DialogResult.OK)
                return false;

            var builder = new StringBuilder();
            foreach (var row in rows)
            {
                builder.AppendLine(string.Join(",", row.Select(Escape)));
            }

            File.WriteAllText(dialog.FileName, builder.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            return true;
        }

        private static string Escape(string? value)
        {
            string text = value ?? string.Empty;
            bool needsQuotes = text.IndexOfAny(new[] { ',', '"', '\r', '\n' }) >= 0;
            return needsQuotes ? "\"" + text.Replace("\"", "\"\"") + "\"" : text;
        }
    }
}
