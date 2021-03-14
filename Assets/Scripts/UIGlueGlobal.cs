using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralToolkit.Samples;

public class UIGlueGlobal : MonoBehaviour
{
    CellularAutomatonConfigurator conf;
    public DD_DataDiagram dd;
    private GameObject lineAlive;
    private GameObject lineDead;
    private int lastStep;

    // Start is called before the first frame update
    void Start()
    {
        conf = GetComponent<CellularAutomatonConfigurator>();
        lineAlive = dd.AddLine("Alive", conf.AliveColor);
        lineDead = dd.AddLine("Dead", conf.DeadColor);
    }

    // Update is called once per frame
    public void Update()
    {
        if (conf.StepCount != lastStep || conf.Dirty) {
            conf.Dirty = false;
            lastStep = conf.StepCount;
            dd.SetLineColor(lineAlive, conf.AliveColor);
            dd.SetLineColor(lineDead, conf.DeadColor);

            bool[,] cells = conf.Automaton.cells;
            int alive = 0;
            int dead = 0;

            for (int x = 0; x < conf.Automaton.config.width; x++) {
                for (int y = 0; y < conf.Automaton.config.height; y++) {
                    if (cells[x, y]) { 
                        alive++; 
                    }
                    else {
                        dead++;
                    }

                } 
            }
        
            dd.InputPoint(lineAlive, new Vector2(1f, alive));
            dd.InputPoint(lineDead, new Vector2(1f, dead));
        }
    }
}
