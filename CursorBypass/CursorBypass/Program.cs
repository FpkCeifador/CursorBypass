using System;
using System.IO;
using System.Text.Json;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\r\n   _________________________    ____  ____  ____ \r\n  / ____/ ____/  _/ ____/   |  / __ \\/ __ \\/ __ \\\r\n / /   / __/  / // /_  / /| | / / / / / / / /_/ /\r\n/ /___/ /____/ // __/ / ___ |/ /_/ / /_/ / _, _/ \r\n\\____/_____/___/_/   /_/  |_/_____/\\____/_/ |_|  \r\n");
            Console.Write("   Cursor Bypass Made By");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(" fpkceifas (Discord)");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\r\n   [1] Bypass Trial Limit \r\n   [2] Exit");
            Console.ForegroundColor = ConsoleColor.Cyan; Console.Write("\r\n   [>] "); Console.ForegroundColor = ConsoleColor.White;

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            switch (keyInfo.KeyChar)
            {
                case '1':
                    ResetCursorId();
                    Console.WriteLine("\n\n   Press any key to return to menu...");
                    Thread.Sleep(500);
                    break;

                case '2':
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n\n   Exiting...");
                    Thread.Sleep(1500);
                    Environment.Exit(0);
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n\n   Invalid choice!");
                    Thread.Sleep(500);
                    break;
            }
        }
    }

    static void BackupFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            string backupPath = $"{filePath}.backup_{DateTime.Now:yyyyMMdd_HHmmss}";
            File.Copy(filePath, backupPath, true);
        }
    }

    static string GetStorageFile()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            string appData = Environment.GetEnvironmentVariable("APPDATA");
            return Path.Combine(appData, "Cursor", "User", "globalStorage", "storage.json");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(home, "Library", "Application Support", "Cursor", "User", "globalStorage", "storage.json");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(home, ".config", "Cursor", "User", "globalStorage", "storage.json");
        }
        else
        {
            throw new PlatformNotSupportedException($"Unsupported operating system: {RuntimeInformation.OSDescription}");
        }
    }

    static string GenerateRandomHex(int bytes)
    {
        byte[] buffer = new byte[bytes];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(buffer);
        }
        return BitConverter.ToString(buffer).Replace("-", "").ToLower();
    }

    static void ResetCursorId()
    {
        Console.WriteLine("\n\n   Looking For Cursor Processes...");
        KillCursorProcesses();
        Thread.Sleep(3000);
        string storageFile = GetStorageFile();
        Directory.CreateDirectory(Path.GetDirectoryName(storageFile));
        BackupFile(storageFile);

        JsonDocument existingData = null;
        Dictionary<string, JsonElement> data = new Dictionary<string, JsonElement>();

        if (File.Exists(storageFile))
        {
            string jsonContent = File.ReadAllText(storageFile);
            existingData = JsonDocument.Parse(jsonContent);

            foreach (JsonProperty prop in existingData.RootElement.EnumerateObject())
            {
                data[prop.Name] = prop.Value.Clone();
            }
        }

        string machineId = GenerateRandomHex(32);
        string macMachineId = GenerateRandomHex(32);
        string devDeviceId = Guid.NewGuid().ToString();

        // Update only the specific fields we want to change
        var options = new JsonSerializerOptions { WriteIndented = true };
        data["telemetry.machineId"] = JsonSerializer.SerializeToElement(machineId, options);
        data["telemetry.macMachineId"] = JsonSerializer.SerializeToElement(macMachineId, options);
        data["telemetry.devDeviceId"] = JsonSerializer.SerializeToElement(devDeviceId, options);

        string jsonString = JsonSerializer.Serialize(data, options);
        File.WriteAllText(storageFile, jsonString);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("   Device IDs have been successfully reset. The new device IDs are: \n\n");
        Console.ForegroundColor = ConsoleColor.Yellow;
        var newIds = new Dictionary<string, string>
        {
            { "machineId", machineId },
            { "macMachineId", macMachineId },
            { "devDeviceId", devDeviceId }
        };

        string jsonOutput = JsonSerializer.Serialize(newIds, new JsonSerializerOptions { WriteIndented = true });
        string[] lines = jsonOutput.Split('\n');

        string formattedOutput = string.Join("\n", lines.Select(line => "   " + line));
        Console.WriteLine(formattedOutput);
        Console.ReadKey();
        Thread.Sleep(1000);
    }

    static void KillCursorProcesses()
    {
        string[] processNames = { "Cursor", "cursor" };
        bool foundProcess = false;

        foreach (string processName in processNames)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length > 0)
            {
                foundProcess = true;
                Console.WriteLine($"   Found {processes.Length} Cursor process(es). Closing...");

                foreach (Process process in processes)
                {
                    try
                    {
                        process.Kill();
                        process.WaitForExit(3000);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"   Error closing process: {ex.Message}");
                    }
                }
            }
        }

        if (foundProcess)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("   Cursor Closed, Starting Bypass");
            Thread.Sleep(2000);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("   Cursor Already Closed, Starting Bypass");
        }
    }
}