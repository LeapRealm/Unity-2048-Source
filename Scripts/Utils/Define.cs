public static class Define
{
	public enum UIEvent
	{
		Click = 0,
		Pressed,
		PointerDown,
		PointerUp,
		Drag,
		EndDrag,
		PointerEnter,
		PointerExit,
	}
	
	public enum SceneType
	{
		None = -1,
		MenuScene,
		GameScene,
	}

	public enum SoundType
	{
		BGM = 0,
		SFX,
		Max,
	}
	
	public enum Language
	{
		English = 0,
		Korean,
		Japanese,
	}
	
	public enum TextID
	{
		Play      = 1,
		Score     = 2,
		Best      = 3,
		GameOver  = 4,
		GameClear = 5,
		Restart   = 6,
		Menu      = 7,
	}
}