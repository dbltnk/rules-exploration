# Dr. Conny's Lab?

Build: https://dbltnk.itch.io/connie-weis-lab

## TODO

### P1 species identifiability
* P1 hashed "scientific names"
* P2 user-editable nickname (with presets for the known ones)

### P1 Game Modes
* P1 menu to select the different modes
* P2 tutorial / intro
    * explain birth/survive(death) basics	
    * start with conway's life, the  introduce alternatives
* P1 challenge: create a state
    * P1 fixed list of challenges / puzzles to pick from
    * P1 [maximize / minimize] number of cells for species [X] by only making [n] changes in [m] steps (how to prevent bruteforce solutions)?
    * P1 create [a specific / any] [stable / oscillating / moving] structures (https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#Examples_of_patterns)
    * P1 Make [N] mutations to a species to get to a specific result
* P1 detective: what are the rules?
    * identify the [birth / survival] rules for species [X]
    * input for "what the rules are" mode (just leave number slots for birth, survive & then border behaviour)
    * Submit paper for peer review
    * If wrong, peer rewievers (or the doc) reveal one part of the solution
    * Score = fewer guesses
* P2 free play mode
    * just a chill experimentation mode where you set rules and see what you can get to emerge

### Refactoring

* P1 make the ideas above easily possible for a novice coder like me
* P1 make it start up super fast (became really slow with the addition of the charts)
* P1 remove unnecessary assets/code (-> make shit open quickly, less importing)
* P1 a nicely editable and extendable data format for the species & level setups
* P2 get rid of sample code (procgen toolkit, chart tool) & their potential copyright issues
* P2 all UI should always visible in edit mode, even when not playing
* P2 support drag & drop movement of tiles
* P2 support different grid sizes
* P2 support zoom & drag-move of camera
* P3 support animations, shaders, etc

### Other Ideas

* P3 wrapping as an alternative to borders
* P2 multiple layers 
    * independent by default
    * option: cells above/below effect individuals
    * option: aggregate stats from one layer affect all individuals in another layer
    * top: mammals/birds/fish, middle: insects/krill, bottom: bacteria/algae/slime/plants
* P1 more states & stats per cell than just dead/alive
* P3 moving creatures
* P2 Sharing for solving a task cooperatively
    * P2 Hash of species (rules config)
    * P3 Hash of state (species + seed + density + cells)
* P2 more complex rules?
    * P2 specific neighbours ("hibernate if near a floop")
    * P2 general stats of my own species (die if more than X others anywhere)
    * P3 chance-based
* P3 do we need animations to figure out what has had influence on what?
* P2 Complexity Progression
    * Fixed GoL, death/alive
    * Modify min/max rules etc
    * Multiple organisms in one layer (food?)
    * Multiple layers
    * Different types of creatures (movement?)

## Games to try

* https://games.increpare.com/Gestalt_OS/
* Zachtronics++
