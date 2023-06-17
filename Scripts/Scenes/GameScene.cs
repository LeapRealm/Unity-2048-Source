public class GameScene : BaseScene
{
	protected override void OnStart()
	{
		base.OnStart();
		
		Managers.UI.ShowSceneUI<UI_InGame>();
	}

	
	public override void Restart()
	{
		Managers.Clear();
		Managers.UI.ShowSceneUI<UI_InGame>();
	}
	
	public override void Clear() { }
}