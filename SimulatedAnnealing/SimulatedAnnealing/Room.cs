using System;
using System.Runtime.CompilerServices;

namespace SimulatedAnnealing
{
    public class Room
    {
        public int[] StudentsInRoom = new int[4];
        public int RoomScore { get; private set; }

        public Room(int studentOne, int studentTwo, int studentThree, int studentFour)
        {
            StudentsInRoom[0] = studentOne;
            StudentsInRoom[1] = studentTwo;
            StudentsInRoom[2] = studentThree;
            StudentsInRoom[3] = studentFour;
            CalculateRoomScore();
        }

        public int SwapOutStudent(int stuNumber, int newStudent)
        {
            int previousStudentNumber = StudentsInRoom[stuNumber];
            StudentsInRoom[stuNumber] = newStudent;
            CalculateRoomScore();
            return previousStudentNumber;
        }

        private void CalculateRoomScore()
        {
            RoomScore = RoomOrganizer.StudentArray[StudentsInRoom[0], StudentsInRoom[1]] +
                        RoomOrganizer.StudentArray[StudentsInRoom[0], StudentsInRoom[2]] +
                        RoomOrganizer.StudentArray[StudentsInRoom[0], StudentsInRoom[3]] +
                        RoomOrganizer.StudentArray[StudentsInRoom[1], StudentsInRoom[2]] +
                        RoomOrganizer.StudentArray[StudentsInRoom[1], StudentsInRoom[3]] +
                        RoomOrganizer.StudentArray[StudentsInRoom[2], StudentsInRoom[3]];
        }

        public string GetStudentsInRoom()
        {
            string output = "";
            for (var i = 0; i < StudentsInRoom.Length; i++)
            {
                output += (" " + StudentsInRoom[i] + " ");
            }

            return output;
        }
    }
}