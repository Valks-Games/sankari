namespace Sankari;

public static class ExtensionsInput
{
    private static Dictionary<ulong, string> PrevTexts { get; } = new();
    private static Dictionary<ulong, int> PrevNums { get; } = new();

    public static string Filter(this LineEdit lineEdit, Func<string, bool> filter)
    {
        var text = lineEdit.Text;
        var id = lineEdit.GetInstanceId();

        if (string.IsNullOrWhiteSpace(text))
            return PrevTexts.ContainsKey(id) ? PrevTexts[id] : null;

        if (!filter(text))
        {
            if (!PrevTexts.ContainsKey(id))
            {
                lineEdit.ChangeLineEditText("");
                return null;
            }

            lineEdit.ChangeLineEditText(PrevTexts[id]);
            return PrevTexts[id];
        }

        PrevTexts[id] = text;
        return text;
    }

    public static string Filter(this TextEdit textEdit, Func<string, bool> filter)
    {
        var text = textEdit.Text;
        var id = textEdit.GetInstanceId();

        if (string.IsNullOrWhiteSpace(text))
            return PrevTexts.ContainsKey(id) ? PrevTexts[id] : null;

        if (!filter(text))
        {
            if (!PrevTexts.ContainsKey(id))
            {
                textEdit.ChangeTextEditText("");
                return null;
            }

            textEdit.ChangeTextEditText(PrevTexts[id]);
            return PrevTexts[id];
        }

        PrevTexts[id] = text;
        return text;
    }

    public static int FilterRange(this LineEdit lineEdit, int maxRange, int minRange = 0)
    {
        var text = lineEdit.Text;
        var id = lineEdit.GetInstanceId();

        if (string.IsNullOrWhiteSpace(text))
            return minRange - 1;

        if (text == "-")
            return minRange - 1;

        if (!int.TryParse(text.Trim(), out int num))
        {
            if (!PrevNums.ContainsKey(id))
            {
                lineEdit.ChangeLineEditText("");
                return minRange - 1;
            }

            lineEdit.ChangeLineEditText($"{PrevNums[id]}");
            return PrevNums[id];
        }

        if (text.Length > maxRange.ToString().Length && num <= maxRange)
        {
            var spliced = text.Remove(text.Length - 1);
            PrevNums[id] = int.Parse(spliced);

            lineEdit.Text = spliced;
            lineEdit.CaretColumn = spliced.Length;
            return PrevNums[id];
        }

        if (num > maxRange)
        {
            num = maxRange;
            lineEdit.ChangeLineEditText($"{maxRange}");
        }

        if (num < minRange)
        {
            num = minRange;
            lineEdit.ChangeLineEditText($"{minRange}");
        }

        PrevNums[id] = num;
        return num;
    }

    private static void ChangeLineEditText(this LineEdit lineEdit, string text)
    {
        lineEdit.Text = text;
        lineEdit.CaretColumn = text.Length;
    }

    private static void ChangeTextEditText(this TextEdit textEdit, string text)
    {
        textEdit.Text = text;
        //textEdit.CaretColumn = text.Length;
    }
}
