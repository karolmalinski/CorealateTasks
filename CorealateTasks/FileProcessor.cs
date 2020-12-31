using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CorealateTasks
{
    static class FileProcessor
    {
        /// <summary>
        /// Reads a file and save the content as the new file with reversed lines order
        /// </summary>
        /// <param name="path">Path to the file that has to be reversed</param>
        /// <returns>True if success, false if something went wrong</returns>
        static public bool ReadAndReverse(string path)
        {
            try
            {
                //Read all ines from the given file
                string[] lines = File.ReadAllLines(path);

                //Reverse lines
                Array.Reverse(lines);

                //Determines new name for the output file
                string outputPath = Path.GetDirectoryName(path) + @"\result-1.txt";

                //Write to the file
                File.WriteAllLines(outputPath, lines);

                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Reads a file and save the content as the new file with sorted lines
        /// </summary>
        /// <param name="path">Path to the file that has to be sorted</param>
        /// <returns>True if success, false if something went wrong</returns>
        static public bool ReadAndSort(string path)
        {

            try
            {
                //Read all lines from the given file
                string[] lines = File.ReadAllLines(path);

                //Put lines into the datatable
                DataTable dt = GetTable(lines);

                //Sort DataTable
                dt.DefaultView.Sort = "type ASC";
                dt = dt.DefaultView.ToTable();

                //Two first lines are the same
                string[] newLines = new string[lines.Length];
                newLines[0] = lines[0];
                newLines[1] = lines[1];

                //Start for thenthird lines (indexes start from 0)
                int counter = 2;

                foreach (DataRow row in dt.Rows)
                {
                    newLines[counter] = string.Join(",", row.ItemArray);
                    counter++;
                }

                //Determines new name for the output file
                string outputPath = Path.GetDirectoryName(path) + @"\result-2.txt";

                //Write to the file
                File.WriteAllLines(outputPath, newLines);

                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Makes batch evaulation for given teas.
        /// </summary>
        /// <param name="path">Path to the "database" file.</param>
        /// <param name="inputPath">Path with teas to evaulate</param>
        public static bool MakeBatchTeas(string path, string inputPath)
        {
            try
            {
                //Read all lines from the given file
                string[] teas = File.ReadAllLines(inputPath);

                //The list for results
                List<string> results = new List<string>();

                int counter = 0;
                foreach (string tea in teas)
                {
                    counter++;
                    string[] teaParams = tea.Split(',');

                    //Check wether the array has three items
                    if (teaParams.Length < 3)
                    {
                        results.Add($"Wrong data no. {counter}.");
                        continue;
                    }

                    string name = teaParams[0].Trim();

                    //Parse temp
                    int temp;
                    if (!int.TryParse(teaParams[1], out temp))
                    {
                        results.Add($"Wrong data for tea colled {name} - no. {counter}.");
                        continue;
                    }

                    //Parse time
                    int time;
                    if (!int.TryParse(teaParams[2], out time))
                    {
                        results.Add($"Wrong data for tea colled {name} - no. {counter}.");
                        continue;
                    }

                    //Get result of the comparison
                    string result = CompareParameters(path, name, temp, time);

                    string outcome = $"{name}, {result}";
                    results.Add(outcome);
                }

                //Determines new name for the output file
                string outputPath = Path.GetDirectoryName(path) + @"\result-5.txt";


                File.WriteAllLines(outputPath, results);
                return true;
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                return false;
            }

        }

        /// <summary>
        /// Makes a Touareg tea
        /// </summary>
        /// <param name="path">Database path</param>
        /// <param name="inputPath">Input file path</param>
        /// <returns></returns>
        public static string MakeTouaregTea(string path, string inputPath)
        {
            try
            {
                //Read all lines from the given file
                string[] lines = File.ReadAllLines(inputPath);

                //Determines new name for the output file
                string outputPath = Path.GetDirectoryName(path) + @"\result-6.txt";

                //Some validation
                if (lines.Length != 2)
                {
                    string err = "The recipe contains wrong quantity of ingredients.";
                    File.WriteAllText(outputPath, err);
                    return err;
                }

                if (CheckIngredient(path, /*mint*/ lines[0], "water") && CheckIngredient(path, /*tea*/ lines[1], "Mięta"))
                {
                    string res = "Congratulations, perfect Touareg.";
                    File.WriteAllText(outputPath, res);
                    return res;
                }
                else
                {
                    string res = "Sadly, your Touareg is ruined.";
                    File.WriteAllText(outputPath, res);
                    return res;
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                return "FAILED =(";
            }

        }
        /// <summary>
        /// Checks ingeridients of Touareg tea
        /// </summary>
        /// <param name="path">Path to the database file</param>
        /// <param name="ingredient">String with an ingeridient</param>
        /// <param name="liquid">The liquid used to prepare the tea</param>
        /// <returns></returns>
        private static bool CheckIngredient(string path, string ingredient, string liquid)
        {
            //beverage validation
            string[] beverage = ingredient.Split(',');
            if (beverage.Length < 4)
            {
                Debug.Print("Wrong given data.");
                return false;
            }
            
            //Validate ingeridients
            if (beverage[1].Trim() == "water" && beverage[0].Trim() != "Mięta")
            {
                return false;
            }
            else if (beverage[1].Trim() == "Mięta" && beverage[0].Trim() != "Gunpowder Zielony")
            {
                return false;
            }
            else if (beverage[0].Trim() != "Mięta" && beverage[0].Trim() != "Gunpowder Zielony")
            {
                return false;
            }

            //Get beverage params
            int temp;
            int time;
            if (!int.TryParse(beverage[2].Trim(), out temp) || !int.TryParse(beverage[3].Trim(), out time) || beverage[1].Trim() != liquid)
            {
                return false;
            }

            //Chack the ingredient
            if (CompareParameters(path, beverage[0].Trim(), temp, time) != "perfect")
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Comapres tea parameters against stored into database
        /// </summary>
        /// <param name="path">Database file</param>
        /// <param name="name">Tea name</param>
        /// <param name="temp">Liquid temperature</param>
        /// <param name="time">Bawering time in seconds</param>
        /// <returns></returns>
        public static string CompareParameters(string path, string name, int temp, int time)
        {
            try
            {
                //Read all lines from the given file
                string[] lines = File.ReadAllLines(path);

                //Put lines into the datatable
                DataTable dt = GetTable(lines);

                //Select a result with LINQ
                var result = dt.AsEnumerable().Where(dr => dr.Field<string>("name") == name);

                if (!result.Any()) return $"There is no tea called \"{name}\" in the database.";

                //Get stored temp
                int storedTemp;
                if (!int.TryParse(result.First().Field<string>("temp").Trim(), out storedTemp)) return "Internal database error (temp).";

                //Get stored time
                int storedTime;
                if (!int.TryParse(result.First().Field<string>("time").Trim(), out storedTime)) return "Internal database error (time).";

                //Calculations
                float tempDiff = Math.Abs(storedTemp - temp) / (float)storedTemp;
                float timeDiff = Math.Abs(storedTime - time) / (float)storedTime;

                //Determine wether tea is perfect or not
                string outcome = string.Empty;
                if (tempDiff < 0.1 && timeDiff < 0.1) outcome = "perfect";
                else if (storedTemp < temp && storedTime < time) outcome = "yucky";
                else if (storedTemp > temp && storedTime > time) outcome = "weak";
                else outcome = "yucky";

                return outcome;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "FAILED =(";
            }
        }

        private static DataTable GetTable(string[] lines)
        {
            try
            {
                //Create a DataTable to store the data
                DataTable dt = new DataTable();
                dt.Columns.Add("name");
                dt.Columns.Add("type");
                dt.Columns.Add("temp");
                dt.Columns.Add("time");

                //Put lines to the datatable
                foreach (string line in lines)
                {
                    if (string.IsNullOrEmpty(line) || line[0] == '#') continue;

                    string[] rowData = line.Split(',');

                    DataRow row = dt.NewRow();
                    row["name"] = rowData[0];
                    row["type"] = rowData[1];
                    row["temp"] = rowData[2];
                    row["time"] = rowData[3];
                    dt.Rows.Add(row);
                }

                return dt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        internal static void SaveTeaResult(string path, string name, int temp, int time, string result)
        {
            //Determines new name for the output file
            string outputPath = Path.GetDirectoryName(path) + @"\result-4.txt";

            string outcome = $"{name}, {temp}, {time}, {result}";
            File.WriteAllText(outputPath, outcome);
        }
    }
}
