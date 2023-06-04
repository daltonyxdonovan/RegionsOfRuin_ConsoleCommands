using BepInEx;
using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;
using System.Reflection;
using System.Collections;

namespace RegionsOfRuin_ConsoleCommands
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        GameObject player;
        GameObject canvas;
        GameObject command_text;
        GameObject suggestions_text;
        public GameObject clear;
        public Text clearText;
        private GameObject info;
        private GameObject suggestions_info;
        public Text infoText;
        public Text suggestionsText;
        bool patched = false;
        string command_string = "";
        int ticker_playerfind = 0;
        int popup_timer = 0;
        bool daytime = false;
        bool nighttime = false;
        bool gui_ready = false;
        bool selected = false;
        bool strengthmod = false;
        int strengthchoice = 0;
        bool nowounds = false;
        customInteractInfo interactionInfo;
        Suggestions suggestions;
        string[] commands_slash = new string[] { "/help", "/moon", "/sun", "/strength", "/dexterity", "/constitutions", "/addmoney", "/addxp", "/addworker", "/settime", "/setxp", "/setmaxhp", "/sethp", "/setlvl", "/setrep", "/setcap", "/achget", "/unlockhard", "/heal", "/nowounds", "shieldcondition", "/shieldconditionmax", "/weaponweight", "/wornmindamage", "/wornmaxdamage", "/devtools", "/timeup", "/timedown", "/kill", "/chest", "/flip", "/kit", "/giveall"};
        string[] commands_noslash = new string[] { "help", "moon", "sun", "strength", "dexterity", "constitutions", "addmoney", "addxp", "addworker", "settime", "setxp", "setmaxhp", "sethp", "setlvl", "setrep", "setcap", "achget", "unlockhard", "heal", "nowounds", "shieldcondition", "shieldconditionmax", "weaponweight", "wornmindamage", "wornmaxdamage", "devtools", "timeup", "timedown", "kill", "chest", "flip", "kit", "giveall"};
        
        // Token: 0x04000C39 RID: 3129
        //dev helm
	private int[] helm = new int[]
	{
		1, /* image */ 
		99, /* ability damage? maybe? on the axe it's 'max cleaving' */
		6969, /* cost */
		0,
		0,
		0,
		0,
		27, /* crit chance */
		29, /* crit damage */
		31, /* armor pen */
		33, /* second half of armour rating */
		35, /* physical res */
		37, /* fire res */
		39, /* cold res */
		41, /* poison res */
		43, /* electric res */
		45, /* strength */
		47, /* dexterity */
		49, /* constitution */
		0,
		53, /* first half of armour rating */
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0
	};

	// Token: 0x04000C3A RID: 3130

    //unused
	private int[] shield = new int[]
	{
		1, /* image */ 
		99, /* ability damage? maybe? on the axe it's 'max cleaving' */
		6969, /* cost */
		0,
		0,
		0,
		0,
		99, /* crit chance */
		99, /* crit damage */
		99, /* armor pen */
		99, /* second half of armour rating */
		99, /* physical res */
		99, /* fire res */
		99, /* cold res */
		99, /* poison res */
		99, /* electric res */
		99, /* strength */
		99, /* dexterity */
		99, /* constitution */
		0,
		99, /* first half of armour rating */
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0
	};

	// Token: 0x04000C3B RID: 3131
    //dev axe
	private int[] axe = new int[]
	{
		1,
		99,
		6969,
		2,
		0,
		1,
		3,
		99,
		99,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		30,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0
	};

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} by Daltonyx is loaded!");
        }

        public void achget()
        {
            
            Harmony.CreateAndPatchAll(typeof(PatchStatUnlock));
            //Steamworks.SteamUserStats.SetAchievement(id);
            stats.Unlock("achievement_money_1M");
            PatchStatUnlock patch = new PatchStatUnlock("achievement_money_1M");
            stats.Unlock("achievement_money_10M");
            PatchStatUnlock patch2 = new PatchStatUnlock("achievement_money_10M");
            stats.Unlock("achievement_money_100M");
            PatchStatUnlock patch3 = new PatchStatUnlock("achievement_money_100M");
            stats.Unlock("achievement_buildings_max");
            PatchStatUnlock patch4 = new PatchStatUnlock("achievement_buildings_max");
            Log("All achievements i can find are unlocked!");
        }

        public void popup(string text)
        {
            informBox.handler.openBox(text, 0f);
        }

        public void addore(long amount)
        {
            //clickerController.ore += amount;
        }

        public void unlockhard()
        {
            PlayerPrefsX.SetBool("finishedGame", true);
        }

        public void giveWood(int amount)
        {
            resourceDisplay.res[0] += amount;
            Log("Gave " + amount + " wood!");
        }

        public void giveLeather(int amount)
        {
            resourceDisplay.res[1] += amount;
            Log("Gave " + amount + " leather!");
        }

        public void giveIron(int amount)
        {
            resourceDisplay.res[7] += amount;
            Log("Gave " + amount + " iron!");
        }

        public void giveBronze(int amount)
        {
            resourceDisplay.res[8] += amount;
            Log("Gave " + amount + " bronze!");
        }

        public void giveSteel(int amount)
        {
            resourceDisplay.res[10] += amount;
            Log("Gave " + amount + " steel!");
        }

        public void giveAdamantine(int amount)
        {
            resourceDisplay.res[11] += amount;
            Log("Gave " + amount + " adamantine!");
        }

        public void giveAll(int amount)
        {
            for (int i = 0; i < 12; i++)
            {
                resourceDisplay.res[i] += amount;
            }
        }

        public void Update()
        {
            //if game's scene's name is 'title', skip the rest of the loop to gracefully handle the main menu
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.ToString() == "title")
            {
                if (!patched)
                {
                    //man I don't understand reflection very well, no patching today
                    //Harmony.CreateAndPatchAll(typeof(PatchUpdateStats));
                    patched = !patched;
                }
                return;
            }
                

            //if we don't know what object the player is yet, find it
            if (player == null)
            {
                ticker_playerfind++;
                if (ticker_playerfind > 60)
                {
                    //only scan every half second, so we don't flood the log with errors
                    ticker_playerfind = 0;
                    player = GameObject.Find("dwarf");
                }
            }
            //we have our player 'dwarf', now we can continue the loop
            else if (player != null)
            {
                if (gui_ready == false)
                {
                    gui_ready = true;
                    command_text = new GameObject("command_text");
                    suggestions_text = new GameObject("suggestions_text");
                    info = new GameObject("info");
                    suggestions_info = new GameObject("suggestions_info");
                    command_text.AddComponent<Text>();
                    suggestions_text.AddComponent<Text>();
                    command_text.AddComponent<RectTransform>();
                    suggestions_text.AddComponent<RectTransform>();
                    command_text.AddComponent<CanvasRenderer>();
                    suggestions_text.AddComponent<CanvasRenderer>();
                    info.AddComponent<Text>();
                    suggestions_info.AddComponent<Text>();
                    info.AddComponent<RectTransform>();
                    suggestions_info.AddComponent<RectTransform>();
                    info.AddComponent<CanvasRenderer>();
                    suggestions_info.AddComponent<CanvasRenderer>();
                    //set info's parent to command_text
                    info.transform.SetParent(command_text.transform);
                    suggestions_info.transform.SetParent(suggestions_text.transform);
                    //set command_text's parent to canvas
                    canvas = GameObject.Find("Canvas(Clone)");
                    //command_text.transform.SetParent(canvas.transform);
                    command_text.AddComponent<customInteractInfo>();
                    suggestions_text.AddComponent<Suggestions>();
                    interactionInfo = command_text.GetComponentInChildren<customInteractInfo>();
                    suggestions = suggestions_text.GetComponentInChildren<Suggestions>();

                    interactionInfo.infoText.text = "Press / for commands";
                    suggestions.infosText.text = "Suggestions: ";

                    popup_timer = 99999;

                    selected = false;
                    command_string = "";
                    Logger.LogInfo($"interaction text is {interactionInfo.infoText.text}");
                    Logger.LogInfo($"suggestions text is {suggestions.infosText.text}");
                    //set a font
                    command_text.GetComponentInChildren<customInteractInfo>().infoText.font = Font.CreateDynamicFontFromOSFont("Arial", 14);
                    suggestions_text.GetComponentInChildren<Suggestions>().infosText.font = Font.CreateDynamicFontFromOSFont("Arial", 6);
                    //get window width and height from player prefs
                    float screenWidth = Screen.currentResolution.width;
                    float screenHeight = Screen.currentResolution.height;
                    command_text.transform.position = new Vector3(0, 0, 0);
                    suggestions_text.transform.position = new Vector3(0, 0, 0);
                    command_text.transform.localPosition = new Vector3(0,0,0);
                    suggestions_text.transform.localPosition = new Vector3(0, 0, 0);

                    
                }
                else
                {
                    if (command_text == null)
                    {
                        gui_ready = false;
                        return;
                    }
                        
                    //overrides go here
                    if (daytime)
                    {
                        dayCycleController day_cycle_controller = FindObjectOfType<dayCycleController>();
                        day_cycle_controller.time = 1000;
                    }
                    if (nighttime)
                    {
                        dayCycleController day_cycle_controller = FindObjectOfType<dayCycleController>();
                        day_cycle_controller.time = 400;
                    }
                    if (strengthmod)
                        DwarfController.strength = strengthchoice;
                    if (nowounds)
                    {
                        DwarfController.woundsStatic = string.Empty;
		                DwarfController.bigwoundsStatic = string.Empty;
                    }


                    //if canvas is not the parent of command_text, set it to be
                    if (command_text.transform.parent != canvas.transform)
                        command_text.transform.SetParent(canvas.transform);
                    if (suggestions_text.transform.parent != canvas.transform)
                        suggestions_text.transform.SetParent(canvas.transform);
                    
                    float screenWidth = PlayerPrefs.GetFloat("screenWidth");
                    float screenHeight = PlayerPrefs.GetFloat("screenHeight");
                    //I'm just trying to get the resolution of the window, and not the screen. WHY IS IT THIS HARD. screw it, people play fullscreen or native resolution at least, right?
                    float windowWidth = Screen.width;
                    float windowHeight = Screen.height;
                    float textWidth = command_text.GetComponentInChildren<customInteractInfo>().infoText.GetComponent<RectTransform>().rect.width;
                    command_text.transform.position = new Vector3(0, 0, 0);
                    suggestions_text.transform.position = new Vector3(0, 0, 0);
                    command_text.transform.localPosition = new Vector3(-200,-500,0);
                    suggestions_text.transform.localPosition = new Vector3(-180, -460, 0);
                    //command_text.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 400);
                    //if canvas isn't null, we can attempt to access values and stuff, but it's still unsafe in title without a check
                    if (popup_timer > 0)
                    {
                        popup_timer--;
                        if (popup_timer == 0)
                        {
                            
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.color = Color.white;
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = "";
                            //suggestions_text.GetComponentInChildren<Suggestions>().infoText.text = "";
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.Slash))
                    {
                        if (selected)
                        {
                            selected = false;
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = "Press / for commands";
                            command_string = "";
                        }
                        else
                        {
                            selected = true;
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = "/";
                            command_string = "/";
                        }
                        popup_timer = 400;
                    }

                    if (Input.GetKeyDown(KeyCode.Backspace))
                    {
                        if (selected)
                        {
                            if (command_string.Length > 0)
                            {
                                command_string = command_string.Substring(0, command_string.Length - 1);
                                command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            }
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        if (selected)
                        {
                            selected = false;
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = "Press / for commands";
                            command_string = "";
                        }
                        popup_timer = 0;
                    }

                    if (Input.GetKeyDown(KeyCode.Return))
                    {   
                        popup_timer = 0;
                        
                        //command parsing goes here

                        if (command_string.StartsWith("/addmoney"))
                        {
                            string[] strings = command_string.Split(' ');
                            int choice = int.Parse(strings[1]);
                            DwarfController.self.getCoin(choice);
                            Log($"Added {choice} money");
                        }

                        if (command_string.StartsWith("/moon"))
                        {
                            nighttime = true;
                            daytime = false;
                        }

                        if (command_string.StartsWith("/sun"))
                        {
                            nighttime = false;
                            daytime = true;
                        }

                        if (command_string.StartsWith("/achget"))
                        {
                            achget();
                        }

                        if (command_string.StartsWith("/settime"))
                        {
                            string[] strings = command_string.Split(' ');
                            int choice = int.Parse(strings[1]);
                            dayCycleController day_cycle_controller = FindObjectOfType<dayCycleController>();
                            day_cycle_controller.time = choice;
                            Log($"Set time to {choice}");
                        }

                        if (command_string.StartsWith("/strength"))
                        {
                            //still working on it
                            string[] strings = command_string.Split(' ');
                            int choice = int.Parse(strings[1]);
                            strengthmod = true;
                            strengthchoice = choice;
                            DwarfController.strength = choice;
                            Log($"Added {choice} strength");
                        }

                        if (command_string.StartsWith("/addxp"))
                        {
                            string[] strings = command_string.Split(' ');
                            int choice = int.Parse(strings[1]);
                            DwarfController.self.gainExp(choice);
                            Log($"Added {choice} xp");
                        }

                        if (command_string.StartsWith("/unlockhard"))
                        {
                            unlockhard();
                        }

                        if (command_string.StartsWith("/setxp"))
                        {
                            string[] strings = command_string.Split(' ');
                            int choice = int.Parse(strings[1]);
                            DwarfController.experience=choice;
                            Log($"Set {choice} as xp");
                        }

                        if (command_string.StartsWith("/setmaxhp"))
                        {
                            string[] strings = command_string.Split(' ');
                            int choice = int.Parse(strings[1]);
                            DwarfController.maxHealth = choice;
                            Log($"Set {choice} as max hp");
                        }
                        
                        if (command_string.StartsWith("/sethp"))
                        {
                            string[] strings = command_string.Split(' ');
                            int choice = int.Parse(strings[1]);
                            DwarfController.currentHealth = choice;
                            Log($"Set {choice} as hp");
                        }

                        if (command_string.StartsWith("/heal"))
                        {
                            string[] strings = command_string.Split(' ');
                            if (strings.Length > 1)
                            {
                                int choice = int.Parse(strings[1]);
                                DwarfController.currentHealth = DwarfController.currentHealth + choice;
                                Log($"Healed {choice} hp");
                            }
                            else
                            {
                                DwarfController.currentHealth = DwarfController.maxHealth;
                                Log($"Healed to max hp");
                            }

                            
                            
                        }

                        if (command_string.StartsWith("/dexterity"))
                        {
                            string[] strings = command_string.Split(' ');
                            int choice = int.Parse(strings[1]);
                            DwarfController.dexterity = choice;
                            Log($"Set {choice} as dexterity");
                        }

                        if (command_string.StartsWith("/constitution"))
                        {
                            string[] strings = command_string.Split(' ');
                            int choice = int.Parse(strings[1]);
                            DwarfController.constitution = choice;
                            Log($"Set {choice} as constitution");
                        }

                        if (command_string.StartsWith("/nowounds"))
                        {
                            string[] strings = command_string.Split(' ');
                            bool choice = bool.Parse(strings[1]);
                            
                            nowounds = choice;
                            Log($"Set nowounds {choice}");
                        }

                        if (command_string.StartsWith("/shieldcondition"))
                        {
                            string[] strings = command_string.Split(' ');
                            int choice = int.Parse(strings[1]);
                            DwarfController.shieldCondition = choice;
                            Log($"Set {choice} as shield condition");
                        }

                        if (command_string.StartsWith("/shieldconditionmax"))
                        {
                            string[] strings = command_string.Split(' ');
                            int choice = int.Parse(strings[1]);
                            DwarfController.shieldConditionMax = choice;
                            Log($"Set {choice} as shield condition max");
                        }
                        
                        if (command_string.StartsWith("/weaponweight"))
                        {
                            string[] strings = command_string.Split(' ');
                            int choice = int.Parse(strings[1]);
                            DwarfController.weaponWeight = choice;
                            Log($"Set {choice} as weapon weight");
                        }

                        if (command_string.StartsWith("/wornmindamage"))
                        {
                            string[] strings = command_string.Split(' ');
                            int choice = int.Parse(strings[1]);
                            DwarfController.wornMinDamage = choice;
                            Log($"Set {choice} as worn min damage");
                        }

                        if (command_string.StartsWith("/wornmaxdamage"))
                        {
                            string[] strings = command_string.Split(' ');
                            int choice = int.Parse(strings[1]);
                            DwarfController.wornMaxDamage = choice;
                            Log($"Set {choice} as worn max damage");
                        }

                        if (command_string.StartsWith("/setlvl"))
                        {
                            string[] strings = command_string.Split(' ');
                            int choice = int.Parse(strings[1]);
                            DwarfController.overallLevel = choice;
                            Log($"Set {choice} as level");
                        }

                        if (command_string.StartsWith("/setrep"))
                        {
                            string[] strings = command_string.Split(' ');
                            int choice = int.Parse(strings[1]);
                            gameStatsLog.reputation = choice;
                            Log($"Set {choice} as reputation");
                        }

                        if (command_string.StartsWith("/devtools"))
                        {
                            DwarfController.devTools = true;
                            Log($"Dev tools enabled! Godspeed, spiderman");
                        }

                        if (command_string.StartsWith("/timeup"))
                        {
                            Time.timeScale += 0.5f;
                        }

                        if (command_string.StartsWith("/timedown"))
                        {
                            Time.timeScale -= 0.5f;
                        }

                        if (command_string.StartsWith("/addworker"))
                        {
                            string[] strings = command_string.Split(' ');
                            int choice = int.Parse(strings[1]);
                            for (int i = 0; i < choice; i++)
                            {
                                workerHandler.addWorker(1);
                            }
                            Log($"Added {choice} workers");
                        }

                        if (command_string.StartsWith("/kill"))
                        {
                            AIScript[] array = UnityEngine.Object.FindObjectsOfType(typeof(AIScript)) as AIScript[];
                            for (int l = 0; l < array.Length; l++)
                            {
                                if (array[l].alliance > 0)
                                {
                                    array[l].health = 0;
                                }
                            }
                        }

                        if (command_string.StartsWith("/chest"))
                        {
                            GameObject original = (GameObject)Resources.Load("Prefabs/chestSpawn");
                            Vector3 position5 = base.transform.position;
                            position5.y += 1.5f;
                            position5.x += UnityEngine.Random.Range(-1f, 1f);
                            original = UnityEngine.Object.Instantiate<GameObject>(original, position5, Quaternion.identity);
                        }

                        if (command_string.StartsWith("/flip"))
                        {
                            DwarfController.facingRight = !DwarfController.facingRight;
                            Vector3 localScale = base.transform.localScale;
                            localScale.x *= -1f;
                            base.transform.localScale = localScale;
                            localScale.x *= 0.1f;
                            localScale.y *= 0.1f;
                            
                        }

                        if (command_string.StartsWith("/kit"))
                        {
                            
                            /*
                                here is a method I found for when the tutorial ends, it gives you starting equipment. Using this I am trying
                                to figure out how the give command should function. It seems that there may be 29 infos (kind of, all three loops iterate
                                over 29 spots, I'm thinking they're the stats of the weapon) and during the entire iteration, they are setting a specific inventory slot (the second variable accessed through inventory.inv). I have not quite
                                figured out what the first variable is. I'm going to poke around some more and see if it is the amount maybe? That would be weird though,
                                as all resources are handled through the resourceInfo and dwarfController (i can't remember the exact name, maybe resourceDisplay) scripts.

                            */

                            for (int j = 0; j < 9; j++)
                            {
                                if (j > 6 && j < 8)
                                {

                                    for (int i = 0; i < 29; i++)
                                    {
                                        inventory.inv[0, j, i] = helm[i];
                                    }
                                    inventory.itemNames[0, j] = "Daltonyx's Helm";
                                }
                                else if (j == 8)
                                {
                                    for (int i = 0; i < 29; i++)
                                    {
                                        inventory.inv[0, j, i] = axe[i];
                                        
                                    }
                                    inventory.itemNames[0, j] = "Daltonyx's Axe";
                                }
                            }
/*
                            for (int j = 0; j < 29; j++)
                            {
                                inventory.inv[0, 3, j] = shield[j];
                            }
                            inventory.itemNames[0, 3] = "PWNbuckler";


                            for (int k = 0; k < 29; k++)
                            {
                                inventory.inv[0, 1, k] = armour[k];
                            }
                            inventory.itemNames[0, 1] = "PWNvest";
                            */

                            inventory.refreshWornStats();



                            
                        }

                        if (command_string.StartsWith("/setcap"))
                        {
                            string[] strings = command_string.Split(' ');
                            int choice = int.Parse(strings[1]);
                            for (int i = 0; i < 12; i++)
                            {
                                resourceDisplay.cap[i] = choice;
                            }
                            Log($"Set {choice} as max resource cap");
                        }

                        if (command_string.StartsWith("/giveall"))
                        {
                            string[] strings = command_string.Split(' ');
                            int choice = int.Parse(strings[1]);
                            giveAll(choice);
                            Log($"Gave {choice} of all resources!");
                        }

                        if (command_string.StartsWith("/help"))
                        {
                            string[] strings = command_string.Split(' ');
                            if (strings.Length > 1)
                            {
                                string command = strings[1];
                                if (command == "addmoney")
                                    Log("/addmoney <amount> - Adds money to your inventory");
                                else if (command == "moon")
                                    Log("sets it to permanent nighttime");
                                else if (command == "sun")
                                    Log("sets it to permanent daytime");
                                else if (command == "settime")
                                    Log("/settime <time> - Sets the time of day");
                                else if (command == "strength")
                                    Log("/strength <amount> - Sets your strength");
                                else if (command == "dexterity")
                                    Log("/dexterity <amount> - Sets your dexterity");
                                else if (command == "constitution")
                                    Log("/constitution <amount> - Sets your constitution");
                                else if (command == "setxp")
                                    Log("/setmaxhp <amount> - Sets your max hp");
                                else if (command == "sethp")
                                    Log("/sethp <amount> - Sets your current hp");
                                else if (command == "setmaxhp")
                                    Log("/setmaxhp <amount> - Sets your max hp");
                                else if (command == "giveall")
                                    Log("/giveall <amount> - Gives you <amount> of all resources");
                                else if (command == "setcap")
                                    Log("/setcap <amount> - Sets your max resource capacity");
                                else if (command == "heal")
                                    Log("/heal <amount> - Heals you OR /heal - heals to max health");
                                else if (command == "nowounds")
                                    Log("/nowounds <bool> - sets wounds on or off (true or false)");
                                else if (command == "shieldcondition")
                                    Log("/shieldcondition <amount> - sets your shield's condition");
                                else if (command == "shieldconditionmax")
                                    Log("/shieldconditionmax <amount> - sets your shield's max condition");
                                else if (command == "weaponweight")
                                    Log("/weaponweight <amount> - sets your weapon's weight");
                                else if (command == "setlvl")
                                    Log("/setlvl <amount> - sets your level");
                                else if (command == "setrep")
                                    Log("/setrep <amount> - sets your reputation");
                                else if (command == "devtools")
                                    Log("/devtools - enables dev keybinds");
                                else if (command == "addworker")
                                    Log("/addworker <amount> - adds workers to your inventory");
                                else if (command == "kill")
                                    Log("/kill - kills all enemies");
                                else if (command == "chest")
                                    Log("/chest - spawns a chest");
                                else if (command == "flip")
                                    Log("/flip - flips your character");
                                else if (command == "kit")
                                    Log("/kit - spawns dev weapon in inventory");
                                else if (command == "help")
                                    Log("/help <command> - gives help on a command");
                                

                            }
                            else
                            {
                                Log("/help usage: /help <command>");
                            }
                        }

                        selected = false;
                        command_text.GetComponentInChildren<customInteractInfo>().infoText.text = "Press / for commands";
                        command_string = "";
                    }

                    if (selected)
                    {
                        //I've tried like four different ways to do this without an if/else block for the _entire alphabet and all 10 numbers_, it's already taken too long so fine I guess here we go-
                        
                        if (Input.GetKeyDown(KeyCode.A))
                        {
                            command_string += "a";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.B))
                        {
                            command_string += "b";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.C))
                        {
                            command_string += "c";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.D))
                        {
                            command_string += "d";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.E))
                        {
                            command_string += "e";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.F))
                        {
                            command_string += "f";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.G))
                        {
                            command_string += "g";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.H))
                        {
                            command_string += "h";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.I))
                        {
                            command_string += "i";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.J))
                        {
                            command_string += "j";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.K))
                        {
                            command_string += "k";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.L))
                        {
                            command_string += "l";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.M))
                        {
                            command_string += "m";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.N))
                        {
                            command_string += "n";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.O))
                        {
                            command_string += "o";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.P))
                        {
                            command_string += "p";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.Q))
                        {
                            command_string += "q";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.R))
                        {
                            command_string += "r";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.S))
                        {
                            command_string += "s";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.T))
                        {
                            command_string += "t";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.U))
                        {
                            command_string += "u";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.V))
                        {
                            command_string += "v";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.W))
                        {
                            command_string += "w";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.X))
                        {
                            command_string += "x";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.Y))
                        {
                            command_string += "y";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.Z))
                        {
                            command_string += "z";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha0))
                        {
                            command_string += "0";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha1))
                        {
                            command_string += "1";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha2))
                        {
                            command_string += "2";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha3))
                        {
                            command_string += "3";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha4))
                        {
                            command_string += "4";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha5))
                        {
                            command_string += "5";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha6))
                        {
                            command_string += "6";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha7))
                        {
                            command_string += "7";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha8))
                        {
                            command_string += "8";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha9))
                        {
                            command_string += "9";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }
                        else if (Input.GetKeyDown(KeyCode.Space))
                        {
                            command_string += " ";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                            popup_timer = 400;
                        }

                    }
                }
            }
        
            string[] textToDisplay = suggestions_text.GetComponentInChildren<Suggestions>().infosText.text.Split();
            if (textToDisplay.Length > 1)
            {
                if (textToDisplay[0] == "/help")
                {
                    for (int i = 0; i < commands_noslash.Length; i++)
                    {
                        if (textToDisplay[1] != "" && textToDisplay[1] != " " && commands_noslash[i].StartsWith(textToDisplay[1].ToString()))
                        {
                            
                            suggestions_text.GetComponentInChildren<Suggestions>().infosText.color = Color.white;
                            suggestions_text.GetComponentInChildren<Suggestions>().infosText.text = suggestions_text.GetComponentInChildren<Suggestions>().infosText.text + " " + commands_noslash[i];
                            
                        }
                    }
                }
                
            }
            else if (textToDisplay.Length == 1)
            {
                for (int i = 0; i < commands_slash.Length; i++)
                {
                    if (textToDisplay[0] != "" && textToDisplay[0] != " " && commands_slash[i].StartsWith(textToDisplay[0].ToString()))
                    {
                        suggestions_text.GetComponentInChildren<Suggestions>().infosText.color = Color.white;
                        suggestions_text.GetComponentInChildren<Suggestions>().infosText.text = suggestions_text.GetComponentInChildren<Suggestions>().infosText.text + " " + commands_slash[i];
                    }
                }
            }

            
        }
        
        public static void ResetDwarf()
        {
            DwarfController.coins = 0;
            DwarfController.overallLevel = 1;
            DwarfController.totalGoldEarned = 0;
            DwarfController.totalDamageDealt = 0;
            DwarfController.totalDamageTaken = 0;
            DwarfController.totalExperienceGained = 0f;
            DwarfController.experience = 0f;
            DwarfController.experiencePoints = 0;
            DwarfController.experienceToNextLevel = 300f;
            DwarfController.currentHealth = 100f;
            DwarfController.maxHealth = 100f;
            DwarfController.strength = 0;
            DwarfController.dexterity = 0;
            DwarfController.constitution = 0;
            DwarfController.axes = 2;
            DwarfController.weaponWeight = 0;
            DwarfController.wornMinDamage = 0;
            DwarfController.wornMaxDamage = 0;
            DwarfController.weaponWeight = 1;
            DwarfController.headCondition = 0;
            DwarfController.headConditionMax = 0;
            DwarfController.torsoCondition = 0;
            DwarfController.torsoConditionMax = 0;
            DwarfController.leftCondition = 0;
            DwarfController.leftConditionMax = 0;
            DwarfController.rightCondition = 0;
            DwarfController.rightConditionMax = 0;
            DwarfController.shieldCondition = 0;
            DwarfController.shieldConditionMax = 0;
            DwarfController.woundsStatic = string.Empty;
            DwarfController.bigwoundsStatic = string.Empty;
            gameStatsLog.reputation = 0;
            sceneSaveData.caches = new int[100, 100];
            sceneSaveData.npcs = new int[100, 100];
        }

        public void Log(string message)
        {
            Logger.LogInfo(message);
            if (canvas != null)
            {
                interactionInfo iInfo = FindObjectOfType<interactionInfo>();
                iInfo.inform(message, Color.green);
                popup_timer = 400;
            }
        }
    }

    class PatchStatUnlock
    {
        private static string _id;
        public PatchStatUnlock(string id)
        {
            _id = id;
        }

        [HarmonyPatch(typeof(stats), "Unlock")]
        [HarmonyPrefix]
        public static void RealUnlock(string id)
        {
            
            Steamworks.SteamUserStats.SetAchievement(id);
        }
    }
}
