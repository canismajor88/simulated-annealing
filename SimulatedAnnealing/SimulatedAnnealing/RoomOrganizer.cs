using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Security.Cryptography.X509Certificates;

namespace SimulatedAnnealing
{
    public class RoomOrganizer
    {
        private static int _numberOfRooms;
        public static int[,] StudentArray { get; private set; }
        private Room[] _roomArray = new Room[50];

        public RoomOrganizer(int[,] studentArray, int numberOfRooms)
        {
            StudentArray = studentArray;
            _numberOfRooms = numberOfRooms;
            PopulateRoomArray();
        }

        private void PopulateRoomArray()
        {
            int studentCount = 0;
            for (var i = 0; i < _roomArray.Length; i++)
            {
                _roomArray[i] = new Room(studentCount, studentCount + 1, studentCount + 2, studentCount + 3);
                studentCount = (4 * (i + 1));
            }
        }


        private int CalculateTotalScore()
        {
            int totalRoomScore = 0;
            for (var i = 0; i < _roomArray.Length; i++)
            {
                totalRoomScore += _roomArray[i].RoomScore;
            }

            return totalRoomScore;
        }

        private void printOrgaziedRoomList(double temperature, double coolingCoefficient)
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
            var bestScore = _roomArray[0].RoomScore;
            foreach (var t in _roomArray)
            {
                if (bestScore > t.RoomScore)
                {
                    bestScore = t.RoomScore;
                }
            }

            return bestScore;
        }

        private int FindWorstRoomScore()
        {
            var worstScore = _roomArray[0].RoomScore;
            foreach (var t in _roomArray)
            {
                if (worstScore < t.RoomScore)
                {
                    worstScore = t.RoomScore;
                }
            }

            return worstScore;
        }

        private int FindAverageScore()
        {
            return CalculateTotalScore() / _roomArray.Length;
        }

        public void OrganizeRooms()
        {
            //via Simulated Annealing
            double temperature = 100000;
            double initTemp = temperature;
            var count = 10000000;
            var coolingCoefficient = .95;
            var coolingSchedualChanges = 0;
            var coolingSchedualAttemps = 0;
            float e = (float) System.Math.E;
            var progressionScore = 0;
            var r = new Random();
            float chanceToChange = 0;

            while (count >= 0 && progressionScore< count)
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
                    if (method == 0)
                    {
                        var oldScore = oldFirstRoom.RoomScore + oldSecondRoom.RoomScore;
                        var randomStudentForRoomOne = r.Next(4);
                        var randomStudentForRoomTwo = r.Next(4);
                        var studentFromRoomOne = newRoomFirst.SwapOutStudent(randomStudentForRoomOne,
                            newRoomSecond.StudentsInRoom[randomStudentForRoomTwo]);
                        newRoomSecond.SwapOutStudent(randomStudentForRoomTwo, studentFromRoomOne);
                        var newScore = newRoomFirst.RoomScore + newRoomSecond.RoomScore;
                        chanceToChange = (float) Math.Pow(e, (-(newScore - oldScore) / temperature));
                    }
                    else
                    {
                        var oldScore = oldFirstRoom.RoomScore + oldSecondRoom.RoomScore;
                        var firstStudentFromRoomOne = newRoomFirst.SwapOutStudent(0, newRoomSecond.StudentsInRoom[2]);
                        var secondStudentFromRoomOne = newRoomFirst.SwapOutStudent(1, newRoomSecond.StudentsInRoom[3]);
                        newRoomSecond.SwapOutStudent(2, firstStudentFromRoomOne);
                        newRoomSecond.SwapOutStudent(3, secondStudentFromRoomOne);
                        var newScore = newRoomFirst.RoomScore + newRoomSecond.RoomScore;
                        chanceToChange = (float) Math.Pow(e, (-(newScore - oldScore) / temperature));
                    }

                    if ((float) r.NextDouble() < chanceToChange)
                    {
                        _roomArray[firstRoomNumber] = newRoomFirst;
                        _roomArray[secondRoomNumber] = newRoomSecond;
                        if (coolingSchedualChanges % 2000 == 0 && coolingSchedualChanges != 0)
                        {
                            temperature *= coolingCoefficient;
                        }
                        
                        coolingSchedualChanges++;
                        progressionScore = 0;
                    }
                    else
                    {
                        if (coolingSchedualAttemps % 20000 == 0 && coolingSchedualAttemps != 0)
                        {
                            temperature *= coolingCoefficient;
                        }

                        coolingSchedualAttemps++;
                    }

                    var newTotalScore = CalculateTotalScore();
                    if (oldTotalScore > newTotalScore)
                    {
                        progressionScore++;
                    }
                }
            }

            printOrgaziedRoomList(initTemp, coolingCoefficient);
        }
    }
}