using UnityEngine;

public delegate void OnAnimComplete(GameObject gameObject);

public class AnimManager
{
	private MoveData[] moveDatas;
	private int moveDataCapacity;
	private int moveDataCnt;

	private ZoomData[] zoomDatas;
	private int zoomDataCapacity;
	private int zoomDataCnt;

	private AlphaData[] alphaDatas;
	private int alphaDataCapacity;
	private int alphaDataCnt;
	
	public void Init()
	{
		moveDataCnt = 0;
		moveDataCapacity = 20;
		moveDatas = new MoveData[moveDataCapacity];
		
		zoomDataCnt = 0;
		zoomDataCapacity = 20;
		zoomDatas = new ZoomData[zoomDataCapacity];
		
		alphaDataCnt = 0;
		alphaDataCapacity = 20;
		alphaDatas = new AlphaData[alphaDataCapacity];
	}

	public void OnUpdate()
	{
		UpdateMoveData(Time.deltaTime);
		UpdateZoomData(Time.deltaTime);
		UpdateAlphaData(Time.deltaTime);
	}
	
	public void AddMoveData(ref MoveData moveData)
	{
		if (moveDataCnt == moveDataCapacity)
		{
			MoveData[] prev = moveDatas;
			moveDatas = new MoveData[moveDataCapacity * 2];
			for (int i = 0; i < moveDataCapacity; i++)
				moveDatas[i] = prev[i];
			moveDataCapacity *= 2;
		}
		
		moveDatas[moveDataCnt++] = moveData;
	}

	public void AddZoomData(ref ZoomData zoomData)
	{
		if (zoomDataCnt == zoomDataCapacity)
		{
			ZoomData[] prev = zoomDatas;
			zoomDatas = new ZoomData[zoomDataCapacity * 2];
			for (int i = 0; i < zoomDataCapacity; i++)
				zoomDatas[i] = prev[i];
			zoomDataCapacity *= 2;
		}

		zoomDatas[zoomDataCnt++] = zoomData;
	}
	
	public void AddAlphaData(ref AlphaData alphaData)
	{
		if (alphaDataCnt == alphaDataCapacity)
		{
			AlphaData[] prev = alphaDatas;
			alphaDatas = new AlphaData[alphaDataCapacity * 2];
			for (int i = 0; i < alphaDataCapacity; i++)
				alphaDatas[i] = prev[i];
			alphaDataCapacity *= 2;
		}

		alphaDatas[alphaDataCnt++] = alphaData;
	}

	public void UpdateMoveData(float dt)
	{
		for (int i = moveDataCnt - 1; i > -1; i--)
		{
			ref MoveData md = ref moveDatas[i];
			md.currDuration += dt;
			if (md.currDuration >= md.duration)
			{
				md.transform.localPosition = md.startPos + md.distance;
				if (md.onAnimComplete != null)
				{
					md.onAnimComplete(md.transform.gameObject);
					if (moveDataCnt == 0)
						return;
				}
				
				moveDatas[i] = moveDatas[moveDataCnt - 1];
				moveDataCnt--;
				continue;
			}
			
			Vector3 pos = Interpolate.Ease(md.easeFunc, md.startPos, md.distance, 
				md.currDuration, md.duration);
			md.transform.localPosition = pos;
		}
	}
	
	public void UpdateZoomData(float dt)
	{
		for (int i = zoomDataCnt - 1; i > -1; i--)
		{
			ref ZoomData zd = ref zoomDatas[i];
			zd.currDuration += dt;
			if (zd.currDuration >= zd.duration)
			{
				zd.transform.localScale = zd.startScale + zd.amount;
				if (zd.onAnimComplete != null)
				{
					zd.onAnimComplete(zd.transform.gameObject);
					if (zoomDataCnt == 0)
						return;
				}

				zoomDatas[i] = zoomDatas[zoomDataCnt - 1];
				zoomDataCnt--;
				continue;
			}

			Vector3 scale = Interpolate.Ease(zd.easeFunc, zd.startScale, zd.amount,
				zd.currDuration, zd.duration);
			zd.transform.localScale = scale;
		}
	}
	
	public void UpdateAlphaData(float dt)
	{
		for (int i = alphaDataCnt - 1; i > -1; i--)
		{
			ref AlphaData ad = ref alphaDatas[i];
			ad.currDuration += dt;
			if (ad.currDuration >= ad.duration)
			{
				ad.group.alpha = ad.startAlpha + ad.amount;
				if (ad.onAnimComplete != null)
				{
					ad.onAnimComplete(ad.group.gameObject);
					if (alphaDataCnt == 0)
						return;
				}

				alphaDatas[i] = alphaDatas[alphaDataCnt - 1];
				alphaDataCnt--;
				continue;
			}

			float alpha = Interpolate.Ease(ad.easeFunc, ad.startAlpha, ad.amount,
				ad.currDuration, ad.duration);
			ad.group.alpha = alpha;
		}
	}

	public void Clear()
	{
		moveDataCnt = 0;
		zoomDataCnt = 0;
		alphaDataCnt = 0;
	}
}

public struct MoveData
{
	public MoveData(Transform transform, EaseFunc easeFunc, Vector3 startPos, 
		Vector3 distance, float duration, OnAnimComplete onAnimComplete)
	{
		this.transform = transform;
		this.easeFunc = easeFunc;
		this.startPos = startPos;
		this.distance = distance;
		this.duration = duration;
		this.onAnimComplete = onAnimComplete;
		currDuration = 0;
	}
		
	public Transform transform;
	public EaseFunc easeFunc;
	public Vector3 startPos, distance;
	public float currDuration, duration;
	public OnAnimComplete onAnimComplete;
}

public struct ZoomData
{
	public ZoomData(Transform transform, EaseFunc easeFunc, Vector3 startScale, 
		Vector3 amount, float duration, OnAnimComplete onAnimComplete)
	{
		this.transform = transform;
		this.easeFunc = easeFunc;
		this.startScale = startScale;
		this.amount = amount;
		this.duration = duration;
		this.onAnimComplete = onAnimComplete;
		currDuration = 0;
	}

	public Transform transform;
	public EaseFunc easeFunc;
	public Vector3 startScale, amount;
	public float currDuration, duration;
	public OnAnimComplete onAnimComplete;
}

public struct AlphaData
{
	public AlphaData(CanvasGroup group, EaseFunc easeFunc, float startAlpha,
		float amount, float duration, OnAnimComplete onAnimComplete)
	{
		this.group = group;
		this.easeFunc = easeFunc;
		this.startAlpha = startAlpha;
		this.amount = amount;
		this.duration = duration;
		this.onAnimComplete = onAnimComplete;
		currDuration = 0;
	}
	
	public CanvasGroup group;
	public EaseFunc easeFunc;
	public float startAlpha, amount;
	public float currDuration, duration;
	public OnAnimComplete onAnimComplete;
}