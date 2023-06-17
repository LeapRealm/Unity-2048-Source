public abstract class UI_Scene : UI_Base
{
	public override void OnStart()
	{
		base.OnStart();
		
		RefreshUI();
	}

	public override void RefreshUI() { }
}