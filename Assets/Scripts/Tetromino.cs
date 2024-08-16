using System.Collections.Generic;
using UnityEngine;

namespace Avenyrh
{
	public class Tetromino : MonoBehaviour
	{
        [SerializeField]
        private List<Transform> _blocks = null;

        private Board _board = null;
        private int _rotation = 0;

        public void Init(Board board, Vector2Int pos)
        {
            _board = board;
            SetPosition(pos);
        }

        public bool CheckMove(Vector2Int move)
        {
            return CheckMoveReal(move.x * _board.Scale, move.y * _board.Scale);
        }

        public bool CheckMove(int x, int y)
        {
            return CheckMoveReal(x * _board.Scale, y * _board.Scale);
        }

        private bool CheckMoveReal(float x, float y)
        {
            foreach (Transform t in _blocks)
            {
                Vector2 blockLocalPos = t.localPosition * _board.Scale;
                Vector2 newPos = new Vector2(x + blockLocalPos.x + transform.localPosition.x, y + blockLocalPos.y + transform.localPosition.y);
                Vector2Int gridPos = _board.GetGridPosFormLocalPos(newPos);
                bool isBlockEmpty = _board.IsBlockEmpty(gridPos);

                if(!isBlockEmpty)
                    return false;
            }

            return true;
        }

        public bool CheckRotation(bool clockwise)
        {
            return false;
        }

        public void SetPosition(Vector2Int pos)
        {
            Vector3 localPos = _board.GetLocalPosFromGridPos(pos);
            transform.localPosition = localPos;
        }

        public void Rotate(bool clockwise)
        {
            _rotation += clockwise ? 1 : -1;
        }

        public void Lock()
        {
            foreach (Transform t in _blocks)
            {
                Vector2 blockLocalPos = t.localPosition * _board.Scale;
                Vector2 pos = new Vector2(blockLocalPos.x + transform.localPosition.x, blockLocalPos.y + transform.localPosition.y);
                Vector2Int gridPos = _board.GetGridPosFormLocalPos(pos);
                _board.SetGridValue(gridPos, true);
            }
        }

        public Vector2Int GridPos => _board.GetGridPosFormLocalPos(transform.localPosition);
    }
}