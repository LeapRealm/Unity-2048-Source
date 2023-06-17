using UnityEngine;

public enum Direction { None = -1, Right = 0, Down, Left, Up }

public class UI_Node : UI_SubItem
{
	public UI_InGame inGameUI;
	public Vector2Int?[] neighborNodes;
	public Vector2Int point;
	public UI_Block placedBlock;
	public Vector2 localPosition;
	public bool combined;

	public void Setup(UI_InGame inGameUI, Vector2Int?[] neighborNodes, Vector2Int point)
	{
		this.inGameUI = inGameUI;
		this.neighborNodes = neighborNodes;
		this.point = point;
		this.placedBlock = null;
		this.localPosition = Vector2.zero;
		this.combined = false;
	}

	public UI_Node FindTarget(UI_Node originalNode, Direction direction, UI_Node farthestEmptyNode = null)
	{
		if (neighborNodes[(int)direction].HasValue)
		{
			Vector2Int p = neighborNodes[(int)direction].Value;
			UI_Node neighborNode = inGameUI.nodeList[p.y * inGameUI.blockCnt.x + p.x];

			if (neighborNode != null && neighborNode.combined)
				return this;

			if (neighborNode.placedBlock != null && originalNode.placedBlock != null)
			{
				if (neighborNode.placedBlock.Num == originalNode.placedBlock.Num)
					return neighborNode;
				else
					return farthestEmptyNode;
			}

			return neighborNode.FindTarget(originalNode, direction, neighborNode);
		}

		return farthestEmptyNode;
	}
}