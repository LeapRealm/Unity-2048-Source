using UnityEngine;

public class Managers : MonoBehaviour
{
	private static Managers instance;
	private static Managers Instance { get { Init(); return instance; } }

	private AnimManager anim = new AnimManager();
	private DataManager data = new DataManager();
	private ResourceManager resource = new ResourceManager();
	private SceneManagerEx scene = new SceneManagerEx();
	private SoundManager sound = new SoundManager();
	private PoolManager pool = new PoolManager();
	private UIManager ui = new UIManager();
	private AdsManager ads = new AdsManager();
	private GSManager gs = new GSManager();

	public static AnimManager Anim => Instance.anim;
	public static DataManager Data => Instance.data;
	public static ResourceManager Resource => Instance.resource;
	public static SceneManagerEx Scene => Instance.scene;
	public static SoundManager Sound => Instance.sound;
	public static PoolManager Pool => Instance.pool;
	public static UIManager UI => Instance.ui;
	public static AdsManager Ads => Instance.ads;
	public static GSManager GS => Instance.gs;

	private void Update()
	{
		anim.OnUpdate();
	}

	private static bool init = false;
	
	private static void Init()
	{
		if (init) return;
		init = true;
		
		GameObject go = GameObject.Find("@Managers");
		if (go == null)
			go = new GameObject { name = "@Managers" };

		Managers m = go.GetComponent<Managers>();
		if (m == null)
			m = go.AddComponent<Managers>();
		instance = m;

		DontDestroyOnLoad(go);

		instance.anim.Init();
		instance.data.Init();
		instance.resource.Init();
		instance.scene.Init();
		instance.sound.Init();
		instance.pool.Init();
		instance.ui.Init();
		instance.ads.Init();
		instance.gs.Init();
	}

	public static void Clear()
	{
		Anim.Clear();
		// Data.Clear();
		// Resource.Clear();
		Scene.Clear();
		// Sound.Clear();
		Pool.Clear();
		UI.Clear();
		Ads.Clear();
		GS.Clear();
	}
}