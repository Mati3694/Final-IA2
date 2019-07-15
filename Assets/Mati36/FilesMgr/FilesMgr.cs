using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.IO;

static public class FilesMgr
{

    static string appName = "DefaultName";

    #region TXT FILE

    public static bool CheckFile(string filename, string extension)
    {
        return File.Exists(intPath + filename + extension);
    }

    public static bool CheckFile(string filenameWithExtension)
    {
        return File.Exists(intPath + filenameWithExtension);
    }

    public static void CreateDirectory(string dirName)
    {
        if (!Directory.Exists("Builds/Windows/GameData/" + dirName))
        {
            Directory.CreateDirectory("Builds/Windows/GameData/" + dirName);
            Debug.Log("Creating directory " + dirName);
        }
    }

    public static string LoadTextFile(string filename, string extension)
    {
        if (!File.Exists(intPath + filename + extension))
            return "";

        string text;
        using (var reader = new StreamReader(intPath + filename + extension))
        {
            text = reader.ReadToEnd();
        }
        return text;
    }

    public static string LoadTextFile(string filenameWithExtension)
    {
        if (!File.Exists(intPath + filenameWithExtension))
            return "";

        string text;
        using (var reader = new StreamReader(intPath + filenameWithExtension))
        {
            text = reader.ReadToEnd();
        }
        return text;
    }

    public static void SaveTextFile(string filename, string extension, IEnumerable<string> content)
    {
        File.WriteAllLines(intPath + filename + extension, content.ToArray());
    }

    public static void SaveTextFile(string filename, string extension, string content, bool overwrite = true)
    {
        string new_filename = filename + extension;
        int existingFileIndex = 0;

        if (!overwrite)
        {
            while (File.Exists(intPath + new_filename))
            {
                existingFileIndex++;
                new_filename = filename + "(" + existingFileIndex + ")" + extension;
            }
        }

        Debug.Log("Writing text to file: " + intPath + new_filename);

        try
        {
            File.WriteAllText(intPath + new_filename, content, System.Text.Encoding.UTF8);
            Debug.Log("Writing succesful");
        }
        catch
        {
            Debug.Log("An error has ocurred writing file to: " + intPath + new_filename);
        }
    }
    #endregion

    //static string extPath; //Para cargar audios largos (canciones), imagenes
    static string intPath; //Para datos de jugadores, preguntas, settings, etc

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        //if (!Directory.Exists(Path.GetFullPath(Application.dataPath + "/../GameFiles")))
        //    Directory.CreateDirectory(Path.GetFullPath(Application.dataPath + "/../GameFiles"));
        //intPath = Path.GetFullPath(Application.dataPath + "/../GameFiles");
        //extPath = "C:\\ExternalFiles";
        Debug.Log("--INITIALIZING FileMgr");

#if UNITY_ANDROID && !UNITY_EDITOR
        string pathToFolder = "mnt/sdcard/" + Application.productName + "/";
        if (!Directory.Exists(pathToFolder))
            Directory.CreateDirectory(pathToFolder);
        intPath = pathToFolder;
#else
        string pathToFolder = "Builds/Windows/GameData/";
        if (!Directory.Exists(pathToFolder))
            Directory.CreateDirectory(pathToFolder);
        intPath = pathToFolder;
#endif
    }

    #region CSV
    /// <summary>
    /// Generates a CSV representation of the public fields of a custom type list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataList"></param>
    /// <returns></returns>
    public static string ToCsv<T>(List<T> dataList)
    {
        string str = "";

        System.Reflection.FieldInfo[] info = typeof(T).GetFields();

        int i, j;

        for (i = 0; i < info.Length; i++)
        {
            str += info[i].Name;
            if (i < info.Length - 1)
                str += ";";
        }

        str += "\n";

        for (i = 0; i < dataList.Count; i++)//for each element on the list
        {
            for (j = 0; j < info.Length; j++)//for each field on the element
            {
                str += info[j].GetValue(dataList[i]);
                if (j < info.Length - 1)
                    str += ";";
            }
            if (i < dataList.Count - 1)
                str += "\n";
        }

        return str;
    }

    /// <summary>
    /// Reads a CSV formatted string, and returns a list of type <T> with the data found
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="csv"></param>
    /// <returns></returns>
    public static List<T> FromCsv<T>(string csv) where T : new()
    {
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        

        List<T> objectList = new List<T>();
        T newObject;

        string[] lines = csv.Split('\n'); //csv lines
        string[] headers = lines[0].Split(';'); //field names
        string[] values; //csv values per field

        for (int l = 1; l < lines.Length; l++) //ignores the first line
        {
            if (lines[l].Trim() == "") continue; //ignores empty lines
            newObject = new T();
            values = lines[l].Split(';');
            for (int v = 0; v < values.Length; v++)
            {
                System.Reflection.FieldInfo field = typeof(T).GetField(headers[v].Trim());
                if (field != null)
                {
                    if (values[v].GetType() != field.FieldType)
                        field.SetValue(newObject, Convert.ChangeType(values[v], field.FieldType));
                    else
                        field.SetValue(newObject, values[v]);
                }
                else
                    Debug.LogWarning("CSV reading problem. The field " + headers[v] + " doesn't exist in the type " + typeof(T));
            }
            objectList.Add(newObject);
        }
        watch.Stop();
        Debug.Log("Csv readed in " + watch.ElapsedMilliseconds + " ms.");
        return objectList;
    }
    #endregion
}