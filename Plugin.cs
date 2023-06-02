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
        Text popup_text;
        Text command_text;


        string command_string = "";

        int ticker_playerfind = 0;
        int popup_timer = 0;

        bool gui_ready = false;
        bool selected = false;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} by Daltonyx is loaded!");
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
                if (canvas == null)
                {
                    //create a new canvas
                    canvas = GameObject.Find("Canvas(Clone)");
                    //create popup_text and set it's parent to canvas
                    popup_text = new GameObject("Popup Text", typeof(Text)).GetComponent<Text>();
                    popup_text.transform.SetParent(canvas.transform);
                    popup_text.fontSize = 10;
                    popup_text.color = Color.white;
                    popup_text.text = "";

                    //do the same for command_text
                    command_text =  new GameObject("Command Text", typeof(Text)).GetComponent<Text>();
                    command_text.transform.SetParent(canvas.transform);
                    interactionInfo interactionInfo = FindObjectOfType<interactionInfo>();


                    GameObject info = interactionInfo.infoText.gameObject;
                    Vector3 origin = info.transform.position;

                    int screen_width = Screen.width;
                    int screen_height = Screen.height;

                    command_text.transform.position = new Vector3(origin.x, screen_height-(int)screen_height/8, origin.z);

                    
                    command_text.fontSize = 10;
                    command_text.color = Color.white;
                    command_text.text = "Press / for commands";
                    selected = false;
                    command_string = "";
                    
                    
                    

                    
                }
                else
                {
                    //if canvas isn't null, we can attempt to access values and stuff, but it's still unsafe in title
                    if (popup_timer > 0)
                    {
                        popup_timer--;
                        if (popup_timer == 0)
                        {
                            interactionInfo interactionInfo = FindObjectOfType<interactionInfo>();
                            interactionInfo.infoText.color = Color.white;
                            interactionInfo.infoText.text = "";
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.Slash))
                    {
                        if (selected)
                        {
                            selected = false;
                            command_text.text = "Press / for commands";
                            command_string = "";
                        }
                        else
                        {
                            selected = true;
                            command_text.text = "/";
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
                                command_text.text = command_string;
                            }
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        if (selected)
                        {
                            selected = false;
                            command_text.text = "Press / for commands";
                            command_string = "";
                        }
                        popup_timer = 0;
                    }

                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            clickerController.bar[i] += 100000L;
                        }
                            
                            
                        
                        Log("All achievements unlocked!");
                    }
                }
            }
        }
        
    
        public void Log(string message)
        {
            Logger.LogInfo(message);
            if (canvas != null)
            {
                interactionInfo interactionInfo = FindObjectOfType<interactionInfo>();
                //      vvv      this is the way the game does it's popup text, so we'll do it the same way but get rid of the alpha manipulation
                //      vvv      longer than 2 seconds lol
                //  interactionInfo.inform(message, Color.white);
                
                interactionInfo.infoText.color = Color.white;
                interactionInfo.infoText.text = message;

                popup_timer = 400;

                
            }
        }
    }
}
