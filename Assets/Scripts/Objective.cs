using UnityEngine;
using UnityEngine.Tilemaps;

namespace Avenyrh
{
    public class Objective : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap = null;
        [SerializeField] private int _maxRow = 0;
        [SerializeField] private Tile _lockedTile = null;

        public bool HasTile(Vector3Int pos)
        {
            return _tilemap.HasTile(pos);
        }

        public int MaxRow => _maxRow;
    }
}
