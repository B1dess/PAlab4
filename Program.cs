using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static Random random = new Random();

    // Параметри задачі
    const int Capacity = 200;
    const int NumItems = 100;
    const int PopulationSize = 100;
    const double MutationProbability = 0.1;
    const int MaxGenerations = 1000;

    // Дані про предмети
    static int[] weights = new int[NumItems];
    static int[] values = new int[NumItems];

    static void Main(string[] args)
    {
        // Генеруємо випадкові предмети
        for (int i = 0; i < NumItems; i++)
        {
            weights[i] = random.Next(1, 11);
            values[i] = random.Next(2, 21);
        }

        Console.WriteLine("i      Weights   Values");
        for (int i = 0; i < NumItems; i++)
        {
            Console.WriteLine($"{i,-7}{weights[i],-10}{values[i]}");
        }
        

        // Ініціалізуємо початкову популяцію
        List<int[]> population = InitializePopulation();

        for (int generation = 0; generation < MaxGenerations; generation++)
        {
            // Обчислюємо пристосованість
            List<(int[], int)> fitness = population
                .Select(chromosome => (chromosome, Fitness(chromosome)))
                .OrderByDescending(pair => pair.Item2)
                .ToList();

            // Відбираємо найкращих
            List<int[]> newPopulation = fitness.Take(PopulationSize / 2).Select(pair => pair.Item1).ToList();

            // Генеруємо нові особини через схрещування
            while (newPopulation.Count < PopulationSize)
            {
                var parent1 = newPopulation[random.Next(newPopulation.Count)];
                var parent2 = newPopulation[random.Next(newPopulation.Count)];
                var (child1, child2) = Crossover(parent1, parent2);

                newPopulation.Add(child1);
                if (newPopulation.Count < PopulationSize)
                    newPopulation.Add(child2);
            }

            // Мутація
            for (int i = 0; i < newPopulation.Count; i++)
            {
                if (random.NextDouble() < MutationProbability)
                {
                    Mutate(newPopulation[i]);
                }
            }

            // Локальне покращення
            for (int i = 0; i < newPopulation.Count; i++)
            {
                Improve(newPopulation[i]);
            }

            population = newPopulation;

            // Вивід найкращого результату
            int bestFitness = fitness.First().Item2;
            Console.WriteLine($"Generation {generation + 1}: Best Fitness = {bestFitness}");
            //Console.WriteLine($"{bestFitness}");
        }

        var bestSolution = population.OrderByDescending(Fitness).First();
        Console.WriteLine("Best Solution:");
        Console.WriteLine($"Fitness: {Fitness(bestSolution)}, Weight: {CalculateWeight(bestSolution)}");
        Console.WriteLine("Chosen items: ");
        for (int i = 0; i < NumItems; i++)
        {
            if (bestSolution[i] == 1)
            {
                Console.Write($"({i}: {weights[i]}, {values[i]})\n");
            }
        }
    }

    static List<int[]> InitializePopulation()
    {
        var population = new List<int[]>();
        for (int i = 0; i < PopulationSize; i++)
        {
            var chromosome = new int[NumItems];
            chromosome[random.Next(NumItems)] = 1; // Кожна особина починається з одного предмета
            population.Add(chromosome);
        }
        return population;
    }

    static int Fitness(int[] chromosome)
    {
        int totalValue = 0, totalWeight = 0;

        for (int i = 0; i < NumItems; i++)
        {
            if (chromosome[i] == 1)
            {
                totalWeight += weights[i];
                totalValue += values[i];
            }
        }

        return totalWeight <= Capacity ? totalValue : 0;
    }

    static int CalculateWeight(int[] chromosome)
    {
        int totalWeight = 0;
        for (int i = 0; i < NumItems; i++)
        {
            if (chromosome[i] == 1)
                totalWeight += weights[i];
        }
        return totalWeight;
    }

    static (int[], int[]) Crossover(int[] parent1, int[] parent2)
    {
        int length = parent1.Length;
        int point1 = random.Next(length);
        int point2 = random.Next(length);

        if (point1 > point2)
            (point1, point2) = (point2, point1);

        int[] child1 = new int[length];
        int[] child2 = new int[length];

        for (int i = 0; i < length; i++)
        {
            if (i >= point1 && i <= point2)
            {
                child1[i] = parent2[i];
                child2[i] = parent1[i];
            }
            else
            {
                child1[i] = parent1[i];
                child2[i] = parent2[i];
            }
        }

        return (child1, child2);
    }

    static void Mutate(int[] chromosome)
    {
        int index = random.Next(chromosome.Length);
        chromosome[index] = 1 - chromosome[index];
    }

    static void Improve(int[] chromosome)
    {
        // Пробуємо додати новий предмет, якщо він покращить результат
        for (int i = 0; i < NumItems; i++)
        {
            if (chromosome[i] == 0 && CalculateWeight(chromosome) + weights[i] <= Capacity)
            {
                chromosome[i] = 1;
                if (Fitness(chromosome) > Fitness(chromosome))
                    break;
                else
                    chromosome[i] = 0; // Відкат, якщо не покращило
            }
        }
    }
}