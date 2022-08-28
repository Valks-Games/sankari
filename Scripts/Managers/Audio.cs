namespace Sankari;

public class Audio 
{
    private Dictionary<string, AudioStream> _sfx = new();
    private Dictionary<string, AudioStream> _music = new();

    private GAudioStreamPlayer _sfxPlayer;
    private GAudioStreamPlayer _musicPlayer;
    private float _lastPitch;

    public Audio(GAudioStreamPlayer sfxPlayer, GAudioStreamPlayer musicPlayer) 
    {
        _sfxPlayer = sfxPlayer;
        _musicPlayer = musicPlayer;
        _musicPlayer.Volume = 100;
        _sfxPlayer.Volume = 80;

        LoadSoundEffects();
        LoadSoundTracks();
    }

    private void LoadSoundEffects()
    {
        LoadSFX("player_jump", "SubspaceAudio/sfx_movement_jump1.wav");
        LoadSFX("coin_pickup", "SubspaceAudio/sfx_coin_single1.wav");
        LoadSFX("dash", "Movement/Dash/swish-9.wav");

        LoadSFX("game_over_1", "Game Over/1/retro-game-over.wav");
        LoadSFX("game_over_2", "Game Over/2/game-over-dark-orchestra.wav");
        LoadSFX("game_over_3", "Game Over/3/musical-game-over.wav");
        LoadSFX("game_over_4", "Game Over/4/orchestra-game-over.wav");
    }

    private void LoadSoundTracks()
    {
        LoadMusic("map_grassy", "Joth/bossa nova/8bit Bossa.mp3");
        LoadMusic("grassy_1", "SubspaceAudio/4 chiptunes adventure/Juhani Junkala [Chiptune Adventures] 1. Stage 1.ogg");
        LoadMusic("grassy_2", "SubspaceAudio/4 chiptunes adventure/Juhani Junkala [Chiptune Adventures] 2. Stage 2.ogg");
        LoadMusic("ice_1", "Ice/Icy_Expanse.mp3");
    }

    public void PlaySFX(string name, int volume = 100)
    {
        _sfxPlayer.Volume = volume;
        _sfxPlayer.Stream = _sfx[name];

        var rng = new RandomNumberGenerator();
        rng.Randomize();
        var pitchScale = rng.RandfRange(0.8f, 1.2f);

        while (Mathf.Abs(pitchScale - _lastPitch) < 0.1f) 
        {
            rng.Randomize();
            pitchScale = rng.RandfRange(0.8f, 1.2f);
        }
        
        _lastPitch = pitchScale;

        _sfxPlayer.Pitch = pitchScale;
        _sfxPlayer.Play();
    }

    public void PlayMusic(string name, float pitch = 1) 
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            // This level does not have any music set
            return;
        }

        if (!_music.ContainsKey(name)) 
        {
            Logger.LogWarning($"The music track for '{name}' does not exist");
        }

        _musicPlayer.Stream = _music[name];
        _musicPlayer.Pitch = pitch;
        _musicPlayer.Play();
    }

    public void StopMusic() => _musicPlayer.Stop();

    /// <summary>
    /// Values range from 0 to 100
    /// </summary>
    public void SetSFXVolume(int v) => _sfxPlayer.Volume = v;

    private void LoadSFX(string name, string path) => _sfx[name] = ResourceLoader.Load<AudioStream>($"res://Audio/SFX/{path}");
    private void LoadMusic(string name, string path) => _music[name] = ResourceLoader.Load<AudioStream>($"res://Audio/Music/{path}");
}