using UnityEngine;
using System.Collections.Generic;

namespace OneKnight.Loading {
    public class FullDescription {

        public delegate bool ReadAction<T>(IEnumerator<SavingUtils.TableBit> enumerator, T item);

        public static void ReadDescriptions<T>(string filepath, Dictionary<string, T> readInto) where T : FullDescription {
            ReadDescriptions(filepath, readInto, null);
        }

        public static void ReadDescriptions<T>(string filepath, Dictionary<string, T> readInto, ReadAction<T> extraLineReader) where T : FullDescription{
            IEnumerator<SavingUtils.TableBit> enumerator = SavingUtils.ReadOKTable(filepath).GetEnumerator();
            T item;
            bool dontMove = false;
            while(dontMove || enumerator.MoveNext()) {
                dontMove = false;
                SavingUtils.TableBit bit = enumerator.Current;
                if(!bit.newEntry)
                    Debug.LogWarning("Next entry flagged incorrectly while parsing item descriptions for " + filepath);
                string id = bit.value;
                if(!readInto.ContainsKey(id)) {
                    Debug.LogWarning("Skipping description for " + id + " because its data was not read in.");
                    enumerator.MoveNext();
                    dontMove = SavingUtils.NextEntry(enumerator);
                    continue;
                }
                item = readInto[id];
                enumerator.MoveNext();
                //guarantee no infinite loop
                try {
                    bit = enumerator.Current;
                    item.name = bit.value;
                    enumerator.MoveNext();
                    bit = enumerator.Current;
                    item.description = bit.value;
                    enumerator.MoveNext();
                    bit = enumerator.Current;
                    item.flavor = bit.value;
                    if(extraLineReader != null)
                        dontMove = extraLineReader(enumerator, item);
                } catch(System.Exception ex) {
                    Debug.LogWarning("Error while loading description: " + id + "\n" + ex);
                    dontMove = SavingUtils.NextEntry(enumerator);
                }
            }
        }

        public string id;
        public string name;
        public string description;
        public string flavor;
    }
}