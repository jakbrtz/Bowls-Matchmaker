namespace Matchmaker.Data
{
    public enum Grade
    {
        G1,
        G2,
        G3,
        None = -1
    }

    public static partial class Enums
    {
        public static Grade[] Grades = new Grade[] {
            Grade.G1,
            Grade.G2,
            Grade.G3,
        };

        public static Grade[] GradesIncludingNone = new Grade[] {
            Grade.G1,
            Grade.G2,
            Grade.G3,
            Grade.None,
        };
    }
}
