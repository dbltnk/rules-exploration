using ProceduralToolkit.Samples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGlue : MonoBehaviour
{
    CellularAutomatonConfigurator conf;

    // Start is called before the first frame update
    void Start()
    {
        conf = FindObjectOfType<CellularAutomatonConfigurator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateCell () {
        Cell c = GetComponent<ProceduralToolkit.Samples.Cell>();
        conf.FlipCell(c.X, c.Y);
    }

    public void ActivateCellDrag () {
        // so we can drag the mouse over the buttons and "draw" the on/off state
        if (Input.GetMouseButton(0)) {
            Cell c = GetComponent<ProceduralToolkit.Samples.Cell>();
            conf.FlipCell(c.X, c.Y);
        }
    }
}
