/******************************************************************************
* Filename    = TextRankSummarizer.cs
*
* Author      = Pratham Ravindra Nagpure
*
* Roll number = 112001054
*
* Product     = Messenger 
* 
* Project     = MessengerDashboard
*
* Description = This file contains an implementation of a text rank summarizer.
*****************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MessengerDashboard.Summarization
{
    /// <summary>
    /// Implements the TextRank algorithm for text summarization.
    /// </summary>
    public class TextRankSummarizer : ITextSummarizer
    {
        public TextSummary Summarize(string[] sentences, TextSummarizationOptions options)
        {
            List<string> preprocessedSentences = sentences.Select(PreprocessSentence).ToList();
            double[,] similarityMatrix = CalculateSimilarityMatrix(preprocessedSentences);
            double[] sentenceScores = CalculateTextRank(similarityMatrix);
            List<string> rankedSentences = RankSentences(sentences.ToList(), sentenceScores);
            List<string> result = new();
            for (int i = 0; i < Math.Min(options.MaxSummarySentences, (options.MaxSummaryPercentage / 100.0) * sentences.Length); ++i)
            {
                result.Add(rankedSentences[i]);
            }
            TextSummary textSummary = new(result);
            return textSummary;
        }

        private string PreprocessSentence(string sentence)
        {
            sentence = sentence.ToLower();
            sentence = Regex.Replace(sentence, @"[^\w\s]", "");
            return sentence;
        }

        private double CosineSimilarity(string sentence1, string sentence2)
        {
            string[] words1 = sentence1.Split(' ');
            string[] words2 = sentence2.Split(' ');

            IEnumerable<string> commonWords = words1.Intersect(words2);
            double dotProduct = commonWords.Count();

            double magnitude1 = Math.Sqrt(words1.Length);
            double magnitude2 = Math.Sqrt(words2.Length);

            if (magnitude1 == 0 || magnitude2 == 0)
            {
                return 0;
            }

            return dotProduct / (magnitude1 * magnitude2);
        }

        private double[,] CalculateSimilarityMatrix(List<string> sentences)
        {
            int numSentences = sentences.Count;
            double[,] similarityMatrix = new double[numSentences, numSentences];

            for (int i = 0; i < numSentences; i++)
            {
                for (int j = 0; j < numSentences; j++)
                {
                    if (i == j)
                    {
                        similarityMatrix[i, j] = 1.0;
                    }
                    else
                    {
                        similarityMatrix[i, j] = CosineSimilarity(sentences[i], sentences[j]);
                    }
                }
            }

            return similarityMatrix;
        }

        private double[] CalculateTextRank(double[,] similarityMatrix, double damping = 0.85, double tolerance = 0.0001, int maxSteps = 100)
        {
            int numNodes = similarityMatrix.GetLength(0);

            // Initialize scores with equal values
            double[] scores = new double[numNodes];
            for (int i = 0; i < numNodes; i++)
            {
                scores[i] = 1.0 / numNodes;
            }

            for (int step = 0; step < maxSteps; step++)
            {
                double[] newScores = new double[numNodes];

                double maxChange = 0.0;

                for (int i = 0; i < numNodes; i++)
                {
                    double weightedSum = 0.0;

                    for (int j = 0; j < numNodes; j++)
                    {
                        if (i != j)
                        {
                            weightedSum += similarityMatrix[i, j] * scores[j] /
                                (SumEdgeWeights(j, similarityMatrix) + double.Epsilon);
                        }
                    }

                    newScores[i] = (1 - damping) + damping * weightedSum;

                    maxChange = Math.Max(maxChange, Math.Abs(newScores[i] - scores[i]));
                }

                scores = newScores;

                if (maxChange < tolerance)
                {
                    break; // Convergence reached
                }
            }

            return scores;
        }

        /// <summary>
        /// Helper function to calculate the sum of edge weights for a given node
        /// </summary>
        /// <param name="nodeIndex"></param>
        /// <param name="similarityMatrix"></param>
        /// <returns></returns>
        private double SumEdgeWeights(int nodeIndex, double[,] similarityMatrix)
        {
            int numNodes = similarityMatrix.GetLength(0);
            double sum = 0.0;

            for (int j = 0; j < numNodes; j++)
            {
                if (j != nodeIndex)
                {
                    sum += similarityMatrix[nodeIndex, j];
                }
            }

            return sum;
        }


        List<string> RankSentences(List<string> sentences, double[] sentenceScores)
        {
            var rankedSentences = sentences
                .Select((sentence, index) => new { Sentence = sentence, Score = sentenceScores[index] })
                .OrderByDescending(item => item.Score)
                .Select(item => item.Sentence)
                .ToList();

            return rankedSentences;
        }

    }
}
