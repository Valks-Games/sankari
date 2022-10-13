namespace Sankari;

public partial class SFXSound : AudioStreamPlayer
{
	private void OnFinished() 
	{
		QueueFree();	
	}
}
