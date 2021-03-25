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
            CallStopSimulating();
        }
        else
        {
            CallStartSimulating();
        }
    }

    public void StepOnce()
    {
        if(simulationRunning)
        {
            CallStopSimulating();
        }

        cellManager.IncrementTime();
    }

    void CallStopSimulating()
    {
        simulationRunning = false;
        cellManager.StopConstantSimulate();
    }

    void CallStartSimulating()
    {
        simulationRunning = true;
        cellManager.StartConstantSimulate();
    }

    /// <summary>
    /// Sets the map to the exact condition it started.
    /// </summary>
    public void ResetExperement()
    {
        CallStopSimulating();
        cellManager.ReInitializeAllCells();
    }

    /// <summary>
    /// Removes all living cells.
    /// </summary>
    public void ClearDish()
    {
        cellManager.ClearAllLife();
    }

    /// <summary>
    /// Puts a living cell in every position.
    /// </summary>
    public void FillDish()
    {
        cellManager.AllCellsLiving();
    }

    public void SetSimulationSpeed()
    {
        float stepsPerSecond = simulationSpeedSlider.value;

        simulationSpeedReadout.SetText(string.Format("Steps per Second: {0}", stepsPerSecond));
        cellManager.SetSimulationSpeed(stepsPerSecond);
    }

    bool inputCooling = false;

    private void Update()
    {
        if(inputCooling)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            PlayPausePressed();
            CallInputCooldown();
        }
        else if(Input.GetKeyDown(KeyCode.N))
        {
            StepOnce();
            CallInputCooldown();
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            ResetExperement();
            CallInputCooldown();
        }
        else if(Input.GetKeyDown(KeyCode.C))
        {
            ClearDish();
            CallInputCooldown();
        }
        else if(Input.GetKeyDown(KeyCode.F))
        {
            FillDish();
            CallInputCooldown();
        }
    }

    void CallInputCooldown()
    {
        if(inputCooling)
        {
            return;
        }

        inputCooling = true;
        StartCoroutine(InputCooldown());
    }

    IEnumerator InputCooldown()
    {
        yield return new WaitForSeconds(0.1f);

        inputCooling = false;
    }
}
