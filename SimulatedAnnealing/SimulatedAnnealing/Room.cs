using System.Linq;
using static SimulatedAnnealing.RoomOrganizer;

namespace SimulatedAnnealing
{
    public class Room
    {
        public readonly int[] StudentsInRoom = new int[4];
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
            var previousStudentNumber = StudentsInRoom[stuNumber];
            StudentsInRoom[stuNumber] = newStudent;
            CalculateRoomScore();
            return previousStudentNumber;
        }

        private void CalculateRoomScore()
        {
            RoomScore = StudentArray[StudentsInRoom[0], StudentsInRoom[1]] +
                        StudentArray[StudentsInRoom[0], StudentsInRoom[2]] +
                        StudentArray[StudentsInRoom[0], StudentsInRoom[3]] +
                        StudentArray[StudentsInRoom[1], StudentsInRoom[2]] +
                        StudentArray[StudentsInRoom[1], StudentsInRoom[3]] +
                        StudentArray[StudentsInRoom[2], StudentsInRoom[3]];
        }

        public string GetStudentsInRoom()
        {
            return StudentsInRoom.Aggregate("", (current, t) => current + (" " + t + " "));
        }
    }
}
