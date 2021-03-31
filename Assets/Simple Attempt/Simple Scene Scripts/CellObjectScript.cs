using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellObjectScript : MonoBehaviour
{
    Coords coords;
    public Coords GetCoords() { return coords; }
    public void SetCoords(Coords coords) { this.coords = coords; }
}
