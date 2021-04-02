# Dr. Conny's Lab

Build: https://dbltnk.itch.io/connie-weis-lab







## Issues in New Version
**P0-1**: Needs a fix by Tony ASAP
**P2-3:** Alex will deal with this in the future (or just ignore it)

* **P0** - Adding new Levels + Rules takes a lot of clicks since you have to manually add them to the Level/RuleBanks. Can the banks be auto-generated from the scriptable objects in certain folders? Also adding a new rule to the RuleBank triggers the following errors:
  * IndexOutOfRangeException: Index was outside the bounds of the array.
    SpeciesBank.DeserializeSpecies (System.String defaultName, System.Int32[] speciesGroups, System.Single[] color, System.Int32 startingPopulation, System.Int32 birthRuleIndex, System.Int32 deathRuleIndex) (at Assets/Simple Attempt/Simple Scene Scripts/SpeciesBank.cs:156)
    SpeciesBank.InitializeSavedSpecies (SaveData saveData) (at Assets/Simple Attempt/Simple Scene Scripts/SpeciesBank.cs:55)
    GameManagerScript.Awake () (at Assets/Simple Attempt/Simple Scene Scripts/GameManagerScript.cs:39)
    UnityEngine.Object:Instantiate(GameObject)
    LevelSetupScript:Awake() (at Assets/Simple Attempt/Simple Scene Scripts/LevelSetupScript.cs:29)
  * NullReferenceException: Object reference not set to an instance of an object
    SpeciesBank.AddSpecies (Species[] speciesArray) (at Assets/Simple Attempt/Simple Scene Scripts/SpeciesBank.cs:187)
    CellManagerScript.AssignLevel (Level level, GameManagerScript gameManager, SpeciesBank speciesBank) (at Assets/Simple Attempt/Simple Scene Scripts/CellManagerScript.cs:104)
    GridManagerScript.AssignLevel (Level level) (at Assets/Simple Attempt/Simple Scene Scripts/GridManagerScript.cs:69)
    GridManagerScript.Awake () (at Assets/Simple Attempt/Simple Scene Scripts/GridManagerScript.cs:64)

* **P0 -** Can no longer create species within a pre-defined range, ProcGen only picks from manually created species (can be fixed by adding birth/death rules that randomly pick between ComparyInts.X and .Y)
* **P1** - Grid zoom should automatically fit the screen when a level is loaded
* **P?** - Should BorderType be in the level or in the species?
* **P? -** Can no longer edit a species in play mode and/or see its rules (-> show when a cell is selected in the grid?)



* **P2 -** "Drawing" cells is less comfortable now (but more flexible) 
* **P2 -** Cannot use different sprites for species anymore (but can be added easily)
* **P2 -** Steps per second slider: You often select the Seed text box when using the slider (collider too big?)
* **P3 -** Dish background image is missing
* **P3 -** Typing into the "Theory" text box still triggers hotkeys like "Fill Dish"




## Refactoring

* P1 focus on prototyping iteration speed over everything else
* P1 make the ideas above easily accessible to a novice coder like me
* P1 make it start up super fast (got a bit slower with the addition of the charts)
* P1 remove unnecessary assets/code (less importing)
* P1 a nicely editable and extendable data format for the species & level setups (level =  species config, initial game & potentially grid state)
* P1 all code accessible from anywhere (cannot currently access new scripts from procgen toolkit files)
* P1 most UI should always visible in edit mode, even when not playing
* P2 support different grid sizes
* P2 support zoom & camera movement (for interaction with larger grids)
* P3 support drag & drop movement of tiles (https://ncase.me/polygons/)



## Immediate Next Steps / WiP

* P1 List of known species (click them to load a scene with that species configuration, random grid)
* P2 "knowledge graph" (DAG) of levels that unlock one after another, clicking opens a scene with that species configuration, goal definition (& maybe grid state) 
  * P2 in addition to the "challenge" levels you also unlock "classification" levels where similar CA rules are being used but with a random config
* P3 Cap for multi-digit random rule selection to not go beyond useful (Moor neigh. = max 4 etc)



## P1 Game Modes

* P1 CHALLENGE: create a state
  * P1 fixed list of challenges / puzzles to pick from
  * P1 [maximize / minimize] number of cells for species [X] by only making [n] changes in [m] steps (how to prevent brute force solutions)?
  * minimize works w/ mazentric
  * P1 create [a specific / any] [stable / oscillating / moving / replcating] structures (https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#Examples_of_patterns // https://conwaylife.com/wiki/Cellular_automaton)
  * P1 Make [N] mutations to a species to get to a specific result
    * https://codegolf.stackexchange.com/questions/221091/minimally-destroy-cgcc-in-game-of-life
* P1 IDENTIFICATION
  * Mark a pattern (mouse selection) and store it under a name
* P1 CLASSIFICATION: Class 1-4
  * Compare with known classifications from DB (?) or scripts (?)
  * "Citizen science": Send your results to the global DB, get more credibility for more thorough checking.
  * Class 1: All on or off after N steps?
  * Class 2: Mark repeating pattern
  * Class 3: Define chaos by entropy metric?
  * Class 4: Class 2+3 behaviours need to be shown?
  * Graphs useful here?
  * Major tool:  (AI-assisted) pattern matching, highlighted by player!
* P1 DETECTIVE: what are the rules?
  * difficulty scaling: increase digits for rules
  * needs a history of random species, otherwise naming is useless
  * identify the [birth / survival] rules for species [X]
  * input for "what the rules are" mode (just leave number slots for birth, survive & then border behavior)
  * Submit paper for peer review
  * If wrong, peer reviewers (or the doc) reveal one part of the solution
  * Score = fewer guesses
* P2 tutorial / intro
  * explain birth/survive(death) basics	
  * start with Conway's life, the  introduce alternatives
* P2 free play mode
  * just a chill experimentation mode where you set rules and see what you can get to emerge



## Progression

1. Neumann neighborhood (cell + 4 neighbours)
   * Basic border types
     * Alive
     *  Dead
   * Modify min/max rules etc
2. Moore neighborhood (cell + 8 neighbours)
   1. Game of Life
   2. https://en.wikipedia.org/wiki/Seeds_(cellular_automaton)
3. 3+ states = "multi species"
  * Rock / paper / scissors: https://youtu.be/TvZI6Xc0J1Y
  * https://en.wikipedia.org/wiki/Brian%27s_Brain
  * Multiple organisms in one layer (food?)
4. Probabilistic transitions
5. Non-totalistic CA (https://conwaylife.com/wiki/Non-totalistic_Life-like_cellular_automaton)
   1. Non-isotropic https://conwaylife.com/wiki/Non-isotropic_Life-like_cellular_automaton
   2. Isotropic https://conwaylife.com/wiki/Isotropic_non-totalistic_Life-like_cellular_automaton
6. Continuous state
7. Advanced border types
   * Wrapping (Torus, Moebius strip)
   * Custom
8. Reversible CA
   * Second-order CA
   * Block CA https://en.wikipedia.org/wiki/Block_cellular_automaton
* Multiple layers
* Rules based on layer / species analytics (count of species X etc)
* Lists of known CA:
  * https://conwaylife.com/wiki/List_of_Life-like_cellular_automata
  * https://softology.com.au/voc.htm
  * https://catagolue.hatsya.com/rules
* Generations? https://conwaylife.com/wiki/Generations



## Other Ideas

* P1 Multiple species in one layer

* P1 more states & stats per cell than just dead/alive

* P1 aggregate stats

* P1 more complex rules
    * P2 specific neighbors ("hibernate if near a floop")
    * P2 general stats of my own species (die if more than X others anywhere)
    * P3 chance-based state change
    
* P2 multiple layers 
    * independent by default
    * option: cells above/below effect individuals
    * option: aggregate stats from one layer affect all individuals in another layer
    * top: mammals/birds/fish, middle: insects/krill, bottom: bacteria/algae/slime/plants
    
* P2 Sharing for solving a task cooperatively
    * P2 Hash of species (rules config)
    * P3 Hash of state (species + seed + density + cells)
    
* P2 Mouse wheel to magnify

* P3 Nested layers (zoom into a cell to see another layer inside)

* P3 moving creatures

* P3 wrapping as an alternative to borders

* P3 do we need animations to figure out what has had influence on what?

* P3 Load nickname presets for the known species

* P3 Different types of creatures (movement?)

* P3 Other grids (https://conwaylife.com/wiki/Cellular_automaton)

    * Hex
    * Triangle

    

## Usability Improvements

* Pattern stamps (1-9 blocks)
* Visualize more cell properties in sprite (walls alive/dead)



## Potential Collaborators

* Lior Ben-Gai (London):  https://www.youtube.com/watch?v=coL5GKxYg90



## Games to try

* https://games.increpare.com/Gestalt_OS/
* Zachtronics++
