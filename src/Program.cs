using System;
using System.Text.Json;
using CellTower;

namespace CLICellFreqAllocator
{
    class Program
    {
        const int MinimumDistance = 0;
        const string DataDirectory = "../data/";
        const string InputFile = "CellTowerList.txt";
        const string JsonOutputFile = "distanceBetweenCellTowers.json";
        static void Main(string[] args)
        {
            Console.WriteLine("STARTING CELL TOWER FREQUENCY ALLOCATION");

            List<CellTower.CellTower> cellTowers = new List<CellTower.CellTower>();
            cellTowers = ReadCellTowersData(InputFile);

            Dictionary<string, Dictionary<string, double>> distanceBetweenCellTowers = GenerateJsonFile(cellTowers);
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
                        List<CellTower.CellTower> otherCellTowers = (List<CellTower.CellTower>)cellTowers.Where(tower => tower.getID() != cellTower.getID());

                        foreach (CellTower.CellTower otherCellTower in otherCellTowers)
                        {
                            double distance = CalculateDistance(cellTower.getLatitude(), cellTower.getLongitude(), otherCellTower.getLatitude(), otherCellTower.getLongitude());

                            distanceBetweenCellTowers[cellTower.getID()].Add(otherCellTower.getID(), distance);
                        }
                    }

                    string json = JsonSerializer.Serialize(distanceBetweenCellTowers);
                    File.WriteAllText(DataDirectory + JsonOutputFile, json);

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

            string? line;
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
    }
}