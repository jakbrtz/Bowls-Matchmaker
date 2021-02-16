using Matchmaker.Collections;
using Matchmaker.Data;
using System.Collections.Generic;

namespace Matchmaker.Algorithms.Structures
{
    public class DayGeneratorParameters
    {
        public IList<Player> players;
        public IList<Day> history;
        public Weights weights;
        public Counter<MatchSize> numMatchSizes;
    }
}
