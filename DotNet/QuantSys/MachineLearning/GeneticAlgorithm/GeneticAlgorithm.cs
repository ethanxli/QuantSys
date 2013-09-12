using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Scripting.Runtime;
using QuantSys.MachineLearning.GeneticAlgorithm.Genes;
using QuantSys.Util;

namespace QuantSys.MachineLearning.GeneticAlgorithm
{
    public class GeneticAlgorithm
    {
        private const string OUTPUT_FILEPATH = QSConstants.DEFAULT_DATA_FILEPATH;

        private const int DefaultPopulationSize = 30;
        private const int DefaultNumGenerations = 50;
        private const int DefaultNumTrials = 10;
        private const double DefaultMutationRate = 0.02;
        private readonly List<Gene> ChromosomeFootprint;

        private readonly Func<Chromosome, double> _fitnessFunction;
        private readonly Random _randomSeed;
        private Population _currentPopulation;
        private int _currentTrial;
        public Chromosome maxC = new Chromosome();

        public GeneticAlgorithm(Func<Chromosome, double> fitness,
            List<Gene> ChromosomeFootprint,
            int popSize = DefaultPopulationSize,
            int generations = DefaultNumGenerations,
            int trials = DefaultNumTrials,
            double mutRate = DefaultMutationRate
            )
        {

            _fitnessFunction = fitness;
            _randomSeed = new Random();
            this.ChromosomeFootprint = ChromosomeFootprint;

            PopulationSize = popSize;
            Generations = generations;
            Trials = trials;
            MutationRate = mutRate;
        }

        public int PopulationSize { get; set; }
        public int Generations { get; set; }
        public int Trials { get; set; }
        public double MutationRate { get; set; }

        private void InitializePopulation()
        {
            var c = new List<Chromosome>();

            for (int i = 0; i < PopulationSize; i++)
            {
                var gList = new List<Gene>();
                foreach (Gene g in ChromosomeFootprint)
                {
                    Gene gene = g.Clone();
                    gene.InitializeGene();
                    gList.Add(gene);
                }

                var cx = new Chromosome(gList);
                c.Add(cx);
            }

            _currentPopulation = new Population(c);
            _currentPopulation.CalculateAllFitness(_fitnessFunction);
        }

        public void Run()
        {
            InitializePopulation();

            double maxFitness = 0;

            while (_currentTrial++ < Trials)
            {
                
                Console.WriteLine("Starting trial " + _currentTrial + "...");

                string filename = QSConstants.DEFAULT_DATA_FILEPATH + "result" + _currentTrial + ".txt";

                using (var file = new StreamWriter(@filename))
                {
                    for (int generation = 0; generation < Generations; generation++)
                    {
                        Console.WriteLine(generation);
                        var newPop = new Population();

                        //elitism
                        Population elite = _currentPopulation.ElitismSelection(2);

                        newPop.JoinPopulation(elite);

                        //populate rest of population with children
                        while (newPop.PopulationSize < PopulationSize)
                        {
                            //selection
                            Population parents = _currentPopulation.SimpleRouletteSelection(2, _randomSeed);

                            //crossover
                            Population children = Crossover.BinaryCrossover(parents[0], parents[1], _randomSeed);

                            newPop.JoinPopulation(children);
                        }

                        newPop.MutatePopulation(MutationRate, _randomSeed);

                        double average = 0;

                        //calculate fitness for new population
                        foreach (Chromosome chromosome in newPop.Chromosomes)
                        {
                            if (chromosome.Fitness.Equals(double.NaN))
                                chromosome.CalculateFitness(_fitnessFunction);
                            if (chromosome.Fitness > maxFitness)
                            {
                                maxFitness = chromosome.Fitness;
                                maxC = chromosome;

                                file.WriteLine("Generation: {0}", generation);
                                file.WriteLine("Fitness: {0}", maxFitness);
                                for (int i = 0; i < maxC.Count(); i++)
                                {
                                    file.WriteLine("Gene " + i + ":" + maxC[i]);
                                }

                                file.WriteLine();
                                file.Flush();
                            }

                            average += chromosome.Fitness;
                            //Console.WriteLine(chromosome.Fitness + ": x:" + (double)chromosome.GeneList[0].geneValue + "; y:" + (double)chromosome.GeneList[1].geneValue);
                        }

                        //Console.WriteLine(average/newPop.Chromosomes.Count);
                        //Console.WriteLine("-----------------------------------");

                        _currentPopulation = newPop;
                    }
                }

                maxC.CalculateFitness(_fitnessFunction);
            }

            Console.WriteLine("Execution complete.");
        }
    }
}