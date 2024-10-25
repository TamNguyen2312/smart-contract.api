namespace FS.Commons.Models;

public class Dropdown
{
    public string Text { get; set; }
    public string Value { get; set; }
    public bool IsDisabled { get; set; }
    public string Label => Text;
    public int Level { get; set; }

    public Dropdown()
    {
    }

    public Dropdown(string value, string text)
    {
        Value = value;
        Text = text;
    }

    public Dropdown(int value, string text)
    {
        Value = value.ToString();
        Text = text;
    }

    public Dropdown(long value, string text)
    {
        Value = value.ToString();
        Text = text;
    }
}