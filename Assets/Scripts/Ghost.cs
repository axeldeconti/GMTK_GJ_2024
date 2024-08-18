using UnityEngine;
using UnityEngine.Tilemaps;

namespace Avenyrh
{
    public class Ghost : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap = null;
        [SerializeField] private Tile _tile = null;
        [SerializeField] private Board _board = null;
        [SerializeField] private Piece _piece = null;

        private Vector3Int[] _cells = null;
        private Vector3Int _position = Vector3Int.zero;

        private void Awake()
        {
            _cells = new Vector3Int[4];
        }

        private void LateUpdate()
        {
            if (_board.Objective == null || _piece.Cells == null || _piece.Cells.Length == 0)
                return;

            Clear();
            Copy();
            Drop();
            Set();
        }

        private void Clear()
        {
            for (int i = 0; i < _cells.Length; i++)
            {
                Vector3Int tilePosition = _cells[i] + _position;
                _tilemap.SetTile(tilePosition, null);
            }
        }

        private void Copy()
        {
            for (int i = 0; i < _cells.Length; i++)
            {
                _cells[i] = _piece.Cells[i];
            }
        }

        private void Drop()
        {
            Vector3Int position = _piece.Position;

            int current = position.y;
            int bottom = -_board.BoardSize.y / 2 - 1;

            _board.ClearMap(_piece);

            for (int row = current; row >= bottom; row--)
            {
                position.y = row;

                if (_board.IsValidPosition(_piece, position))
                {
                    this._position = position;
                }
                else
                {
                    break;
                }
            }

            _board.Set(_piece);
        }

        private void Set()
        {
            for (int i = 0; i < _cells.Length; i++)
            {
                Vector3Int tilePosition = _cells[i] + _position;
                _tilemap.SetTile(tilePosition, _tile);
            }
        }
    }
}