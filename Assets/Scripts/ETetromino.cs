using UnityEngine;
using UnityEngine.Tilemaps;

namespace Avenyrh
{
    public enum ETetromino
    {
        I, J, L, O, S, T, Z,
        None
    }

    [System.Serializable]
    public struct TetrominoData
    {
        public Tile tile;
        public ETetromino tetromino;
        public Sprite preview;

        public Vector2Int[] cells { get; private set; }
        public Vector2Int[,] wallKicks { get; private set; }

        public void Initialize()
        {
            cells = Data.Cells[tetromino];
            wallKicks = Data.WallKicks[tetromino];
        }
    }
}