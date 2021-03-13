using System.IO;

namespace SimulatedAnnealing
{
    class Program
    {
        private const string FileName = "roommates.txt";

        static void Main(string[] args)
        {
            SolveInputFile(Path.Combine(Directory.GetCurrentDirectory(), FileName));
        }

        public static void SolveInputFile(string fileName)
        {
            var studentArray = new int[200, 200];
            var row = 0;
            var col = 0;
            var number = "";
            if (File.Exists(fileName))
            {
                foreach (var t in File.ReadAllText(FileName))
                {
                    if (t != ' ')
                    {
                        number += t;
                    }
                    else
                    {
                        studentArray[row, col] = int.Parse(number);
                        number = "";
                        col++;
                        if (col == 200)
                        {
                            row += 1;
                            col = 0;
                        }
                    }
                }

                var studentRoomOrganizer = new RoomOrganizer(studentArray, 50);
                studentRoomOrganizer.OrganizeRooms();
            }
        }
    }
}