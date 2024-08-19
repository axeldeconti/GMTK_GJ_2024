using MoreMountains.Feedbacks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Avenyrh
{
    [DefaultExecutionOrder(-1)]
    public class Board : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private AudioManager _audioManager = null;

        [Header("References")]
        [SerializeField] private Tilemap _boardTilemap = null;
        [SerializeField] private Piece _piece = null;
        [SerializeField] private Transform _objParent = null;
        [SerializeField] private TetrominoData[] _tetrominoes = null;
        [SerializeField] private Tilemap _ghostTilemap = null;

        [Header("Validation")]
        [SerializeField] private Tilemap _validatedTilemap = null;
        [SerializeField] private Tile _validatedTile = null;

        [Header("Previews")]
        [SerializeField] private SpriteRenderer _preview = null;
        [SerializeField] private SpriteRenderer _store = null;

        [Header("Board")]
        [SerializeField] private Vector2Int _boardSize = new Vector2Int(10, 20);
        [SerializeField] private Vector3Int _spawnPosition = new Vector3Int(-1, 8, 0);

        [Header("Objectives")]
        [SerializeField] private List<Objective> _objectives = null;

        [Header("Score")]
        [SerializeField] private TextMeshProUGUI _scoreText = null;
        [SerializeField] private TextMeshProUGUI _multiplierText = null;
        [SerializeField] private int _scorePerTile = 10;
        [SerializeField] private float _multiplierPerTile = 0.0f;

        [Header("Feedbacks")]
        [SerializeField] private GameObject _finalScoreParent = null;
        [SerializeField] private TextMeshProUGUI _finalScoreText = null;

        [Header("Feedbacks")]
        [SerializeField] private MMF_Player _newObjectiveFeedback = null;
        [SerializeField] private MMF_Player _validatedFeedback = null;
        [SerializeField] private MMF_Player _destroyLineFeedback = null;
        [SerializeField] private MMF_Player _nextFeedback = null;
        [SerializeField] private MMF_Player _storeFeedback = null;
        [SerializeField] private MMF_Player _scoreFeedback = null;
        [SerializeField] private MMF_Player _winFeedback = null;
        [SerializeField] private MMF_Player _looseFeedback = null;
        [SerializeField] private MMF_Player _finalScoreFeedback = null;

        private Queue<ETetromino> _tetroQueue = null;
        private Objective _objective = null;
        private Controls _controls = null;
        private int _currentObjectiveRow = 0;
        private bool _hasStoredThisPiece = false;
        private ETetromino _storeTetro = ETetromino.None;

        private int _currentScore = 0;
        private float _currentMultiplier = 0.0f;

        private void Awake()
        {
            EventManager.Subscribe(Ev.OnStartGame, OnStartGame);
            EventManager.Subscribe(Ev.OnEndGame, OnEndGame);

            _tetroQueue = new Queue<ETetromino>();
            for (int i = 0; i < _tetrominoes.Length; i++)
            {
                _tetrominoes[i].Initialize();
            }

            ResetScore();
            _store.sprite = null;
            _finalScoreParent.SetActive(false);
        }

        private void OnDestroy()
        {
            EventManager.Unsubscribe(Ev.OnStartGame, OnStartGame);
            EventManager.Unsubscribe(Ev.OnEndGame, OnEndGame);
        }

        public void SetControls(Controls controls)
        {
            _controls = controls;
        }

        public void SetCanPlay(bool canPlay)
        {
            _piece.CanMove = canPlay;
        }

        #region Piece
        public void SpawnPiece()
        {
            // Reset tetro queue
            if (_tetroQueue.Count == 0)
                RefreshQueue();

            ETetromino tetro = _tetroQueue.Dequeue();
            TetrominoData data = _tetrominoes[tetro.GetHashCode()];
            _piece.Initialize(this, _controls, _spawnPosition, data);

            if (IsValidPosition(_piece, _spawnPosition))
            {
                Set(_piece);

                if (_tetroQueue.Count == 0)
                    RefreshQueue();

                ETetromino t = _tetroQueue.Peek();
                TetrominoData d = _tetrominoes[t.GetHashCode()];
                _preview.sprite = d.preview;
                _hasStoredThisPiece = false;
            }
            else
            {
                ObjectiveLost();
            }
        }

        private void RefreshQueue()
        {
            List<ETetromino> tetros = new List<ETetromino>() { ETetromino.I, ETetromino.J, ETetromino.L, ETetromino.O, ETetromino.S, ETetromino.T, ETetromino.Z };

            while (tetros.Count > 0)
            {
                ETetromino t = tetros.GetRandom();
                tetros.Remove(t);
                _tetroQueue.Enqueue(t);
            }
        }

        public void Set(Piece piece)
        {
            for (int i = 0; i < piece.Cells.Length; i++)
            {
                Vector3Int tilePosition = piece.Cells[i] + piece.Position;
                _boardTilemap.SetTile(tilePosition, piece.TetroData.tile);
            }
        }

        public void ClearMap(Piece piece)
        {
            for (int i = 0; i < piece.Cells.Length; i++)
            {
                Vector3Int tilePosition = piece.Cells[i] + piece.Position;
                _boardTilemap.SetTile(tilePosition, null);
            }
        }

        public bool IsValidPosition(Piece piece, Vector3Int position)
        {
            RectInt bounds = Bounds;

            // The position is only valid if every cell is valid
            for (int i = 0; i < piece.Cells.Length; i++)
            {
                Vector3Int tilePosition = piece.Cells[i] + position;

                // An out of bounds tile is invalid
                if (!bounds.Contains((Vector2Int)tilePosition))
                    return false;

                // A tile already occupies the position, thus invalid
                if (_boardTilemap.HasTile(tilePosition))
                    return false;

                // Tiles should always be above the current objective line
                if (tilePosition.y < _currentObjectiveRow)
                    return false;
            }

            return true;
        }

        public bool StorePiece()
        {
            if (!_hasStoredThisPiece)
            {
                ETetromino current = _piece.TetroData.tetromino;

                if (_storeTetro != ETetromino.None)
                {
                    // Change current piece with stored one
                    TetrominoData data = _tetrominoes[_storeTetro.GetHashCode()];
                    _piece.Initialize(this, _controls, _spawnPosition, data);

                    if (IsValidPosition(_piece, _spawnPosition))
                    {
                        Set(_piece);
                    }
                    else
                    {
                        ObjectiveLost();
                    }
                }
                else
                {
                    SpawnPiece();
                }

                _storeTetro = current;
                TetrominoData d = _tetrominoes[_storeTetro.GetHashCode()];
                _store.sprite = d.preview;
                _storeFeedback.PlayFeedbacks();
                _audioManager.PlayStore();
                _hasStoredThisPiece = true;

                return true;
            }

            return false;
        }
        #endregion

        #region Lines
        public void ClearLines()
        {
            RectInt bounds = Bounds;
            int row = _currentObjectiveRow;
            bool clear = false;
            bool lineWasCleared = false;

            // Clear from bottom to top
            while (row < bounds.yMax)
            {
                // Only advance to the next row if the current is not cleared
                // because the tiles above will fall down when a row is cleared
                clear = clear || IsLineFull(row);
                if (clear && !IsLineEmpty(row))
                {
                    LineClear(row);
                    lineWasCleared = true;
                }
                else
                {
                    row++;
                }
            }

            if (lineWasCleared)
            {
                _currentMultiplier = 1.0f;
                _multiplierText.text = "X1,0";
                _destroyLineFeedback.PlayFeedbacks();
                _audioManager.PlayDestroy();
            }
        }

        public bool IsLineFull(int row)
        {
            RectInt bounds = Bounds;

            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);

                // The line is not full if a tile is missing
                if (!_boardTilemap.HasTile(position))
                    return false;
            }

            return true;
        }

        public bool IsLineEmpty(int row)
        {
            RectInt bounds = Bounds;

            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);

                // The line is not full if a tile is missing
                if (_boardTilemap.HasTile(position))
                    return false;
            }

            return true;
        }

        public void LineClear(int row)
        {
            RectInt bounds = Bounds;

            // Clear all tiles in the row
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);
                _boardTilemap.SetTile(position, null);
            }

            // Shift every row above down one
            while (row < bounds.yMax)
            {
                for (int col = bounds.xMin; col < bounds.xMax; col++)
                {
                    Vector3Int position = new Vector3Int(col, row + 1, 0);
                    TileBase above = _boardTilemap.GetTile(position);

                    position = new Vector3Int(col, row, 0);
                    _boardTilemap.SetTile(position, above);
                }

                row++;
            }
        }

        public void ValidateLine(int row)
        {
            RectInt bounds = Bounds;
            int nbOfTiles = 0;

            _validatedFeedback.PlayFeedbacks();
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, _currentObjectiveRow, 0);

                if (_objective.HasTile(position))
                    nbOfTiles++;

                _validatedTilemap.SetTile(position, _validatedTile);
            }

            Score(nbOfTiles);
        }
        #endregion

        #region Objective
        public void SetObjective(Objective obj)
        {
            _currentObjectiveRow = Bounds.yMin;

            _objective = Instantiate(obj, Vector3.zero, Quaternion.identity, _objParent);
            _objective.transform.localPosition = Vector3.zero;
            _newObjectiveFeedback.PlayFeedbacks();

            SpawnPiece();
            _piece.CanMove = true;
        }

        public void LockPiece()
        {
            Set(_piece);
            bool objFinished = CheckObjective();
            ClearLines();

            if(!objFinished)
                SpawnPiece();

            _nextFeedback.PlayFeedbacks();
        }

        public bool CheckObjective()
        {
            while (IsObjectiveLineValidated())
            {
                ValidateLine(_currentObjectiveRow);
                _currentObjectiveRow++;

                if (_currentObjectiveRow >= _objective.MaxRow)
                {
                    //Validate objective
                    _piece.CanMove = false;
                    _piece.ResetCells();
                    _winFeedback.PlayFeedbacks();

                    _validatedTilemap.ClearAllTiles();
                    _boardTilemap.ClearAllTiles();
                    _ghostTilemap.ClearAllTiles();

                    Destroy(_objective.gameObject);

                    Invoke("SetNewRandomObjective", 1);
                    return true;
                }
            }

            return false;
        }

        private void SetNewRandomObjective()
        {
            SetObjective(_objectives.GetRandom());
        }

        public bool IsObjectiveLineValidated()
        {
            RectInt bounds = Bounds;

            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, _currentObjectiveRow, 0);

                bool isInObj = _objective.HasTile(position);
                bool isInBoard = _boardTilemap.HasTile(position);

                if ((isInBoard && !isInObj) || (!isInBoard && isInObj))
                    return false;
            }

            return true;
        }

        public void ObjectiveLost()
        {
            ResetScore();
            _looseFeedback.PlayFeedbacks();

            _validatedTilemap.ClearAllTiles();
            _boardTilemap.ClearAllTiles();

            Invoke("SetNewRandomObjective", 1);
        }
        #endregion

        #region Score
        private void Score(int nbOfTiles)
        {
            _currentMultiplier += nbOfTiles * _multiplierPerTile;
            _currentScore += Mathf.FloorToInt(nbOfTiles * _scorePerTile * _currentMultiplier);

            _multiplierText.text = $"X{_currentMultiplier:0.0}";
            _scoreText.text = _currentScore.ToString();

            _scoreFeedback.PlayFeedbacks();
        }

        private void ResetScore()
        {
            _currentScore = 0;
            _scoreText.text = "0";
            _currentMultiplier = 1.0f;
            _multiplierText.text = "X1,0";
        }
        #endregion

        #region Events
        private void OnStartGame(object[] args)
        {
            SetNewRandomObjective();
        }

        private void OnEndGame(object[] args)
        {
            _piece.CanMove = false;
            Invoke("ShowFinalScore", 1);
        }

        private void ShowFinalScore()
        {
            _finalScoreParent.SetActive(true);
            _finalScoreText.text = _currentScore.ToString();
            _finalScoreFeedback.PlayFeedbacks();
        }
        #endregion

        private void OnDrawGizmosSelected()
        {
            RectInt bounds = Bounds;
            Vector3 from = new Vector3Int(bounds.xMin, _currentObjectiveRow, 0) + transform.position;
            Vector3 to = new Vector3Int(bounds.xMax, _currentObjectiveRow, 0) + transform.position;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(from, to);

            if (_objective == null)
                return;

            from = new Vector3Int(bounds.xMin, _objective.MaxRow, 0) + transform.position;
            to = new Vector3Int(bounds.xMax, _objective.MaxRow, 0) + transform.position;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(from, to);
        }

        #region Getters & Setters
        public AudioManager AudioManager => _audioManager;
        public Vector2Int BoardSize => _boardSize;
        public Objective Objective => _objective;

        public RectInt Bounds
        {
            get
            {
                Vector2Int position = new Vector2Int(-_boardSize.x / 2, -_boardSize.y / 2);
                return new RectInt(position, _boardSize);
            }
        }
        #endregion
    }
}