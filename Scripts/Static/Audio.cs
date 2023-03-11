namespace Sankari;

public static class Audio
{
    private static Dictionary<string, AudioStream> Sfx { get; } = new();
    private static Dictionary<string, AudioStream> Music { get; } = new();

    private static Node SFXPlayers { get; set; }
    private static GAudioStreamPlayer MusicPlayer { get; set; }
    private static float LastPitch { get; set; }

    public static void Init(GAudioStreamPlayer musicPlayer, Node sfxPlayers)
    {
        SFXPlayers = sfxPlayers;
        MusicPlayer = musicPlayer;
        MusicPlayer.Volume = 100;

		LoadSoundEffects();
		LoadSoundTracks();

		var eventsPlayer = GameManager.EventsPlayer;

		// Player
		eventsPlayer.AddListener(EventPlayer.OnJump, (args) => 
		{
			PlaySFX("player_jump", 80);
		});

		eventsPlayer.AddListener(EventPlayer.OnDied, (args) => 
		{
			StopMusic();
			PlaySFX("game_over_1");	
		});

		eventsPlayer.AddListener(EventPlayer.OnDash, (args) => 
		{
			PlaySFX("dash");
		});

		// Game
		var events = GameManager.Events;

		events.AddListener(Event.OnCoinPickup, (args) => 
		{
			PlaySFX("coin_pickup_1", 30);	
		});

		events.AddListener(Event.OnMapLoaded, (args) => 
		{
			//PlayMusic("map_grassy");	
		});
    }

	private static void LoadSoundEffects()
    {
        LoadSFX("player_jump", "Movement/Jump/sfx_movement_jump1.wav");
        LoadSFX("coin_pickup_1", "Environment/Coin Pickup/1/sfx_coin_single1.wav");
        LoadSFX("coin_pickup_2", "Environment/Coin Pickup/2/coin.wav");
        LoadSFX("dash", "Movement/Dash/swish-9.wav");
		LoadSFX("dash_replenish", "Movement/Dash/sfx_sounds_powerup17.wav");

        LoadSFX("game_over_1", "Game Over/1/retro-game-over.wav");
        LoadSFX("game_over_2", "Game Over/2/game-over-dark-orchestra.wav");
        LoadSFX("game_over_3", "Game Over/3/musical-game-over.wav");
        LoadSFX("game_over_4", "Game Over/4/orchestra-game-over.wav");
    }

    private static void LoadSoundTracks()
    {
        LoadMusic("map_grassy", "Map/8bit Bossa/8bit Bossa.mp3");
        LoadMusic("grassy_1", "Level/Grassy Peaceful/Chiptune Adventures/Juhani Junkala [Chiptune Adventures] 1. Stage 1.ogg");
        LoadMusic("grassy_2", "Level/Grassy Peaceful/Chiptune Adventures/Juhani Junkala [Chiptune Adventures] 2. Stage 2.ogg");
        LoadMusic("ice_1", "Level/Ice/Icy_Expanse.mp3");
    }

	/// <summary>
	/// Play a sound (volume ranges from 0 to 100)
	/// </summary>
    public static void PlaySFX(string name, int volume = 100)
    {
		if (!Sfx.ContainsKey(name))
		{
			Logger.LogWarning($"SFX '{name}' was not loaded so it could not be played");
			return;
		}

		var sfxPlayer = new GAudioStreamPlayer(SFXPlayers, true);

        sfxPlayer.Volume = volume;
        sfxPlayer.Stream = Sfx[name];

        var rng = new RandomNumberGenerator();
        rng.Randomize();
        var pitchScale = rng.RandfRange(0.8f, 1.2f);

        while (Mathf.Abs(pitchScale - LastPitch) < 0.1f)
        {
            rng.Randomize();
            pitchScale = rng.RandfRange(0.8f, 1.2f);
        }

        LastPitch = pitchScale;

        sfxPlayer.Pitch = pitchScale;
        sfxPlayer.Play();
    }

	/// <summary>
	/// Play a music track (pitch values between 0.1 and 1 are slower, values higher than 1.0 are faster)
	/// </summary>
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

	/// <summary>
	/// Set the music volume. Values range from 0 to 100.
	/// </summary>
	public static void SetVolumeMusic(int v) => MusicPlayer.Volume = v;

	/// <summary>
	/// Stop the music track currently being played
	/// </summary>
    public static void StopMusic() => MusicPlayer.Stop();

	/// <summary>
	/// Load a sound effect, for e.g. LoadSFX("player_jump", "Movement/Jump/sfx_movement_jump1.wav")
	/// </summary>
    public static void LoadSFX(string name, string path) => Sfx[name] = ResourceLoader.Load<AudioStream>($"res://Audio/SFX/{path}");
    
	/// <summary>
	/// Load a music track, for e.g. LoadMusic("ice_1", "Level/Ice/Icy_Expanse.mp3")
	/// </summary>
	public static void LoadMusic(string name, string path) => Music[name] = ResourceLoader.Load<AudioStream>($"res://Audio/Music/{path}");
}
