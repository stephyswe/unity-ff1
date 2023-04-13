# Phantasia
Multiplatform Final Fantasy 1 port in Unity

![Phantasia](Assets/misc/title_screen.png)

## Rescue Plan.

BIN FOLDER:
C:/Users/Stephanie/AppData/LocalLow/Charles Averill/Phantasia/party.json


## 01. Openining. Clean.

- Open Project in Unity
- Add All to Git
- Add Git Ignore
- Remove all Changed fluff.
- Re-open Unity
- Remove all CSPROJ File in Preferences - Edit. 


## Game handle.

* Fight
- Cursor: "p" , "w/s" move

## Checkup.

## MENU - Open "Q" Close "Cursor X" 
* BUG! Close on Button, not Cursor
* WORK! Bag click.
* WORK! Character click
 
""
NAME		ICON
L1
35-35
0-0-0-0
0-0-0-0
""
 
### Menu - Character - 
* > - Go to Menu
* x - Quit Menu
* WORK! Can click Weapon/Armor. Close with ">"
* BUG! Cannot click Item/Magic. Close with ">"
* BUG! Why NA on DAMAGE
* BUG! Why NA on ABSORB

""
name x ICON white mage > LVL1
ITEM 	EXP.POINTS	0	WEAPON
MAGIC	FOR LEV UP	40	ARMOR
STR
AGI		5		DAMAGE	NA
INT		5		HIT		5
VIT		15		ABSORB	NA
LUCK	5		EVADE	53
""

## City Zone
* BUG! Fight shouldn't be possible.

## World Zone
* WORK! Fight, "p", then "p" until complete.

## City. 
- Talk
* BUG! "p" re-open dialogue fast. 
* WORK! Dialogue show.

- - Shops 

- Shop Armor
* WORK! UI Correct
* WORK! Buy Menu show.
* BUG! Sell Menu not show.
* WORK! Buy Armor work.
* WORK! Buy Armor twice work.
* BUG! Quit Menu work But have error LOG. // Same as Weapon.

- Shop Weapon
* WORK! UI Correct
* WORK! Buy Menu show.
* BUG! Sell Menu not show.
* WORK! Buy Weapon work.
* WORK! Buy Weapon twice work.
* BUG! Quit Menu work But have error LOG.
"""
MissingReferenceException: The object of type 'CursorController' has been destroyed but you are still trying to access it.
Your script should either check if it is null or you should not destroy the object.
CursorController.move () (at Assets/Scripts/Battling/CursorController.cs:184)
CursorController.Update () (at Assets/Scripts/Battling/CursorController.cs:154)
"""

- - Shop - Check Weapon in ESC MENU.
* "" Weapon can be assigned but not armor. ""
- WORK! Bag show weapon/armor

- Assign 
* OPEN GIVE/DROP Menu on Click. 

* Weapon show in Character menu after GIVE.
* Click open Equip Party.
* Equip close Menu and show stats.
* !BUG Don't change stats after adding.

- Inn
* BUG! UI - Show names to LEFT SCREEN.
* BUG! "NoName if not provided"
* WORK! Name show if provided. 
* BUG! Cursor move in name and "yes/no"
* WORK! Save game data to binary.
* WORK! Money for save - 30G.

- Finish First Boss
- Trigger Story Start- City. Save 100%

## Bugs

## Open Project in 2019

- Play 
* Starts Shop Scene with "Name: " for each.
* Open Title Scene.
* Write Instructions

** Add JSON instead of Binary. **
* Package com.unity.nuget.newtonsoft-json
* Assets\Utils\SaveGame\Scripts\SaveSystem\SaveSystemData.cs

using Newtonsoft.Json;
...
public class SerializatorBinary
{

    public static void SaveBinary(DataState state, string dataPath)
    {
        string jsonData = JsonConvert.SerializeObject(state);
        File.WriteAllText(dataPath, jsonData);
    }

    public static DataState LoadBinary(string dataPath)
    {
        string jsonData = File.ReadAllText(dataPath);
        DataState state = JsonConvert.DeserializeObject<DataState>(jsonData);


        return state;
    }
}

-

* Assets\Utils\SaveGame\Scripts\SaveSystemSetup.cs
[SerializeField] private string fileName = "Profile.json"; // file to save with the specified resolution

* EDITOR - Save System Setup - Change Filename to party.json

- Test Play.
** WORK? ** 
- Load? Yes 
- Start? Yes 
- Save? Yes

** Redo Check LOG **
## ESC Menu.
- Work fine. Impressed.


