using System.Text;

namespace APIDiff.Utils
{
    public static class FileComparer
    {
        public static string[] GetDifferences(
            string content1,
            string content2,
            string originalFileName,
            string updatedFileName
        )
        {
            string FileName;

            //simple name comparisson
            if (!(originalFileName == updatedFileName))
            {
                FileName = $"File name changed from {originalFileName} --> {updatedFileName}";
            }
            else
            {
                FileName = $"File name does not changed {originalFileName}";
            }

            // separate line by line
            var original = content1.Split("\n");
            var updated = content2.Split("\n");

            // max number of lines between them
            int maxLengthLines = Math.Max(original.Length, updated.Length);

            // string builder init
            StringBuilder sbLineDifferences = new StringBuilder();
            StringBuilder sbLineOriginal = new StringBuilder();

            for (int i = 0; i < maxLengthLines; i++)
            {
                // to avoid out of index error if anyone of those is out of index return No Line
                var originalLine = i < original.Length ? original[i] : "No Line";
                var updatedLine = i < updated.Length ? updated[i] : "No Line";

                if (originalLine != "No Line")
                {
                    sbLineOriginal.AppendLine($"<pre id='old'>{originalLine}</pre>");
                }

                // if updated is different than original = difference
                if (updatedLine != originalLine)
                {
                    sbLineDifferences.AppendLine($"<pre id='new'>{updatedLine}</pre>");
                }
                else
                {
                    sbLineDifferences.AppendLine($"<pre id='same'>{updatedLine}</pre>");
                }
            }

            return [sbLineDifferences.ToString(), sbLineOriginal.ToString(), FileName];
        }
    }
}
