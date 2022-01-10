using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder
{
    class LabyrinthGenerator
    {
        public static void generateMaze(PathField mainField, int tickDelay)
        {
            int gridSize = mainField.fieldGrid.GetLength(0);
            bool[,] visited = new bool[gridSize, gridSize];
            List<FieldItem> toVisit = new List<FieldItem>();

            Random randomizer = new Random();
            FieldItem candidate = mainField.fieldGrid[randomizer.Next(gridSize), randomizer.Next(gridSize)];

            while (candidate.GetStatus() == FieldStatus.Start || candidate.GetStatus() == FieldStatus.Target)
            {
                candidate = mainField.fieldGrid[randomizer.Next(gridSize), randomizer.Next(gridSize)];
            }
            toVisit.Add(candidate); //random Start point
            visited[candidate.position.X, candidate.position.Y] = true;

            genTick(toVisit, visited, mainField, tickDelay);
        }

        private static void genTick(List<FieldItem> toVisit, bool[,] visited, PathField mainField, int tickDelay)
        {
            FieldItem currentPos = toVisit.ElementAt(0);
            toVisit.RemoveAt(0);
            List<FieldItem> currentItems = discoverNeighbours(currentPos, mainField.fieldGrid, visited);
            if (currentItems.Count > 0)
            {
                FieldItem luckyCandidate = chooseRandomNeighbour(currentItems);
                toVisit.Add(luckyCandidate);
                foreach (FieldItem item in currentItems)
                {
                    item.updateStatus(FieldStatus.Blocked);
                    toVisit.Add(item);
                }
            }

            if(toVisit.Count > 0)
            {
                if(tickDelay > 0)
                {
                    FieldStatus oldStatus = currentPos.GetStatus();
                    currentPos.updateStatus(FieldStatus.Evaluating);
                    Task.Delay(tickDelay).ContinueWith(_ => { currentPos.updateStatus(oldStatus); genTick(toVisit, visited, mainField, tickDelay);});
                }
                else
                    genTick(toVisit, visited, mainField, tickDelay);
            }
        }

        private static List<FieldItem> discoverNeighbours(FieldItem item, FieldItem[,] grid, bool[,] visited)
        {
            List<FieldItem> neighbours = new List<FieldItem>();
            int itemX = item.position.X;
            int itemY = item.position.Y;
            if(itemX -1 >= 0)
            {
                if (!visited[itemX - 1, itemY] && grid[itemX - 1, itemY].GetStatus() != FieldStatus.Start && grid[itemX - 1, itemY].GetStatus() != FieldStatus.Target) 
                {
                    neighbours.Add(grid[itemX - 1, itemY]);
                    visited[itemX - 1, itemY] = true;
                }
                if(itemY - 1 >= 0)
                {
                    if (!visited[itemX - 1, itemY - 1] && grid[itemX - 1, itemY - 1].GetStatus() != FieldStatus.Start && grid[itemX - 1, itemY - 1].GetStatus() != FieldStatus.Target)
                    {
                        neighbours.Add(grid[itemX - 1, itemY - 1]);
                        visited[itemX - 1, itemY - 1] = true;
                    }
                }
                if(itemY + 1 < grid.GetLength(1))
                {
                    if (!visited[itemX - 1, itemY + 1] && grid[itemX - 1, itemY + 1].GetStatus() != FieldStatus.Start && grid[itemX - 1, itemY + 1].GetStatus() != FieldStatus.Target)
                    {
                        neighbours.Add(grid[itemX - 1, itemY + 1]);
                        visited[itemX - 1, itemY + 1] = true;
                    }
                }
            }
            if (itemX + 1 < grid.GetLength(0))
            {
                if (!visited[itemX + 1, itemY] && grid[itemX + 1, itemY].GetStatus() != FieldStatus.Start && grid[itemX + 1, itemY].GetStatus() != FieldStatus.Target)
                {
                    neighbours.Add(grid[itemX + 1, itemY]);
                    visited[itemX + 1, itemY] = true;
                }
                if (itemY - 1 >= 0)
                {
                    if (!visited[itemX + 1, itemY - 1] && grid[itemX + 1, itemY - 1].GetStatus() != FieldStatus.Start && grid[itemX + 1, itemY - 1].GetStatus() != FieldStatus.Target)
                    {
                        neighbours.Add(grid[itemX + 1, itemY - 1]);
                        visited[itemX + 1, itemY - 1] = true;
                    }
                }
                if (itemY + 1 < grid.GetLength(1))
                {
                    if (!visited[itemX + 1, itemY + 1] && grid[itemX + 1, itemY + 1].GetStatus() != FieldStatus.Start && grid[itemX + 1, itemY + 1].GetStatus() != FieldStatus.Target)
                    {
                        neighbours.Add(grid[itemX + 1, itemY + 1]);
                        visited[itemX + 1, itemY + 1] = true;
                    }
                }
            }
            if(itemY - 1 >= 0)
            {
                if (!visited[itemX, itemY - 1] && grid[itemX, itemY - 1].GetStatus() != FieldStatus.Start && grid[itemX, itemY - 1].GetStatus() != FieldStatus.Target)
                {
                    neighbours.Add(grid[itemX, itemY - 1]);
                    visited[itemX, itemY - 1] = true;
                }
            }

            if (itemY + 1 < grid.GetLength(1))
            {
                if (!visited[itemX, itemY + 1] && grid[itemX, itemY + 1].GetStatus() != FieldStatus.Start && grid[itemX, itemY + 1].GetStatus() != FieldStatus.Target)
                {
                    neighbours.Add(grid[itemX, itemY + 1]);
                    visited[itemX, itemY + 1] = true;
                }
            }

            return neighbours;
        }
        private static FieldItem chooseRandomNeighbour(List<FieldItem>  list)
        {
            FieldItem luckyWinner = null;
            Random randomizer = new Random();
            int roll = randomizer.Next(list.Count);
            luckyWinner = list.ElementAt(roll);
            list.RemoveAt(roll);
            return luckyWinner;
        }
    }
}
