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
        DeviceType = "Windows";

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
        conn = "URI=file:" + filePath;
        Debug.Log("<color=#00FF00>Establishing " + deviceType + " connection to: " + conn + "</color>");
        dbconn = new SqliteConnection(conn);
        dbconn.Open();

        try
        {
            // Validate if tables already exist in the database
            string[] tableNames = { "Users", "Badges", "Progress", "Achievements", "Levels" };
            bool insertData = false;

            foreach (string tableName in tableNames)
            {
                string tableQuery = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'";
                dbcmd = dbconn.CreateCommand();
                dbcmd.CommandText = tableQuery;
                reader = dbcmd.ExecuteReader();

                if (!reader.Read())
                {
                    // Table doesn't exist, create it
                    reader.Close();
                    string createTableQuery = "";

                    switch (tableName)
                    {
                        case "Users":
                            createTableQuery = "CREATE TABLE IF NOT EXISTS Users (id INTEGER PRIMARY KEY AUTOINCREMENT, language CHAR(2), current_level INT, completed_levels INT, completed_tutorial BOOLEAN, created_at TIMESTAMP)";
                            break;
                        case "Badges":
                            createTableQuery = "CREATE TABLE IF NOT EXISTS Badges (id INTEGER PRIMARY KEY NOT NULL, name VARCHAR(50), description VARCHAR(50), image_path VARCHAR(30), created_at TIMESTAMP)";
                            break;
                        case "Progress":
                            createTableQuery = "CREATE TABLE IF NOT EXISTS Progress (id INTEGER PRIMARY KEY NOT NULL, best_score INT, objects_avoided INT, created_at TIMESTAMP)";
                            break;
                        case "Achievements":
                            createTableQuery = "CREATE TABLE IF NOT EXISTS Achievements (id INTEGER PRIMARY KEY NOT NULL, drops_needed_to_unlock INT, unlocked BOOLEAN, badge_id INT, position INT, created_at TIMESTAMP)";
                            break;
                        case "Levels":
                            createTableQuery = "CREATE TABLE IF NOT EXISTS Levels (id INTEGER PRIMARY KEY NOT NULL, name VARCHAR(100), image_path VARCHAR(30), unlocked BOOLEAN, user_id INT, progress_id INT, achievements_id INT, created_at TIMESTAMP)";
                            break;
                    }

                    dbcmd.CommandText = createTableQuery;
                    reader = dbcmd.ExecuteReader();
                    Debug.Log($"<color=cyan>Table {tableName} created successfully!</color>");
                    insertData = true;
                }
                else
                {
                    Debug.Log($"<color=yellow>[INFO] Table {tableName} already exists</color>");
                    insertData = false;
                }

                reader.Close();
            }

            if (insertData) 
            { 
                // Call method to insert default data
                Debug.Log("<color=yellow>[INFO] Inserting default data...</color>");
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error when creating tables: " + e.Message);
        }
        finally
        {
            dbconn.Close();
            dbconn.Dispose();
            Debug.Log("<color=" + limaColor + ">Closed connection to database!</color>");
        }
    }

    private void InsertDefaultData()
    {
        string[] defaultData = {
            "(10, 'Julio', 2023, '0.00', '3.00', '1.00', '4.00')",
            "(11, 'Julio', 2023, '0.75', '3.00', '0.75', '4.50')",
            "(12, 'Julio', 2023, '0.75', '3.00', '0.75', '4.50')",
            "(13, 'Julio', '2023', '0.75', '2.50', '0.75', '4.00')",
            "(14, 'Julio', '2023', '0.75', '3.00', '0.75', '4.50')",

            "(17, 'Julio', '2023', '0.75', '2.50', '0.75', '4.00')",
            "(18, 'Julio', '2023', '0.75', '3.00', '0.75', '4.50')",
            "(19, 'Julio', '2023', '0.75', '2.50', '0.75', '4.00')",
            "(20, 'Julio', '2023', '0.75', '2.50', '0.75', '4.00')",
            "(21, 'Julio', '2023', '0.75', '2.75', '0.75', '4.25')",

            "(24, 'Julio', '2023', '0.75', '0.00', '0.75', '1.50')",
            "(25, 'Julio', '2023', '0.75', '0.00', '0.75', '1.50')",
            "(26, 'Julio', '2023', '0.75', '0.25', '0.75', '1.75')",
            "(27, 'Julio', '2023', '0.75', '1.25', '0.75', '2.75')",
            "(28, 'Julio', '2023', '0.75', '5.50', '0.75', '7.00')",

            "(7, 'Agosto', '2023', '0.75', '0.00', '0.75', '1.50')",
            "(8, 'Agosto', '2023', '0.75', '0.00', '0.75', '1.50')",
            "(9, 'Agosto', '2023', '0.75', '3.00', '0.75', '4.50')",
            "(10, 'Agosto', '2023', '0.75', '4.00', '0.75', '5.50')",
            "(11, 'Agosto', '2023', '0.75', '1.75', '0.75', '3.25')",

            "(21, 'Agosto', '2023', '0.75', '0.25', '0.75', '1.75')",
            "(22, 'Agosto', '2023', '0.75', '0.00', '0.75', '1.50')",
            "(23, 'Agosto', '2023', '0.75', '0.25', '0.75', '1.75')",
            "(24, 'Agosto', '2023', '0.75', '0.25', '0.75', '1.75')",
            "(25, 'Agosto', '2023', '1.90', '3.25', '0.75', '5.90')",

            "(28, 'Agosto', '2023', '0.75', '0.00', '0.75', '1.50')",
            "(29, 'Agosto', '2023', '0.75', '0.50', '1.25', '2.50')",
            "(30, 'Agosto', '2023', '0.75', '0.25', '0.75', '1.75')",
            "(31, 'Agosto', '2023', '0.75', '5.25', '0.75', '6.75')",
            "(1, 'Septiembre', '2023', '0.75', '5.00', '0.75', '6.50')",

            "(4, 'Septiembre', '2023', '0.75', '0.00', '0.75', '1.50')",
            "(5, 'Septiembre', '2023', '0.75', '1.25', '0.75', '2.75')",
            "(6, 'Septiembre', '2023', '0.75', '0.25', '0.75', '1.75')",
            "(7, 'Septiembre', '2023', '0.75', '0.75', '0.75', '2.25')",
            "(8, 'Septiembre', '2023', '0.75', '6.75', '0.75', '8.25')"
        };

        try
        {
            // Open a new database connection
            IDbConnection insertConnection = new SqliteConnection(conn);
            insertConnection.Open();

            // Create a command to execute the insert queries
            IDbCommand insertCommand = insertConnection.CreateCommand();

            foreach (string data in defaultData)
            {
                // Create the query
                string query = $"INSERT INTO Expenses (day, month_letter, year, outward_price, launch_price, return_price, total_expenses) VALUES {data}";

                insertCommand.CommandText = query;
                insertCommand.ExecuteNonQuery();
            }

            // Close the connection
            insertConnection.Close();

            Debug.Log("<color=cyan>Default data inserted successfully!</color>");
        }
        catch (Exception e)
        {
            Debug.LogError("Error when inserting default data: " + e.Message);
        }
    }


}
