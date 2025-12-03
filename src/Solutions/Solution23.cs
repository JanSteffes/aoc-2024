using aoc_2024.Interfaces;
using aoc_2024.SolutionUtils;

namespace aoc_2024.Solutions
{
    public class Solution23 : ISolution
    {
        public string RunPartA(string inputData)
        {
            var computerConnections = ParseUtils.ParseIntoLines(inputData).Select(ComputerConnection.New).ToList();
            var computerList = computerConnections.SelectMany(c => new string[] { c.ComputerA, c.ComputerA }).Distinct().ToList();
            var computerConnectionsMap = BuildComputerConnectionsMap(computerConnections, computerList);
            var computerPairs = GetComputerPairsFromMap(computerConnectionsMap);
            var filters = computerPairs.Where(c => c.Split("-").Any(e => e.StartsWith("t"))).ToList();
            return filters.Count.ToString();
        }

        private HashSet<string> GetComputerPairsFromMap(Dictionary<string, List<string>> computerConnectionsMap)
        {
            var triCons = new HashSet<string>();
            foreach (var computerConnection in computerConnectionsMap)
            {
                // search for each connected computer, if theres a entry in that computers connection, that's also in the current computers connections
                // except this current one
                foreach (var connectedComputer in computerConnection.Value)
                {
                    var connectionsToCheckForCurrent = computerConnection.Value.Except([connectedComputer]);
                    var connectedComputersConnections = computerConnectionsMap[connectedComputer];
                    var foundPairs = connectedComputersConnections.Where(c => connectionsToCheckForCurrent.Contains(c)).ToList();
                    foreach (var pair in foundPairs)
                    {
                        triCons.Add(string.Join("-", (new string[] { computerConnection.Key, connectedComputer, pair }).OrderBy(c => c)));
                    }
                }
            }
            return triCons;
        }

        private static Dictionary<string, List<string>> BuildComputerConnectionsMap(List<ComputerConnection> computerConnections, List<string> computerList)
        {
            var computerConnectionMap = new Dictionary<string, List<string>>();
            foreach (var computer in computerList)
            {
                List<string> connectedComputers = [];
                computerConnectionMap.TryAdd(computer, connectedComputers);
                foreach (var computerConnection in computerConnections)
                {
                    if (computerConnection.TryGetConnectedComputer(computer, out var connectedComputer))
                    {
                        connectedComputers.Add(connectedComputer);
                    }

                }
            }
            return computerConnectionMap;
        }

        public string RunPartB(string inputData)
        {
            var computerConnections = ParseUtils.ParseIntoLines(inputData).Select(ComputerConnection.New).ToList();
            var computerList = computerConnections.SelectMany(c => new string[] { c.ComputerA, c.ComputerA }).Distinct().ToList();
            var computerConnectionsMap = BuildComputerConnectionsMap(computerConnections, computerList);
            var computers = BuildComputersList(computerConnectionsMap);
            var coConnected = computers.First(c => c.Name == "co").GetConnectionSets();
            var allComputerSets = computers.SelectMany(c => c.GetConnectionSets()).ToList();
            var maxSet = allComputerSets.MaxBy(q => q.Count)!;
            var resultString = string.Join(",", maxSet.OrderBy(c => c.Name));
            return resultString;
        }

        private static List<Computer> BuildComputersList(Dictionary<string, List<string>> computerConnectionsMap)
        {
            var resultList = computerConnectionsMap.Keys.Select(c => new Computer(c)).ToList();
            foreach (var entry in computerConnectionsMap)
            {
                var current = resultList.First(c => c.Name == entry.Key);
                var connectedComputers = resultList.Where(c => entry.Value.Contains(c.Name)).ToList();
                current.SetConnectedComputers(connectedComputers);
            }
            return resultList;
        }

        private List<string> GetAllConnectedSetsNotWorking(Dictionary<string, List<string>> computerConnectionsMap)
        {
            // foreach computer
            List<string> resultList = [];
            foreach (var entry in computerConnectionsMap)
            {
                resultList.AddRange(GetConnectedSetsForComputerNotWorking(entry.Key, computerConnectionsMap));
            }

            return resultList;
        }

        private List<string> GetConnectedSetsForComputerNotWorking(string currentComputerSetsToDiscover, Dictionary<string, List<string>> computerConnectionsMap)
        {
            var resultList = new List<string>();
            foreach (var connection in computerConnectionsMap[currentComputerSetsToDiscover])
            {
                var currentConnections = GetConnectedSetWithComputerRecursiveNotWorking(computerConnectionsMap, [currentComputerSetsToDiscover, connection]);
                resultList.AddRange(currentConnections);
            }
            return resultList;
        }

        private static List<string> GetConnectedSetWithComputerRecursiveNotWorking(Dictionary<string, List<string>> computerConnectionsMap, HashSet<string> currentSet)
        {
            if (currentSet.Last() == currentSet.First())
            {
                return [string.Join(",", currentSet.Distinct().OrderBy(a => a))];
            }
            var resultList = new List<string>();
            var connectionsOfCurrent = computerConnectionsMap[currentSet.Last()];
            // don't check for previous last one, as to not create circles taht never end
            var connectionsToCheck = connectionsOfCurrent.Except(currentSet).ToList();
            foreach (var connection in connectionsToCheck)
            {
                var newSet = currentSet.ToHashSet();
                if (newSet.Add(connection))
                {
                    var currentResultList = GetConnectedSetWithComputerRecursiveNotWorking(computerConnectionsMap, newSet);
                    if (currentResultList.Count != 0)
                    {
                        resultList.AddRange(currentResultList);
                    }
                }
            }
            return resultList;


        }
    }

    class Computer
    {
        public string Name { get; set; }

        public List<Computer> ConnectedComputers { get; set; }

        public Computer(string name)
        {
            Name = name;
            ConnectedComputers = [];
        }

        public Computer(string name, List<Computer> connectedComputers)
        {
            Name = name;
            ConnectedComputers = connectedComputers;
        }

        public void SetConnectedComputers(List<Computer> connectedComputers)
        {
            ConnectedComputers = connectedComputers;
        }

        public bool ContainsConnectionToComputer(string computer)
        {
            return ConnectedComputers.Any(c => c.Name == computer || c.ContainsConnectionToComputer(computer));
        }

        internal List<List<Computer>> GetConnectionSets()
        {
            var resultList = new List<List<Computer>>();
            foreach (var connectedComputer in ConnectedComputers)
            {
                // all directly connected computers, that have computers, that are connected to me
                // e.g. get subset where my connectedComputers and their connected computers are the same
                //var alsoConnected = connectedComputer.ConnectedComputers.Where(c => c.ConnectedComputers.Contains(this));
                var possibleSet = connectedComputer.ConnectedComputers.Intersect(ConnectedComputers).ToList();
                // check where they have each other connected
                foreach (var connectedSubComp in possibleSet)
                {
                    var computersToContain = possibleSet.Except([connectedSubComp]).ToList();
                    var containedComputers = connectedSubComp.ConnectedComputers.Intersect(computersToContain).ToList();
                    if (containedComputers.Count() == computersToContain.Count)
                    {
                        resultList.Add([this, connectedComputer, .. possibleSet]);
                    }
                }
                //var isSet = possibleSet.Where(c => c.ConnectedComputers.Intersect(possibleSet).Equals(possibleSet)).ToList();
                //resultList.Add(isSet);
            }
            return resultList;
        }

        private void GetExtendedSet(List<Computer> currentSet)
        {
            var newSet = currentSet.ToList();
            // check if i'm first

        }
    }

    public class ComputerConnection
    {
        public string ComputerA { get; set; }

        public string ComputerB { get; set; }

        public ComputerConnection(string computerA, string computerB)
        {
            ComputerA = computerA;
            ComputerB = computerB;
        }

        public bool ContainsComputer(string computer)
        {
            return ComputerA == computer || ComputerB == computer;
        }

        internal static ComputerConnection New(string connectionLineString)
        {
            var split = connectionLineString.Split("-");
            return new ComputerConnection(split[0], split[1]);
        }

        internal bool TryGetConnectedComputer(string connectedComputerSource, out string connectedComputerTarget)
        {
            if (!ContainsComputer(connectedComputerSource))
            {
                connectedComputerTarget = string.Empty;
                return false;
            }
            connectedComputerTarget = connectedComputerSource == ComputerA ? ComputerB : ComputerA;
            return true;
        }
    }
}