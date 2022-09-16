namespace Sankari;

public static class ExtensionsInput
{
    private static readonly Dictionary<ulong, string> _prevTexts = new();
    private static readonly Dictionary<ulong, int> _prevNums = new();

    public static string Filter(this LineEdit lineEdit, Func<string, bool> filter)
    {
        var text = lineEdit.Text;
        var id = lineEdit.GetInstanceId();

        if (string.IsNullOrWhiteSpace(text))
            return _prevTexts.ContainsKey(id) ? _prevTexts[id] : null;

        if (!filter(text))
        {
            if (!_prevTexts.ContainsKey(id))
            {
                lineEdit.ChangeLineEditText("");
                return null;
            }

            lineEdit.ChangeLineEditText(_prevTexts[id]);
            return _prevTexts[id];
        }

        _prevTexts[id] = text;
        return text;
    }

    public static string Filter(this TextEdit textEdit, Func<string, bool> filter)
    {
        var text = textEdit.Text;
        var id = textEdit.GetInstanceId();

        if (string.IsNullOrWhiteSpace(text))
            return _prevTexts.ContainsKey(id) ? _prevTexts[id] : null;

        if (!filter(text))
        {
            if (!_prevTexts.ContainsKey(id))
            {
                textEdit.ChangeTextEditText("");
                return null;
            }

            textEdit.ChangeTextEditText(_prevTexts[id]);
            return _prevTexts[id];
        }

        _prevTexts[id] = text;
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

        if (!int.TryParse(text.Trim(), out int numAttempts))
        {
            if (!_prevNums.ContainsKey(id))
            {
                lineEdit.ChangeLineEditText("");
                return minRange - 1;
            }

            lineEdit.ChangeLineEditText($"{_prevNums[id]}");
            return _prevNums[id];
        }

        if (text.Length > maxRange.ToString().Length && numAttempts <= maxRange)
        {
            var spliced = text.Remove(text.Length - 1);
            _prevNums[id] = int.Parse(spliced);

            lineEdit.Text = spliced;
            lineEdit.CaretColumn = spliced.Length;
            return _prevNums[id];
        }

        if (numAttempts > maxRange)
        {
            numAttempts = maxRange;
            lineEdit.ChangeLineEditText($"{maxRange}");
        }

        if (numAttempts < minRange)
        {
            numAttempts = minRange;
            lineEdit.ChangeLineEditText($"{minRange}");
        }

        _prevNums[id] = numAttempts;
        return numAttempts;
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
