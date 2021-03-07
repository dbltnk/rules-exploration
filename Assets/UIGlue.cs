using ProceduralToolkit.Samples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGlue : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateCell () {
        Cell c = GetComponent<ProceduralToolkit.Samples.Cell>();
        //FindObjectOfType<ProceduralToolkit.Samples.CellularAutomatonConfigurator>().ActivateCell(c.counter);
        FindObjectOfType<ProceduralToolkit.Samples.CellularAutomatonConfigurator>().FlipCell(c.X, c.Y);
    }
}
