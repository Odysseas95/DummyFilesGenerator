using System;
using System.IO;
using System.Threading;
using System.Timers;

namespace DummyFilesGenerator
{
    static class DummyFilesGenerator
    {

        public static Random Gen = new Random();
        public static Random GenDirs = new Random();
        public static Random Size = new Random();

        public static System.Timers.Timer Tmr;

        public static DateTime StartDate = new DateTime(2022, 1, 1);
        public static int Range;
        public static DateTime RndDate;
        public static string SubsubFolder;
        public static int SubDirs;

        public static string Path = Properties.Settings.Default.Path;
        public static string FileName;
        public static int FilesNum = Properties.Settings.Default.FilesNum;
        public static int DirNum = Properties.Settings.Default.DirNum;
        public static int SizeMod = Properties.Settings.Default.SizeMod;
        public static int IntervalMins = Properties.Settings.Default.IntervalMins;
        public static string FileEx = Properties.Settings.Default.FileEx;
        public static bool Flag = false;

        static void Main()
        {
            try
            {
                
                IntervalMins = IntervalMins * 60000;

                Console.WriteLine("Timer started");
                Tmr = new System.Timers.Timer();
                Tmr.Interval = IntervalMins;
                Tmr.Elapsed += OnTimedEvent;
                Tmr.Enabled = true;

                Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);
                Console.ReadLine();
                Console.WriteLine("Terminating the application...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            try
            {
                string path = Path;
                Tmr.Enabled = false;
                Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
                Console.WriteLine("Generating...");

                using (var progress = new ProgressBar())
                {
                    for (int i = 0; i < DirNum; i++)
                    {

                        progress.Report((double)i / DirNum);
                        Thread.Sleep(20);

                        SubDirs = GenDirs.Next(DirNum);

                        if (!Flag)
                        {
                            while (SubDirs <= DirNum)
                            {
                                SubsubFolder = new DirectoryInfo(path).CreateSubdirectory("subfolder" + SubDirs).FullName;

                                using (var progress1 = new ProgressBar())
                                {
                                    for (int j = 0; j < FilesNum; j++)
                                    {

                                        progress1.Report((double)j / FilesNum);
                                        Thread.Sleep(20);

                                        RndDate = RandomDay();
                                        Generate(SubsubFolder);

                                    }
                                }
                                SubDirs++;
                            }
                            path = SubsubFolder;
                        }
                        else
                        {
                            using (var progress1 = new ProgressBar())
                            {

                                string[] subdirectoryEntries = Directory.GetDirectories(Path);

                                foreach (string subdirectory in subdirectoryEntries)
                                {
                                    for (int j = 0; j < FilesNum; j++)
                                    {

                                        progress1.Report((double)j / FilesNum);
                                        Thread.Sleep(20);

                                        RndDate = RandomDay();
                                        Generate(subdirectory);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (OverflowException ex)
            {
                Console.WriteLine(ex.ToString() + " \nSettings values too high, getting default values");
                FilesNum = 10;
                DirNum = 5;
                SizeMod = 500;
                Console.WriteLine(ex.ToString() + $" \nFiles Number = {FilesNum} Directories Number = {DirNum} Size Modifier = {SizeMod}");

            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.ToString() + " \nTHE APP WILL TERMINATE!");


            }
            catch (PathTooLongException ex)
            {
                Flag = true;
                Console.WriteLine(ex.ToString());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

            }
            finally
            {
                Console.WriteLine("DONE!");
                Tmr.Enabled = true;
            }
        }
        private static void Generate(string path)
        {
            string fileEx;
            string[] extensionArray;
            Random rnd = new Random();
            if (!Flag)
            {
                extensionArray = FileEx.Split(' ');
                int index = rnd.Next(extensionArray.Length);
                fileEx = extensionArray[index];
                FileName = System.IO.Path.ChangeExtension(System.IO.Path.GetRandomFileName(), fileEx);

            }
            else
            {
                FileName = System.IO.Path.ChangeExtension(System.IO.Path.GetRandomFileName(), ".log");
            }
            
            path = System.IO.Path.Combine(path, FileName);

            //Find another way to generate large files
            var bytes = new byte[SizeMod * DirNum * FilesNum];
            Size.NextBytes(bytes);

            using (StreamWriter w = File.AppendText(path))
            {
                foreach (byte byteValue in bytes)
                {
                    w.WriteLine(byteValue);
                }
            }

            File.SetLastWriteTime(path, RndDate);

        }
        private static DateTime RandomDay()
        {
            Range = (DateTime.Today - StartDate).Days;
            return StartDate.AddDays(Gen.Next(Range));
        }
    }
}


