using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerControlScript : MonoBehaviour
{
    [SerializeField] CellManagerScript cellManager = null;

    [SerializeField] Slider simulationSpeedSlider = null;
    [SerializeField] TMP_Text simulationSpeedReadout = null;

    bool simulationRunning = true;

    public void PlayPausePressed()
    {
        if(simulationRunning)
        {
            cellManager.StopConstantSimulate();
        }
        else
        {
            cellManager.StartConstantSimulate();
        }

        simulationRunning = !simulationRunning;
    }

    public void StepOnce()
    {
        if(simulationRunning)
        {
            cellManager.StopConstantSimulate();
        }

        cellManager.IncrementTime();
    }

    public void SetSimulationSpeed()
    {
        float stepsPerSecond = simulationSpeedSlider.value;

        simulationSpeedReadout.SetText(string.Format("Steps per Second: {0}", stepsPerSecond));
        cellManager.SetSimulationSpeed(stepsPerSecond);
    }
}
