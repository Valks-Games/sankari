namespace Sankari;

public class GodotFileManager
{
    public string GetProjectPath() => OS.HasFeature("standalone")
        ? System.IO.Directory.GetParent(OS.GetExecutablePath())!.FullName
        : ProjectSettings.GlobalizePath("res://");

    public string ReadFile(string path)
    {
        var file = new File();
        var error = file.Open($"res://{path}", File.ModeFlags.Read);
        if (error != Godot.Error.Ok)
        {
            Logger.LogWarning(error);
            return "";
        }
        var content = file.GetAsText();
        file.Close();
        return content;
    }

    public bool LoadDir(string path, Action<Directory, string> action)
    {
        var dir = new Directory();

        path = path.Replace('\\', '/');

        var error = dir.Open($"res://{path}");

        if (error != Error.Ok)
        {
            Logger.LogWarning($"Failed to open res://{path}, Error: '{error}'");
            return false;
        }

        dir.ListDirBegin(true);
        var fileName = dir.GetNext();

        while (fileName != "")
        {
            action(dir, fileName);
            fileName = dir.GetNext();
        }

        dir.ListDirEnd();
        return true;
    }
}