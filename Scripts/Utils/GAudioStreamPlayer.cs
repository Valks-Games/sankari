namespace Sankari;

public class GAudioStreamPlayer 
{
    private AudioStreamPlayer _audioStreamPlayer;

    /// <summary>
    /// The volume ranging from a value of 0 to 100
    /// </summary>
    public float Volume 
    {
        get { return _audioStreamPlayer.VolumeDb.Remap(-40, 0, 0, 100); }
        set 
        {
            var v = value.Remap(0, 100, -40, 0);

            if (value == 0)
                v = 0;

            _audioStreamPlayer.VolumeDb = v; 
        }
    }

    /// <summary>
    /// The pitch value ranging from 0.01 to 4.00
    /// </summary>
    public float Pitch 
    {
        get { return _audioStreamPlayer.PitchScale; }
        set { _audioStreamPlayer.PitchScale = value; }
    }

    public AudioStream Stream 
    {
        get { return _audioStreamPlayer.Stream; }
        set { _audioStreamPlayer.Stream = value; }
    }

    public GAudioStreamPlayer(Node target) 
    {
        _audioStreamPlayer = new();
        target.AddChild(_audioStreamPlayer);
    }

    public void Play() => _audioStreamPlayer.Play();
    public void Stop() => _audioStreamPlayer.Stop();
}