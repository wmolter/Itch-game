using UnityEngine;
using System.Collections.Generic;
using System.IO;
using OneKnight.Loading;

namespace OneKnight {
    public static class ItemInfo {

        public static class Category {
            public const string Consumable = "Consumable";
        }

        private static Dictionary<string, InventoryItem.Data> allItems;

        public static void LoadData() {
            allItems = new Dictionary<string, InventoryItem.Data>();

            ReadItems(Application.dataPath + "/items.oktable", allItems);
            LoadItemText();
        }

        private static void LoadItemText() {
            string filepath = Application.dataPath + "/item_descriptions_english.oktable";
            ReadDescriptions(filepath, null);
        }

        public static void ReadDescriptions(string filepath, Loading.FullDescription.ReadAction<InventoryItem.Data> extraLineAction) {
            Loading.FullDescription.ReadDescriptions(filepath, ItemInfo.allItems, extraLineAction);
        }

        public static void ReadItems<T>(string filepath, Dictionary<string, T> readInto) where T : InventoryItem.Data {
            IEnumerator<SavingUtils.TableBit> enumerator = SavingUtils.ReadOKTable(filepath).GetEnumerator();
            T item;
            enumerator.MoveNext();
            bool hasEntries = true;
            while(hasEntries) {
                SavingUtils.TableBit bit = enumerator.Current;
                if(!bit.newEntry)
                    Debug.LogWarning("Next entry flagged incorrectly while parsing for " + filepath);
                string id = bit.value;
                int sortingID = SavingUtils.NextInt(enumerator);
                try {
                    string category = SavingUtils.Next(enumerator);
                    item = (T)InitializeWithCategory(category);
                    item.id = id;
                    item.category = category;
                    readInto[id] = item;

                    item.spriteName = "Sprites/" + SavingUtils.Next(enumerator);
                    item.baseValue = SavingUtils.NextFloat(enumerator);
                    item.volumePer = SavingUtils.NextFloat(enumerator);

                    if(!enumerator.MoveNext())
                        break;
                    bit = enumerator.Current;
                    if(bit.newEntry)
                        continue;


                    //here, either it's the stack limit, or a throwaway line declaring property start.
                    if(int.TryParse(bit.value, out item.stackLimit)) {
                        //go on to the throwaway line if it works
                        if(!enumerator.MoveNext())
                            break;
                        bit = enumerator.Current;
                        if(bit.newEntry)
                            continue;
                    }
                    //throw away line
                    if(!enumerator.MoveNext())
                        break;
                    bit = enumerator.Current;
                    if(bit.newEntry)
                        continue;

                    //now we read in the object args.
                    SavingUtils.ReadArguments(enumerator, item, id);
                    item.AssignProperties();

                    if(!enumerator.MoveNext())
                        break;
                    bit = enumerator.Current;
                    if(bit.newEntry)
                        continue;
                } catch (System.Exception ex) {
                    Debug.LogWarning("Error while loading item: " + id + "\n" + ex);
                    if(readInto.ContainsKey(id))
                        readInto.Remove(id);
                    SavingUtils.NextEntry(enumerator);
                }
            }
        }

        public static InventoryItem.Data InitializeWithCategory(string category) {
            
                return new InventoryItem.Data();
            
        }
        

        private static void InitTools() {

        }

        private static InventoryItem.Data GetDefaultData() {
            return new InventoryItem.Data {
                id = "",
                name = "",
                stackLimit = 1,
                baseValue = 0,
                description = "",
                flavor = ""
            };
        }

        public static InventoryItem Create(string id, int count) {
            return allItems[id].Create(count);
        }

        public static InventoryItem Create(string id) {
            return allItems[id].Create();
        }

        public static InventoryItem Create(InventoryItem drop) {
            return Create(drop.ID, drop.count);
        }

        public static int SortingID(string id) {
            return allItems[id].sortingID;
        }

        public static string GetCategory(string id) {
            return allItems[id].category;
        }

        public static string SpriteName(string id) {
            return allItems[id].spriteName;
        }

        public static string Name(string id) {
            return allItems[id].name;
        }

        public static string Description(string id) {
            return allItems[id].description;
        }

        public static string Flavor(string id) {
            return allItems[id].flavor;
        }

        public static int StackLimit(string id) {
            return allItems[id].stackLimit;
        }

        public static float VolumePer(string id) {
            return allItems[id].volumePer;
        }

        public static float BaseValue(string id) {
            return allItems[id].baseValue;
        }

        public static T GetProperty<T>(string id, string property) {
            if(allItems[id].args == null || !allItems[id].args.ContainsKey(property))
                return default(T);
            return (T)allItems[id].args[property];
        }

        public static bool HasProperty(string id, string property) {
            return allItems[id].args != null && allItems[id].args.ContainsKey(property);
        }

        public static InventoryItem.Data GetData(string id) {
            return allItems[id];
        }

        public static void AddData(string id, InventoryItem.Data toAdd) {
            allItems[id] = toAdd;
        }
    }
}