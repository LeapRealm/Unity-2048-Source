public abstract class UI_Popup : UI_Base
{
	public override void OnAwake()
	{
		base.OnAwake();
		
		RefreshUI();
	}

	public override void RefreshUI() { }

	public virtual void ClosePopupUI()
	{
		Managers.UI.ClosePopupUI(this);
	}
}