using System;
using UnityEngine;
using UnityEngine.Events;

namespace WMG
{
    [System.Serializable]
    public class Grid<T>
    {
        #region Variables
        public int Width => width;
        public int Height => height;
        public float CellSize => cellSize;
        public Vector3 OriginPosition =>  GetOriginPosition(gridOrigin);
        public Quaternion Rotation => Quaternion.Euler(rotationVector);

        [SerializeField, Tooltip("Defines the number of cells for the width")] private int width;
        [SerializeField, Tooltip("Defines the number of cells for the height")] private int height;
        [SerializeField, Tooltip("Defines the cell size")] private float cellSize;
        [SerializeField] private Vector3 originPosition;
        [SerializeField] private Vector3 rotationVector;
        [SerializeField] private GridOrientation gridOrientation;
        [SerializeField] private GridOrigin gridOrigin;
        [SerializeField] private bool debug = true;
        [SerializeField] private Color debugColor = Color.white;

        public enum GridOrigin
        {
            BottomLeft,
            BottomRight,
            TopLeft,
            TopRight,
            Center
        }

        public enum GridOrientation
        {
            XY,
            XZ
        }

        [System.Serializable]
        public class OnGridValueChangedEvent : UnityEvent<int, int> { }
        [Space]
        public OnGridValueChangedEvent OnGridValueChanged;

        private T[,] gridArray; 
        #endregion

        #region Constructors
        public Grid(int width, int height, float cellSize, Vector3 originPosition = default, Quaternion rotation = default)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.originPosition = originPosition;
            this.rotationVector = rotation * Vector3.forward;

            gridArray = new T[width, height];
        }

        public Grid() : this(0, 0, 0f) { }

        public Grid(int width, int height, float cellSize, Func<Grid<T>, int, int, T> createGridObject, Vector3 originPosition = default, Quaternion rotation = default) : this(width, height, cellSize, originPosition, rotation)
        {
            InitializeAllValues(createGridObject);
        }
        #endregion

        #region Methods
        public void New() => gridArray = new T[width, height];

        public void New(Func<Grid<T>, int, int, T> createGridObject)
        {
            New();
            InitializeAllValues(createGridObject);
        }

        private void InitializeAllValues(Func<Grid<T>, int, int, T> createGridObject)
        {
            if (gridArray == null)
                return;

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    gridArray[x, y] = createGridObject(this, x, y);
                }
            }
        }

        private Vector3 GetOriginPosition(GridOrigin origin)
        {
            switch (origin)
            {
                case GridOrigin.BottomLeft:
                    return originPosition;
                case GridOrigin.BottomRight:
                    return originPosition - Rotation * /*new Vector3(cellSize * width, 0f);*/ GetCellPositionNoCellSize(cellSize * width, 0);
                case GridOrigin.TopLeft:
                    return originPosition - Rotation * /*new Vector3(0f, cellSize * height);*/ GetCellPositionNoCellSize(0, cellSize * height);
                case GridOrigin.TopRight:
                    return originPosition - Rotation * /*new Vector3(cellSize * width, cellSize * height);*/ GetCellPositionNoCellSize(cellSize * width, cellSize * height);
                case GridOrigin.Center:
                    return originPosition - Rotation * /*new Vector3(cellSize * width, cellSize * height) / 2;*/ GetCellPositionNoCellSize(cellSize * width, cellSize * height) / 2;
                default:
                    return originPosition;
            }
        }

        private Vector3 GetCellPositionNoCellSize(float x, float y)
        {
            switch (gridOrientation)
            {
                case GridOrientation.XY:
                default:
                    return new Vector3(x, y);
                case GridOrientation.XZ:
                    return new Vector3(x, 0f, y);
            }
        }

        public Vector3 GetWorldPosition(int x, int y) => (Rotation * (GetCellPositionNoCellSize(x, y) * cellSize)) + OriginPosition;

        public Vector3 GetCellPositionCenter(int x, int y) => GetWorldPosition(x, y) + Rotation * ((GetCellPositionNoCellSize(1, 1) * cellSize) / 2);
        
        public void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            worldPosition -= OriginPosition;
            worldPosition = Quaternion.Inverse(Rotation) * worldPosition;
            if (gridOrientation == GridOrientation.XY)
            {
                x = Mathf.FloorToInt(worldPosition.x / cellSize);
                y = Mathf.FloorToInt(worldPosition.y / cellSize);
            }
            else
            {
                x = Mathf.FloorToInt(worldPosition.x / cellSize);
                y = Mathf.FloorToInt(worldPosition.z / cellSize);
            }
        }

        public bool GetGridObject(int x, int y, out T result)
        {
            bool test = x >= 0 && y >= 0 && x < width && y < height;

            result = test ? gridArray[x, y] : default;

            return test;
        }

        public bool GetGridObject(Vector3 worldPosition, out T result)
        {
            GetXY(worldPosition, out int x, out int y);
            return GetGridObject(x, y, out result);
        }

        public void SetGridObject(int x, int y, T value)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                gridArray[x, y] = value;
                OnGridValueChanged.Invoke(x, y);
            }
        }

        public void SetGridObject(Vector3 worldPosition, T value)
        {
            GetXY(worldPosition, out int x, out int y);
            SetGridObject(x, y, value);
        }

        public void TriggerGridObjectChanged(int x, int y) => OnGridValueChanged.Invoke(x, y);
        
        public void GizmosDrawDebug()
        {
            if (!debug)
                return;

            Gizmos.color = debugColor;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1));
                    Gizmos.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y));
                }
            }
            Gizmos.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height));
            Gizmos.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height));
        }
        #endregion
    } 
}
