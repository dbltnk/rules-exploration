using ProceduralToolkit.Samples.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
using System;

namespace ProceduralToolkit.Samples
{
    /// <summary>
    /// A demonstration of CellularAutomaton from the main library, draws the automaton simulation on a texture.
    /// Note that some of the rulesets need noise value different from the default setting.
    /// </summary>
    /// 

    public class CellularAutomatonConfigurator : ConfiguratorBase
    {
        [Header("Game Mode")]
        public gameMode Mode = gameMode.RandomCustom;

        [Header("For Classification")]
        [Range(0, 999999999)]
        public int MaxCustomBirthRule = 999999999;
        [Range(0, 999999999)]
        public int MaxCustomSurvivalRule = 999999999;

        [Header("For Fixed")]
        public RulesetName FixedRuleset = RulesetName.Life;
        public int FixedSeed = 1;
        [Range(0f, 1f)]
        public float FixedStartNoise;
        public bool FixedBorderIsAwake = true;

        [Header("Probably should not be changed ...")]
        public CellularAutomaton.Config config = new CellularAutomaton.Config();

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

        [Header("Asset References For Manual Assignment")]
        public GameObject PrefCell4;
        public GameObject PrefCell8;
        public GameObject PrefConnieWei;
        public RectTransform RulesPopup;
        public RectTransform LeftPanel;
        public RectTransform RightPanel;
        public ToggleGroup ToggleGroup;
        public RawImage Image;

        private Color[] pixels;
        private Texture2D texture;
        private TextControl header;
        private TextControl hash;
        private float stepsPerSecond = 2f;
        private float lastStep = 0f;
        private int customBirthRule = 0;
        private int customSurvivalRule = 0;
        private int seedMin = 0;
        private int seedMax = 100;
        private bool confirmNewGame = false;
        private ToggleControl answerAliveBorder;
        private TextBoxControl answerBirth;
        private TextBoxControl answerSurvival;
        private SliderControl birthControl;
        private SliderControl survivalControl;
        private bool setupDone = false;
        private SliderControl speedSlider;

        // Only public for code reasons, not to manually override
        [HideInInspector]
        public enum gameMode { RandomCustom, Fixed };
        [HideInInspector]
        public enum RulesetName
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
        [HideInInspector]
        public CellularAutomaton Automaton;
        [HideInInspector]
        public Color DeadColor;
        [HideInInspector]
        public Color AliveColor;
        [HideInInspector]
        public bool IsPlaying = true;
        [HideInInspector]
        public int StepCount = 0;
        [HideInInspector]
        public bool Dirty = false;
        [HideInInspector]
        public bool IsTyping = false;

        private void Awake() {
            // FIRST UI SETUP 

            header = InstantiateControl<TextControl>(RulesPopup);
            header.transform.SetAsFirstSibling();

            hash = InstantiateControl<TextControl>(RulesPopup);
            hash.transform.SetAsFirstSibling();

            // RULE SETUP

            int timestamp = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
            UnityEngine.Random.InitState(timestamp);
            RulesetName currentRulesetName = RulesetName.Custom;
            bool pickBordersRandomly = true;
            if (Mode == gameMode.RandomCustom) {
                config.seed = UnityEngine.Random.Range(seedMin, seedMax);
                RandomizeRules();
            } else if (Mode == gameMode.Fixed) {
                MaxCustomBirthRule = 999999999;
                MaxCustomSurvivalRule = 999999999;
                currentRulesetName = FixedRuleset;
                config.seed = FixedSeed;
                config.startNoise = FixedStartNoise;
                pickBordersRandomly = false;
                config.aliveBorders = FixedBorderIsAwake;
            } else {
                Debug.LogError("UNKNOWN GAME MODE");
            }
            SelectRuleset(currentRulesetName);
            if (pickBordersRandomly) {
                if (UnityEngine.Random.Range(0f, 1f) < 0.5f) {
                    config.aliveBorders = true;
                } else {
                    config.aliveBorders = false;
                }
            }
            Generate();
            SetupSkyboxAndPalette();
            pixels = new Color[config.width * config.height];
            texture = PTUtils.CreateTexture(config.width, config.height, Color.clear);
            Image.texture = texture;

            // SECOND UI SETUP

            var goal = InstantiateControl<TextControl>(LeftPanel);
            goal.transform.SetAsFirstSibling();
            goal.headerText.text = "<b>Dr. Connie Wei:</b> <i>\"Can you find out what rules these miraculous little creatures live by?\"</i>";

            var connie = Instantiate(PrefConnieWei);
            connie.transform.SetParent(LeftPanel);
            connie.transform.SetAsFirstSibling();

            InstantiateToggle(RulesetName.Life, currentRulesetName).transform.SetParent(RulesPopup);
            InstantiateToggle(RulesetName.Mazectric, currentRulesetName).transform.SetParent(RulesPopup);
            InstantiateToggle(RulesetName.Coral, currentRulesetName).transform.SetParent(RulesPopup);
            InstantiateToggle(RulesetName.WalledCities, currentRulesetName).transform.SetParent(RulesPopup);
            InstantiateToggle(RulesetName.Coagulations, currentRulesetName).transform.SetParent(RulesPopup);
            InstantiateToggle(RulesetName.Anneal, currentRulesetName).transform.SetParent(RulesPopup);
            InstantiateToggle(RulesetName.Majority, currentRulesetName).transform.SetParent(RulesPopup);
            InstantiateToggle(RulesetName.Custom, currentRulesetName).transform.SetParent(RulesPopup);

            var randomizeRulesControl = InstantiateControl<ButtonControl>(RulesPopup);
            randomizeRulesControl.Initialize("Randomize rules", RandomizeRules);

            birthControl = InstantiateControl<SliderControl>(RulesPopup);
            birthControl.Initialize("Birth rule", 0, MaxCustomBirthRule-1, config.seed, value => {
                customBirthRule = Mathf.FloorToInt(value);
                currentRulesetName = RulesetName.Custom;
                SelectRuleset(RulesetName.Custom);
                GameObject.Find("Canvas").GetComponentInChildren<ToggleGroup>().SetAllTogglesOff();
                if (GameObject.Find("Custom") != null) GameObject.Find("Custom").GetComponentInChildren<Toggle>().isOn = true;
                Generate();
            });

            survivalControl = InstantiateControl<SliderControl>(RulesPopup);
            survivalControl.Initialize("Survive rule", 0, MaxCustomSurvivalRule-1, config.seed, value => {
                customSurvivalRule = Mathf.FloorToInt(value);
                currentRulesetName = RulesetName.Custom;
                SelectRuleset(RulesetName.Custom);
                GameObject.Find("Canvas").GetComponentInChildren<ToggleGroup>().SetAllTogglesOff();
                if (GameObject.Find("Custom") != null) GameObject.Find("Custom").GetComponentInChildren<Toggle>().isOn = true;
                Generate();
            });

            var aliveBordersControl = InstantiateControl<ToggleControl>(RulesPopup);
            aliveBordersControl.Initialize("Awake borders", config.aliveBorders, value => {
                config.aliveBorders = value;
                Generate();
            });

            InstantiateControl<ButtonControl>(LeftPanel).Initialize("Play / pause [SPACE]", PlayPause);
            InstantiateControl<ButtonControl>(LeftPanel).Initialize("Next step [N]", Step);
            InstantiateControl<ButtonControl>(LeftPanel).Initialize("Reset experiment [R]", Generate);
            InstantiateControl<ButtonControl>(LeftPanel).Initialize("Clear dish [C]", Clear);
            InstantiateControl<ButtonControl>(LeftPanel).Initialize("Fill dish [F]", Fill);

            InstantiateControl<SliderControl>(LeftPanel).Initialize("Seed value", seedMin, seedMax, config.seed, value => {
                config.seed = Mathf.FloorToInt(value);
                Generate();
            });

            InstantiateControl<SliderControl>(LeftPanel).Initialize("Start noise", 0, 1, config.startNoise, value => {
                config.startNoise = value;
                Generate();
            });

            speedSlider = InstantiateControl<SliderControl>(LeftPanel);
            speedSlider.Initialize("Steps per second", 0, 100, stepsPerSecond, value => {
                stepsPerSecond = value;
            });

            InstantiateControl<ButtonControl>(LeftPanel).Initialize("Peek at solution", SuggestSolution);
            InstantiateControl<ButtonControl>(RulesPopup).Initialize("Hide configuration", SuggestSolution);

            InstantiateControl<ButtonControl>(LeftPanel).Initialize("New experiment", NewGame);

            InstantiateControl<ButtonControl>(RightPanel).Initialize("Exit To Menu", Exit);
            var answer = InstantiateControl<TextControl>(RightPanel).headerText.text = "<b>My answer</b> for Dr. Connie is:";
            answerBirth = InstantiateControl<TextBoxControl>(RightPanel);
            answerBirth.Initialize("<i>Birth rule: (e.g. 123)</i>");
            answerSurvival = InstantiateControl<TextBoxControl>(RightPanel);
            answerSurvival.Initialize("<i>Survival rule: (e.g. 2578)</i>");
            answerAliveBorder = InstantiateControl<ToggleControl>(RightPanel);
            answerAliveBorder.Initialize("Border Is Awake", false, value => {
                //config.aliveBorders = value;
            });
            InstantiateControl<ButtonControl>(RightPanel).Initialize("Submit your theory", Answer);
            setupDone = true;

            RawImage dish = GameObject.Find("Dish").GetComponent<RawImage>();
            dish.texture = Resources.Load("dish-asleep") as Texture;
            print("asdas");
        }

        private void Exit () {
            SceneManager.LoadScene("KnowledgeGraph", LoadSceneMode.Single);
        }

        private string SetSpeciesName () {
            string configToHash = config.ruleset.ToString() + config.aliveBorders.ToString();
            hash.headerText.text = "Species " + md5(configToHash).Substring(0, 8);
            GameObject.Find("SpeciesHash").GetComponent<Text>().text = hash.headerText.text;
            return configToHash;
        }

        private void Answer () {
            string b = "";
            foreach (var digit in config.ruleset.birthRule) {
                b += digit;
            }
            string s = "";
            foreach (var digit in config.ruleset.survivalRule) {
                s += digit;
            }
            bool birthIsCorrect = answerBirth.headerText.text == b;
            bool survivalIsCorrect = answerSurvival.headerText.text == s;
            bool bordersArecorrect = config.aliveBorders == answerAliveBorder.toggle.isOn;

            if (birthIsCorrect && survivalIsCorrect && bordersArecorrect) {
                print("CORRECT!");
             }
            else { 
                print("WRONG!");
            }

            print(answerBirth.headerText.text + " vs " + b);
            print(answerSurvival.headerText.text + " vs " + s);
            print(answerAliveBorder.toggle.isOn + " vs " + config.aliveBorders);
        }

        private void Fill () {
            for (int x = 0; x < config.width; x++) {
                for (int y = 0; y < config.height; y++) {
                    Automaton.cells[y, x] = true;
                }
            }
        }

        private void Clear () {
            for (int x = 0; x < config.width; x++) {
                for (int y = 0; y < config.height; y++) {
                    Automaton.cells[y, x] = false;
                }
            }
        }

        private void NewGame () {
            if (confirmNewGame == true) {
                Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
            }
            else {
                confirmNewGame = true;
                LeftPanel.Find("New experiment").GetComponentInChildren<Text>().text = "Click to confirm!";
            }
        }

        private void SuggestSolution () {
            LeftPanel.gameObject.SetActive(!LeftPanel.gameObject.activeSelf);
            RulesPopup.gameObject.SetActive(!RulesPopup.gameObject.activeSelf);
        }

        private void RandomizeRules () {
            customBirthRule = UnityEngine.Random.Range(0, MaxCustomBirthRule);
            if (birthControl != null) birthControl.slider.value = customBirthRule;
            customSurvivalRule = UnityEngine.Random.Range(0, MaxCustomSurvivalRule);
            if (survivalControl != null) survivalControl.slider.value = customSurvivalRule;
            SelectRuleset(RulesetName.Custom);
            if (GameObject.Find("Canvas") != null) GameObject.Find("Canvas").GetComponentInChildren<ToggleGroup>().SetAllTogglesOff();
            if (GameObject.Find("Custom") != null) GameObject.Find("Custom").GetComponentInChildren<Toggle>().isOn = true;
            Generate();
        }

        private void Update()
        {
            if (!IsTyping) {
                if (Input.GetKeyDown(KeyCode.Space)) PlayPause();
                if (Input.GetKeyDown(KeyCode.N)) Step();
                if (Input.GetKeyDown(KeyCode.R)) Generate();
                if (Input.GetKeyDown(KeyCode.C)) Clear();
                if (Input.GetKeyDown(KeyCode.F)) Fill();

                if (Input.GetKeyDown(KeyCode.Alpha1)) speedSlider.slider.value = 1f;
                if (Input.GetKeyDown(KeyCode.Alpha2)) speedSlider.slider.value = 2f;
                if (Input.GetKeyDown(KeyCode.Alpha3)) speedSlider.slider.value = 5f;
                if (Input.GetKeyDown(KeyCode.Alpha4)) speedSlider.slider.value = 10f;
                if (Input.GetKeyDown(KeyCode.Alpha5)) speedSlider.slider.value = 20f;
                if (Input.GetKeyDown(KeyCode.Alpha6)) speedSlider.slider.value = 30f;
                if (Input.GetKeyDown(KeyCode.Alpha7)) speedSlider.slider.value = 50f;
                if (Input.GetKeyDown(KeyCode.Alpha8)) speedSlider.slider.value = 75f;
                if (Input.GetKeyDown(KeyCode.Alpha9)) speedSlider.slider.value = 100f;
                if (Input.GetKeyDown(KeyCode.Alpha0)) speedSlider.slider.value = 0f;
            }

            DrawCells();
            UpdateSkybox();
            if (IsPlaying) {
                float timeSinceLastStep = Time.time - lastStep;
                if (timeSinceLastStep >= 1f / stepsPerSecond) {
                    Step();
                    lastStep = Time.time;
                }
            }

            // hack to update this after initialization because the order is wrong
            if (birthControl != null && setupDone) birthControl.slider.value = customBirthRule;
            if (survivalControl != null && setupDone) survivalControl.slider.value = customSurvivalRule;
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

            string s = "";
            foreach (byte b in config.ruleset.birthRule) {
                s += b.ToString();
            }
            int.TryParse(s, out customBirthRule);

            string bi = "";
            foreach (byte b in config.ruleset.survivalRule) {
                bi += b.ToString();
            }
            int.TryParse(bi, out customSurvivalRule);
        }

        private void Generate()
        {
            Automaton = new CellularAutomaton(config);
            string n = SetSpeciesName();
            GeneratePalette(n);
            DeadColor = GetMainColorHSV().WithSV(0.3f, 0.2f).ToColor();
            AliveColor = GetMainColorHSV().ToColor();

            Step();
        }

        private void PlayPause () {
            IsPlaying = !IsPlaying;
        }

        private void Step () {
            Automaton.Simulate();
            StepCount++;
        }

        private void DrawCells()
        {
            for (int x = 0; x < config.width; x++)
            {
                for (int y = 0; y < config.height; y++)
                {
                    int cellsNeeded = config.width * config.height;
                    int cellCount = Image.transform.childCount;
                    if (cellCount < cellsNeeded) {
                        GameObject cell = null;
                        if (config.useMooreNeighbourhood) {
                            cell = Instantiate(PrefCell4);
                        }
                        else {
                            cell = Instantiate(PrefCell8);
                        }
                        cell.transform.SetParent(Image.transform, false);
                        Cell c = cell.AddComponent<Cell>();
                        c.X = x;
                        c.Y = y;
                        c.counter = y * config.width + x;
                    }

                    if (Automaton.cells[x, y])
                    {
                        pixels[y*config.width + x] = AliveColor;
                        if (cellCount >= cellsNeeded) Image.transform.GetComponentsInChildren<Image>()[y * config.width + x].color = AliveColor;
                    }
                    else
                    {
                        pixels[y*config.width + x] = DeadColor;
                        if (cellCount >= cellsNeeded) Image.transform.GetComponentsInChildren<Image>()[y * config.width + x].color = DeadColor;
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
        }

        private GameObject InstantiateToggle (RulesetName rulesetName, RulesetName selectedRulesetName)
        {
            var toggle = InstantiateControl<ToggleControl>(ToggleGroup.transform);
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
                toggleGroup: ToggleGroup);
            return toggle.gameObject;
        }

        public void FlipCell (int x, int y) {
            // no idea why I am flipping these =D 
            Automaton.cells[y, x] = !Automaton.cells[y, x];
            Dirty = true;
        }

        public void ActivateCell (int x, int y) {
            // no idea why I am flipping these =D 
            Automaton.cells[y, x] = true;
            Dirty = true;
        }

        public void DeactivateCell (int x, int y) {
            // no idea why I am flipping these =D 
            Automaton.cells[y, x] = false;
            Dirty = true;
        }
        public static string md5 (string str) {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(str);
            var sha = new System.Security.Cryptography.MD5CryptoServiceProvider();
            return System.BitConverter.ToString(sha.ComputeHash(bytes));
        }
    }
}
