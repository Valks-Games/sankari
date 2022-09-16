namespace Sankari;

public partial class GAudioStreamPlayer 
{
    private readonly AudioStreamPlayer audioStreamPlayer;

    /// <summary>
    /// The volume ranging from a value of 0 to 100
    /// </summary>
    public float Volume 
    {
        get { return audioStreamPlayer.VolumeDb.Remap(-40, 0, 0, 100); }
        set 
        {
            var v = value.Remap(0, 100, -40, 0);

            if (value == 0)
                v = 0;

            audioStreamPlayer.VolumeDb = v; 
        }
    }

    /// <summary>
    /// The pitch value ranging from 0.01 to 4.00
    /// </summary>
    public float Pitch 
    {
        get { return audioStreamPlayer.PitchScale; }
        set { audioStreamPlayer.PitchScale = value; }
    }

    public AudioStream Stream 
    {
        get { return audioStreamPlayer.Stream; }
        set { audioStreamPlayer.Stream = value; }
    }

    public GAudioStreamPlayer(Node target) 
    {
        audioStreamPlayer = new();
        target.AddChild(audioStreamPlayer);
    }

    public void Play() => audioStreamPlayer.Play();
    public void Stop() => audioStreamPlayer.Stop();
}