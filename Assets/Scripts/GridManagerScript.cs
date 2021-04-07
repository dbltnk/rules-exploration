using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Coords
{
    public Coords(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public int x;
    public int y;
}

public enum NEIGHBORS
{
    NW,
    DN,
    NE,
    DE,
    SE,
    DS,
    SW,
    DW,
}

public class GridManagerScript : MonoBehaviour
{
    [SerializeField] CellManagerScript cellManager = null;
    [SerializeField] PlayerControlScript playerControl = null;

    GameManagerScript gameManager;

    [SerializeField] GameObject gameManagerPrefab = null;

    int gridWidth = 15;
    int gridHeight = 10;
    float cellSize = 1f;

    [SerializeField] Camera sceneCamera = null;
    [Tooltip("If true, camera size adjusts on start depending on cell size and grid size.")]
    bool autoAdjustCameraSize = true;

    [SerializeField] GameObject cellPrefab = null;
    [SerializeField] GameObject dishPrefab = null;

    Coords[] allCoords;
    public Coords[] GetAllCoords() { return allCoords; }
    Dictionary<Coords, GameObject> coordsToGameObject;
    Dictionary<Coords, Vector3> coordsToPosition;
    Dictionary<Coords, SpriteRenderer> coordsToSpriteRenderer;

    public Level currentLevel;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManagerScript>();
        if(gameManager == null)
        {
            gameManager = Instantiate(gameManagerPrefab).GetComponent<GameManagerScript>();
        }

        AssignLevel(gameManager.GetCurrentLevel());
    }

    void AssignLevel(Level level)
    {
        cellManager.AssignLevel(level, gameManager,  gameManager.GetSpeciesBank());

        playerControl.AssignGameManager(gameManager);

        currentLevel = level;

        gridWidth = level.levelWidth;
        gridHeight = level.levelHeight;

        BuildGrid();
    }

    void BuildGrid()
    {
        coordsToGameObject = new Dictionary<Coords, GameObject>();
        coordsToPosition = new Dictionary<Coords, Vector3>();
        coordsToSpriteRenderer = new Dictionary<Coords, SpriteRenderer>();

        // Hack to make the grid cells show over the background when loading a level from the main menu. Unclear why I need this.
        sceneCamera.gameObject.SetActive(false);
        sceneCamera.gameObject.SetActive(true);

        float zoomOffset = 1.2f;
        float offset = 0f;
        float longerSide = Mathf.Max(gridWidth, gridHeight);
        if (longerSide % 2 == 0) offset = -0.5f;
        if (autoAdjustCameraSize)
        {
            sceneCamera.orthographicSize = (cellSize * longerSide / 2) * zoomOffset;
        }
        sceneCamera.transform.position = new Vector3((gridWidth / 2) * cellSize + offset, 
                                                     (gridHeight / 2) * cellSize + offset, 
                                                     -10);

        allCoords = new Coords[gridWidth * gridHeight];
        int cellIncrementor = 0;
        GameObject GridRoot = new GameObject();
        GridRoot.name = "Grid";
        GameObject dish = Instantiate(dishPrefab);
        float dishOffset = -0.5f;
        dish.transform.position = new Vector3(GridRoot.transform.position.x + longerSide / 2 + dishOffset, 
                                              GridRoot.transform.position.y + dishOffset + longerSide / 2, 
                                              GridRoot.transform.position.z);
        dish.transform.localScale *= longerSide;

        for (int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y < gridHeight; y++)
            {
                Coords currentCoords = new Coords(x, y);
                Vector3 currentPosition = new Vector3(x * cellSize, y * cellSize, 0);
                GameObject currentCell = Instantiate(cellPrefab, currentPosition, Quaternion.identity);
                currentCell.transform.SetParent(GridRoot.transform, true);
                CellObjectScript cellScript = currentCell.GetComponent<CellObjectScript>();

                cellScript.SetCoords(currentCoords);

                coordsToGameObject[currentCoords] = currentCell;
                coordsToPosition[currentCoords] = currentPosition;
                coordsToSpriteRenderer[currentCoords] = currentCell.GetComponentInChildren<SpriteRenderer>();

                currentCell.name = string.Format("{0}, {1}", x, y);

                allCoords[cellIncrementor] = currentCoords;
                cellIncrementor++;
            }
        }

        cellManager.InitializeAllCells(allCoords);
    }

    public void SetCellColor(Coords coords, Color color)
    {
        coordsToSpriteRenderer[coords].color = color;
    }

    public bool CheckWallAdjacent(Coords coords)
    {
        if(GetAllValidNeighbors(coords, NEIGHBOR_STYLE.ALL).Count < 8)
        {
            return true;
        }

        return false;
    }

    public List<Coords> GetAllValidNeighbors(Coords baseCoords, NEIGHBOR_STYLE neighborStyle)
    {
        List<Coords> validNeighbors = new List<Coords>();
        List<int> excludedDirections = new List<int>();

        switch(neighborStyle)
        {
            case NEIGHBOR_STYLE.CARDINAL_ONLY:
                excludedDirections.Add((int)NEIGHBORS.NE);
                excludedDirections.Add((int)NEIGHBORS.NW);
                excludedDirections.Add((int)NEIGHBORS.SE);
                excludedDirections.Add((int)NEIGHBORS.SW);
                break;
            case NEIGHBOR_STYLE.DIAGONAL_ONLY:
                excludedDirections.Add((int)NEIGHBORS.DE);
                excludedDirections.Add((int)NEIGHBORS.DN);
                excludedDirections.Add((int)NEIGHBORS.DS);
                excludedDirections.Add((int)NEIGHBORS.DW);
                break;
        }

        for(int i = 0; i < 8; i++)
        {
            if(excludedDirections.Contains(i)) { continue; }//Skips over excluded directions.
            Coords potentialNeighbor = GetSpecificNeighbor(baseCoords, (NEIGHBORS)i);

            if(potentialNeighbor.x < 0 || potentialNeighbor.y < 0)
            {
                continue;
            }

            validNeighbors.Add(potentialNeighbor);
        }

        return validNeighbors;
    }

    public Coords GetSpecificNeighbor(Coords baseCoords, NEIGHBORS neighbor)
    {
        Coords newCoords = new Coords(-1, -1);//Universal bad coords since they are below zero.

        int baseX = baseCoords.x;
        int baseY = baseCoords.y;

        switch(neighbor)
        {
            case NEIGHBORS.NW:
                newCoords = new Coords(baseX - 1, baseY + 1);
                break;
            case NEIGHBORS.DN:
                newCoords = new Coords(baseX, baseY + 1);
                break;
            case NEIGHBORS.NE:
                newCoords = new Coords(baseX + 1, baseY + 1);
                break;
            case NEIGHBORS.DE:
                newCoords = new Coords(baseX + 1, baseY);
                break;
            case NEIGHBORS.SE:
                newCoords = new Coords(baseX + 1, baseY - 1);
                break;
            case NEIGHBORS.DS:
                newCoords = new Coords(baseX, baseY - 1);
                break;
            case NEIGHBORS.SW:
                newCoords = new Coords(baseX - 1, baseY - 1);
                break;
            case NEIGHBORS.DW:
                newCoords = new Coords(baseX - 1, baseY);
                break;
        }

        if(CheckValidCell(newCoords))
        {
            return newCoords;
        }
        else
        {
            return new Coords(-1, -1);//Universal bad coords since they are below zero.
        }
    }

    public bool CheckValidCell(Coords coords)
    {
        if(coords.x < 0)
        {
            return false;
        }

        if(coords.x >= gridWidth)
        {
            return false;
        }

        if(coords.y < 0)
        {
            return false;
        }

        if(coords.y >= gridHeight)
        {
            return false;
        }

        return true;
    }
}
