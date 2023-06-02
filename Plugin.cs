using BepInEx;
using UnityEngine;
using UnityEngine.UI;

namespace RegionsOfRuin_ConsoleCommands
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        GameObject player;
        GameObject canvas;
        GameObject command_text;
        public GameObject clear;
        public Text clearText;
        private GameObject info;
        public Text infoText;


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

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} by Daltonyx is loaded!");
        }

        public void achget()
        {
            //      what i'm trying to do here is set all achievement get's to true, then trigger an actual achievement to 
            //      pop them all, but it isn't quite right yet

            for (int i = 0; i < clickerController.achievements.Length; i++)
            {
                Log(i.ToString());
                clickerController.achievements[i] = true;
            }
            for (int i = 0; i < 6; i++)
            {
                clickerController.bar[i] += 100000L;
                
            }
            Log("All achievements unlocked!");
        }

        public void Update()
        {
            //if game's scene's name is 'title', skip the rest of the loop to gracefully handle the main menu
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.ToString() == "title")
                return;

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
                    info = new GameObject("info");
                    command_text.AddComponent<Text>();
                    command_text.AddComponent<RectTransform>();
                    command_text.AddComponent<CanvasRenderer>();
                    info.AddComponent<Text>();
                    info.AddComponent<RectTransform>();
                    info.AddComponent<CanvasRenderer>();
                    //set info's parent to command_text
                    info.transform.SetParent(command_text.transform);
                    //set command_text's parent to canvas
                    canvas = GameObject.Find("Canvas(Clone)");
                    //command_text.transform.SetParent(canvas.transform);
                    command_text.AddComponent<customInteractInfo>();
                    interactionInfo = command_text.GetComponentInChildren<customInteractInfo>();

                    interactionInfo.infoText.text = "Press / for commands";

                    popup_timer = 99999;

                    selected = false;
                    command_string = "";
                    Logger.LogInfo($"interaction text is {interactionInfo.infoText.text}");
                    //set a font
                    command_text.GetComponentInChildren<customInteractInfo>().infoText.font = Font.CreateDynamicFontFromOSFont("Arial", 14);
                    //get window width and height from player prefs
                    float screenWidth = Screen.currentResolution.width;
                    float screenHeight = Screen.currentResolution.height;
                    command_text.transform.position = new Vector3(0, 0, 0);
                    command_text.transform.localPosition = new Vector3(0,0,0);

                    
                }
                else
                {
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
                    
                    float screenWidth = PlayerPrefs.GetFloat("screenWidth");
                    float screenHeight = PlayerPrefs.GetFloat("screenHeight");
                    //I'm just trying to get the resolution of the window, and not the screen. WHY IS IT THIS HARD. screw it, people play fullscreen or native resolution at least, right?
                    float windowWidth = Screen.width;
                    float windowHeight = Screen.height;
                    float textWidth = command_text.GetComponentInChildren<customInteractInfo>().infoText.GetComponent<RectTransform>().rect.width;
                    command_text.transform.position = new Vector3(0, 0, 0);
                    command_text.transform.localPosition = new Vector3(-200,-500,0);
                    //command_text.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 400);
                    //if canvas isn't null, we can attempt to access values and stuff, but it's still unsafe in title without a check
                    if (popup_timer > 0)
                    {
                        popup_timer--;
                        if (popup_timer == 0)
                        {
                            
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.color = Color.white;
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = "";
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
}
