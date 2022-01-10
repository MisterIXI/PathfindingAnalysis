using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder
{
    class PathField
    {
        private GridWindow parentWindow;

        private bool areMouseEventsRegisterd = false;
        public bool setStartPointFlag = false;
        public bool setTargetPointFlag = false;

        public FieldItem[,] FieldGrid;
        public Coord StartPoint;
        public Coord TargetPoint;

        public PathField(int gridSize, GridWindow gridWindow)
        {
            CreateNewGrid(gridSize);
            parentWindow = gridWindow;
        }

        public void CreateNewGrid(int gridSize)
        {
            FieldGrid = new FieldItem[gridSize, gridSize];
            StartPoint = new Coord(gridSize - 1, 0);
            TargetPoint = new Coord(0, gridSize - 1);
        }

        public FieldStatus GetItemStatus(int x, int y)
        {
            return FieldGrid[x, y].GetStatus();
        }

        public void UpdateItem (int x, int y, FieldStatus newStatus)
        {
            //todo: only one target and start can exist --> save pos and kill old one accordingly

            FieldGrid[x, y].updateStatus(newStatus);
        }

        public List<FieldItem> getSurrounding (Coord position, bool diagAllowed)
        {
            
            List <FieldItem> surrounding = new List<FieldItem>();
            int x = position.X;
            int y = position.Y;
            if(x+1 < FieldGrid.GetLength(0))//right side
            {
                if(isImportantField(x + 1, y))
                    surrounding.Add(FieldGrid[x + 1, y]);
                if (diagAllowed)
                {
                    if (y - 1 >= 0)
                        if (isImportantField(x + 1, y - 1))
                            surrounding.Add(FieldGrid[x + 1, y - 1]);
                    if (y + 1 < FieldGrid.GetLength(1))
                        if (isImportantField(x + 1, y + 1))
                            surrounding.Add(FieldGrid[x + 1, y + 1]);
                }
            }
            if(x-1 >= 0) //left side
            {
                if (isImportantField(x - 1, y))
                    surrounding.Add(FieldGrid[x - 1, y]);
                if (diagAllowed)
                {
                    if (y - 1 >= 0)
                        if (isImportantField(x - 1, y - 1))
                            surrounding.Add(FieldGrid[x - 1, y - 1]);
                    if (y + 1 < FieldGrid.GetLength(1))
                        if (isImportantField(x - 1, y + 1))
                            surrounding.Add(FieldGrid[x - 1, y + 1]);
                }
            }
            if (y - 1 >= 0) //upper side
                if (isImportantField(x, y - 1))
                    surrounding.Add(FieldGrid[x, y - 1]);
            if (y + 1 < FieldGrid.GetLength(1)) //lower side
                if (isImportantField(x, y + 1))
                    surrounding.Add(FieldGrid[x, y + 1]);
            List<FieldItem> results = new List<FieldItem>();
            foreach (FieldItem item in surrounding) //fix this shit
            {
                if(item.GetStatus() == FieldStatus.Target)
                {
                    item.sourceDirection = getFIFromCoord(position);
                    results.Add(item);
                }
                else
                if(!(item.GetStatus() == FieldStatus.Evaluated && getFIFromCoord(position).costToSource + 1 >= item.costToSource))
                    if(item.GetStatus() != FieldStatus.ToEvaluate || getFIFromCoord(position).costToSource + 1  < item.costToSource)
                    {
                        item.sourceDirection = getFIFromCoord(position);
                        item.costToSource = getFIFromCoord(position).costToSource + 1;
                        item.totalCost = item.costToSource + item.costToTarget;
                        if (item.GetStatus() != FieldStatus.ToEvaluate )//&& item.GetStatus() != FieldStatus.Evaluated)
                        {
                            results.Add(item);
                            item.updateStatus(FieldStatus.ToEvaluate);
                        }
                    }
                
                
            }
            //getFIFromCoord(position).updateStatus(FieldStatus.Evaluated);
            return results;
        }

        public bool isImportantField(int x, int y)
        {
            switch (FieldGrid[x, y].GetStatus())
            {
                case FieldStatus.Blocked:
                case FieldStatus.Start:
                    return false;
                default:
                    return true;
            }
        }
        public void SetMouseRegistered (bool shouldBeEnabled)
        {
            if(areMouseEventsRegisterd != shouldBeEnabled)
            {
                for (int i = 0; i < FieldGrid.GetLength(0); i++)
                {
                    for (int j = 0; j < FieldGrid.GetLength(1); j++)
                    {
                        if (shouldBeEnabled)
                        {
                            FieldGrid[i, j].RegisterMouseEvents();
                        }
                        else
                        {
                            FieldGrid[i, j].UnregisterMouseEvents();
                        }
                    }
                }
                areMouseEventsRegisterd = shouldBeEnabled;
            }
        }

        public FieldItem getStartPoint()
        {
            return FieldGrid[StartPoint.X, StartPoint.Y];
        }

        public FieldItem getTargetPoint()
        {
            return FieldGrid[TargetPoint.X, TargetPoint.Y];
        }

        public FieldItem getFIFromCoord(Coord pos)
        {
            return FieldGrid[pos.X, pos.Y];
        }
    }

}
