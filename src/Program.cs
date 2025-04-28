using System;
using System.Text.Json;
using CellTower;

namespace CLICellFreqAllocator
{
    class Program
    {
        const double MinimumDistance = 0.5; // I decided it based on the given data and distances form the json file
        const string DataDirectory = "data/";
        const string InputFile = DataDirectory + "CellTowerList.txt";
        const string JsonOutputFile = DataDirectory + "distanceBetweenCellTowers.json";
        private static readonly List<int> Frequencies = new List<int> { 110, 111, 112, 113, 114, 115 };
        const int DefaultFrequency = -1;

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(new string('=', 100));
                Console.WriteLine(new string(' ', 25) + "Cell Frequency Allocator" + new string(' ', 25));
                Console.WriteLine(new string('-', 100));
                Console.WriteLine("Select an option below:");
                Console.WriteLine("1) Run the program and allocate frequencies.");
                Console.WriteLine("2) Terminate the program.\n");

                string? userInput = Console.ReadLine();

                if (userInput != null && (userInput == "1" || userInput == "2"))
                {
                    if (userInput == "1")
                    {
                        Console.WriteLine("Running program...\n");

                        List<CellTower.CellTower> cellTowers = new List<CellTower.CellTower>();
                        cellTowers = ReadCellTowersData(InputFile);

                        Dictionary<string, Dictionary<string, double>> distanceBetweenCellTowers = GenerateJsonFile(cellTowers);

                        IdentifyCloseAndFarTowers(cellTowers, distanceBetweenCellTowers);

                        Dictionary<string, List<string>> cellTowerFrequencies = AllocateFrequencies(cellTowers, distanceBetweenCellTowers);

                        DisplayAllocatedFrequencies(cellTowerFrequencies);

                        Console.WriteLine("\nProgram Terminated.");
                        Console.WriteLine(new string('=', 100));
                    }
                    else
                    {
                        Console.WriteLine("Program terminated.");
                        Console.WriteLine(new string('=', 100));
                    }
                }
                else
                {
                    Console.WriteLine("Program terminated due to no input or invalid inputs. Ensure that you enter either '1' or '2'.");
                    Console.WriteLine(new string('=', 100));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in Program.cs -> Main(): " + e.Message);
            }
        }

        // ---- HELPER METHODS ---

        public static Dictionary<string, List<string>> AllocateFrequencies(List<CellTower.CellTower> cellTowers, Dictionary<string, Dictionary<string, double>> distanceBetweenCellTowers)
        {
            // TODO
            // Go through all the cell towers and allocate frequencies to them.
            // Start by the first cell tower and assign it a frequency. The farthest tower will get the same frequency.
            // Go through a cell tower's close towers and assign it a different frequency. For each close tower, loop through its close towers.
            // -- If it has a close tower that is also in the outer loop's tower's close towers, assign it a different frequency.
            // -- If it has a close tower that is not in the outer loop's tower's close towers, but it is in the outer loop's far towers, assign it the same frequency as the outer loop's tower.

            Console.WriteLine("Allocating frequencies...");

            Dictionary<string, List<string>> cellTowerFrequencies = new Dictionary<string, List<string>>();

            try
            {
                cellTowers[0].setFrequency(Frequencies[0]);
                cellTowers[0].setCannotAssignFrequency(true);

                foreach (CellTower.CellTower cellTower in cellTowers)
                {
                    Dictionary<string, double> distances = distanceBetweenCellTowers[cellTower.getID()];
                    CellTower.CellTower? furthestCellTower = GetFurthestCellTower(cellTower, distances);
                    List<int> availableFrequencies = GetAvailableFrequenciesForCloseTowers(cellTower.getFrequency(), cellTower.getCloseTowers());

                    if (furthestCellTower != null)
                    {
                        cellTower.setFurthestTower(furthestCellTower);
                        List<int> closeTowerFrequencies = cellTower.getCloseTowers().Select(tower => tower.getFrequency()).ToList();

                        if (furthestCellTower.getCannotAssignFrequency() == true)
                        {
                            // if none of the current tower's close towers have the same frequency as the furthest tower
                            if (!closeTowerFrequencies.Contains(furthestCellTower.getFrequency()))
                            {
                                cellTower.setFrequency(furthestCellTower.getFrequency());
                                cellTower.setCannotAssignFrequency(true);
                            }
                            else
                            {
                                // Uncomment the line below to see which cell towers have the same tower as their furthest tower
                                // CellTower.CellTower? closeCellTowerWithSameFurthestTower = null;

                                // foreach (CellTower.CellTower closeTower in cellTower.getCloseTowers())
                                // {
                                //     CellTower.CellTower? furthestTowerForCloseTower = closeTower.getFurthestTower();
                                //     if (furthestTowerForCloseTower != null && furthestTowerForCloseTower.getID() == furthestCellTower.getID())
                                //     {
                                //         closeCellTowerWithSameFurthestTower = closeTower;
                                //         break;
                                //     }
                                // }

                                // if (closeCellTowerWithSameFurthestTower != null)
                                // {
                                //     Console.WriteLine($"" +
                                //         $"Cell tower {cellTower.getID()} and Cell Tower {closeCellTowerWithSameFurthestTower.getID()}, " +
                                //         $"which are close towers, share Cell Tower {furthestCellTower.getID()} as their furthest tower. " +
                                //         $"Keeping the current frequency for Cell Tower {cellTower.getID()}"
                                //     );
                                // }
                            }
                        }
                        else
                        {
                            furthestCellTower.setFrequency(cellTower.getFrequency());
                            furthestCellTower.setCannotAssignFrequency(true);
                        }
                    }
                    else
                    {
                        throw new Exception("foreach() -> Received null value when getting the furthest cell tower.");
                    }

                    if (availableFrequencies.Count > 0 && TowerHasNoAssignedFrequency(cellTower))
                    {
                        // throw new Exception("foreach() -> No available frequencies");
                        cellTower.setFrequency(availableFrequencies[0]);
                    }

                    // close towers
                    foreach (CellTower.CellTower closeTower in cellTower.getCloseTowers())
                    {
                        List<int> availableCloseFrequencies = GetAvailableFrequenciesForCloseTowers(closeTower.getFrequency(), closeTower.getCloseTowers());

                        // if the current tower has the same frequency as its close tower
                        if (closeTower.getFrequency() == cellTower.getFrequency() && closeTower.getFrequency() != DefaultFrequency)
                        {
                            // if there are available frequencies for this close tower
                            if (availableCloseFrequencies.Count > 0)
                            {
                                closeTower.setFrequency(availableCloseFrequencies[0]);
                            }
                            // else there are no available frequencies for this close tower, so we assign it the default value again
                            else
                            {
                                closeTower.setFrequency(DefaultFrequency);
                                closeTower.setCannotAssignFrequency(true);
                            }
                        }
                        else
                        {
                            if (availableCloseFrequencies.Count > 0 && TowerHasNoAssignedFrequency(closeTower))
                            {
                                closeTower.setFrequency(availableCloseFrequencies[0]);
                            }
                        }
                    }

                    // far towers
                    foreach (CellTower.CellTower farTower in cellTower.getFarTowers())
                    {
                        if (farTower.getFrequency() == DefaultFrequency && farTower.getCannotAssignFrequency() == false)
                        {
                            farTower.setFrequency(cellTower.getFrequency());
                        }
                    }
                    if (cellTowers[0].getFrequency() == 111)
                    {
                        Console.WriteLine();
                    }
                }

                // Building the JSON Object to store cell towers according to frequency
                BuildFrequencyJsonObject(cellTowerFrequencies, cellTowers);

                Console.WriteLine("Creating JSON ouput file for the frequency list.");
                string json = JsonSerializer.Serialize(cellTowerFrequencies);
                File.WriteAllText(DataDirectory + "frequencyList.json", json);

                Console.WriteLine("JSON cell tower frequency list output file created.");

                return cellTowerFrequencies;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in Program.cs -> AllocateFrequencies(): " + e.Message);
            }
            return cellTowerFrequencies;
        }

        public static void BuildFrequencyJsonObject(Dictionary<string, List<string>> cellTowerFrequencies, List<CellTower.CellTower> cellTowers)
        {
            try
            {
                foreach (int frequency in Frequencies)
                {
                    cellTowerFrequencies.Add(frequency.ToString(), new List<string>());
                }

                cellTowerFrequencies.Add("-1", new List<string>()); // for the cell towers with unassigned frequencies

                foreach (CellTower.CellTower cellTower in cellTowers)
                {
                    if (cellTower.getID() == "M")
                    {
                        Console.WriteLine();
                    }
                    cellTowerFrequencies.Where(freq => freq.Key == cellTower.getFrequency().ToString()).First().Value.Add(cellTower.getID());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in Program.cs -> BuildFrequencyJsonObject(): " + e.Message);
            }
        }

        public static List<int> GetAvailableFrequenciesForCloseTowers(int currentTowerFrequency, List<CellTower.CellTower> otherCloseTowers)
        {
            try
            {
                List<int> closeTowersFrequencies = otherCloseTowers.Select(tower => tower.getFrequency()).ToList();

                return Frequencies.Where(freq => !closeTowersFrequencies.Contains(freq) && freq != currentTowerFrequency).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in Program.cs -> GetAvailableFrequenciesForCloseTowers(): " + e.Message);
            }
            return [];
        }

        public static bool TowerHasNoAssignedFrequency(CellTower.CellTower cellTower)
        {
            try
            {
                return !Frequencies.Contains(cellTower.getFrequency());
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in Program.cs -> TowerHasNoAssignedFrequency(): " + e.Message);
            }
            return false;
        }

        public static CellTower.CellTower? GetFurthestCellTower(CellTower.CellTower currentTower, Dictionary<string, double> distances)
        {
            try
            {
                double longestDistance = distances.Values.ToArray().Max();
                string furthestTowerID = distances.Where(keyPair => keyPair.Value == longestDistance).First().Key;

                return currentTower.getFarTowers().Where(tower => tower.getID() == furthestTowerID).First();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in Program.cs -> GetFurthestCellTower(): " + e.Message);
            }
            return null;
        }

        public static void IdentifyCloseAndFarTowers(List<CellTower.CellTower> cellTowers, Dictionary<string, Dictionary<string, double>> distanceBetweenCellTowers)
        {
            Console.WriteLine("Identifying close and far towers...");

            try
            {
                // Looping through the dictionary of distances to identify towers that are close/far to each other
                foreach (string cellTowerID in distanceBetweenCellTowers.Keys)
                {
                    List<CellTower.CellTower> closeTowers = new List<CellTower.CellTower>();
                    List<CellTower.CellTower> farTowers = new List<CellTower.CellTower>();

                    foreach (string otherCellTowerID in distanceBetweenCellTowers[cellTowerID].Keys)
                    {
                        if (distanceBetweenCellTowers[cellTowerID][otherCellTowerID] < MinimumDistance)
                        {
                            closeTowers.Add(cellTowers.Where(tower => tower.getID() == otherCellTowerID).First());
                        }
                        else
                        {
                            farTowers.Add(cellTowers.Where(tower => tower.getID() == otherCellTowerID).First());
                        }
                    }

                    cellTowers.Where(tower => tower.getID() == cellTowerID).First().setCloseTowers(closeTowers);
                    cellTowers.Where(tower => tower.getID() == cellTowerID).First().setFarTowers(farTowers);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in Program.cs -> IdentifyCloseAndFarTowers(): " + e.Message);
            }
        }

        public static Dictionary<string, Dictionary<string, double>> GenerateJsonFile(List<CellTower.CellTower> cellTowers)
        {
            Console.WriteLine("Generating JSON output file...");

            Dictionary<string, Dictionary<string, double>> distanceBetweenCellTowers = new Dictionary<string, Dictionary<string, double>>();

            try
            {
                if (cellTowers == null)
                {
                    throw new Exception("Exception in Program.cs -> GenerateJsonFile(): cellTowers list is null.");
                }
                else
                {
                    foreach (CellTower.CellTower cellTower in cellTowers)
                    {
                        distanceBetweenCellTowers.Add(cellTower.getID(), new Dictionary<string, double>());
                        List<CellTower.CellTower> otherCellTowers = cellTowers.Where(tower => tower.getID() != cellTower.getID()).ToList();

                        foreach (CellTower.CellTower otherCellTower in otherCellTowers)
                        {
                            double distance = CalculateDistance(cellTower.getLatitude(), cellTower.getLongitude(), otherCellTower.getLatitude(), otherCellTower.getLongitude());

                            distanceBetweenCellTowers[cellTower.getID()].Add(otherCellTower.getID(), distance);
                        }
                    }

                    string json = JsonSerializer.Serialize(distanceBetweenCellTowers);
                    File.WriteAllText(JsonOutputFile, json);

                    Console.WriteLine("JSON output file created.");

                    return distanceBetweenCellTowers;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in Program.cs -> GenerateJsonFile(): " + e.Message);
            }
            return distanceBetweenCellTowers;
        }

        public static double CalculateDistance(double latitude_1, double longitude_1, double latitude_2, double longitude_2)
        {
            // Using the Haversine formula approach

            double R = 6371; // earth's radius (km)

            // Convert values to radians
            double phi_1 = latitude_1 * Math.PI / 180;
            double phi_2 = latitude_2 * Math.PI / 180;
            double delta_phi = (latitude_2 - latitude_1) * Math.PI / 180;
            double delta_lambda = (longitude_2 - longitude_1) * Math.PI / 180;

            double a = Math.Sin(delta_phi / 2) * Math.Sin(delta_phi / 2) + Math.Cos(phi_1) * Math.Cos(phi_2) * Math.Sin(delta_lambda / 2) * Math.Sin(delta_lambda / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        public static List<CellTower.CellTower> ReadCellTowersData(string filePath)
        {
            Console.WriteLine("Reading cell tower data...");

            string? headerLine, line;
            List<CellTower.CellTower> cellTowers = new List<CellTower.CellTower>();
            StreamReader sr = new StreamReader(filePath);

            try
            {
                if (sr == null)
                {
                    throw new Exception("Exception thrown in Program.cs -> ReadCellTowers(): Exception when reading input file path.");
                }
                else
                {
                    headerLine = sr.ReadLine();
                    line = sr.ReadLine();

                    while (line != null)
                    {
                        string[] values = line.Split(',');

                        cellTowers.Add(new CellTower.CellTower(values[0], Convert.ToInt32(values[1]), Convert.ToInt32(values[2]), Convert.ToDouble(values[3]), Convert.ToDouble(values[4])));

                        //Read the next line
                        line = sr.ReadLine();
                    }

                    //Closing the file
                    sr.Close();
                    Console.WriteLine("Completed reading input data...");
                }
                return cellTowers;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in Program.cs -> ReadCellTowers(): " + e.Message);
            }
            finally
            {
                sr.Close();
            }

            return cellTowers;
        }

        public static void PrintCellTowersData(List<CellTower.CellTower> cellTowers)
        {
            // print the close and far towers of each cell tower
            Console.WriteLine();
            foreach (CellTower.CellTower cellTower in cellTowers)
            {
                Console.WriteLine("Close towers of " + cellTower.getID() + ": " + string.Join(", ", cellTower.getCloseTowers().Select(tower => tower.getID())));
                Console.WriteLine("Far towers of " + cellTower.getID() + ": " + string.Join(", ", cellTower.getFarTowers().Select(tower => tower.getID())));
                Console.WriteLine();
            }
        }
        public static void DisplayAllocatedFrequencies(Dictionary<string, List<string>> cellTowerFrequencies)
        {
            try
            {
                Console.WriteLine('|' + new string('-', 64) + '|');
                Console.WriteLine('|' + new string(' ', 5) + "Frequency" + new string(' ', 5) + '|' + new string(' ', 5) + "Cell Tower IDs" + new string(' ', 5) + '|');

                foreach (string key in cellTowerFrequencies.Keys)
                {
                    string cellIDs = string.Join(", ", cellTowerFrequencies[key]);
                    string frequencyColumnFormat = key.PadLeft(5).PadRight(5);
                    string cellIDsColumnFormat = cellIDs.PadLeft(5).PadRight(5);

                    Console.WriteLine('|' + frequencyColumnFormat + '|' + cellIDsColumnFormat + '|');
                }

                Console.WriteLine('|' + new string('-', 64) + '|');
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in Program.cs -> DisplayAllocatedFrequencies(): " + e.Message);
            }
        }
    }
}