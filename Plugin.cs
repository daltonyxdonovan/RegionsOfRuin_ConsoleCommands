using BepInEx;
using UnityEngine;

namespace RegionsOfRuin_ConsoleCommands
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        GameObject player;
        Canvas canvas;
        TextMesh popup_text;
        TextMesh command_text;

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
                    canvas = new GameObject("Canvas", typeof(Canvas)).GetComponent<Canvas>();
                    //create popup_text and set it's parent to canvas
                    popup_text = new GameObject("Popup Text", typeof(TextMesh)).GetComponent<TextMesh>();
                    popup_text.transform.SetParent(canvas.transform);
                    
                    popup_text.fontSize = 1;
                    popup_text.color = Color.white;
                    popup_text.text = "";

                    //do the same for command_text, but it's position should be -13.1818 -8.1 0
                    command_text = new GameObject("Command Text", typeof(TextMesh)).GetComponent<TextMesh>();
                    command_text.transform.SetParent(canvas.transform);
                    
                    command_text.fontSize = 1;
                    command_text.color = Color.white;
                    command_text.text = "Press / for commands";
                    selected = false;
                    command_string = "";
                    //set position to (-12,8)
                    
                    

                    //bring it to the front of the 2d screen
                    canvas.planeDistance = 1;
                    //set it to the 'UI' layer
                    canvas.gameObject.layer = 5;
                }
            }
        }
    }
}
