using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class DiceRollProbability
    {
        // A dictionary to hold the probabilities of each possible sum
        private readonly Dictionary<int, double> _sumProbabilities = new Dictionary<int, double>();

        // A 2D array to hold the probabilities of each possible dice roll combination
        private readonly double[,] _rollProbabilities = new double[6, 6];

        public DiceRollProbability()
        {
            // Calculate the probabilities of each possible sum in backgammon
            _sumProbabilities.Add(1, 1.0 / 6); // For 1 in die
            _sumProbabilities.Add(2, (1.0 / 36) + (1.0 / 6)); // For double 1-1, or 2 in die
            _sumProbabilities.Add(3, 0.25); // For double 1-1, or 3 in die, or dice 2-1 (1.0 / 36) + (6.0 / 36) + (2.0 / 36)
            _sumProbabilities.Add(4, 5.0 / 18); // For double 1-1, or 4 in die, or 3-1, or double 2-2 (1.0 / 36) + (6.0 / 36) + (2.0 / 36) + (1.0 / 36)
            _sumProbabilities.Add(5, 5.0 / 18); // For 5 in die, or 3-2, or 4-1 (1.0 / 6) + (2.0 / 36) + (2.0 / 36)
            _sumProbabilities.Add(6, 1.0 / 3); // For 6 in die, or double 2-2, or double 3-3, or dice 5-1, or dice 4-2 (1.0 / 6) + (1.0 / 36) + (1.0 / 36) + (2.0 / 36) + (2.0 / 36)
            _sumProbabilities.Add(7, (2.0 / 36) + (2.0 / 36) + (2.0 / 36)); // For dice 4-3, or dice 5-2, or dice 6-1
            _sumProbabilities.Add(8, (1.0 / 36) + (1.0 / 36) + (2.0 / 36) + (2.0 / 36)); // For double 2-2, or double 4-4, or dice 6-2, or dice 5-3
            _sumProbabilities.Add(9, (1.0 / 36) + (2.0 / 36) + (2.0 / 36)); // For double 3-3, or dice 6-3, or dice 5-4
            _sumProbabilities.Add(10, (1.0 / 36) + (2.0 / 36)); // For double 5-5, or dice 6-4
            _sumProbabilities.Add(11, 2.0 / 36); // For dice 6-5
            _sumProbabilities.Add(12, (1.0 / 36) + (1.0 / 36) + (1.0 / 36)); // For double 6-6, or double 3-3, or double 4-4
            _sumProbabilities.Add(15, 1.0 / 36); // For double 5-5
            _sumProbabilities.Add(16, 1.0 / 36); // For double 4-4
            _sumProbabilities.Add(18, 1.0 / 36); // For double 6-6
            _sumProbabilities.Add(20, 1.0 / 36); // For double 5-5
            _sumProbabilities.Add(24, 1.0 / 36); // For double 6-6

            // Calculate the probabilities of each possible dice roll combination
            for (int i = 1; i <= 6; i++)
            {
                for (int j = 1; j <= 6; j++)
                {
                    _rollProbabilities[i - 1, j - 1] = 1.0 / 36;
                }
            }
        }

        public double GetSumProbability(int sum)
        {
            if (_sumProbabilities.ContainsKey(sum))
            {
                return _sumProbabilities[sum];
            }
            return 0;
        }

        public double GetRollProbability(int die1, int die2)
        {
            if (die1 == die2) return _rollProbabilities[die1 - 1, die2 - 1];
            return _rollProbabilities[die1 - 1, die2 - 1];
        }
    }
}
