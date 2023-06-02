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

        bool gui_ready = false;
        bool selected = false;
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
                    //if canvas is not the parent of command_text, set it to be
                    if (command_text.transform.parent != canvas.transform)
                        command_text.transform.SetParent(canvas.transform);
                    
                    float screenWidth = PlayerPrefs.GetFloat("screenWidth");
                    float screenHeight = PlayerPrefs.GetFloat("screenHeight");
                    //I'm just trying to get the resolution of the window, and not the screen. WHY IS IT THIS HARD
                    float windowWidth = Screen.width;
                    float windowHeight = Screen.height;
                    float textWidth = command_text.GetComponentInChildren<customInteractInfo>().infoText.GetComponent<RectTransform>().rect.width;
                    command_text.transform.position = new Vector3(0, 0, 0);
                    command_text.transform.localPosition = new Vector3(-200,-500,0);
                    //command_text.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 400);
                    //if canvas isn't null, we can attempt to access values and stuff, but it's still unsafe in title
                    if (popup_timer > 0)
                    {
                        popup_timer--;
                        if (popup_timer == 0)
                        {
                            
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.color = Color.red;
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
                        
                    }

                    if (selected)
                    {
                        //I've tried like four different ways to do this without an if/else block for the _entire alphabet and all 10 numbers_, it's already taken too long so fine I guess here we go-
                        
                        if (Input.GetKeyDown(KeyCode.A))
                        {
                            command_string += "a";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.B))
                        {
                            command_string += "b";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.C))
                        {
                            command_string += "c";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.D))
                        {
                            command_string += "d";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.E))
                        {
                            command_string += "e";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.F))
                        {
                            command_string += "f";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.G))
                        {
                            command_string += "g";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.H))
                        {
                            command_string += "h";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.I))
                        {
                            command_string += "i";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.J))
                        {
                            command_string += "j";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.K))
                        {
                            command_string += "k";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.L))
                        {
                            command_string += "l";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.M))
                        {
                            command_string += "m";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.N))
                        {
                            command_string += "n";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.O))
                        {
                            command_string += "o";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.P))
                        {
                            command_string += "p";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.Q))
                        {
                            command_string += "q";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.R))
                        {
                            command_string += "r";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.S))
                        {
                            command_string += "s";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.T))
                        {
                            command_string += "t";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.U))
                        {
                            command_string += "u";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.V))
                        {
                            command_string += "v";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.W))
                        {
                            command_string += "w";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.X))
                        {
                            command_string += "x";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.Y))
                        {
                            command_string += "y";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.Z))
                        {
                            command_string += "z";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha0))
                        {
                            command_string += "0";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha1))
                        {
                            command_string += "1";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha2))
                        {
                            command_string += "2";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha3))
                        {
                            command_string += "3";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha4))
                        {
                            command_string += "4";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha5))
                        {
                            command_string += "5";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha6))
                        {
                            command_string += "6";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha7))
                        {
                            command_string += "7";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha8))
                        {
                            command_string += "8";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.Alpha9))
                        {
                            command_string += "9";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }
                        else if (Input.GetKeyDown(KeyCode.Space))
                        {
                            command_string += " ";
                            command_text.GetComponentInChildren<customInteractInfo>().infoText.text = command_string;
                        }

                    }
                }
            }
        }
        
    
        public void Log(string message)
        {
            Logger.LogInfo(message);
            if (canvas != null)
            {
                
                //      vvv      this is the way the game does it's popup text, so we'll do it the same way but get rid of the alpha manipulation
                //      vvv      longer than 2 seconds lol
                //  interactionInfo.inform(message, Color.red);
                
                command_text.GetComponentInChildren<customInteractInfo>().infoText.color = Color.red;
                command_text.GetComponentInChildren<customInteractInfo>().infoText.text = message;

                popup_timer = 400;

                
            }
           
        }
    }
}
