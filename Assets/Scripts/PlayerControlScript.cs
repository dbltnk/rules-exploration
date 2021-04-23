using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public enum MOUSE_MODE
{
    NONE,
    CELL_SELECTED,
    CLONE,
    DELETE,
    DRAW
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

    [SerializeField] GameObject speciesRenameGameObject = null;
    [SerializeField] TMP_InputField speciesRenameInput = null;
    [SerializeField] TMP_Text speciesNameReadout = null;
    [SerializeField] TMP_Text cellStateReadout = null;
    [SerializeField] TMP_Text mouseModeReadout = null;

    [SerializeField] TMP_Dropdown dropdownDrawSpecies = null;
    [SerializeField] TMP_Dropdown dropdownDrawState = null;
    [SerializeField] TMP_Dropdown dropdownDrawAlive = null;

    GameManagerScript gameManager;
    GridManagerScript gridManager;
    SpeciesBank speciesBank;

    public bool simulationRunning = true;

    bool speciesRenameFieldOpen = false;

    Species selectedSpecies = null;
    CellObjectScript selectedCellObjectScript = null;

    MOUSE_MODE mouseMode = MOUSE_MODE.NONE;

    public void AssignGameManager(GameManagerScript gameManager)
    {
        this.gameManager = gameManager;
        seedInput.text = gameManager.GetCurrentSeed().ToString();
        speciesBank = gameManager.GetSpeciesBank();
        gridManager = gameManager.GetComponent<GridManagerScript>();

        List<string> speciesNames = new List<string>();
        foreach (Species s in cellManager.enabledSpecies) speciesNames.Add(s.defaultName);
        dropdownDrawSpecies.AddOptions(speciesNames);
        var states = System.Enum.GetValues(typeof (STATE));
        List<string> statesList = new List<string>();
        foreach (STATE s in states) {
            statesList.Add(s.ToString());
        }
        dropdownDrawState.AddOptions(statesList);
        dropdownDrawAlive.AddOptions(new List<string>() { "alive", "dead" });
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

        selectedSpecies = cellManager.GetSpecies(cellObjectScript.GetCoords());

        //if(!cellManager.IsAlive(cellObjectScript.GetCoords())) { selectedSpecies = null; }

        if(selectedSpecies == null)
        {
            SetSelectedSpeciesReadoutToNone();
            cellStateReadout.text = "";
        }
        else
        {
            speciesNameReadout.text = speciesBank.GetSpeciesName(selectedSpecies);
            cellStateReadout.text = cellManager.GetCellStateAtCoords(cellObjectScript.GetCoords()).ToString();
        }


        mouseMode = MOUSE_MODE.CELL_SELECTED;
        UpdateMouseModeReadout();
    }

    void DeselectCell()
    {
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
        if(mouseMode == MOUSE_MODE.CELL_SELECTED ||
                mouseMode == MOUSE_MODE.NONE)
        {
            DeselectCell();
            mouseMode = MOUSE_MODE.NONE;
        }
        else
        {
            mouseMode = MOUSE_MODE.CELL_SELECTED;
        }
        
        UpdateMouseModeReadout();
    }

    public void MouseModeCopy()
    {
        mouseMode = MOUSE_MODE.CLONE;
        UpdateMouseModeReadout();
    }

    public void MouseModeDelete()
    {
        mouseMode = MOUSE_MODE.DELETE;
        UpdateMouseModeReadout();
    }

    public void MouseModeDraw () {
        mouseMode = MOUSE_MODE.DRAW;
        UpdateMouseModeReadout();
    }

    void UpdateMouseModeReadout()
    {
        mouseModeReadout.text = mouseMode.ToString();
    }

    public void RenameSpeciesButton()
    {
        speciesRenameGameObject.SetActive(!speciesRenameGameObject.activeSelf);
        speciesRenameFieldOpen = !speciesRenameFieldOpen;

        if(speciesRenameGameObject.activeSelf)
        {
            speciesRenameInput.text = speciesBank.GetSpeciesName(selectedSpecies);
        }
        else
        {
            gameManager.SaveGame();
        }
    }

    public void RenameSpecies()
    {
        string newName = speciesRenameInput.text;
        speciesBank.SetSpeciesName(selectedSpecies, newName);        
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
    public void ResetExperiment()
    {
        Random.InitState(gameManager.GetCurrentSeed());
        CallStopSimulating();
        cellManager.ReInitializeAllCells();

        //gameManager.LoadScene(SCENE.PLAY_SCREEN);
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
        float stepsPerSecond = Mathf.Pow(2, simulationSpeedSlider.value - 1);

        simulationSpeedReadout.SetText(string.Format("Steps per Second: {0}", stepsPerSecond));
        cellManager.SetSimulationSpeed(stepsPerSecond);
    }

    public void BackToMenu()
    {
        gameManager.LoadScene(SCENE.LEVEL_SETUP);
        gameManager.SpeciesFromCollection = null;
    }

    public void GoToSpeciesCollection () {
        gameManager.LoadScene(SCENE.SPECIES_COLLECTION);
        gameManager.SpeciesFromCollection = null;
    }

    public void NewExperiment()
    {
        Random.InitState(gameManager.GetCurrentSeed());
        gameManager.SetCurrentLevel(null);//Trying to load a null level will force a random roll.
        gameManager.LoadScene(SCENE.PLAY_SCREEN);
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
                //SetSelectedSpeciesReadoutToNone();
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
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;            
        }

        Collider2D[] foundColliders = Physics2D.OverlapPointAll(theCamera.ScreenToWorldPoint(Input.mousePosition));

        if(foundColliders.Length < 1)
        {
            if(mouseMode == MOUSE_MODE.CELL_SELECTED)
            {
                DeselectCell();
            }            
            return;
        }

        CellObjectScript cellScriptToSelect = null;

        foreach(Collider2D collider in foundColliders)
        {
            CellObjectScript newCellScript = collider.gameObject.GetComponent<CellObjectScript>();

            if(newCellScript == null) { continue; }

            cellScriptToSelect = newCellScript;
        }

        if(cellScriptToSelect == null) { return; }

        switch(mouseMode)
        {
            case MOUSE_MODE.NONE:
                SelectSpecies(cellScriptToSelect);
                break;
            case MOUSE_MODE.CELL_SELECTED:
                SelectSpecies(cellScriptToSelect);
                break;
            case MOUSE_MODE.CLONE:
                Clone(cellScriptToSelect.GetCoords());
                break;
            case MOUSE_MODE.DELETE:
                Delete(cellScriptToSelect.GetCoords());
                break;
            case MOUSE_MODE.DRAW:
                Draw(cellScriptToSelect.GetCoords());
                break;
        }
    }

    void Clone(Coords destination)
    {
        if (selectedCellObjectScript) cellManager.CopyCellStateOntoNewCell(selectedCellObjectScript.GetCoords(), destination);
    }

    void Draw(Coords destination) {
        //Species species = gameManager.GetSpeciesBank().speciesBank[0];
        Species species = cellManager.enabledSpecies[dropdownDrawSpecies.value];
        STATE state = (STATE)dropdownDrawState.value;
        bool alive = dropdownDrawAlive.value == 0 ? true : false;
        cellManager.SetSpeciesAndStateOnCell(destination, species, state, alive);
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

        if(speciesRenameFieldOpen)
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
            ResetExperiment();
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
