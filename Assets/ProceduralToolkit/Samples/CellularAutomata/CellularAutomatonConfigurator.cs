using System;
using System.Collections.Generic;
using ProceduralToolkit.Samples.UI;
using UnityEngine;
using UnityEngine.UI;

namespace ProceduralToolkit.Samples
{
    /// <summary>
    /// A demonstration of CellularAutomaton from the main library, draws the automaton simulation on a texture.
    /// Note that some of the rulesets need noise value different from the default setting.
    /// </summary>
    public class CellularAutomatonConfigurator : ConfiguratorBase
    {
        public GameObject PrefCell;
        public RectTransform leftPanel;
        public ToggleGroup toggleGroup;
        public RawImage image;
        [Space]
        public CellularAutomaton.Config config = new CellularAutomaton.Config();

        private enum RulesetName
        {
            Life,
            Mazectric,
            Coral,
            WalledCities,
            Coagulations,
            Anneal,
            Majority,
            Custom
        }

        private Color[] pixels;
        private Texture2D texture;
        public CellularAutomaton automaton;
        public Color deadColor;
        public Color aliveColor;
        private TextControl header;
        public bool IsPlaying = true;
        private float stepsPerSecond = 2f;
        private float lastStep = 0f;
        public int StepCount = 0;
        private int customBirthRule = 0;
        private int customSurvivalRule = 0;
        private int maxCustomBirthRule = 9;
        private int maxCustomSurvivalRule = 9;
        private int seedMin = 0;
        private int seedMax = 100;
        public bool dirty = false;

        private Dictionary<RulesetName, CellularAutomaton.Ruleset> nameToRuleset = new Dictionary<RulesetName, CellularAutomaton.Ruleset>
        {
            {RulesetName.Life, CellularAutomaton.Ruleset.life},
            {RulesetName.Mazectric, CellularAutomaton.Ruleset.mazectric},
            {RulesetName.Coral, CellularAutomaton.Ruleset.coral},
            {RulesetName.WalledCities, CellularAutomaton.Ruleset.walledCities},
            {RulesetName.Coagulations, CellularAutomaton.Ruleset.coagulations},
            {RulesetName.Anneal, CellularAutomaton.Ruleset.anneal},
            {RulesetName.Majority, CellularAutomaton.Ruleset.majority},
        };

        private void Awake()
        {
            pixels = new Color[config.width*config.height];
            texture = PTUtils.CreateTexture(config.width, config.height, Color.clear);
            image.texture = texture;

            header = InstantiateControl<TextControl>(leftPanel);
            header.transform.SetAsFirstSibling();

            //RandomizeRules();
            config.seed = UnityEngine.Random.Range(seedMin, seedMax);

            var currentRulesetName = RulesetName.Custom;
            SelectRuleset(currentRulesetName);

            InstantiateToggle(RulesetName.Life, currentRulesetName);
            InstantiateToggle(RulesetName.Mazectric, currentRulesetName);
            InstantiateToggle(RulesetName.Coral, currentRulesetName);
            InstantiateToggle(RulesetName.WalledCities, currentRulesetName);
            InstantiateToggle(RulesetName.Coagulations, currentRulesetName);
            InstantiateToggle(RulesetName.Anneal, currentRulesetName);
            InstantiateToggle(RulesetName.Majority, currentRulesetName);
            InstantiateToggle(RulesetName.Custom, currentRulesetName);

            InstantiateControl<ButtonControl>(leftPanel).Initialize("Randomize rules", RandomizeRules);

            InstantiateControl<SliderControl>(leftPanel).Initialize("Birth rule", 0, maxCustomBirthRule, config.seed, value => {
                customBirthRule = Mathf.FloorToInt(value);
                currentRulesetName = RulesetName.Custom;
                SelectRuleset(RulesetName.Custom);
                GameObject.Find("Canvas").GetComponentInChildren<ToggleGroup>().SetAllTogglesOff();
                GameObject.Find("Custom").GetComponentInChildren<Toggle>().isOn = true;
                Generate();
            });

            InstantiateControl<SliderControl>(leftPanel).Initialize("Survive rule", 0, maxCustomSurvivalRule, config.seed, value => {
                customSurvivalRule = Mathf.FloorToInt(value);
                currentRulesetName = RulesetName.Custom;
                SelectRuleset(RulesetName.Custom);
                GameObject.Find("Canvas").GetComponentInChildren<ToggleGroup>().SetAllTogglesOff();
                GameObject.Find("Custom").GetComponentInChildren<Toggle>().isOn = true;
                Generate();
            });

            InstantiateControl<SliderControl>(leftPanel).Initialize("Seed", seedMin, seedMax, config.seed, value => {
                config.seed = Mathf.FloorToInt(value);
                Generate();
            });

            InstantiateControl<SliderControl>(leftPanel).Initialize("Start noise", 0, 1, config.startNoise, value =>
            {
                config.startNoise = value;
                Generate();
            });

            InstantiateControl<ToggleControl>(leftPanel).Initialize("Alive borders", config.aliveBorders, value =>
            {
                config.aliveBorders = value;
                Generate();
            });

            InstantiateControl<ButtonControl>(leftPanel).Initialize("Generate/Reset", Generate);
            InstantiateControl<SliderControl>(leftPanel).Initialize("Steps per second", 0, 100, stepsPerSecond, value => {
                stepsPerSecond = value;
            });
            InstantiateControl<ButtonControl>(leftPanel).Initialize("Play/Pause", PlayPause);
            InstantiateControl<ButtonControl>(leftPanel).Initialize("Step", Step);

            Generate();
            SetupSkyboxAndPalette();
        }

        private void RandomizeRules () {
            // TODO Figure out why this always returns the same values.
            customBirthRule = UnityEngine.Random.Range(0, maxCustomBirthRule);
            customSurvivalRule = UnityEngine.Random.Range(0, maxCustomSurvivalRule);
            SelectRuleset(RulesetName.Custom);
            if (GameObject.Find("Canvas") != null) GameObject.Find("Canvas").GetComponentInChildren<ToggleGroup>().SetAllTogglesOff();
            if (GameObject.Find("Custom") != null) GameObject.Find("Custom").GetComponentInChildren<Toggle>().isOn = true;
            Generate();
        }

        private void Update()
        {
            DrawCells();
            UpdateSkybox();
            if (IsPlaying) {
                float timeSinceLastStep = Time.time - lastStep;
                if (timeSinceLastStep >= 1f / stepsPerSecond) {
                    Step();
                    lastStep = Time.time;
                }
            }
        }


        private void SelectRuleset(RulesetName rulesetName)
        {
            if (rulesetName == RulesetName.Custom) {
                config.ruleset = new CellularAutomaton.Ruleset(customBirthRule.ToString(), customSurvivalRule.ToString());
            }
            else {
                config.ruleset = nameToRuleset[rulesetName];
            }
            header.Initialize("Rulestring: " + config.ruleset);
        }

        private void Generate()
        {
            automaton = new CellularAutomaton(config);

            GeneratePalette();

            deadColor = GetMainColorHSV().WithSV(0.3f, 0.2f).ToColor();
            aliveColor = GetMainColorHSV().ToColor();

            Step();
        }

        private void PlayPause () {
            IsPlaying = !IsPlaying;
        }

        private void Step () {
            automaton.Simulate();
            StepCount++;
        }

        private void DrawCells()
        {
            for (int x = 0; x < config.width; x++)
            {
                for (int y = 0; y < config.height; y++)
                {
                    int cellsNeeded = config.width * config.height;
                    int cellCount = image.transform.childCount;
                    if (cellCount < cellsNeeded) {
                        GameObject cell = Instantiate(PrefCell);
                        cell.transform.SetParent(image.transform, false);
                        Cell c = cell.AddComponent<Cell>();
                        c.X = x;
                        c.Y = y;
                        c.counter = y * config.width + x;
                    }

                    if (automaton.cells[x, y])
                    {
                        pixels[y*config.width + x] = aliveColor;
                        if (cellCount >= cellsNeeded) image.transform.GetComponentsInChildren<Image>()[y * config.width + x].color = aliveColor;
                    }
                    else
                    {
                        pixels[y*config.width + x] = deadColor;
                        if (cellCount >= cellsNeeded) image.transform.GetComponentsInChildren<Image>()[y * config.width + x].color = deadColor;
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
        }

        private void InstantiateToggle(RulesetName rulesetName, RulesetName selectedRulesetName)
        {
            var toggle = InstantiateControl<ToggleControl>(toggleGroup.transform);
            toggle.Initialize(
                header: rulesetName.ToString(),
                value: rulesetName == selectedRulesetName,
                onValueChanged: isOn =>
                {
                    if (isOn)
                    {
                        SelectRuleset(rulesetName);
                        Generate();
                    }
                },
                toggleGroup: toggleGroup);
        }

        public void FlipCell (int x, int y) {
            // no idea why I am flipping these =D 
            automaton.cells[y, x] = !automaton.cells[y, x];
            dirty = true;
        }
    }
}
