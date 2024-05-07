using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JA.Numerics;
using JA.Numerics.Geometry;

namespace LinearAlgebraConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Point2 center = new Point2(Vector2.One);
            Ellipse2 ellipse = new Ellipse2(center, 7, 1);
            Point2 point1 = new Point2(8, 4, 1) + Vector2.One;
            Point2 point2 = new Point2(27, 0, 1) + Vector2.One;
            Line2 line = Line2.Join(point1, point2);
            Point2 contact = ellipse.GetClosestPoint(line, NumericalMethods.LooseTolerance);
            Point2 target = line.GetClosestPoint(contact);

            Console.WriteLine($"Ellipse = {ellipse}");
            Console.WriteLine($"Line    = {line}");
            Console.WriteLine($"Contact = {contact}");
            Console.WriteLine($"Target  = {target}");

            //Ellipse = Ellipse(x=0, y=0, rx=7, ry=1)
            //Line    = 4*x + 19*y + -108 = 0
            //Contact = Point(x=5.79233084048069, y=0.561501329971278)
            //Target  = Point(x=6.57919716799924, y=4.29911638568437)

            //Ellipse = Ellipse(x=1, y=1, rx=7, ry=1)
            //Line    = 4*x + 19*y + -131 = 0
            //Contact = Point(x=6.79233084048069, y=1.56150132997128)
            //Target  = Point(x=7.57919716799924, y=5.29911638568437)
        }
    }
}
