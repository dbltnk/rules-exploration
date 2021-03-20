# Dr. Conny's Lab

Build: https://dbltnk.itch.io/connie-weis-lab

## Immediate Next Steps / WiP

* P1 List of known species (click them to load a scene with that species configuration, random grid)
* P2 "knowledge graph" (DAG) of levels that unlock one after another, clicking opens a scene with that species configuration, goal definition (& maybe grid state) 
  * P2 in addition to the "challenge" levels you also unlock "classification" levels where similar CA rules are being used but with a random config

### Refactoring

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

### P1 Game Modes

* P1 challenge: create a state
    * P1 fixed list of challenges / puzzles to pick from
    * P1 [maximize / minimize] number of cells for species [X] by only making [n] changes in [m] steps (how to prevent brute force solutions)?
    * P1 create [a specific / any] [stable / oscillating / moving] structures (https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#Examples_of_patterns)
    * P1 Make [N] mutations to a species to get to a specific result
* P1 CLASSIFICATION: what are the rules?
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

### Other Ideas

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
* P2 Complexity Progression / Cell types
    * Fixed GoL, death/alive
    * Modify min/max rules etc
    * Pattern-matching cells (specific neighbor structure)
    * Multiple organisms in one layer (food?)
    * Multiple layers
    * Different types of creatures (movement?)
* P3 Nested layers (zoom into a cell to see another layer inside)
* P3 moving creatures
* P3 wrapping as an alternative to borders
* P3 do we need animations to figure out what has had influence on what?
* P3 Load nickname presets for the known species

## Usability Improvements

* Pattern stamps (1-9 blocks)

## Games to try

* https://games.increpare.com/Gestalt_OS/
* Zachtronics++
