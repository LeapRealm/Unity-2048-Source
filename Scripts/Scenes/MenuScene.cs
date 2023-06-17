public class MenuScene : BaseScene
{
	protected override void OnStart()
	{
		base.OnStart();

		Managers.UI.ShowSceneUI<UI_Menu>();
	}

	public override void Restart() { }
	public override void Clear() { }
}