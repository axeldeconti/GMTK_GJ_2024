using MoreMountains.Feedbacks;
using UnityEngine;

namespace Avenyrh
{
    public class Piece : MonoBehaviour
    {
        [SerializeField] private float _stepDelay = 1f;
        [SerializeField] private float _moveDelay = 0.1f;
        [SerializeField] private float _lockDelay = 0.5f;

        [Header("Debug")]
        [SerializeField] private bool _canMove = false;

        [Header("Feedbacks")]
        [SerializeField] private MMF_Player _hardDropFeedback = null;

        private Board _board = null;
        private Controls _controls = null;
        private TetrominoData _data;
        private Vector3Int[] _cells = null;
        private Vector3Int _position = Vector3Int.zero;
        private int _rotationIndex = 0;

        private float _stepTime = 0;
        private float _moveTime = 0;
        private float _lockTime = 0;

        public void Initialize(Board board, Controls controls, Vector3Int position, TetrominoData data)
        {
            _board = board;
            _controls = controls;
            _data = data;
            _position = position;

            _rotationIndex = 0;
            _stepTime = Time.time + _stepDelay;
            _moveTime = Time.time + _moveDelay;
            _lockTime = 0f;

            if (_cells == null)
                _cells = new Vector3Int[data.cells.Length];

            for (int i = 0; i < _cells.Length; i++)
                _cells[i] = (Vector3Int)data.cells[i];
        }

        private void Update()
        {
            if (!_canMove)
                return;

            _board.ClearMap(this);

            // We use a timer to allow the player to make adjustments to the piece
            // before it locks in place
            _lockTime += Time.deltaTime;

            // Handle rotation
            if (_controls.RotateTrigo())
            {
                Rotate(-1);
            }
            else if (_controls.RotateClockwise())
            {
                Rotate(1);
            }

            // Handle hard drop
            if (_controls.Up())
            {
                HardDrop();
            }

            // Allow the player to hold movement keys but only after a move delay
            // so it does not move too fast
            if (Time.time > _moveTime)
            {
                HandleMoveInputs();
            }

            if (_controls.Store())
            {
                if (_board.StorePiece())
                {
                    _stepTime = Time.time + _stepDelay;
                }
            }

            // Advance the piece to the next row every x seconds
            if (Time.time > _stepTime)
            {
                Step();
            }

            _board.Set(this);
        }

        private void HandleMoveInputs()
        {
            // Soft drop movement
            if (_controls.Down())
            {
                // Update the step time to prevent double movement
                if (Move(Vector2Int.down))
                {
                    _stepTime = Time.time + _stepDelay;
                    _board.AudioManager.PlayMove();
                }
            }

            // Left/right movement
            if (_controls.Left())
            {
                Move(Vector2Int.left);
                _board.AudioManager.PlayMove();
            }
            else if (_controls.Right())
            {
                Move(Vector2Int.right);
                _board.AudioManager.PlayMove();
            }
        }

        private void Step()
        {
            _stepTime = Time.time + _stepDelay;

            // Step down to the next row
            Move(Vector2Int.down);

            _board.AudioManager.PlayStep();

            // Once the piece has been inactive for too long it becomes locked
            if (_lockTime >= _lockDelay)
            {
                _board.LockPiece();
            }
        }

        private void HardDrop()
        {
            while (Move(Vector2Int.down))
            {
                continue;
            }

            _board.LockPiece();
            _hardDropFeedback.PlayFeedbacks();
        }

        private bool Move(Vector2Int translation)
        {
            Vector3Int newPosition = _position;
            newPosition.x += translation.x;
            newPosition.y += translation.y;

            bool valid = _board.IsValidPosition(this, newPosition);

            // Only save the movement if the new position is valid
            if (valid)
            {
                _position = newPosition;
                _moveTime = Time.time + _moveDelay;
                _lockTime = 0f; // Reset
            }

            return valid;
        }

        private void Rotate(int direction)
        {
            // Store the current rotation in case the rotation fails
            // and we need to revert
            int originalRotation = _rotationIndex;

            // Rotate all of the cells using a rotation matrix
            _rotationIndex = Wrap(_rotationIndex + direction, 0, 4);
            ApplyRotationMatrix(direction);

            // Revert the rotation if the wall kick tests fail
            if (!TestWallKicks(_rotationIndex, direction))
            {
                _rotationIndex = originalRotation;
                ApplyRotationMatrix(-direction);
            }
            else
            {
                _board.AudioManager.PlayRotate();
            }
        }

        private void ApplyRotationMatrix(int direction)
        {
            float[] matrix = Data.RotationMatrix;

            // Rotate all of the cells using the rotation matrix
            for (int i = 0; i < _cells.Length; i++)
            {
                Vector3 cell = _cells[i];

                int x, y;

                switch (_data.tetromino)
                {
                    case ETetromino.I:
                    case ETetromino.O:
                        // "I" and "O" are rotated from an offset center point
                        cell.x -= 0.5f;
                        cell.y -= 0.5f;
                        x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                        y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                        break;

                    default:
                        x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                        y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                        break;
                }

                _cells[i] = new Vector3Int(x, y, 0);
            }
        }

        private bool TestWallKicks(int rotationIndex, int rotationDirection)
        {
            int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

            for (int i = 0; i < _data.wallKicks.GetLength(1); i++)
            {
                Vector2Int translation = _data.wallKicks[wallKickIndex, i];

                if (Move(translation))
                    return true;
            }

            return false;
        }

        private int GetWallKickIndex(int rotationIndex, int rotationDirection)
        {
            int wallKickIndex = rotationIndex * 2;

            if (rotationDirection < 0)
                wallKickIndex--;

            return Wrap(wallKickIndex, 0, _data.wallKicks.GetLength(0));
        }

        private int Wrap(int input, int min, int max)
        {
            if (input < min)
                return max - (min - input) % (max - min);
            else
                return min + (input - min) % (max - min);
        }

        public TetrominoData TetroData => _data;
        public Vector3Int[] Cells => _cells;
        public Vector3Int Position => _position;
        public bool CanMove
        {
            get => _canMove;
            set => _canMove = value;
        }
    }
}