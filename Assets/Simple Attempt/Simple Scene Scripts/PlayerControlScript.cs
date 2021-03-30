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

    [SerializeField] Camera theCamera = null;

    [SerializeField] TMP_InputField seedInput = null;

    GameManagerScript gameManager;

    bool simulationRunning = true;

    public void AssignGameManager(GameManagerScript gameManager)
    {
        this.gameManager = gameManager;
        seedInput.text = gameManager.GetCurrentSeed().ToString();
    }

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

    public void RerollSeed()
    {
        seedInput.text = gameManager.RollNewSeed().ToString();
    }

    public void UpdateSeed()
    {
        gameManager.SetSpecificSeed(int.Parse(seedInput.text));
    }

    /// <summary>
    /// Sets the map to the exact condition it started.
    /// </summary>
    public void ResetExperement()
    {
        Random.InitState(gameManager.GetCurrentSeed());
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

    public void BackToMenu()
    {
        gameManager.LoadScene(SCENE.LEVEL_SETUP);
    }

    public void NewExperement()
    {
        Random.InitState(gameManager.GetCurrentSeed());
        gameManager.SetCurrentLevel(null);//Trying to load a null level will force a random roll.
        gameManager.LoadScene(SCENE.EXPEREMENT);
    }

    bool inputCooling = false;

    private void Update()
    {        
        KeyboardControls();
    }

    private void LateUpdate()
    {
        CameraControls();
    }

    Vector2 mouseDownPosition;//Point where the mouse was clicked. Used to calculate dragging.
    bool mouseHeld = false;

    Vector2 oldMousePosition;

    void CameraControls()
    {
        Vector2 mouseScrollDelta = Input.mouseScrollDelta;

        if(mouseScrollDelta.y != 0)
        {
            theCamera.orthographicSize += mouseScrollDelta.y * -0.25f;
        }

        if(Input.GetMouseButtonUp(1))
        {
            mouseHeld = false;
        }

        if(mouseHeld)
        {
            Vector2 currentMousePostion = Input.mousePosition;

            //Vector2 rawPosition = theCamera.ScreenToWorldPoint(Input.mousePosition);

            Vector2 changeVector = currentMousePostion - oldMousePosition;
            
            theCamera.transform.position -= (Vector3)changeVector * 0.02f;
            //mouseDownPosition = rawPosition;

            oldMousePosition = currentMousePostion;
        }

        if(Input.GetMouseButtonDown(1))
        {
            mouseDownPosition = theCamera.ScreenToWorldPoint(Input.mousePosition);
            oldMousePosition = Input.mousePosition;

            mouseHeld = true;
        }        
    }

    void KeyboardControls()
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
