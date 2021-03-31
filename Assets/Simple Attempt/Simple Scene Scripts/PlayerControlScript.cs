using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum MOUSE_MODE
{
    NONE,
    CELL_SELECTED,
    CLONE_HERE,
    DELETE,
}

public class PlayerControlScript : MonoBehaviour
{
    [SerializeField] CellManagerScript cellManager = null;

    [SerializeField] Slider simulationSpeedSlider = null;
    [SerializeField] TMP_Text simulationSpeedReadout = null;

    [SerializeField] Camera theCamera = null;

    [SerializeField] TMP_InputField seedInput = null;

    [SerializeField] GameObject selectedSpeciesPanel = null;

    [SerializeField] Transform selectorTransform = null;
    [SerializeField] SpriteRenderer selectorRenderer = null;

    [SerializeField] TMP_InputField speciesRenameInput = null;
    [SerializeField] TMP_Text speciesNameReadout = null;

    [SerializeField] TMP_Text mouseModeReadout = null;

    GameManagerScript gameManager;

    bool simulationRunning = true;

    Species selectedSpecies = null;
    CellObjectScript selectedCellObjectScript = null;

    MOUSE_MODE mouseMode = MOUSE_MODE.NONE;

    public void AssignGameManager(GameManagerScript gameManager)
    {
        this.gameManager = gameManager;
        seedInput.text = gameManager.GetCurrentSeed().ToString();
    }

    private void Awake()
    {
        UpdateMouseModeReadout();
    }

    void SelectSpecies(CellObjectScript cellObjectScript)
    {
        if(cellObjectScript == null) { return; }

        selectedCellObjectScript = cellObjectScript;
        selectedSpeciesPanel.SetActive(true);

        selectorTransform.position = cellObjectScript.transform.position;

        selectorRenderer.enabled = true;

        Species newSpecies = cellManager.GetSpecies(cellObjectScript.GetCoords());

        if(newSpecies == null)
        {
            SetSelectedSpeciesReadoutToNone();
        }
        else
        {
            speciesNameReadout.text = newSpecies.name;
        }        

        mouseMode = MOUSE_MODE.CELL_SELECTED;
        UpdateMouseModeReadout();
    }

    void DeselectCell()
    {
        Debug.Log("Deselect");

        selectedSpeciesPanel.SetActive(false);
        selectorRenderer.enabled = false;

        DeselectSpecies();
    }

    void DeselectSpecies()
    {
        if(selectedSpecies == null) { return; }        

        selectedCellObjectScript = null;
        

        selectedSpecies = null;

        SetSelectedSpeciesReadoutToNone();
    }

    void SetSelectedSpeciesReadoutToNone()
    {
        speciesNameReadout.text = "None Selected";
    }

    public void ClearMouseMode()
    {
        mouseMode = MOUSE_MODE.NONE;
        UpdateMouseModeReadout();
    }

    public void MouseModeCopy()
    {
        mouseMode = MOUSE_MODE.CLONE_HERE;
        UpdateMouseModeReadout();
    }

    public void MouseModeDelete()
    {
        mouseMode = MOUSE_MODE.DELETE;
        UpdateMouseModeReadout();
    }

    void UpdateMouseModeReadout()
    {
        mouseModeReadout.text = mouseMode.ToString();
    }

    public void RenameSpeciesButton()
    {
        speciesRenameInput.enabled = !speciesRenameInput.enabled;
    }

    public void RenameSpecies()
    {
        string newName = speciesRenameInput.text;
        selectedSpecies.name = newName;
        speciesNameReadout.text = newName;        
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
        UpdateSelectedCell();
        KeyboardControls();
        MouseControls();
    }

    bool SCpreviouslyAlive = false;
    Species SCpreviousSpecies = null;

    void UpdateSelectedCell()
    {
        if(selectedCellObjectScript == null) { return; }
        Coords selectedCoords = selectedCellObjectScript.GetCoords();

        bool isAlive = cellManager.IsAlive(selectedCoords);
        Species currentSpecies = cellManager.GetSpecies(selectedCoords);

        if(!SCpreviouslyAlive)
        {
            if(isAlive)
            {
                SelectSpecies(selectedCellObjectScript);
            }
        }
        else//If it WAS alive last step
        {
            if(!isAlive)
            {
                SetSelectedSpeciesReadoutToNone();
            }
            else if(currentSpecies != SCpreviousSpecies)
            {
                SelectSpecies(selectedCellObjectScript);
            }
        }

        SCpreviouslyAlive = isAlive;
        SCpreviousSpecies = currentSpecies;
    }

    Vector2 mouseDownPosition;//Point where the mouse was clicked. Used to calculate dragging.
    bool mouseHeld = false;

    Vector2 oldMousePosition;

    void MouseControls()
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
            if(mouseMode == MOUSE_MODE.CELL_SELECTED)
            {
                DeselectCell();
            }

            ClearMouseMode();

            mouseDownPosition = theCamera.ScreenToWorldPoint(Input.mousePosition);
            oldMousePosition = Input.mousePosition;

            mouseHeld = true;
        }
        else if(Input.GetMouseButtonDown(0))
        {
            ParseClickColliders();
        }
    }

    void ParseClickColliders()
    {
        Collider2D[] foundColliders = Physics2D.OverlapPointAll(theCamera.ScreenToWorldPoint(Input.mousePosition));

        if(foundColliders.Length < 1)
        {
            if(mouseMode == MOUSE_MODE.CELL_SELECTED)
            {
                DeselectCell();
            }            
            return;
        }

        foreach(Collider2D collider in foundColliders)
        {
            CellObjectScript cellScript = collider.gameObject.GetComponent<CellObjectScript>();

            if(cellScript == null) { continue; }

            switch(mouseMode)
            {
                case MOUSE_MODE.NONE:
                    SelectSpecies(cellScript);
                    break;
                case MOUSE_MODE.CELL_SELECTED:
                    SelectSpecies(cellScript);
                    break;
                case MOUSE_MODE.CLONE_HERE:
                    Clone(cellScript.GetCoords());
                    break;
                case MOUSE_MODE.DELETE:
                    Delete(cellScript.GetCoords());
                    break;
            }            
        }
    }

    void Clone(Coords destination)
    {
        cellManager.CopyCellStateOntoNewCell(selectedCellObjectScript.GetCoords(), destination);
    }

    void Delete(Coords coords)
    {
        cellManager.SetAlive(coords, false);
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
