using System;
using System.Linq;

namespace SimulatedAnnealing
{
    public class RoomOrganizer
    {
        public static int[,] StudentArray { get; private set; }
        private readonly Room[] _roomArray = new Room[50];

        public RoomOrganizer(int[,] studentArray)
        {
            StudentArray = studentArray;
            PopulateRoomArray();

        }

        private void PopulateRoomArray()
        {
            var studentCount = 0;
            for (var i = 0; i < _roomArray.Length; i++)
            {
                _roomArray[i] = new Room(studentCount, studentCount + 1, studentCount + 2, studentCount + 3);
                studentCount = (4 * (i + 1));
            }
        }

        private int CalculateTotalScore()
        {
            return _roomArray.Sum(t => t.RoomScore);
        }

        private void PrintOrganizedRoomList(double temperature, double coolingCoefficient)
        {
            Console.WriteLine("Temperature: " + temperature);
            Console.WriteLine("Cooling Coefficient:" + coolingCoefficient);
            Console.WriteLine("Best score:" + FindBestRoomScore());
            Console.WriteLine("Worst score:" + FindWorstRoomScore());
            Console.WriteLine("Average score:" + FindAverageScore());
            Console.WriteLine("Room Listing:");
            Console.WriteLine();
            for (var i = 0; i < _roomArray.Length; i++)
            {
                Console.Write("Room:" + (i + 1) + " ,has students: " + _roomArray[i].GetStudentsInRoom() +
                              " ,room score: " + _roomArray[i].RoomScore);
                Console.WriteLine();
            }
        }

        private int FindBestRoomScore()
        {
            return _roomArray.Min(r => r.RoomScore);
        }

        private int FindWorstRoomScore()
        {
            return _roomArray.Max(r => r.RoomScore);
        }

        private double FindAverageScore()
        {
            return _roomArray.Average(i => i.RoomScore);
        }

        public void OrganizeRooms()
        {
            //via Simulated Annealing
            double temperature = 100000;
            var initTemp = temperature;
            var count = 10000000;
            const double coolingCoefficient = .99;
            var coolingScheduleChanges = 0;
            var coolingScheduleAttempts = 0;
            var progressionScore = 0;
            var r = new Random();

            while ( count >= 0 && progressionScore < count)
            {
                count--;
                var method = r.Next(2);
                var firstRoomNumber = r.Next(50);
                var secondRoomNumber = r.Next(50);
                var oldFirstRoom = _roomArray[firstRoomNumber];
                var oldSecondRoom = _roomArray[secondRoomNumber];
                var newRoomFirst = new Room(oldFirstRoom.StudentsInRoom[0], oldFirstRoom.StudentsInRoom[1],
                    oldFirstRoom.StudentsInRoom[2], oldFirstRoom.StudentsInRoom[3]);
                var newRoomSecond = new Room(oldSecondRoom.StudentsInRoom[0], oldSecondRoom.StudentsInRoom[1],
                    oldSecondRoom.StudentsInRoom[2], oldSecondRoom.StudentsInRoom[3]);
                var oldTotalScore = CalculateTotalScore();
                if (oldFirstRoom != oldSecondRoom)
                {
                    float chanceToChange = 0;
                    if (method == 0)
                    {
                        var oldScore = oldFirstRoom.RoomScore + oldSecondRoom.RoomScore;
                        var randomStudentForRoomOne = r.Next(4);
                        var randomStudentForRoomTwo = r.Next(4);
                        var studentFromRoomOne = newRoomFirst.SwapOutStudent(randomStudentForRoomOne,
                            newRoomSecond.StudentsInRoom[randomStudentForRoomTwo]);
                        newRoomSecond.SwapOutStudent(randomStudentForRoomTwo, studentFromRoomOne);
                        var newScore = newRoomFirst.RoomScore + newRoomSecond.RoomScore;
                        chanceToChange = (float) Math.Pow((float) Math.E, (-(newScore - oldScore) / temperature));
                    }
                    else
                    {
                        var oldScore = oldFirstRoom.RoomScore + oldSecondRoom.RoomScore;
                        var firstStudentFromRoomOne = newRoomFirst.SwapOutStudent(0, newRoomSecond.StudentsInRoom[2]);
                        var secondStudentFromRoomOne = newRoomFirst.SwapOutStudent(1, newRoomSecond.StudentsInRoom[3]);
                        newRoomSecond.SwapOutStudent(2, firstStudentFromRoomOne);
                        newRoomSecond.SwapOutStudent(3, secondStudentFromRoomOne);
                        var newScore = newRoomFirst.RoomScore + newRoomSecond.RoomScore;
                        chanceToChange = (float) Math.Pow((float) Math.E, (-(newScore - oldScore) / temperature));
                    }

                    if ((float) r.NextDouble() < chanceToChange)
                    {
                        _roomArray[firstRoomNumber] = newRoomFirst;
                        _roomArray[secondRoomNumber] = newRoomSecond;
                        if (coolingScheduleChanges % 2000 == 0 && coolingScheduleChanges != 0)
                        {
                            temperature *= coolingCoefficient;
                        }

                        coolingScheduleChanges++;
                        progressionScore = 0;
                    }
                    else
                    {
                        if (coolingScheduleAttempts % 20000 == 0 && coolingScheduleAttempts != 0)
                        {
                            temperature *= coolingCoefficient;
                        }

                        coolingScheduleAttempts++;
                    }

                    var newTotalScore = CalculateTotalScore();
                    if (oldTotalScore > newTotalScore)
                    {
                        progressionScore++;
                    }
                }
            }

            PrintOrganizedRoomList(initTemp, coolingCoefficient);
        }
    }
}
