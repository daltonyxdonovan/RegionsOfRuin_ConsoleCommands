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
        GameObject command_text;
        public GameObject clear;
        public Text clearText;
        private Vector2 origin;
        private float timer = 1.25f;
        private GameObject info;
        public Text infoText;


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
                    
                    int screen_width = Screen.width;
                    int screen_height = Screen.height;

                    //do the same for command_text
                    command_text = new GameObject("command_text");
                    command_text.AddComponent<interactionInfo>();
                    this.info = this.infoText.gameObject;
                    this.infoText.color = Color.white;
                    this.origin = new Vector3((int)screen_width/2, screen_height-(int)screen_height/8, 0);

                    command_text.transform.SetParent(canvas.transform);
                    interactionInfo interactionInfo = command_text.GetComponentInChildren<interactionInfo>();

                    GameObject info = interactionInfo.infoText.gameObject;
                    Vector3 origin = info.transform.position;

                    

                    

                    
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
                            
                            command_text.GetComponentInChildren<interactionInfo>().infoText.color = Color.white;
                            command_text.GetComponentInChildren<interactionInfo>().infoText.text = "";
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.Slash))
                    {
                        if (selected)
                        {
                            selected = false;
                            command_text.GetComponentInChildren<interactionInfo>().infoText.text = "Press / for commands";
                            command_string = "";
                        }
                        else
                        {
                            selected = true;
                            command_text.GetComponentInChildren<interactionInfo>().infoText.text = "/";
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
                                command_text.GetComponentInChildren<interactionInfo>().infoText.text = command_string;
                            }
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        if (selected)
                        {
                            selected = false;
                            command_text.GetComponentInChildren<interactionInfo>().infoText.text = "Press / for commands";
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
                
                //      vvv      this is the way the game does it's popup text, so we'll do it the same way but get rid of the alpha manipulation
                //      vvv      longer than 2 seconds lol
                //  interactionInfo.inform(message, Color.white);
                
                command_text.GetComponentInChildren<interactionInfo>().infoText.color = Color.white;
                command_text.GetComponentInChildren<interactionInfo>().infoText.text = message;

                popup_timer = 400;

                
            }
        }
    }
}
