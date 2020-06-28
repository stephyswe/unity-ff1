﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{

    [Serializable]
    public class product_text
    {
        public Text name;
        public Text cost;
    }

    public Transform[] party;
    public List<GameObject> party_clones;

    public string[] party_names;

    public string shopmode;

    public Text prompt_text;
    public Text gil_display_text;
    public Text shop_type_text;

    public GameObject buy_container;
    public GameObject buy_cursor;
    public product_text[] product_Texts;

    public GameObject yes_no;
    public GameObject buy_sell_quit;
    public GameObject only_quit;

    private bool yes;
    private bool no;
    private bool sell;

    private int inn_clinic_price;
    private int player_gil;

    private SpriteController sc;

    int buy_index;

    void setup()
    {
        foreach (product_text t in product_Texts)
        {
            t.name.transform.parent.gameObject.SetActive(false);
        }

        yes_no.SetActive(false);
        buy_sell_quit.SetActive(true);
        only_quit.SetActive(false);
        buy_container.SetActive(false);

        for (int i = 0; i < GlobalControl.instance.shop_products.Count; i++)
        {
            string prod_name = GlobalControl.instance.shop_products.ElementAt(i).Key;
            int prod_cost = GlobalControl.instance.shop_products.ElementAt(i).Value;

            product_Texts[i].name.transform.parent.gameObject.SetActive(true);

            product_Texts[i].name.text = prod_name;
            product_Texts[i].cost.text = "" + prod_cost;
        }
        prompt_text.text = "What do you need?";
    }

    // Start is called before the first frame update
    void Start()
    {
        party_clones = new List<GameObject>();
        party_names = new string[4];

        buy_container.SetActive(false);

        for (int i = 0; i < 4; i++)
        {
            party_names[i] = SaveSystem.GetString("player" + (i + 1) + "_name");
        }
        
        //if (shopmode == null)
        shopmode = GlobalControl.instance.shopmode;

        if(shopmode != "inn" && shopmode != "clinic")
        {
            setup();
        }
        else
        {
            yes_no.SetActive(true);
            buy_sell_quit.SetActive(false);
            only_quit.SetActive(false);
            inn_clinic_price = GlobalControl.instance.inn_clinic_price;

            if(shopmode == "inn")
            {
                prompt_text.text = "Stay to heal and save your data? A room will be " + inn_clinic_price + "G per night.";
            }
            if(shopmode == "clinic")
            {
                int dead = 0;

                for(int i = 0; i < 4; i++)
                {
                    if (SaveSystem.GetInt("player" + (i + 1) + "_HP") <= 0)
                        dead += 1;
                }

                if (dead > 0)
                {
                    prompt_text.text = "A party member of yours has fallen. Would you like to revive them for " + inn_clinic_price + "G?";
                }
                else {
                    prompt_text.text = "You do not need my help right now.";
                    yes_no.SetActive(false);
                    only_quit.SetActive(true);
                }
            }
        }

        sc = GetComponentInChildren<SpriteController>();
        switch (shopmode)
        {
            case "armor":
                sc.set_character(0);
                shop_type_text.text = "ARMOR";
                break;
            case "b_magic":
                sc.set_character(1);
                shop_type_text.text = "BLACK MAGIC";
                break;
            case "clinic":
                sc.set_character(2);
                shop_type_text.text = "CLINIC";
                break;
            case "inn":
                sc.set_character(3);
                shop_type_text.text = "INN";
                break;
            case "item":
                sc.set_character(4);
                shop_type_text.text = "ITEM";
                break;
            case "oasis":
                sc.set_character(5);
                shop_type_text.text = "OASIS";
                break;
            case "w_magic":
                sc.set_character(6);
                shop_type_text.text = "WHITE MAGIC";
                break;
            case "weapon":
                sc.set_character(7);
                shop_type_text.text = "WEAPON";
                break;
        }
        sc.change_direction("up");

        for (int i = 0; i < 4; i++)
        {
            string player_n = "player" + (i + 1) + "_";
            string job = SaveSystem.GetString("player" + (i + 1) + "_class");
            switch (job)
            {
                case "fighter":
                    GameObject fighter = Instantiate(Resources.Load<GameObject>("party/fighter"), party[i].position, Quaternion.identity);
                    party_clones.Add(fighter);
                    fighter.transform.localScale = party[i].localScale;
                    Destroy(fighter.GetComponent<PartyMember>());
                    break;
                case "black_belt":
                    GameObject bb = Instantiate(Resources.Load<GameObject>("party/black_belt"), party[i].position, Quaternion.identity);
                    party_clones.Add(bb);
                    bb.transform.localScale = party[i].localScale;
                    Destroy(bb.GetComponent<PartyMember>());
                    break;
                case "red_mage":
                    GameObject red_mage = Instantiate(Resources.Load<GameObject>("party/red_mage"), party[i].position, Quaternion.identity);
                    party_clones.Add(red_mage);
                    red_mage.transform.localScale = party[i].localScale;
                    Destroy(red_mage.GetComponent<PartyMember>());
                    break;
                case "thief":
                    GameObject thief = Instantiate(Resources.Load<GameObject>("party/thief"), party[i].position, Quaternion.identity);
                    party_clones.Add(thief);
                    thief.transform.localScale = party[i].localScale;
                    Destroy(thief.GetComponent<PartyMember>());
                    break;
                case "white_mage":
                    GameObject white_mage = Instantiate(Resources.Load<GameObject>("party/white_mage"), party[i].position, Quaternion.identity);
                    party_clones.Add(white_mage);
                    white_mage.transform.localScale = party[i].localScale;
                    Destroy(white_mage.GetComponent<PartyMember>());
                    break;
                case "black_mage":
                    GameObject black_mage = Instantiate(Resources.Load<GameObject>("party/black_mage"), party[i].position, Quaternion.identity);
                    party_clones.Add(black_mage);
                    black_mage.transform.localScale = party[i].localScale;
                    Destroy(black_mage.GetComponent<PartyMember>());
                    break;
            }
        }

        player_gil = SaveSystem.GetInt("gil");

        gil_display_text.text = "G: " + player_gil;
    }

    public void yes_true()
    {
        yes = true;
        no = false;
    }

    public void no_true()
    {
        yes = false;
        no = true;
    }

    public void buy()
    {
        buy_container.SetActive(true);
        buy_cursor.SetActive(true);
        buy_sell_quit.SetActive(false);
        prompt_text.text = "What would you like?";
    }

    public void sell_true()
    {
        sell = true;
    }

    public void exit_shop()
    {
        foreach (GameObject t in party_clones)
        {
            Destroy(t);
        }

        GlobalControl.instance.overworld_scene_container.SetActive(true);
        SceneManager.UnloadScene("Shop");
    }

    // Update is called once per frame
    void Update()
    {
        if(player_gil != SaveSystem.GetInt("gil"))
        {
            player_gil = SaveSystem.GetInt("gil");

            gil_display_text.text = "G: " + player_gil;
        }

        if((shopmode == "inn" || shopmode == "clinic") && (yes || no))
        {
            if(yes && player_gil < inn_clinic_price)
            {
                Debug.Log("You don't have enough money!");
            }
            else if (yes)
            {
                SaveSystem.SetInt("gil", player_gil - inn_clinic_price);
                for(int i = 0; i < 4; i++)
                {
                    if(SaveSystem.GetInt("player" + (i + 1) + "_HP") > 0)
                        SaveSystem.SetInt("player" + (i + 1) + "_HP", SaveSystem.GetInt("player" + (i + 1) + "_maxHP"));
                }

                GlobalControl.instance.player.map_handler.save_inn();

                SaveSystem.SaveToDisk();
            }

            exit_shop();
        }
    }

    public void buy_1()
    {
        StartCoroutine(buy_select(0));
    }

    public void buy_2()
    {
        StartCoroutine(buy_select(1));
    }

    public void buy_3()
    {
        StartCoroutine(buy_select(2));
    }

    public void buy_4()
    {
        StartCoroutine(buy_select(3));
    }

    public void buy_5()
    {
        StartCoroutine(buy_select(4));
    }

    IEnumerator buy_select(int index)
    {
        buy_cursor.SetActive(false);

        yes = false;
        no = false;

        string p_name = product_Texts[index].name.text;
        int p_cost = Int32.Parse(product_Texts[index].cost.text);

        prompt_text.text = "Is " + p_cost + " G for a " + p_name + " okay?";
        yes_no.SetActive(true);

        while (!yes && !no)
            yield return null;

        if (yes)
        {
            if(player_gil >= p_cost)
            {
                string category = item_category(p_name);

                switch (category)
                {
                    case "items":
                        Dictionary<string, int> items = SaveSystem.GetStringIntDict("items");
                        if (items.ContainsKey(p_name))
                            items[p_name] = items[p_name] + 1;
                        else
                            items.Add(p_name, 1);
                        SaveSystem.SetStringIntDict("items", items);
                        SaveSystem.SetInt("gil", player_gil - p_cost);
                        break;
                    case "weapons":
                        List<string> weapons = SaveSystem.GetStringList("weapons");
                        weapons.Add(p_name);
                        SaveSystem.SetStringList("weapons", weapons);
                        SaveSystem.SetInt("gil", player_gil - p_cost);
                        break;
                    case "armor":
                        List<string> armor = SaveSystem.GetStringList("armor");
                        armor.Add(p_name);
                        SaveSystem.SetStringList("armor", armor);
                        SaveSystem.SetInt("gil", player_gil - p_cost);
                        break;
                }
            }
            else
            {
                prompt_text.text = "You don't have enough money";
                yes_no.SetActive(false);
                yield return new WaitForSeconds(1.5f);
            }
        }

        setup();
    }

    public string item_category(string item)
    {
        List<string> items = new List<string> {"Heal Potion", "Pure Potion", "Tent"};
        List<string> weapons = new List<string> {"Wooden Staff", "Small Dagger", "Wooden Nunchuck", "Rapier", "Iron Hammer"};
        List<string> armor = new List<string> {"Cloth", "Wooden Armor", "Chain Armor" };
        List<string> w_magic = new List<string> {"Cure", "Harm", "Fog", "Ruse"};
        List<string> b_magic = new List<string> {"Fire", "Slep", "Lock", "Lit"};

        if (items.Contains(item))
            return "items";
        if (weapons.Contains(item))
            return "weapons";
        if (b_magic.Contains(item))
            return "black magic";
        if (w_magic.Contains(item))
            return "white magic";
        return "armor";
    }
}
