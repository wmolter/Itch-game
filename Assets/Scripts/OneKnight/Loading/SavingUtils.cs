﻿using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using OneKnight.PropertyManagement;

namespace OneKnight.Loading {
    public class SavingUtils : MonoBehaviour{

        //random strings for obfuscation

        public const string saveExtension = ".itch";
        //public static string savefilePath = Application.dataPath + "/saves/";
        public static string saveName = "test";

        private static FileStream stream;
        private static BinaryFormatter format;
        private static long planetDataOffset;
        private static string currFile;
        


        public static string[] SaveFiles(){
            string filepath = Application.dataPath + "/saves/";
            return Directory.GetFiles(filepath, "*" + saveExtension);
        }

        public static void Save() {
            Save(saveName);
        }

        public static void Save(string name) {
            string filepath = Application.dataPath + "/saves/";
            if(!Directory.Exists(filepath))
                Directory.CreateDirectory(filepath);
            filepath += "/" + name + saveExtension;
            currFile = filepath;
            string temppath = filepath + "temp";
            if(stream == null || stream.Name != filepath) {
                stream = new FileStream(filepath, FileMode.OpenOrCreate);
                format = new BinaryFormatter();
            } else {
                stream.Seek(0, SeekOrigin.Begin);
            }
            FileStream tempStream = new FileStream(temppath, FileMode.Create);
            
            //my save code here;
            tempStream.Close();
            stream.Close();
            stream = null;
            File.Delete(filepath);
            File.Move(temppath, filepath);
            SavingUtils.saveName = name;
            //format.Serialize(tempStream, Effect.GetSaveInfo());
        }

        public static void NewGame(string name) {
            Debug.Log("New Game");
            saveName = name;
            LoadGameData();
        }

        private static void LoadGameData() {
            ItemInfo.LoadData();
        }

        //for overall savefile loading
        public static void Load(string name) {
            LoadGameData();
            string filepath = Application.dataPath + "/saves/" + name + saveExtension;
            if(File.Exists(filepath)) {
                currFile = filepath;
                if(stream == null || stream.Name != filepath) {
                    format = new BinaryFormatter();
                    stream = new FileStream(filepath, FileMode.Open);
                } else {
                    stream.Seek(0, SeekOrigin.Begin);
                }

                stream.Close();
                stream = null;
                saveName = name;
                

            }
        }
        

        public static void Close() {
            if(stream != null) {
                stream.Close();
            }
        }

        public const char multiLineChar = '*';
        public struct TableBit {
            public string value;
            public bool newEntry;
            public TableBit(string value, bool newEntry) {
                this.value = value;
                this.newEntry = newEntry;
            }
        }

        public static IEnumerable<TableBit> ReadOKTable(string tablepath) {
            if(!File.Exists(tablepath))
                throw new UnityException("Corrupted file: " + tablepath);
            StreamReader read = new StreamReader(tablepath);
            string rawEntry = "";
            string line;
            bool multi = false;
            bool newEntry = true;
            while(read.Peek() != -1) {
                line = read.ReadLine();
                if(line.Length != 0 && line[0] == multiLineChar) {
                    if(multi) {
                        multi = false;
                        yield return new TableBit(rawEntry.Trim(), newEntry);
                        rawEntry = "";
                        newEntry = false;
                    } else {
                        multi = true;
                        rawEntry += line.Substring(1);
                    }
                } else if(multi) {
                    rawEntry += "\n" + line;
                } else if(line.Length == 0 || line.Trim().Length == 0){
                    newEntry = true;
                } else {
                    yield return new TableBit(line.Trim(), newEntry);
                    newEntry = false;
                }
            }
            if(multi)
                yield return new TableBit(rawEntry.Trim(), newEntry);

        }

        public static bool NextEntry(IEnumerator<TableBit> enumerator) {
            while(!enumerator.Current.newEntry)
                if(!enumerator.MoveNext())
                    return false;
            return true;
        }


        public static void ReadArguments(IEnumerator<TableBit> enumerator, ArgumentHolder readInto, string source) {
            TableBit bit = enumerator.Current;
            int argCount = int.Parse(bit.value);
            string[] argnames = new string[argCount];
            //object[] argvalues = new object[argCount];
            readInto.args = new Dictionary<string, object>();

            for(int i = 0; i < argCount; i++) {
                try {
                    enumerator.MoveNext();
                    bit = enumerator.Current;
                    string[] split = bit.value.Split(' ');
                    string argname = split[0];
                    enumerator.MoveNext();
                    bit = enumerator.Current;
                    object thisArg;
                    argnames[i] = argname;
                    if(split.Length < 2) {
                        int temp;
                        float ftemp;
                        bool btemp;
                        if(int.TryParse(bit.value, out temp)) {
                            thisArg = temp;
                        } else if(float.TryParse(bit.value, out ftemp)) {
                            thisArg = ftemp;
                        } else if(bool.TryParse(bit.value, out btemp)) {
                            thisArg = btemp;
                        } else {
                            thisArg = bit.value;
                        }
                        readInto.args[argname] = thisArg;
                    } else {
                        if(split[1] == "intarray") {
                            thisArg = ReadAllInts(bit.value);
                            readInto.args[argname] = thisArg;
                        } else if(split[1] == "floatarray") {
                            thisArg = ReadAllFloats(bit.value);
                            readInto.args[argname] = thisArg;
                        } else if(split[1] == "boolarray") {
                            thisArg = ReadAllBools(bit.value);
                            readInto.args[argname] = thisArg;
                        } else if(split[1] == "stringarray") {
                            thisArg = bit.value.Split(' ');
                            readInto.args[argname] = thisArg;
                        } else if(split[1] == "int") {
                            thisArg = int.Parse(bit.value);
                            readInto.args[argname] = thisArg;
                        } else if(split[1] == "float") {
                            thisArg = float.Parse(bit.value);
                            readInto.args[argname] = thisArg;
                        } else if(split[1] == "bool") {
                            thisArg = bool.Parse(bit.value);
                            readInto.args[argname] = thisArg;
                        } else if(split[1] == "adjustment") {
                            PropertyAdjustment adjust = ReadAdjustment(bit.value, argname, source);
                            thisArg = adjust.adjustment;
                            readInto.args[argname] = adjust;
                        } else if(split[1] == "adjustmentarray") {
                            readInto.args[argname] = ReadAdjustmentArray(enumerator, source);
                        } else {
                            thisArg = bit.value;
                            readInto.args[argname] = thisArg;
                        }
                    }
                } catch(System.Exception ex) {
                    Debug.LogWarning("Error while reading arguments for: " + source + "\n" + ex);
                }
            }

            readInto.argOrder = argnames;
            //readInto.argValues = argvalues;
        }

        public static float NextFloat(IEnumerator<TableBit> enumerator) {
            enumerator.MoveNext();
            return float.Parse(enumerator.Current.value);
        }

        public static int NextInt(IEnumerator<TableBit> enumerator) {
            enumerator.MoveNext();
            return int.Parse(enumerator.Current.value);
        }

        public static bool NextBool(IEnumerator<TableBit> enumerator) {
            enumerator.MoveNext();
            return bool.Parse(enumerator.Current.value);
        }

        public static string Next(IEnumerator<TableBit> enumerator) {
            enumerator.MoveNext();
            return enumerator.Current.value;
        }


        public static string[] ReadAllString(string line) {
            string[] strings = line.Split(' ');
            if(strings[0] == "")
                return new string[0];
            return strings;
        }

        public static float[] ReadAllFloats(string line) {
            string[] sfloats = line.Split(' ');
            if(sfloats[0] == "")
                return new float[0];
            float[] result = new float[sfloats.Length];
            for(int i = 0; i < sfloats.Length; i++) {
                result[i] = float.Parse(sfloats[i].Trim());
            }
            return result;
        }

        public static int[] ReadAllInts(string line) {
            string[] sints = line.Split(' ');
            if(sints[0] == "")
                return new int[0];
            int[] result = new int[sints.Length];
            for(int i = 0; i < sints.Length; i++) {
                result[i] = int.Parse(sints[i]);
            }
            return result;
        }

        public static bool[] ReadAllBools(string line) {
            string[] sbools = line.Split(' ');
            if(sbools[0] == "")
                return new bool[0];
            bool[] result = new bool[sbools.Length];
            for(int i = 0; i < sbools.Length; i++) {
                result[i] = bool.Parse(sbools[i]);
            }
            return result;
        }

        public static PropertyAdjustment ReadAdjustment(string line, string property, string source) {
            string[] split = line.Split(' ');
            string type = split[0];
            int index = 2;
            //Debug.Log("property: " + property + " line: " + line + " split length: " + split.Length);
            if(!float.TryParse(split[1], out float value)) {
                value = float.Parse(split[2]);
                type = split[1];
                property = split[0];
                index = 3;
            }

            PropertyAdjustment adjust;
            if(type == "mod")
                adjust = new PropertyAdjustment(property, PropertyAdjustment.Type.Modifier, value, source);
            else if(type == "max")
                adjust = new PropertyAdjustment(property, PropertyAdjustment.Type.Max, value, source);
            else if(type == "min")
                adjust = new PropertyAdjustment(property, PropertyAdjustment.Type.Min, value, source);
            else if(type == "bonus")
                adjust = new PropertyAdjustment(property, PropertyAdjustment.Type.Bonus, value, source);
            else {
                Debug.LogWarning("Unknown property adjustment type: " + type);
                adjust = new PropertyAdjustment(property, PropertyAdjustment.Type.Modifier, value, source);
            }
            while(index < split.Length) {
                string condition = split[index];
                type = split[index+1];
                value = float.Parse(split[index+2]);
                index += 3;
                if(condition == "base")
                    adjust.SetBaseCondition(value, PropertyAdjustment.ConditionFromString(type));
                else if(condition == "pre")
                    adjust.SetPreCondition(value, PropertyAdjustment.ConditionFromString(type));
                else if(condition == "post")
                    adjust.SetPostCondition(value, PropertyAdjustment.ConditionFromString(type));
                else
                    Debug.LogWarning("Unknown conditon: " + condition + ". Ignoring...");
            }
            return adjust;
        }

        public static PropertyAdjustment[] ReadAdjustmentArray(IEnumerator<TableBit> enumerator, string source) {
            int count = int.Parse(enumerator.Current.value);
            PropertyAdjustment[] result = new PropertyAdjustment[count];
            for(int i = 0; i < count; i++) {
                enumerator.MoveNext();
                result[i] = ReadAdjustment(enumerator.Current.value, null, source);
            }
            return result;
        }


    }

}
