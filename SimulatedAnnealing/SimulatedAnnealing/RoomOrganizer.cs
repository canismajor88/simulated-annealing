using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Security.Cryptography.X509Certificates;

namespace SimulatedAnnealing
{
    public class RoomOrganizer
    {
        private static int _numberOfRooms;
        private int _totalRoomScore = 0;
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
                _roomArray[i] = new Room(studentCount,studentCount+1,studentCount+2,studentCount+3);
                studentCount = (4 * (i + 1) - 1);
            }
            
        }


        private int CalculateTotalScore()
        {
            int totalRoomScore = 0;
            for (var i = 0; i < _roomArray.Length; i++)
            {
                totalRoomScore += _roomArray[i].RoomScore ;
            }

            return totalRoomScore;
        }

        private void printOrgaziedRoomList(double temperature, double coolingCoefficient)
        {
            Console.WriteLine("Best score:" + FindBestRoomScore());
            Console.WriteLine("Worst score:" + FindWorstRoomScore());
            Console.WriteLine("Average score:" + FindAverageScore());
            Console.WriteLine("Room Listing:");
            Console.WriteLine();
            for (var i = 0; i < _roomArray.Length; i++)
            {
                Console.Write("Room:" + (i + 1) + " ,has students: " + _roomArray[i].GetStudentsInRoom());
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
            return _totalRoomScore / _numberOfRooms;
        }

        public void OrganizeRooms()
        {
            //via Simulated Annealing
            double temperature = 100000;
            var count = 10000000;
            var coolingCoefficient = .95;
            var coolingSchedualChanges = 0;
            var coolingSchedualAttemps = 0;
            float e = (float) System.Math.E;
            _totalRoomScore = CalculateTotalScore();
            var r = new Random();
            float chanceToChange = 0;
            Room oldRoomOne = null;
            Room oldRoomTwo = null;
            while (count >= 0 && _totalRoomScore != 0)
            {
                count--;
                var method = r.Next(0, 2);
                var firstRoomChosen = r.Next(0, 49);
                var secondRoomChosen = r.Next(0, 49);
                if (method == 1)
                {
                    var tempRoomOne = _roomArray[firstRoomChosen];
                    var tempRoomTwo = _roomArray[secondRoomChosen];
                    oldRoomOne = tempRoomOne;
                    oldRoomTwo = tempRoomTwo;
                    var oldTotalScore = CalculateTotalScore();
                    var firstStuChosen = r.Next(0, 3);
                    var secondStuChosen = r.Next(0, 3);
                    var tempStuOne = tempRoomOne.StudentsInRoom[firstStuChosen];
                    var tempStuTwo = tempRoomTwo.StudentsInRoom[secondStuChosen];
                    tempRoomOne.StudentsInRoom[firstStuChosen] = tempStuTwo;
                    tempRoomTwo.StudentsInRoom[secondStuChosen] = tempStuOne;
                    _roomArray[firstRoomChosen] = tempRoomOne;
                    _roomArray[secondRoomChosen] = tempRoomTwo;
                    _roomArray[firstRoomChosen].CalculateRoomScore();
                    _roomArray[secondRoomChosen].CalculateRoomScore();
                    var newTotalScore = CalculateTotalScore();
                    chanceToChange = (float) Math.Pow(e, (-(newTotalScore - oldTotalScore) / temperature));
                }
                else
                {
                    var tempRoomOne = _roomArray[firstRoomChosen];
                    var tempRoomTwo = _roomArray[secondRoomChosen];
                    oldRoomOne = tempRoomOne;
                    oldRoomTwo = tempRoomTwo;
                    var oldTotalScore = CalculateTotalScore();
                    var stuOne = tempRoomOne.StudentsInRoom[0];
                    var stuTwo = tempRoomOne.StudentsInRoom[1];
                    var stuThree = tempRoomTwo.StudentsInRoom[2];
                    var stuFour = tempRoomTwo.StudentsInRoom[3];
                    tempRoomOne.StudentsInRoom[0] = stuThree;
                    tempRoomOne.StudentsInRoom[1] = stuFour;
                    tempRoomTwo.StudentsInRoom[2] = stuOne;
                    tempRoomTwo.StudentsInRoom[3] = stuTwo;
                    _roomArray[firstRoomChosen] = tempRoomOne;
                    _roomArray[secondRoomChosen] = tempRoomTwo;
                    _roomArray[firstRoomChosen].CalculateRoomScore();
                    _roomArray[secondRoomChosen].CalculateRoomScore();
                    var newTotalScore = CalculateTotalScore();
                    chanceToChange = (float) Math.Pow(e, (-(newTotalScore - oldTotalScore) / temperature));
                }

                if ((float) r.NextDouble() < chanceToChange)
                {
                    coolingSchedualChanges++;
                    if (coolingSchedualChanges % 2000 == 0)
                    {
                        temperature *= coolingCoefficient;
                    }
                }
                else
                {
                    _roomArray[firstRoomChosen] = oldRoomOne;
                    _roomArray[secondRoomChosen] = oldRoomTwo;
                    coolingSchedualAttemps++;
                    if (coolingSchedualAttemps % 20000 == 0)
                    {
                        temperature *= coolingCoefficient;
                        //Console.WriteLine("cooling attempts: " + coolingSchedualAttemps);
                    }
                }
            }

            printOrgaziedRoomList(temperature, coolingCoefficient);
        }
    }
}