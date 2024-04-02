using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//References
using Mono.Data.Sqlite;
using System;
using System.Data;
using System.IO;
using UnityEngine.Networking;


public class InitializeDB : MonoBehaviour
{
    public static InitializeDB Instance { get; private set; }

    private string conn;
    IDbConnection dbconn;
    IDbCommand dbcmd;
    private IDataReader reader;

    private string limaColor = "#76ff03";

    public string DeviceType;
    public string DatabaseName;
    public string CurrentDatabasePath;

    private void Awake()
    {
        // Assign 'this' when Instance method or variable is needed in another script
        if (Instance == null)
        {
            Instance = this;
        }

        // Add device type to database path (SWITCH BEFORE BUILDING GAME)
        DeviceType = "Android";

        // Add the database name
        DatabaseName = "RootsHope.s3db";

        // Filepath to database
        string filePathWindows = Application.dataPath + "/Plugins/" + DatabaseName;
        string filePathAndroid = Application.persistentDataPath + "/" + DatabaseName;

        // Validate device type to set database path
        switch (DeviceType)
        {
            case "Windows":
                CurrentDatabasePath = filePathWindows;
                break;
            case "Android":
                CurrentDatabasePath = filePathAndroid;
                break;
            default:
                Debug.LogError("Device type not found");
                break;
        }

        // Initialize database with device type. Now only works for: Windows/Android
        InitializeSqlite(DeviceType, CurrentDatabasePath);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Emprty...
    }

    private void InitializeSqlite(string deviceType, string filePath)
    {
        // If not exist database, create it with name DatabaseName
        if (!File.Exists(filePath))
        {
            // If not found will create database
            Debug.LogWarning("<color=yellow>File \"" + DatabaseName + "\" doesn't exist. " +
                "Creating new from \"" + filePath + "</color>");

            string url = Path.Combine(Application.streamingAssetsPath, DatabaseName);
            UnityWebRequest loadDB = UnityWebRequest.Get(url);
            loadDB.SendWebRequest();
            Debug.Log("<color=cyan>Database created successfully!</color>");
        }

        CreateTables(filePath, deviceType);
    }

    private void CreateTables(string filePath, string deviceType)
    {
        // Open db connection
        string connString = "URI=file:" + filePath;
        Debug.Log("<color=#00FF00>Establishing " + deviceType + " connection to: " + connString + "</color>");

        using (var dbconn = new SqliteConnection(connString))
        {
            dbconn.Open();

            // Define table creation queries
            var tableQueries = new List<string>
        {
            "CREATE TABLE IF NOT EXISTS Users (id INTEGER PRIMARY KEY AUTOINCREMENT, language CHAR(2), current_level INT, completed_levels INT, completed_tutorial BOOLEAN, created_at TIMESTAMP)",
            "CREATE TABLE IF NOT EXISTS Badges (id INTEGER PRIMARY KEY NOT NULL, name VARCHAR(50), description VARCHAR(50), image_path VARCHAR(30), created_at TIMESTAMP)",
            "CREATE TABLE IF NOT EXISTS Progress (id INTEGER PRIMARY KEY NOT NULL, best_score INT, objects_avoided INT, created_at TIMESTAMP)",
            "CREATE TABLE IF NOT EXISTS Achievements (id INTEGER PRIMARY KEY NOT NULL, drops_needed_to_unlock INT, unlocked BOOLEAN, badge_id INT, position INT, created_at TIMESTAMP)",
            "CREATE TABLE IF NOT EXISTS Levels (id INTEGER PRIMARY KEY NOT NULL, name VARCHAR(100), image_path VARCHAR(30), unlocked BOOLEAN, user_id INT, progress_id INT, achievements_id INT, created_at TIMESTAMP)"
        };

            try
            {
                // Variable to control if insert default data or not when creating tables first time
                bool insertData = true;

                foreach (string query in tableQueries)
                {
                    string tableName = query.Split(' ')[5];

                    // Check if the table already exists
                    using (var dbcmd = dbconn.CreateCommand())
                    {
                        dbcmd.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'";
                        using (var reader = dbcmd.ExecuteReader())
                        {
                            // If table exists, don't insert default data
                            if (reader.Read())
                            {
                                Debug.Log("<color=yellow>[INFO] Table " + tableName + " already exists</color>");
                                insertData = false;
                            }
                            else
                            {
                                // Table doesn't exist, create it
                                reader.Close(); // Cerrar el DataReader antes de cambiar el CommandText
                                dbcmd.CommandText = query;
                                using (var createReader = dbcmd.ExecuteReader())
                                {
                                    Debug.Log("<color=cyan>Table " + tableName + " created successfully!</color>");

                                    int lastIndex = tableQueries.Count - 1;
                                    // If last table, insert default data
                                    if (tableQueries.IndexOf(query) == lastIndex && insertData == true)
                                    {
                                        InsertDefaultData(filePath);
                                        Debug.Log("Inserting default data...");
                                        insertData = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error when creating tables: " + e.Message);
            }
            //Close db connection
            dbconn.Close();
            Debug.Log("<color=" + limaColor + ">Closed connection to database!</color>");
        }
    }

    private void InsertDefaultData(string filePath)
    {
        try
        {
            string insertUserQuery, insertBadgesQuery;
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            insertUserQuery = "INSERT INTO Users (language, current_level, completed_levels, completed_tutorial, created_at) VALUES ('es', 0, 0, 0, '" + timestamp + "')";
            insertBadgesQuery = "INSERT INTO Badges (name, description, image_path, created_at) VALUES " +
                "('Bronze', 'Bronze badge', 'Sprites/Bronze', '" + timestamp + "'), " +
                "('Silver', 'Silver badge', 'Sprites/Silver', '" + timestamp + "'), " +
                "('Gold', 'Gold badge', 'Sprites/Gold', '" + timestamp + "')";

            // Create a list of commands to execute
            List<string> commandsToInsert = new List<string>
        {
            insertUserQuery,
            insertBadgesQuery
        };

            foreach (string command in commandsToInsert)
            {
                // Table name extracted from command query
                string tableName = command.Split(' ')[2];

                string connString = "URI=file:" + filePath;
                using (var dbconn = new SqliteConnection(connString))
                {
                    dbconn.Open();

                    using (dbcmd = dbconn.CreateCommand())
                    {
                        dbcmd.CommandText = command;
                        reader = dbcmd.ExecuteReader();
                    }
                }

                Debug.Log("<color=magenta>Default data inserted on table " + tableName + " successfully!</color>");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error when inserting default data: " + e.Message);
        }
        finally
        {
            // Ensure that the reader and database connection are properly closed
            reader?.Close();
            dbconn?.Close();
        }
    }



}
