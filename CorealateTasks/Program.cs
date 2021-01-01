using System;
using System.IO;

namespace CorealateTasks
{
    class Program
    {

        //https://stackoverflow.com/questions/27181774/get-resources-folder-path-c-sharp

        public static readonly string PATH = GetFilePath("tea-data.txt");
        public static readonly string PATH_INPUT = GetFilePath("input-file.txt");
        public static readonly string PATH_TOUAREG_INPUT = GetFilePath("touareg-input-file.txt");

        static void Main(string[] args)
        {
            Menu();
        }

        /// <summary>
        /// Displays MENU of the TeaMaker
        /// </summary>
        private static void Menu()
        {
            Console.WriteLine("Which command do you wich to run?");
            Console.WriteLine("---------------------------------");
            Console.WriteLine("1 - reverse the records");
            Console.WriteLine("2 - sort the record");
            Console.WriteLine("3 - do nothing (ignore the input)");
            Console.WriteLine("4 - make a tea");
            Console.WriteLine("5 - make several teas");
            Console.WriteLine("6 - make a Touareg tea");
            Console.WriteLine("");
            Console.Write("SELECT THE OPTION: ");

            char option = Console.ReadKey().KeyChar;
            while (option == '3')
            {
                //https://stackoverflow.com/questions/5195692/is-there-a-way-to-delete-a-character-that-has-just-been-written-using-console-wr
                Console.Write("\b \b");
                option = Console.ReadKey().KeyChar;
            }

            switch (option)
            {
                case '1':
                    Console.Clear();
                    if (FileProcessor.ReadAndReverse(PATH))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("SUCCESS!\r\n");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("FAILED =(\r\n");
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                    Menu();
                    break;

                case '2':
                    Console.Clear();
                    if (FileProcessor.ReadAndSort(PATH))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("SUCCESS!\r\n");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("FAILED =(\r\n");
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                    Menu();
                    break;

                case '4':
                    Console.Clear();
                    MakeTea();
                    Menu();
                    break;

                case '5':
                    Console.Clear();
                    if (FileProcessor.MakeBatchTeas(PATH, PATH_INPUT))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("SUCCESS! Check file: result-5.txt\r\n");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("FAILED =( Any file has not been created.\r\n");
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                    Menu();
                    break;

                case '6':
                    Console.Clear();
                    string result = FileProcessor.MakeTouaregTea(PATH, PATH_TOUAREG_INPUT);
                    if (result.Contains("Congratulations"))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Console.WriteLine(result);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("");
                    Menu();
                    break;

                default:
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("There is not such the option. Try again.\r\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    Menu();
                    break;
            }
        }

        /// <summary>
        /// Takes name, temperature and time. Then evaulates it and save into a file.
        /// </summary>
        private static void MakeTea()
        {
            try
            {
                //Read the name of the tea
                Console.Write("Type the name of the tea: ");
                string name = Console.ReadLine();
                while (name.Length < 3)
                {
                    Console.Write("Name has to be 3 charachtes lenght minimum. Try again: ");
                    name = Console.ReadLine();
                }

                //Read the tempatature
                Console.Write("Enter the temparature (\x00B0C) of water: ");
                int temp = 0;
                while (!int.TryParse(Console.ReadLine(), out temp))
                {
                    Console.Write("The given input is not the correct number. Try again: ");
                }

                //Read the time
                Console.Write("Put brewing time (seconds): ");
                int time = 0;

                while (!int.TryParse(Console.ReadLine(), out time))
                {
                    Console.Write("The given input is not the correct number. Try again: ");
                }

                Console.Clear();
                var result = FileProcessor.CompareParameters(PATH, name, temp, time);
                Console.WriteLine(result);
                FileProcessor.SaveTeaResult(PATH, name, temp, time, result);
                Console.WriteLine("");
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        /// <summary>
        /// Determines resource path
        /// </summary>
        /// <param name="v">File name</param>
        /// <returns></returns>
        private static string GetFilePath(string v)
        {
            return string.Format("{0}Resources\\" + v, Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\")));
        }
    }
}
