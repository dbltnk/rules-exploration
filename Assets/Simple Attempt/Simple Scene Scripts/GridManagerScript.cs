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

    [SerializeField] int gridWidth = 15;
    [SerializeField] int gridHeight = 10;
    [SerializeField] float cellSize = 1f;

    [SerializeField] Camera sceneCamera = null;
    [Tooltip("If true, camera size adjusts on start depending on cell size and grid size.")]
    [SerializeField] bool autoAdjustCameraSize = false;

    [SerializeField] GameObject cellPrefab = null;

    Coords[] allCoords;
    public Coords[] GetAllCoords() { return allCoords; }
    Dictionary<Coords, GameObject> coordsToGameObject;
    Dictionary<Coords, Vector3> coordsToPosition;
    Dictionary<Coords, SpriteRenderer> coordsToSpriteRenderer;

    private void Awake()
    {
        BuildGrid();
    }

    void BuildGrid()
    {
        coordsToGameObject = new Dictionary<Coords, GameObject>();
        coordsToPosition = new Dictionary<Coords, Vector3>();
        coordsToSpriteRenderer = new Dictionary<Coords, SpriteRenderer>();

        if(autoAdjustCameraSize)
        {
            sceneCamera.orthographicSize = (cellSize * Mathf.Max(gridWidth, gridHeight) / 2);
        }
        
        sceneCamera.transform.position = new Vector3((gridWidth / 2) * cellSize, (gridHeight / 2) * cellSize, -10);

        allCoords = new Coords[gridWidth * gridHeight];
        int cellIncrementor = 0;

        for(int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y < gridHeight; y++)
            {
                Coords currentCoords = new Coords(x, y);
                Vector3 currentPosition = new Vector3(x * cellSize, y * cellSize, 0);
                GameObject currentCell = Instantiate(cellPrefab, currentPosition, Quaternion.identity);

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
        if(GetAllValidNeighbors(coords).Count < 8)
        {
            return true;
        }

        return false;
    }

    public List<Coords> GetAllValidNeighbors(Coords baseCoords)
    {
        List<Coords> validNeighbors = new List<Coords>();

        for(int i = 0; i < 8; i++)
        {
            Coords potentialNeighbor = GetSpecificNeighbor(baseCoords, (NEIGHBORS)i);

            if(potentialNeighbor.x < 0)
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
