namespace MarioLikeGame;

public class Audio 
{
    private Dictionary<string, AudioStream> _sfx = new();

    private AudioStreamPlayer _audioPlayer;
    private float _lastPitch;

    public Audio(AudioStreamPlayer audioPlayer) 
    {
        _audioPlayer = audioPlayer;

        Load("player_jump", "SubspaceAudio/sfx_movement_jump1.wav");
        Load("coin_pickup", "SubspaceAudio/sfx_coin_single1.wav");
    }

    public void Play(string sound)
    {
        _audioPlayer.Stream = _sfx[sound];

        var rng = new RandomNumberGenerator();
        rng.Randomize();
        var pitchScale = rng.RandfRange(0.8f, 1.2f);

        while (Mathf.Abs(pitchScale - _lastPitch) < 0.1f) 
        {
            rng.Randomize();
            pitchScale = rng.RandfRange(0.8f, 1.2f);
        }
        
        _lastPitch = pitchScale;

        _audioPlayer.PitchScale = pitchScale;
        _audioPlayer.Play();
    }

    private void Load(string name, string path) => _sfx[name] = ResourceLoader.Load<AudioStream>($"res://Audio/SFX/{path}");
}