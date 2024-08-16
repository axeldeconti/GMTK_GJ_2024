using System.Collections.Generic;
using UnityEngine;

namespace Avenyrh
{
    public class Board : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Tetromino _tetroPrefab = null;
        [SerializeField] private Transform _parent = null;
        [SerializeField] private Vector2Int _spawnPos = Vector2Int.zero;

        [Header("Time")]
        [SerializeField] private float _fallTime = 0.5f;

        private bool[,] _grid = null;
        private float _scale = 1f;

        private float _nextTimeToFall = 0;

        private Tetromino _currentTetro = null;
        private List<Tetromino> _onBoardTetros = null;

        private void Start()
        {
            CreateGrid(10, 10, 0.5f);
            SpawnTetro();
        }

        private void Update()
        {
            if (_currentTetro == null)
                return;

            UpdateMove();
        }

        private void UpdateMove()
        {
            //Move current tetro
            if (Input.GetKeyDown(KeyCode.LeftArrow) && _currentTetro.CheckMove(-1, 0))
            {
                MoveTetro(-1, 0);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) && _currentTetro.CheckMove(1, 0))
            {
                MoveTetro(1, 0);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && _currentTetro.CheckMove(0, -1))
            {
                if(_currentTetro.CheckMove(0, -1))
                {
                    MoveTetro(0, -1);
                }
                else
                {
                    _nextTimeToFall = Time.time + _fallTime;
                    SpawnTetro();
                }
            }

            if (Time.time > _nextTimeToFall)
            {
                _nextTimeToFall = Time.time + _fallTime;
                if(!_currentTetro.CheckMove(0, -1))
                {
                    SpawnTetro();
                }
                else
                {
                    MoveTetro(0, -1);
                }
            }
        }

        private void MoveTetro(int x, int y)
        {
            Vector2Int newPos = _currentTetro.GridPos + new Vector2Int(x, y);
            _currentTetro.SetPosition(newPos);
        }

        private void UpdateRotation()
        {
            //TO DO : Rotation
        }

        #region Grid
        public void CreateGrid(int width, int height, float scale)
        {
            _grid = new bool[width, height];
            _scale = scale;
        }

        public void SetGridValue(Vector2Int gridPos, bool value)
        {
            SetGridValue(gridPos.x, gridPos.y, value);
        }

        public void SetGridValue(int x, int y, bool value)
        {
            if(IsInGrid(x, y))
            {
                _grid[x, y] = value;
            }
        }

        public Vector2Int GetGridPosFormLocalPos(Vector2 localPos)
        {
            return GetGridPosFormLocalPos(localPos.x, localPos.y);
        }

        public Vector2Int GetGridPosFormLocalPos(float localX, float localY)
        {
            int x = Mathf.FloorToInt(localX / _scale);
            int y = Mathf.FloorToInt(localY / _scale);
            return new Vector2Int(x, y);
        }

        public Vector2 GetLocalPosFromGridPos(Vector2Int gridPos)
        {
            return GetLocalPosFromGridPos(gridPos.x, gridPos.y);
        }

        public Vector2 GetLocalPosFromGridPos(int gridX, int gridY)
        {
            return new Vector2(gridX * _scale, gridY * _scale);
        }

        public bool IsInGrid(int x, int y)
        {
            return x >= 0 && y >= 0 && x < _grid.GetLength(0) && y < _grid.GetLength(1);
        }

        public bool IsBlockEmpty(Vector2Int gridPos)
        {
            return IsBlockEmpty(gridPos.x, gridPos.y);
        }

        public bool IsBlockEmpty(int x, int y)
        {
            if (!IsInGrid(x, y))
                return false;

            return !_grid[x, y];
        }
        #endregion

        #region Spawn
        private void SpawnTetro()
        {
            Tetromino inst = Instantiate(_tetroPrefab, Vector3.zero, Quaternion.identity, _parent);
            inst.transform.localScale = Vector3.one * _scale;
            inst.Init(this, _spawnPos);

            if (!inst.CheckMove(Vector2Int.zero))
            {
                //TO DO : Lose
                Debug.Log("LOOOOOOSE");
            }
            else
            {
                if(_currentTetro != null)
                    _currentTetro.Lock();

                _currentTetro = inst;
            }
        }
        #endregion

        public float Scale => _scale;
    }
}