namespace Sankari;

public static class Audio
{
    private static Dictionary<string, AudioStream> Sfx { get; } = new();
    private static Dictionary<string, AudioStream> Music { get; } = new();

    private static GAudioStreamPlayer SfxPlayer { get; set; }
    private static GAudioStreamPlayer MusicPlayer { get; set; }
    private static float LastPitch { get; set; }

    public static void Init(GAudioStreamPlayer sfxPlayer, GAudioStreamPlayer musicPlayer)
    {
        SfxPlayer = sfxPlayer;
        MusicPlayer = musicPlayer;
        MusicPlayer.Volume = 100;
        SfxPlayer.Volume = 80;
    }

    public static void PlaySFX(string name, int volume = 100)
    {
        SfxPlayer.Volume = volume;
        SfxPlayer.Stream = Sfx[name];

        var rng = new RandomNumberGenerator();
        rng.Randomize();
        var pitchScale = rng.RandfRange(0.8f, 1.2f);

        while (Mathf.Abs(pitchScale - LastPitch) < 0.1f)
        {
            rng.Randomize();
            pitchScale = rng.RandfRange(0.8f, 1.2f);
        }

        LastPitch = pitchScale;

        SfxPlayer.Pitch = pitchScale;
        SfxPlayer.Play();
    }

    public static void PlayMusic(string name, float pitch = 1)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            // This level does not have any music set
            return;
        }

        if (!Music.ContainsKey(name))
        {
            Logger.LogWarning($"The music track for '{name}' does not exist");
        }

        MusicPlayer.Stream = Music[name];
        MusicPlayer.Pitch = pitch;
        MusicPlayer.Play();
    }

    public static void StopMusic() => MusicPlayer.Stop();

    /// <summary>
    /// Values range from 0 to 100
    /// </summary>
    public static void SetSFXVolume(int v) => SfxPlayer.Volume = v;

    public static void LoadSFX(string name, string path) => Sfx[name] = ResourceLoader.Load<AudioStream>($"res://Audio/SFX/{path}");
    public static void LoadMusic(string name, string path) => Music[name] = ResourceLoader.Load<AudioStream>($"res://Audio/Music/{path}");
}
