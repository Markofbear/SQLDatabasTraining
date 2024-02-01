using System;
using MySql.Data.MySqlClient;

class Program
{
    static void Main()
    {
        // login med localhost eftersom det är med xampp på min dator, med namnet på database, inlogg root utan kod
        string connectionString = "server=localhost;database=gamedatabase;uid=root;pwd=;";

        // Using statement ensures that the MySqlConnection is properly disposed of when done *CHATGPT LÖSNING*
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            // Öppnar databas connection
            connection.Open();

            while (true)
            {
                DisplayEnemyList(connection);

                Console.Write("Enter the Enemy ID (or 'q' to quit): ");
                string? input = Console.ReadLine();

                if (input?.ToLower() == "q")
                {
                    break; 
                }

                if (int.TryParse(input, out int enemyId))
                {
                    DisplayDropRateAndWeaponsMenu(connection, enemyId);
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid Enemy ID or 'q' to quit.");
                }
            }
        }
    }

    static void DisplayEnemyList(MySqlConnection connection)
    {
        // SQL kod för att få ut alla Enemies
        string query = "SELECT * FROM Enemies";

        // Se tidigare kommentar
        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                Console.WriteLine("Enemy List:");
                Console.WriteLine("------------");
                Console.WriteLine("ID\tName");
                Console.WriteLine("------------");

                while (reader.Read())
                {
                    int enemyId = Convert.ToInt32(reader["Enemy_ID"]);
                    string? enemyName = reader["Name"].ToString();
                    Console.WriteLine($"{enemyId}\t{enemyName}");
                }
            }
        }
    }

    static void DisplayDropRateAndWeaponsMenu(MySqlConnection connection, int enemyId)
    {
        // SQL kod för att få fram vald Enemy med droprate och fiender för 100%.
        string query = $"SELECT Weapons.Weapon_ID, Weapons.Name, EnemyWeapons.DropRate, 1 / EnemyWeapons.DropRate AS EnemiesNeeded " +
                       $"FROM EnemyWeapons JOIN Weapons ON EnemyWeapons.Weapon_ID = Weapons.Weapon_ID " +
                       $"WHERE EnemyWeapons.Enemy_ID = {enemyId}";

        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                Console.WriteLine($"Weapons for Enemy ID {enemyId}:");
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine("ID\tName\t\t\tDrop Rate\tEnemies Needed for 100%");
                Console.WriteLine("-------------------------------------------");
                // tar vad vi har bett den om att "läsa in" från tidigare
                while (reader.Read())
                {
                    int weaponId = Convert.ToInt32(reader["Weapon_ID"]);
                    string? weaponName = reader["Name"].ToString();
                    double dropRate = Convert.ToDouble(reader["DropRate"]);
                    double enemiesNeeded = Convert.ToDouble(reader["EnemiesNeeded"]);

                    // Simpel kod för att räkna ut droprate
                    Console.WriteLine($"{weaponId}\t{weaponName,-20}\t{Math.Ceiling(dropRate * 100)}%\t\t{Math.Ceiling(enemiesNeeded)}");
                }
            }
        }
    }
}
