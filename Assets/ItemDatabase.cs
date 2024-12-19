using UnityEngine;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using System.IO;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;



public class ItemDatabase : MonoBehaviour
{
    public List<Item> items;  // Lista kaikista pelin esineistä
    private SheetsService sheetsService;
    private string spreadsheetId = "1KJUwxzbdX_IX0-TKVh7Riq4gR-cUR-8-NPTm0Pi0XUg";  // Vaihda omaan taulukon ID:hen
    private string range = "GameItems!A2:AC";  // Vaihda oman taulukon alueeseen (tämä lukee taulukon rivit A2 alkaen)
    public event Action OnDatabaseLoaded;
    public bool isDatabaseLoaded { get; private set; } = false;

    public async Task LoadDatabaseFromSheetsAsync()
    {
        // Simuloi latausprosessi
        await Task.Delay(1000); // Esimerkki: Sheetsistä lataaminen
        isDatabaseLoaded = true;
        Debug.Log("Database loaded from Sheets!");
    }

    void Start()
    {
        
        // Autentikointi ja yhteyden luominen Google Sheetsiin
        AuthorizeGoogleSheets();
        ReadGameItemsData();
        DebugDatabaseContents();
    }

    void AuthorizeGoogleSheets()
    {
        GoogleCredential credential;

        // Lataa käyttöoikeustiedostot (credentials.json)
        using (var stream = new FileStream("Assets/Resources/credentials.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped(SheetsService.Scope.Spreadsheets);
        }

        // Luo SheetsService
        sheetsService = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "GameItems Google Sheets API",
        });
    }

    void ReadGameItemsData()
    {
        // Haetaan data Google Sheetsistä
        SpreadsheetsResource.ValuesResource.GetRequest request = sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
        ValueRange response = request.Execute();

        // Käydään rivit läpi ja luodaan Item-objektit
        IList<IList<object>> values = response.Values;

        if (values != null && values.Count > 0)
        {
            foreach (var row in values)
            {
            if (row == null || row.Count < 3)
            {
                Debug.LogWarning("Row is null or does not have enough columns.");
                continue; // Hypätään tyhjien tai liian lyhyiden rivien yli
            }
                // Tarkistetaan, minkä tyyppinen esine on kyseessä (Potion, Equipment jne.)
                string itemType = row[2].ToString().Trim();

                Item newItem = null;

                if (itemType == "Potion")
                {
                    // Luo Potion
                    newItem = CreatePotionFromRow(row);
                }
                else if (itemType == "Equipment")
                {
                    // Luo Equipment
                    newItem = CreateEquipmentFromRow(row);
                }
                else
                {
                    
                    // Luo tavallinen Item
                    newItem = CreateItemFromRow(row);
                }

                if (newItem != null)
                {
                    items.Add(newItem);  // Lisää uusi esine listaan
                }
            }
        }
        else
        {
            Debug.Log("No data found.");
        }
    }

    // Luo tavallinen Item
// Luo tavallinen Item
// Luo tavallinen Item
    Item CreateItemFromRow(IList<object> row)
    {
        Item newItem = ScriptableObject.CreateInstance<Item>();

        // Asetetaan oletusarvo, jos kenttä on tyhjä
        newItem.itemName = row[0] != null && !string.IsNullOrEmpty(row[0].ToString()) ? row[0].ToString() : "Unnamed Item";  // Oletusarvona "Unnamed Item"
        newItem.infoText = row[1] != null && !string.IsNullOrEmpty(row[1].ToString()) ? row[1].ToString() : "No description";  // Oletusarvona "No description"
        newItem.itemType = row[2].ToString().Trim();  // Tämä on string, joten se ei tarvitse tarkistusta
      


        // Käsitellään isStackable ja quantity, jos kenttä on tyhjä, asetetaan oletusarvoksi false tai 0
        
        newItem.quantity = row[6] != null && !string.IsNullOrEmpty(row[6].ToString()) ? int.Parse(row[6].ToString()) : 1;  // Asetetaan oletusarvoksi 1
        newItem.sellPrice = row[7] != null && !string.IsNullOrEmpty(row[7].ToString()) ? int.Parse(row[7].ToString()) : 0;  // Asetetaan oletusarvoksi 0
        newItem.usageText = "test";

        // Haetaan sprite Google Sheetsistä (sarakkeen "Sprite" arvon perusteella)
        string spriteName = row[26] != null && !string.IsNullOrEmpty(row[26].ToString()) ? row[26].ToString() : "default_sprite";  // Oletusarvo "default_sprite"
        newItem.icon = LoadSprite(spriteName, newItem.itemType);  // Ladataan sprite "Item"-kansiosta
        newItem.isStackable = row[28] != null && !string.IsNullOrEmpty(row[28].ToString()) ? bool.Parse(row[28].ToString()) : false;

        return newItem;

    }


    // Luo Potion-tyyppinen Item
    // Luo Potion-tyyppinen Item
    Potion CreatePotionFromRow(IList<object> row)
    {
        Potion newPotion = ScriptableObject.CreateInstance<Potion>();
        newPotion.itemName = row[0] != null ? row[0].ToString() : "";  // Asetetaan tyhjä merkkijono, jos itemName on tyhjä
        newPotion.infoText = row[1] != null ? row[1].ToString() : "";  // Asetetaan tyhjä merkkijono, jos infoText on tyhjä
        newPotion.itemType = row[2].ToString().Trim();  // Tämä on string, joten se ei tarvitse tarkistusta
        

        newPotion.quantity = row[6] != null && !string.IsNullOrEmpty(row[6].ToString()) ? int.Parse(row[6].ToString()) : 1;  // Asetetaan oletusarvoksi 1
        newPotion.sellPrice = row[7] != null && !string.IsNullOrEmpty(row[7].ToString()) ? int.Parse(row[7].ToString()) : 0;  // Asetetaan oletusarvoksi 0

        // Käsitellään healAmount ja manaAmount, jos kenttä on tyhjä, asetetaan oletusarvoksi 0
        newPotion.healAmount = row[23] != null && !string.IsNullOrEmpty(row[23].ToString()) ? int.Parse(row[23].ToString()) : 0;
        newPotion.manaAmount = row[24] != null && !string.IsNullOrEmpty(row[24].ToString()) ? int.Parse(row[24].ToString()) : 0;

        // Haetaan sprite Google Sheetsistä (sarakkeen "Sprite" arvon perusteella)
        string spriteName = row[26] != null && !string.IsNullOrEmpty(row[26].ToString()) ? row[26].ToString() : "default_sprite"; // Asetetaan oletusarvo "default_sprite", jos spriteName on tyhjä
        
        newPotion.icon = LoadSprite(spriteName, newPotion.itemType);  // Ladataan sprite "Potion"-kansiosta
        newPotion.isStackable = row[28] != null && !string.IsNullOrEmpty(row[28].ToString()) ? bool.Parse(row[28].ToString()) : false;

        return newPotion;

    }


    // Luo Equipment-tyyppinen Item
// Luo Equipment-tyyppinen Item
    Equipment CreateEquipmentFromRow(IList<object> row)
    {
        Equipment newEquipment = ScriptableObject.CreateInstance<Equipment>();
        newEquipment.itemName = row[0].ToString();  // Tämä on string, joten se ei tarvitse tarkistusta
        newEquipment.infoText = row[1] != null && !string.IsNullOrEmpty(row[1].ToString()) ? row[1].ToString() : "No description";  // Oletusarvona "No description"
        newEquipment.itemType = row[2].ToString().Trim();  // Tämä on string, joten se ei tarvitse tarkistusta
        

        newEquipment.type = string.IsNullOrEmpty(row[3]?.ToString()) ? EquipmentType.Armor : (EquipmentType)Enum.Parse(typeof(EquipmentType), row[3].ToString());
        newEquipment.slot = string.IsNullOrEmpty(row[4]?.ToString()) ? SlotType.Head : (SlotType)Enum.Parse(typeof(SlotType), row[4].ToString());
        newEquipment.element = string.IsNullOrEmpty(row[5]?.ToString()) ? Element.Neutral : (Element)Enum.Parse(typeof(Element), row[5].ToString());
        newEquipment.quantity = string.IsNullOrEmpty(row[6]?.ToString()) ? 0 : int.Parse(row[6].ToString());
        newEquipment.sellPrice = string.IsNullOrEmpty(row[7]?.ToString()) ? 0 : int.Parse(row[7].ToString());
        newEquipment.damageValue = string.IsNullOrEmpty(row[8]?.ToString()) ? 0 : int.Parse(row[8].ToString());
        newEquipment.weaponAttackSpeed = string.IsNullOrEmpty(row[10]?.ToString()) ? 0 : float.Parse(row[10].ToString());
        newEquipment.strValue = string.IsNullOrEmpty(row[11]?.ToString()) || !int.TryParse(row[11]?.ToString(), out int result) ? 0 : result;
        newEquipment.vitValue = string.IsNullOrEmpty(row[12]?.ToString()) ? 0 : int.Parse(row[12].ToString());
        newEquipment.agiValue = string.IsNullOrEmpty(row[13]?.ToString()) ? 0 : int.Parse(row[13].ToString());
        newEquipment.dexValue = string.IsNullOrEmpty(row[14]?.ToString()) ? 0 : int.Parse(row[14].ToString());
        newEquipment.intValue = string.IsNullOrEmpty(row[15]?.ToString()) ? 0 : int.Parse(row[15].ToString());
        newEquipment.spiritValue = string.IsNullOrEmpty(row[16]?.ToString()) ? 0 : int.Parse(row[16].ToString());
        newEquipment.hitValue = string.IsNullOrEmpty(row[17]?.ToString()) ? 0 : int.Parse(row[17].ToString());
        newEquipment.defValue = string.IsNullOrEmpty(row[18]?.ToString()) ? 0 : int.Parse(row[18].ToString());
        newEquipment.critValue = string.IsNullOrEmpty(row[19]?.ToString()) ? 0 : int.Parse(row[19].ToString());
        newEquipment.dodgeValue = string.IsNullOrEmpty(row[20]?.ToString()) ? 0 : int.Parse(row[20].ToString());
        newEquipment.hpRegValue = string.IsNullOrEmpty(row[21]?.ToString()) ? 0f : float.Parse(row[21].ToString());
        newEquipment.spRegValue = string.IsNullOrEmpty(row[22]?.ToString()) ? 0f : float.Parse(row[22].ToString());
        newEquipment.rarity = string.IsNullOrEmpty(row[25]?.ToString()) ? Rarity.Common : (Rarity)Enum.Parse(typeof(Rarity), row[25].ToString());

        // Haetaan sprite
        string spriteName = row[26] != null && !string.IsNullOrEmpty(row[26].ToString()) ? row[26].ToString() : "default_sprite"; // Asetetaan oletusarvo "default_sprite", jos spriteName on tyhjä
        newEquipment.icon = LoadSprite(spriteName, newEquipment.itemType);   // Ladataan sprite "Equipment"-kansiosta
        newEquipment.model = string.IsNullOrEmpty(row[27]?.ToString()) ? EquipmentModel.Sword : (EquipmentModel)Enum.Parse(typeof(EquipmentModel), row[27].ToString());
        newEquipment.isStackable = row[28] != null && !string.IsNullOrEmpty(row[28].ToString()) ? bool.Parse(row[28].ToString()) : false;

        return newEquipment;

    }



    // Lataa sprite Resources-kansiosta
// Lataa sprite Resources-kansiosta
// Lataa sprite Resources-kansiosta
        Sprite LoadSprite(string itemName, string itemType)
        {
            
            // Muodostetaan sprite-nimi ilman "_sprite" -päätettä
            string spriteName = itemName.ToLower().Replace(" ", "_");

            // Tarkistetaan itemType ja määritellään polku oikein
            string path = "";
            if (itemType == "Potion")
            {
                path = "Sprites/Potion/" + spriteName;  // Potion kansio
            }
            else if (itemType == "Equipment")
            {
                path = "Sprites/Equipment/" + spriteName;  // Equipment kansio
            }
            else
            {
                Debug.LogError("Unknown item type: " + itemType);
                return null;
            }

            // Lataa sprite määritellyn polun mukaan
            Sprite loadedSprite = Resources.Load<Sprite>(path);

            // Debug-tuloste, jotta tiedämme, ladataanko sprite oikein
            if (loadedSprite == null)
            {
                Debug.LogError($"Sprite '{path}' not found in Resources!");
            }
            else
            {
                Debug.Log($"Sprite '{path}' loaded successfully.");
            }

            return loadedSprite;
        }




    // Hae esine nimen perusteella
public Item GetItemByName(string name)
{
    foreach (Item item in items)
    {
        
        if (item.itemName.Trim().Equals(name.Trim(), StringComparison.OrdinalIgnoreCase))

        {
            return item;
        }
    }
   
    return null;
}
    public void DebugDatabaseContents()
    {
        foreach (Item item in items)
        {
            Debug.Log($"Database contains: {item.itemName}");
        }
    }
}
