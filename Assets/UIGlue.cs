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
        int c = GetComponent<ProceduralToolkit.Samples.Cell>().counter;
        FindObjectOfType<ProceduralToolkit.Samples.CellularAutomatonConfigurator>().ActivateCell(c);
    }
}
