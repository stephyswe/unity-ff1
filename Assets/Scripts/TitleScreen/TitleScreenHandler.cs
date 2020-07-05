using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenHandler : MonoBehaviour
{
    
    public LoadingCircle loading_circle;
    
    public AudioSource classic;
    public AudioSource remaster;
    public AudioSource GBA;
    
    private string[] names;
    
    public GameObject title;
    public GameObject char_select;
    public GameObject settings_container;
    
    public InputField[] fields;
    public SpriteController[] sprite_controllers;

    public Slider battle_speed_slider;
    public Text bss_text;
    
    private bool new_save = false;

    // Start is called before the first frame update
    void Start()
    {

        title.SetActive(true);
        char_select.SetActive(false);
        settings_container.SetActive(false);
    
        string bin_path = Application.persistentDataPath + "/party.bin";
        
        input_allowed = true;
        
        names = new string[]{"Matt", "Alta", "Ivan", "Cora"};
        
        if(!System.IO.File.Exists(bin_path)){
            init_save_file();
            new_save = true;
        }
        
        if(SaveSystem.GetBool("classic_music")){
            classic.volume = 1f;
            remaster.volume = 0f;
            GBA.volume = 0f;
        }
        else if (SaveSystem.GetBool("remaster_music"))
        {
            remaster.volume = 1f;
            classic.volume = 0f;
            GBA.volume = 0f;
        }
        else
        {
            GBA.volume = 1f;
            classic.volume = 0f;
            remaster.volume = 0f;
        }

        battle_speed_slider.value = SaveSystem.GetFloat("battle_speed");
    }
    
    public void init_save_file(){
        //SaveSystem.SetBool("classic_music", true);
        
        SaveSystem.SetBool("in_submap", false);
        SaveSystem.SetBool("inside_of_room", false);

        RandomEncounterHandler reh = gameObject.AddComponent<RandomEncounterHandler>();
        reh.gen_seed();

        SaveSystem.SetInt("reh_seed", reh.seed);

        SaveSystem.SetInt("gil", 400);
    
        SaveSystem.SetFloat("overworldX", -1f);
        SaveSystem.SetFloat("overworldY", -5f);
        
        SaveSystem.SetString("player1_name", names[0]);
        SaveSystem.SetString("player2_name", names[1]);
        SaveSystem.SetString("player3_name", names[2]);
        SaveSystem.SetString("player4_name", names[3]);

        SaveSystem.SetStringIntDict("items", new Dictionary<string, int>());

        SaveSystem.SetBool("earth_orb", false);
        SaveSystem.SetBool("fire_orb", false);
        SaveSystem.SetBool("water_orb", false);
        SaveSystem.SetBool("wind_orb", false);

        SaveSystem.SaveToDisk();
        Debug.Log("Done initializing");
    }
    
    int frames_since_music_switch = 15;

    // Update is called once per frame
    void Update()
    {
        
        frames_since_music_switch += 1;
        
        if(input_allowed){
            /*
            if(Input.GetKeyDown("h") && names.Length > 0){
                foreach(string name in names){
                    SaveSystem.SetInt(name + "_maxHP", 100);
                    SaveSystem.SetInt(name + "_HP", 100);
                }
                Debug.Log("Healed your party");
            }
            */
        }
        
    }

    public void set_classic_music()
    {
        classic.volume = 1f;
        remaster.volume = 0f;
        GBA.volume = 0f;

        SaveSystem.SetBool("classic_music", true);
        SaveSystem.SetBool("remaster_music", false);

        classic.Play();

        SaveSystem.SaveToDisk();
    }

    public void set_gba_music()
    {
        classic.volume = 0f;
        remaster.volume = 0f;
        GBA.volume = 1f;

        SaveSystem.SetBool("classic_music", false);
        SaveSystem.SetBool("remaster_music", false);

        GBA.Play();

        SaveSystem.SaveToDisk();
    }

    public void set_remastered_music()
    {
        classic.volume = 0f;
        remaster.volume = 1f;
        GBA.volume = 0f;

        SaveSystem.SetBool("classic_music", false);
        SaveSystem.SetBool("remaster_music", true);

        remaster.Play();

        SaveSystem.SaveToDisk();
    }

    public void exit(){
        #if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    public bool input_allowed;
    
    public void continue_game(){
        input_allowed = false;
        StartCoroutine(load());
    }
    
    public void new_game(){
        input_allowed = false;
        title.SetActive(false);
        char_select.SetActive(true);
    }

    public void settings()
    {
        if (!settings_container.active)
        {
            input_allowed = false;
            title.SetActive(false);
            settings_container.SetActive(true);
        }
        else
        {
            input_allowed = true;
            title.SetActive(true);
            settings_container.SetActive(false);
        }
    }

    public void battle_speed_set()
    {
        SaveSystem.SetFloat("battle_speed", battle_speed_slider.value);
        bss_text.text = "" + (int)(battle_speed_slider.value * 1000f);
        SaveSystem.SaveToDisk();
    }
    
    public void new_game_start(){
        for(int i = 0; i < fields.Length; i++){
            names[i] = fields[i].text;
        }
        for(int i = 0; i < sprite_controllers.Length; i++){

            int character_index = 0;

            string player_n = "player" + (i + 1) + "_";

            string p_class = sprite_controllers[i].get_class();
            SaveSystem.SetString(player_n + "class", p_class);
            
            switch(p_class){
                case "fighter":
                    SaveSystem.SetInt(player_n + "strength", 20);
                    SaveSystem.SetInt(player_n + "agility", 5);
                    SaveSystem.SetInt(player_n + "intelligence", 1);
                    SaveSystem.SetInt(player_n + "vitality", 10);
                    SaveSystem.SetInt(player_n + "luck", 5);
                    SaveSystem.SetInt(player_n + "HP", 35);
                    SaveSystem.SetFloat(player_n + "hit_percent", .1f);
                    SaveSystem.SetFloat(player_n + "magic_defense", .15f);

                    character_index = 3;

                    break;
                case "black_belt":
                    SaveSystem.SetInt(player_n + "strength", 5);
                    SaveSystem.SetInt(player_n + "agility", 5);
                    SaveSystem.SetInt(player_n + "intelligence", 5);
                    SaveSystem.SetInt(player_n + "vitality", 20);
                    SaveSystem.SetInt(player_n + "luck", 5);
                    SaveSystem.SetInt(player_n + "HP", 33);
                    SaveSystem.SetFloat(player_n + "hit_percent", .05f);
                    SaveSystem.SetFloat(player_n + "magic_defense", .1f);

                    character_index = 0;

                    break;
                case "red_mage":
                    SaveSystem.SetInt(player_n + "strength", 10);
                    SaveSystem.SetInt(player_n + "agility", 10);
                    SaveSystem.SetInt(player_n + "intelligence", 10);
                    SaveSystem.SetInt(player_n + "vitality", 5);
                    SaveSystem.SetInt(player_n + "luck", 5);
                    SaveSystem.SetInt(player_n + "HP", 30);
                    SaveSystem.SetFloat(player_n + "hit_percent", .07f);
                    SaveSystem.SetFloat(player_n + "magic_defense", .2f);

                    character_index = 7;

                    break;
                case "thief":
                    SaveSystem.SetInt(player_n + "strength", 5);
                    SaveSystem.SetInt(player_n + "agility", 10);
                    SaveSystem.SetInt(player_n + "intelligence", 5);
                    SaveSystem.SetInt(player_n + "vitality", 5);
                    SaveSystem.SetInt(player_n + "luck", 15);
                    SaveSystem.SetInt(player_n + "HP", 30);
                    SaveSystem.SetFloat(player_n + "hit_percent", .05f);
                    SaveSystem.SetFloat(player_n + "magic_defense", .15f);

                    character_index = 9;

                    break;
                case "white_mage":
                    SaveSystem.SetInt(player_n + "strength", 5);
                    SaveSystem.SetInt(player_n + "agility", 5);
                    SaveSystem.SetInt(player_n + "intelligence", 15);
                    SaveSystem.SetInt(player_n + "vitality", 10);
                    SaveSystem.SetInt(player_n + "luck", 5);
                    SaveSystem.SetInt(player_n + "HP", 28);
                    SaveSystem.SetFloat(player_n + "hit_percent", .05f);
                    SaveSystem.SetFloat(player_n + "magic_defense", .2f);

                    character_index = 10;

                    break;
                case "black_mage":
                    SaveSystem.SetInt(player_n + "strength", 1);
                    SaveSystem.SetInt(player_n + "agility", 10);
                    SaveSystem.SetInt(player_n + "intelligence", 20);
                    SaveSystem.SetInt(player_n + "vitality", 1);
                    SaveSystem.SetInt(player_n + "luck", 10);
                    SaveSystem.SetInt(player_n + "HP", 25);
                    SaveSystem.SetFloat(player_n + "hit_percent", .055f);
                    SaveSystem.SetFloat(player_n + "magic_defense", .2f);

                    character_index = 1;

                    break;
            }

            SaveSystem.SetStringList(player_n + "armor_inventory", new List<string>());
            SaveSystem.SetStringList(player_n + "weapons_inventory", new List<string>());

            SaveSystem.SetString(player_n + "shield", "");
            SaveSystem.SetString(player_n + "helmet", "");
            SaveSystem.SetString(player_n + "armor", "");
            SaveSystem.SetString(player_n + "glove", "");

            SaveSystem.SetString(player_n + "weapon", "");

            SaveSystem.SetInt(player_n + "exp", 0);
            SaveSystem.SetBool(player_n + "poison", false);
            SaveSystem.SetBool(player_n + "stone", false);
            SaveSystem.SetInt(player_n + "maxHP", SaveSystem.GetInt(player_n + "HP"));

            if (i == 0)
            {
                SaveSystem.SetInt("character_index", character_index);
            }
        }
        init_save_file();
        StartCoroutine(load());
    }
    
    IEnumerator load(){
        loading_circle.gameObject.SetActive(true);
        loading_circle.start_loading_circle();
        yield return new WaitForSeconds(.5f);
        Cursor.visible = false;
        SceneManager.LoadSceneAsync("Overworld");
    }
}
