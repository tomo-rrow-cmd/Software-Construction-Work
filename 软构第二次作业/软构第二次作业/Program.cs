using System;
using System.Collections.Generic;

namespace ShapeAreaCalculator
{
    /// 形状抽象基类，定义面积计算和合法性判断接口
    public abstract class Shape
    {
        public abstract double GetArea();
        public abstract bool IsValid();
    }

    public class Rectangle : Shape
    {
        public double Length { get; set; }
        public double Width { get; set; }

        public Rectangle(double length, double width)
        {
            Length = length;
            Width = width;
        }

        public override double GetArea() => Length * Width;
        public override bool IsValid() => Length > 0 && Width > 0;
    }

    
    public class Square : Shape
    {
        public double Side { get; set; }

        public Square(double side)
        {
            Side = side;
        }

        public override double GetArea() => Side * Side;
        public override bool IsValid() => Side > 0;
    }

   
    public class Circle : Shape
    {
        public double Radius { get; set; }

        public Circle(double radius)
        {
            Radius = radius;
        }

        public override double GetArea() => Math.PI * Radius * Radius;
        public override bool IsValid() => Radius > 0;
    }

    class Program
    {
        static Random random = new Random();

        /// 随机生成一个形状（可能非法）
        static Shape GenerateRandomShape()
        {
            int type = random.Next(3); // 0:矩形, 1:正方形, 2:圆形
            switch (type)
            {
                case 0:
                    double length = GetRandomDimension();
                    double width = GetRandomDimension();
                    return new Rectangle(length, width);
                case 1:
                    double side = GetRandomDimension();
                    return new Square(side);
                case 2:
                    double radius = GetRandomDimension();
                    return new Circle(radius);
                default:
                    throw new InvalidOperationException("未知形状类型");
            }
        }

  
        /// 生成随机尺寸：90%概率为正数（1~10），10%概率为非正数（-1~0）
        static double GetRandomDimension()
        {
            // 以10%的概率返回一个非正数（-1到0之间），否则返回1到10之间的正数
            if (random.NextDouble() < 0.1)
            {
                return random.NextDouble() * -1; // -1.0 到 0.0
            }
            else
            {
                return random.NextDouble() * 9 + 1; // 1.0 到 10.0
            }
        }

        static void Main(string[] args)
        {
            List<Shape> shapes = new List<Shape>();
            Console.WriteLine("正在生成10个合法形状...\n");

            // 循环生成直到获得10个合法形状
            while (shapes.Count < 10)
            {
                Shape shape = GenerateRandomShape();
                if (shape.IsValid())
                {
                    shapes.Add(shape);
                    Console.WriteLine($"生成合法形状: {shape.GetType().Name,-10} 面积 = {shape.GetArea():F2}");
                }
                else
                {
                    // 输出非法形状信息
                    Console.WriteLine($"生成非法形状: {shape.GetType().Name}，已丢弃");
                }
            }

            // 计算总面积
            double totalArea = 0;
            foreach (var shape in shapes)
            {
                totalArea += shape.GetArea();
            }

            Console.WriteLine($"\n10个形状的总面积: {totalArea:F2}");
        }
    }
}