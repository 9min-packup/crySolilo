using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CrySolilo
{
    public static class SaveDatUtility
    {
        public static string GetSaveDataPath(string fileName)
        {
            string SaveDirectoryPath = Application.persistentDataPath + "/Save";
            if (!Directory.Exists(SaveDirectoryPath))
            {
                Directory.CreateDirectory(SaveDirectoryPath);
#if UNITY_IOS
                UnityEngine.iOS.Device.SetNoBackupFlag(SaveDirectoryPath);
#endif
                Debug.Log("Created Directory: " + SaveDirectoryPath);
            }

            return SaveDirectoryPath + "/" + fileName;
        }

        public static T Read<T>(string file)
        {
            T data;
            try
            {
                StreamReader sr = new StreamReader(GetSaveDataPath(file), Encoding.UTF8);
                data = JsonUtility.FromJson<T>(sr.ReadToEnd());
                sr.Close();
                return data;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return default(T);
            }

        }

        public static bool Write<T>(string fileName, T data)
        {

            try
            {
                StreamWriter sw = new StreamWriter(GetSaveDataPath(fileName), false, Encoding.UTF8);
                sw.Write(JsonUtility.ToJson(data));
                sw.Close();
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("SaveData Faild to Save : file = " + fileName);
                Debug.LogError(e.ToString());
                return false;
            }
        }

        public static bool Copy(string fromFilename, string toFilename)
        {
            try
            {
                FileInfo file = new FileInfo(GetSaveDataPath(fromFilename));
                file.CopyTo(GetSaveDataPath(toFilename), true);
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("SaveData Faild to Copy :  " + fromFilename + " to " + toFilename);
                Debug.LogError(e.ToString());
                return false;
            }
        }

        public static bool Delete(string filename)
        {
            try
            {
                FileInfo file = new FileInfo(GetSaveDataPath(filename));
                file.Delete();
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("SaveData Faild to Delete :  " + filename);
                Debug.LogError(e.ToString());
                return false;
            }
        }


    }

}