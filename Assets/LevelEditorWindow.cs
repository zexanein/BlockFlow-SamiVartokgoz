#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class LevelEditorWindow : EditorWindow
{
    private int _width = 6;
    private int _height = 8;
    private float _timer = 60f;
    private int _selectedTool;
    private int _currentColor;
    private int _currentShapeType;
    private int _currentDirection;
    private int _currentConstraint;
    private int _iceEffectDuration;
    private int _grinderSize = 1;
    private int _grinderColor;

    private HashSet<Vector2Int> _inactiveCells = new();
    private List<SerializedBlockData> _blocks = new();
    private List<SerializedGrinderData> _grinders = new();
    private Dictionary<Vector2Int, SerializedBlockData> _blockCellMap = new();
    private Dictionary<Vector2Int, SerializedGrinderData> _grinderCellMap = new();

    private Vector2 _scrollPos;
    private float _cellDrawSize = 30f;

    private BlockShapeRegistry _shapeRegistry;
    private ColorPalette _colorPalette;

    private LevelDataListWrapper _loadedWrapper;
    private string _loadedPath;
    private int _selectedLevelIndex;

    [MenuItem("Tools/Level Editor")]
    public static void Open()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    private void OnEnable()
    {
        _colorPalette = AssetDatabase.LoadAssetAtPath<ColorPalette>("Assets/Resources/ColorPalette.asset");
        _shapeRegistry = AssetDatabase.LoadAssetAtPath<BlockShapeRegistry>("Assets/ScriptableObjects/BlockShapeRegistry.asset");
    }

    private void OnGUI()
    {
        if (_shapeRegistry == null)
        {
            EditorGUILayout.HelpBox("ShapeRegistry not found.", MessageType.Error);
            _shapeRegistry = EditorGUILayout.ObjectField("Shape Registry", _shapeRegistry, typeof(BlockShapeRegistry), false) as BlockShapeRegistry;
            return;
        }

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        DrawLevelTabs();
        DrawGridSettings();
        DrawToolbar();
        DrawGrid();
        DrawDataList();
        DrawExportButtons();

        EditorGUILayout.EndScrollView();
    }

    private void DrawLevelTabs()
    {
        if (_loadedWrapper == null || _loadedWrapper.levelDataList.Count == 0) return;

        EditorGUILayout.LabelField("Levels", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        for (var i = 0; i < _loadedWrapper.levelDataList.Count; i++)
        {
            var isSelected = i == _selectedLevelIndex;
            var style = isSelected ? EditorStyles.toolbarButton : EditorStyles.miniButton;
            if (GUILayout.Toggle(isSelected, $"Level {i + 1}", style, GUILayout.Width(60)))
            {
                if (i != _selectedLevelIndex)
                {
                    SaveCurrentToWrapper();
                    _selectedLevelIndex = i;
                    LoadLevelAtIndex(i);
                }
            }
        }

        if (GUILayout.Button("+", GUILayout.Width(25)))
        {
            SaveCurrentToWrapper();
            _loadedWrapper.levelDataList.Add(new SerializedLevelData
            {
                gridWidth = 6,
                gridHeight = 8,
                timeLimit = 60f,
                blocks = new(),
                grinders = new(),
                inactiveCells = new()
            });
            _selectedLevelIndex = _loadedWrapper.levelDataList.Count - 1;
            LoadLevelAtIndex(_selectedLevelIndex);
        }

        if (_loadedWrapper.levelDataList.Count > 1 && GUILayout.Button("-", GUILayout.Width(25)))
        {
            _loadedWrapper.levelDataList.RemoveAt(_selectedLevelIndex);
            _selectedLevelIndex = Mathf.Min(_selectedLevelIndex, _loadedWrapper.levelDataList.Count - 1);
            LoadLevelAtIndex(_selectedLevelIndex);
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }

    private void DrawGridSettings()
    {
        EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);
        _width = EditorGUILayout.IntSlider("Width", _width, 3, 12);
        _height = EditorGUILayout.IntSlider("Height", _height, 3, 12);
        _timer = EditorGUILayout.FloatField("Timer", _timer);
        _cellDrawSize = EditorGUILayout.Slider("Cell Size", _cellDrawSize, 20f, 60f);
        EditorGUILayout.Space();
    }

    private void DrawToolbar()
    {
        EditorGUILayout.LabelField("Tool", EditorStyles.boldLabel);
        _selectedTool = GUILayout.Toolbar(_selectedTool, new[] { "Toggle Cell", "Place Block", "Place Grinder" });
        EditorGUILayout.Space();

        if (_selectedTool == 1)
        {
            _currentColor = EditorGUILayout.IntField("Color", _currentColor);
            _currentShapeType = EditorGUILayout.Popup("Shape", _currentShapeType, System.Enum.GetNames(typeof(ShapeType)));
            _currentDirection = EditorGUILayout.Popup("Direction", _currentDirection, System.Enum.GetNames(typeof(Direction)));
            _currentConstraint = EditorGUILayout.Popup("Constraint", _currentConstraint, System.Enum.GetNames(typeof(AxisConstraint)));
            _iceEffectDuration = EditorGUILayout.IntField("Ice Duration (0=none)", _iceEffectDuration);
        }

        if (_selectedTool == 2)
        {
            _grinderColor = EditorGUILayout.IntField("Color", _grinderColor);
            _grinderSize = EditorGUILayout.IntSlider("Size", _grinderSize, 1, 3);
        }

        EditorGUILayout.Space();
    }

    private void DrawGrid()
    {
        EditorGUILayout.LabelField("Grid (click to edit)", EditorStyles.boldLabel);

        var totalW = (_width + 2) * _cellDrawSize + 40f;
        var totalH = (_height + 2) * _cellDrawSize + 40f;
        var gridRect = GUILayoutUtility.GetRect(totalW, totalH);

        var startX = gridRect.x + 20f + _cellDrawSize;
        var startY = gridRect.y + 20f + _cellDrawSize;

        DrawEdgeCells(startX, startY);

        for (var y = _height - 1; y >= 0; y--)
        {
            for (var x = 0; x < _width; x++)
            {
                var cell = new Vector2Int(x, y);
                var drawY = _height - 1 - y;
                var rect = new Rect(
                    startX + x * _cellDrawSize,
                    startY + drawY * _cellDrawSize,
                    _cellDrawSize - 2f,
                    _cellDrawSize - 2f
                );

                var isInactive = _inactiveCells.Contains(cell);
                _blockCellMap.TryGetValue(cell, out var blockHere);

                Color color;
                if (isInactive) color = new Color(0.15f, 0.15f, 0.15f);
                else if (blockHere != null) color = GetColor(blockHere.color);
                else color = new Color(0.4f, 0.4f, 0.4f);

                EditorGUI.DrawRect(rect, color);

                if (blockHere != null)
                {
                    var style = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter };
                    GUI.Label(rect, $"B{blockHere.id}", style);
                }
                else if (!isInactive)
                {
                    var style = new GUIStyle(EditorStyles.miniLabel)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        normal = { textColor = new Color(0.6f, 0.6f, 0.6f) }
                    };
                    GUI.Label(rect, $"{x},{y}", style);
                }

                if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                {
                    OnCellClicked(cell);
                    Event.current.Use();
                    Repaint();
                }
            }
        }

        EditorGUILayout.Space();
    }

    private void DrawEdgeCells(float startX, float startY)
    {
        for (var x = 0; x < _width; x++)
        {
            DrawEdgeCell(
                new Rect(startX + x * _cellDrawSize, startY - _cellDrawSize, _cellDrawSize - 2f, _cellDrawSize - 2f),
                new Vector2Int(x, _height));
            DrawEdgeCell(
                new Rect(startX + x * _cellDrawSize, startY + _height * _cellDrawSize, _cellDrawSize - 2f, _cellDrawSize - 2f),
                new Vector2Int(x, -1));
        }

        for (var y = _height - 1; y >= 0; y--)
        {
            var drawY = _height - 1 - y;
            DrawEdgeCell(
                new Rect(startX - _cellDrawSize, startY + drawY * _cellDrawSize, _cellDrawSize - 2f, _cellDrawSize - 2f),
                new Vector2Int(-1, y));
            DrawEdgeCell(
                new Rect(startX + _width * _cellDrawSize, startY + drawY * _cellDrawSize, _cellDrawSize - 2f, _cellDrawSize - 2f),
                new Vector2Int(_width, y));
        }
    }

    private void DrawEdgeCell(Rect rect, Vector2Int cell)
    {
        _grinderCellMap.TryGetValue(cell, out var grinderHere);

        var color = grinderHere != null ? GetColor(grinderHere.color) : new Color(0.25f, 0.25f, 0.25f);
        EditorGUI.DrawRect(rect, color);

        if (grinderHere != null)
        {
            var style = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter };
            GUI.Label(rect, $"G{grinderHere.id}", style);
        }

        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            OnEdgeCellClicked(cell);
            Event.current.Use();
            Repaint();
        }
    }

    private void OnCellClicked(Vector2Int cell)
    {
        switch (_selectedTool)
        {
            case 0:
                if (_inactiveCells.Contains(cell))
                    _inactiveCells.Remove(cell);
                else
                    _inactiveCells.Add(cell);
                break;

            case 1:
                if (_blockCellMap.TryGetValue(cell, out var existing))
                {
                    _blocks.Remove(existing);
                }
                else
                {
                    _blocks.Add(new SerializedBlockData
                    {
                        id = _blocks.Count > 0 ? _blocks.Max(b => b.id) + 1 : 0,
                        color = _currentColor,
                        shapeType = _currentShapeType,
                        position = cell,
                        direction = _currentDirection,
                        constraint = _currentConstraint,
                        iceEffectDuration = _iceEffectDuration
                    });
                }
                RebuildBlockCellMap();
                break;
        }
    }

    private void OnEdgeCellClicked(Vector2Int cell)
    {
        if (_selectedTool != 2) return;

        if (_grinderCellMap.TryGetValue(cell, out var existing))
        {
            _grinders.Remove(existing);
        }
        else
        {
            _grinders.Add(new SerializedGrinderData
            {
                id = _grinders.Count > 0 ? _grinders.Max(g => g.id) + 1 : 0,
                color = _grinderColor,
                position = cell,
                size = _grinderSize
            });
        }
        RebuildGrinderCellMap();
    }

    private void RebuildBlockCellMap()
    {
        _blockCellMap.Clear();
        foreach (var block in _blocks)
        {
            var shape = GetShapeCells(block.shapeType, block.direction);
            var pos = block.position;
            foreach (var offset in shape)
                _blockCellMap[pos + offset] = block;
        }
    }

    private void RebuildGrinderCellMap()
    {
        _grinderCellMap.Clear();
        foreach (var grinder in _grinders)
        {
            var pos = grinder.position;
            Vector2Int parallel;
            if (pos.x < 0 || pos.x >= _width)
                parallel = Vector2Int.up;
            else
                parallel = Vector2Int.right;

            var startOffset = -(grinder.size / 2);
            for (var i = 0; i < grinder.size; i++)
                _grinderCellMap[pos + parallel * (startOffset + i)] = grinder;
        }
    }

    private Vector2Int[] GetShapeCells(int shapeType, int direction)
    {
        var entry = _shapeRegistry.Get((ShapeType)shapeType);
        if (entry.BaseShape == null || entry.BaseShape.Length == 0)
            return new[] { Vector2Int.zero };
        return StaticMethods.RotateShape(entry.BaseShape, direction);
    }

    private void DrawDataList()
    {
        EditorGUILayout.LabelField("Blocks", EditorStyles.boldLabel);
        for (var i = _blocks.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginHorizontal();
            var b = _blocks[i];
            EditorGUILayout.LabelField(
                $"ID:{b.id} Pos:{b.position} Shape:{(ShapeType)b.shapeType} Dir:{(Direction)b.direction} Color:{b.color}");
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                _blocks.RemoveAt(i);
                RebuildBlockCellMap();
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Grinders", EditorStyles.boldLabel);
        for (var i = _grinders.Count - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginHorizontal();
            var g = _grinders[i];
            EditorGUILayout.LabelField($"ID:{g.id} Pos:{g.position} Size:{g.size} Color:{g.color}");
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                _grinders.RemoveAt(i);
                RebuildGrinderCellMap();
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
    }

    private void DrawExportButtons()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Save"))
        {
            if (_loadedWrapper != null && !string.IsNullOrEmpty(_loadedPath))
            {
                SaveCurrentToWrapper();
                File.WriteAllText(_loadedPath, JsonUtility.ToJson(_loadedWrapper, true));
                AssetDatabase.Refresh();
                Debug.Log($"Saved to: {_loadedPath}");
            }
            else
            {
                ExportLevel(false);
            }
        }

        if (GUILayout.Button("Save As"))
            ExportLevel(false);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (GUILayout.Button("Import JSON"))
            ImportLevel();

        if (GUILayout.Button("Clear All"))
        {
            _blocks.Clear();
            _grinders.Clear();
            _inactiveCells.Clear();
            _blockCellMap.Clear();
            _grinderCellMap.Clear();
            _loadedWrapper = null;
            _loadedPath = null;
        }
    }

    private void ExportLevel(bool append)
    {
        SaveCurrentToWrapper();

        var path = EditorUtility.SaveFilePanel("Save Level", Application.dataPath, "levels", "json");
        if (string.IsNullOrEmpty(path)) return;

        LevelDataListWrapper wrapper;

        if (_loadedWrapper != null)
        {
            wrapper = _loadedWrapper;
        }
        else
        {
            wrapper = new LevelDataListWrapper();
            wrapper.levelDataList.Add(new SerializedLevelData
            {
                gridWidth = _width,
                gridHeight = _height,
                timeLimit = _timer,
                blocks = new List<SerializedBlockData>(_blocks),
                grinders = new List<SerializedGrinderData>(_grinders),
                inactiveCells = new List<Vector2Int>(_inactiveCells)
            });
        }

        File.WriteAllText(path, JsonUtility.ToJson(wrapper, true));
        _loadedPath = path;
        _loadedWrapper = wrapper;
        AssetDatabase.Refresh();
        Debug.Log($"Exported: {path} (Total: {wrapper.levelDataList.Count})");
    }

    private void ImportLevel()
    {
        var path = EditorUtility.OpenFilePanel("Load Level", Application.dataPath, "json");
        if (string.IsNullOrEmpty(path)) return;

        var wrapper = JsonUtility.FromJson<LevelDataListWrapper>(File.ReadAllText(path));
        if (wrapper == null || wrapper.levelDataList.Count == 0) return;

        _loadedWrapper = wrapper;
        _loadedPath = path;
        _selectedLevelIndex = 0;
        LoadLevelAtIndex(0);
        Debug.Log($"Imported {wrapper.levelDataList.Count} levels from: {path}");
    }

    private void LoadLevelAtIndex(int index)
    {
        var level = _loadedWrapper.levelDataList[index];
        _width = level.gridWidth;
        _height = level.gridHeight;
        _timer = level.timeLimit;
        _blocks = new List<SerializedBlockData>(level.blocks);
        _grinders = new List<SerializedGrinderData>(level.grinders);

        _inactiveCells.Clear();
        foreach (var cell in level.inactiveCells)
            _inactiveCells.Add(cell);

        RebuildBlockCellMap();
        RebuildGrinderCellMap();
    }

    private void SaveCurrentToWrapper()
    {
        if (_loadedWrapper == null) return;

        _loadedWrapper.levelDataList[_selectedLevelIndex] = new SerializedLevelData
        {
            gridWidth = _width,
            gridHeight = _height,
            timeLimit = _timer,
            blocks = new List<SerializedBlockData>(_blocks),
            grinders = new List<SerializedGrinderData>(_grinders),
            inactiveCells = new List<Vector2Int>(_inactiveCells)
        };
    }

    private Color GetColor(int colorIndex) => _colorPalette.GetColor(colorIndex);
}
#endif