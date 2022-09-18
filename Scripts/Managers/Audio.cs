namespace Sankari;

public class Audio
{
    private Dictionary<string, AudioStream> Sfx { get; } = new();
    private Dictionary<string, AudioStream> Music { get; } = new();

    private GAudioStreamPlayer SfxPlayer { get; }
    private GAudioStreamPlayer MusicPlayer { get; }
    private float LastPitch { get; set; }

    public Audio(GAudioStreamPlayer sfxPlayer, GAudioStreamPlayer musicPlayer)
    {
        SfxPlayer = sfxPlayer;
        MusicPlayer = musicPlayer;
        MusicPlayer.Volume = 100;
        SfxPlayer.Volume = 80;

        LoadSoundEffects();
        LoadSoundTracks();
    }

    private void LoadSoundEffects()
    {
        LoadSFX("player_jump", "Movement/Jump/sfx_movement_jump1.wav");
        LoadSFX("coin_pickup_1", "Environment/Coin Pickup/1/sfx_coin_single1.wav");
        LoadSFX("coin_pickup_2", "Environment/Coin Pickup/2/coin.wav");
        LoadSFX("dash", "Movement/Dash/swish-9.wav");

        LoadSFX("game_over_1", "Game Over/1/retro-game-over.wav");
        LoadSFX("game_over_2", "Game Over/2/game-over-dark-orchestra.wav");
        LoadSFX("game_over_3", "Game Over/3/musical-game-over.wav");
        LoadSFX("game_over_4", "Game Over/4/orchestra-game-over.wav");
    }

    private void LoadSoundTracks()
    {
        LoadMusic("map_grassy", "Map/8bit Bossa/8bit Bossa.mp3");
        LoadMusic("grassy_1", "Level/Grassy Peaceful/Chiptune Adventures/Juhani Junkala [Chiptune Adventures] 1. Stage 1.ogg");
        LoadMusic("grassy_2", "Level/Grassy Peaceful/Chiptune Adventures/Juhani Junkala [Chiptune Adventures] 2. Stage 2.ogg");
        LoadMusic("ice_1", "Level/Ice/Icy_Expanse.mp3");
    }

    public void PlaySFX(string name, int volume = 100)
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

    public void PlayMusic(string name, float pitch = 1)
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

    public void StopMusic() => MusicPlayer.Stop();

    /// <summary>
    /// Values range from 0 to 100
    /// </summary>
    public void SetSFXVolume(int v) => SfxPlayer.Volume = v;

    private void LoadSFX(string name, string path) => Sfx[name] = ResourceLoader.Load<AudioStream>($"res://Audio/SFX/{path}");
    private void LoadMusic(string name, string path) => Music[name] = ResourceLoader.Load<AudioStream>($"res://Audio/Music/{path}");
}
