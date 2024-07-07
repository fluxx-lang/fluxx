/**
 * @author Bret Johnson
 * @since 4/3/2015
 */

namespace Faml.Interpreter
{
    public class Context
    {
        public static int[] IntStack = new int[100];
        public static double[] DoubleStack = new double[100];
        public static bool[] BooleanStack = new bool[100];
        public static object[] ObjectStack = new object[100];
        public static ObjectAdder[] ObjectAdderStack = new ObjectAdder[100];
        public static int StackIndex = 0;
        public static int BaseIndex = 0;

        public static void Reset()
        {
            StackIndex = 0;
            BaseIndex = 0;
        }
    }
}
