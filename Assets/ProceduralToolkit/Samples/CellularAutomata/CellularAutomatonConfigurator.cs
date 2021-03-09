using ProceduralToolkit.Samples.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ProceduralToolkit.Samples
{
    /// <summary>
    /// A demonstration of CellularAutomaton from the main library, draws the automaton simulation on a texture.
    /// Note that some of the rulesets need noise value different from the default setting.
    /// </summary>
    /// 

    public class HideMe : MonoBehaviour
    {

    }

    public class CellularAutomatonConfigurator : ConfiguratorBase
    {
        public GameObject PrefCell;
        public GameObject PrefConnieWei;
        public RectTransform RulesPopup;
        public RectTransform leftPanel;
        public RectTransform rightPanel;
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
        private int maxCustomBirthRule = 99999;
        private int maxCustomSurvivalRule = 99999;
        private int seedMin = 0;
        private int seedMax = 100;
        public bool dirty = false;
        private bool confirmNewGame = false;

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
            GeneratePalette();

            pixels = new Color[config.width*config.height];
            texture = PTUtils.CreateTexture(config.width, config.height, Color.clear);
            image.texture = texture;

            header = InstantiateControl<TextControl>(RulesPopup);
            header.transform.SetAsFirstSibling();
            header.gameObject.AddComponent<HideMe>();

            var goal = InstantiateControl<TextControl>(leftPanel);
            goal.transform.SetAsFirstSibling();
            goal.headerText.text = "<b>Dr. Connie Wei:</b> <i>\"Can you find out what rules these miraculous little creatures live by?\"</i>";

            var connie = Instantiate(PrefConnieWei);
            connie.transform.SetParent(leftPanel);
            connie.transform.SetAsFirstSibling();

            var epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
            var timestamp = (System.DateTime.UtcNow - epochStart).Milliseconds;
            RandomizeRules();
            UnityEngine.Random.InitState(timestamp);
            config.seed = UnityEngine.Random.Range(seedMin, seedMax);
            int r = UnityEngine.Random.Range(0, 8);
            var currentRulesetName = (RulesetName)r;
            //var currentRulesetName = RulesetName.Custom;
            SelectRuleset(currentRulesetName);

            InstantiateToggle(RulesetName.Life, currentRulesetName).AddComponent<HideMe>().transform.SetParent(RulesPopup);
            InstantiateToggle(RulesetName.Mazectric, currentRulesetName).AddComponent<HideMe>().transform.SetParent(RulesPopup);
            InstantiateToggle(RulesetName.Coral, currentRulesetName).AddComponent<HideMe>().transform.SetParent(RulesPopup);
            InstantiateToggle(RulesetName.WalledCities, currentRulesetName).AddComponent<HideMe>().transform.SetParent(RulesPopup);
            InstantiateToggle(RulesetName.Coagulations, currentRulesetName).AddComponent<HideMe>().transform.SetParent(RulesPopup);
            InstantiateToggle(RulesetName.Anneal, currentRulesetName).AddComponent<HideMe>().transform.SetParent(RulesPopup);
            InstantiateToggle(RulesetName.Majority, currentRulesetName).AddComponent<HideMe>().transform.SetParent(RulesPopup);
            InstantiateToggle(RulesetName.Custom, currentRulesetName).AddComponent<HideMe>().transform.SetParent(RulesPopup);

            var randomizeRulesControl = InstantiateControl<ButtonControl>(RulesPopup);
            randomizeRulesControl.gameObject.AddComponent<HideMe>();
            randomizeRulesControl.Initialize("Randomize rules", RandomizeRules);

            SliderControl birthControl = InstantiateControl<SliderControl>(RulesPopup);
            birthControl.Initialize("Birth rule", 0, maxCustomBirthRule, config.seed, value => {
                customBirthRule = Mathf.FloorToInt(value);
                currentRulesetName = RulesetName.Custom;
                SelectRuleset(RulesetName.Custom);
                GameObject.Find("Canvas").GetComponentInChildren<ToggleGroup>().SetAllTogglesOff();
                GameObject.Find("Custom").GetComponentInChildren<Toggle>().isOn = true;
                Generate();
            });
            birthControl.gameObject.AddComponent<HideMe>();

            SliderControl survivalControl = InstantiateControl<SliderControl>(RulesPopup);
            survivalControl.Initialize("Survive rule", 0, maxCustomSurvivalRule, config.seed, value => {
                customSurvivalRule = Mathf.FloorToInt(value);
                currentRulesetName = RulesetName.Custom;
                SelectRuleset(RulesetName.Custom);
                GameObject.Find("Canvas").GetComponentInChildren<ToggleGroup>().SetAllTogglesOff();
                GameObject.Find("Custom").GetComponentInChildren<Toggle>().isOn = true;
                Generate();
            });
            survivalControl.gameObject.AddComponent<HideMe>();

            if (UnityEngine.Random.Range(0f, 1f) < 0.5f) { 
                config.aliveBorders = true; 
            }
            else {
                config.aliveBorders = false;
            }
            var aliveBordersControl = InstantiateControl<ToggleControl>(RulesPopup);
            aliveBordersControl.gameObject.AddComponent<HideMe>();
            aliveBordersControl.Initialize("Alive borders", config.aliveBorders, value => {
                config.aliveBorders = value;
                Generate();
            });

            InstantiateControl<ButtonControl>(leftPanel).Initialize("Start / pause", PlayPause);
            InstantiateControl<ButtonControl>(leftPanel).Initialize("Next step", Step);
            InstantiateControl<ButtonControl>(leftPanel).Initialize("Reset experiment", Generate);
            InstantiateControl<ButtonControl>(leftPanel).Initialize("Clear dish", Clear);
            InstantiateControl<ButtonControl>(leftPanel).Initialize("Fill dish", Fill);

            InstantiateControl<SliderControl>(leftPanel).Initialize("Seed value", seedMin, seedMax, config.seed, value => {
                config.seed = Mathf.FloorToInt(value);
                Generate();
            });

            InstantiateControl<SliderControl>(leftPanel).Initialize("Start noise", 0, 1, config.startNoise, value =>
            {
                config.startNoise = value;
                Generate();
            });

            InstantiateControl<SliderControl>(leftPanel).Initialize("Steps per second", 0, 100, stepsPerSecond, value => {
                stepsPerSecond = value;
            });

            InstantiateControl<ButtonControl>(leftPanel).Initialize("Peek at solution", SuggestSolution);
            InstantiateControl<ButtonControl>(RulesPopup).Initialize("Hide configuration", SuggestSolution);

            InstantiateControl<ButtonControl>(leftPanel).Initialize("New experiment", NewGame);

            var answer = InstantiateControl<TextControl>(rightPanel).headerText.text = "<b>My answer</b> for Dr. Connie is:";
            var answerBirth = InstantiateControl<TextBoxControl>(rightPanel);
            answerBirth.Initialize("<i>Birth rule: (e.g. 123)</i>");
            var answerSurvival = InstantiateControl<TextBoxControl>(rightPanel);
            answerSurvival.Initialize("<i>Survival rule: (e.g. 2578)</i>");
            var answerAliveBorder = InstantiateControl<ToggleControl>(rightPanel);
            answerAliveBorder.Initialize("Border Is Alive", false, value => {
                //config.aliveBorders = value;
            });
            InstantiateControl<ButtonControl>(rightPanel).Initialize("Submit your theory", Answer);

            Generate();
            SetupSkyboxAndPalette();
        }

        private void Answer () {
            //throw new System.NotImplementedException();
        }

        private void Fill () {
            for (int x = 0; x < config.width; x++) {
                for (int y = 0; y < config.height; y++) {
                    automaton.cells[y, x] = true;
                }
            }
        }

        private void Clear () {
            for (int x = 0; x < config.width; x++) {
                for (int y = 0; y < config.height; y++) {
                    automaton.cells[y, x] = false;
                }
            }
        }

        private void NewGame () {
            if (confirmNewGame == true) {
                Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
            }
            else {
                confirmNewGame = true;
                leftPanel.Find("New experiment").GetComponentInChildren<Text>().text = "Click to confirm!";
            }
        }

        private void SuggestSolution () {
            //HideMe[] elements = Resources.FindObjectsOfTypeAll<HideMe>();
            //foreach (HideMe e in elements) {
            //    e.gameObject.SetActive(!e.gameObject.activeSelf);
            //}

            leftPanel.gameObject.SetActive(!leftPanel.gameObject.activeSelf);
            RulesPopup.gameObject.SetActive(!RulesPopup.gameObject.activeSelf);
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

        private GameObject InstantiateToggle (RulesetName rulesetName, RulesetName selectedRulesetName)
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
            return toggle.gameObject;
        }

        public void FlipCell (int x, int y) {
            // no idea why I am flipping these =D 
            automaton.cells[y, x] = !automaton.cells[y, x];
            dirty = true;
        }

        public void ActivateCell (int x, int y) {
            // no idea why I am flipping these =D 
            automaton.cells[y, x] = true;
            dirty = true;
        }

        public void DeactivateCell (int x, int y) {
            // no idea why I am flipping these =D 
            automaton.cells[y, x] = false;
            dirty = true;
        }
    }
}
