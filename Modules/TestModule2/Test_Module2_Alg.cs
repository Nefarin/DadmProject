using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EKG_Project.Modules.TestModule2
{
    public partial class Test_Module2 : IModule
    {

        private void printFibonacci(uint numberOfElements)
        {
            switch (_moduleParams.Mode)
            {
                case Fibonacci_Mode.RECURSIVE:
                    printFibboRecur(numberOfElements);
                    break;
                case Fibonacci_Mode.ITERATIVE:
                    printFibboIter(numberOfElements);
                    break;
                default:
                    break;
            }
        }

        private void printFibboIter(uint numberOfElements)
        {
            uint first = 1;
            uint second = 1;

            if (numberOfElements == 1)
            {
                Console.WriteLine(first.ToString());
            }
            if (numberOfElements >= 2)
            {
                Console.WriteLine(first.ToString());
                Console.WriteLine(second.ToString());
            }

            for (uint i = 3; i <= numberOfElements; i++)
            {
                uint temp = first + second;
                Console.WriteLine((temp).ToString());
                first = second;
                second = temp;
            }

        }

        private void printFibboRecur(uint numberOfElements)
        {
            for (uint i = 1; i <= numberOfElements; i++)
            {
                Console.WriteLine(calcFibbo(i).ToString());
            }
        }

        private int calcFibbo(uint n)
        {
            if (n == 0)
            {
                return -1;
            }

            if (n == 1 || n == 2)
            {
                return 1;
            }

            else
            {
                return calcFibbo(n - 1) + calcFibbo(n - 2);
            }

        }

        public static void Main(string [] args)
        {
            //Test fibbo.
            Test_Module2 recursive = new Test_Module2();
            recursive.Init(new Test_Module2_Param(Fibonacci_Mode.RECURSIVE));
            Test_Module2 iterative = new Test_Module2();
            iterative.Init(new Test_Module2_Param(Fibonacci_Mode.ITERATIVE));

            recursive.printFibonacci(10);
            iterative.printFibonacci(10);
        }

    }
}
