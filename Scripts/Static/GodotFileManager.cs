namespace Sankari;

public static class GodotFileManager
{
    public static string GetProjectPath() => OS.HasFeature("standalone")
        ? System.IO.Directory.GetParent(OS.GetExecutablePath())!.FullName
        : ProjectSettings.GlobalizePath("res://");

    public static string ReadFile(string path)
    {
        using var file = FileAccess.Open($"res://{path}", FileAccess.ModeFlags.Read);

        var err = file.GetError();
        if (err != Error.Ok)
        {
            Logger.LogWarning(err);
            return "";
        }

        return file.GetAsText();
    }

    public static bool LoadDir(string path, Action<DirAccess, string> action)
    {
        path = path.Replace('\\', '/');

        var dir = DirAccess.Open($"res://{path}");
        
        var errOpen = DirAccess.GetOpenError();

        if (errOpen != Error.Ok)
        {
            Logger.LogWarning($"Failed to open res://{path}, Error: '{errOpen}'");
            return false;
        }

        var errListDirBegin = dir.ListDirBegin();

        if (errListDirBegin != Error.Ok)
        { 
            Logger.LogWarning($"Failed to begin listing the directory, Error: '{errListDirBegin}'");
            return false;
        }

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
