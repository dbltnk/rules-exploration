# rules-exploration or Dr. Conny's Lab?

Build: https://dbltnk.itch.io/connie-weis-lab

## TODO

* modes
** tutorial / intro
*** explain birth/survive(death) basics	
*** start with conway's life, the  introduce alternatives
** challenge: create a state
** detective: what are the rules?
*** input for "what the rules are" mode (just leave number slots for birth, survive & then border behaviour)
** free play

## Ideas

* wrapping as an alternative to borders
* multiple layers 
** independent by default
** option: cells above/below effect individuals
** option: aggregate stats from one layer affect all individuals in another layer
** top: mammals/birds/fish, middle: insects/krill, bottom: bacteria/algae/slime/plants
* more states & stats per cell than just dead/alive
* moving creatures
* Themes
** cells in petri dish
** life on a foreign planet
* Sharing for solving a task cooperatively
** Hash of species (rules config)
** Hash of state (species + seed + density + cells)
* more complex patterns?
** specific neighbours
** general stats of my own species (die if more than X others anywhere)
** chance-based
* do we need animations to figure out what has had influence on what?

## Refactoring

* make it start up super fast (became really slow with the addition of the charts)
* UI should be visible in edit mode

* use some reasonably stable unity version
* remove unnecessary assets/code (-> make shit open quickly, less importing)
* get rid of sample code (procgen toolkit, chart tool) & their potential copyright issues
* make the ideas above easily possible for a novice coder like me

* support drag & drop movement of tiles
* support zoom & drag-move of camera
* support animations, shaders, etc

## Potential goals

* [maximize / minimize] number of cells for species [X] by only making [n] changes in [m] steps (how to prevent bruteforce solutions)?
* create [a specific / any] [stable / oscillating / moving] structures (https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#Examples_of_patterns)
* identify the [birth / survival] rules for species [X]
* and then just a chill experimentation mode where you set rules and see what you can get to emerge
