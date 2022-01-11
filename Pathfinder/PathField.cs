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

        public FieldItem[,] fieldGrid;
        public DetailField[,] detailsGrid;
        public Coord StartPoint;
        public Coord TargetPoint;

        public PathField(int gridSize, GridWindow gridWindow)
        {
            CreateNewGrid(gridSize);
            parentWindow = gridWindow;
        }

        public void CreateNewGrid(int gridSize)
        {
            fieldGrid = new FieldItem[gridSize, gridSize];
            detailsGrid = new DetailField[GridWindow.DETAIL_SIZE, GridWindow.DETAIL_SIZE];
            StartPoint = new Coord(gridSize - 1, 0);
            TargetPoint = new Coord(0, gridSize - 1);
        }

        public FieldStatus GetItemStatus(int x, int y)
        {
            return fieldGrid[x, y].GetStatus();
        }

        public void UpdateItem(int x, int y, FieldStatus newStatus)
        {
            //todo: only one target and start can exist --> save pos and kill old one accordingly

            fieldGrid[x, y].updateStatus(newStatus);
        }

        public List<FieldItem> getSurrounding(FieldItem fieldItem, bool diagAllowed)
        {

            List<FieldItem> surrounding = new List<FieldItem>();
            int x = fieldItem.position.X;
            int y = fieldItem.position.Y;
            if (x + 1 < fieldGrid.GetLength(0))//right side
            {
                if (isImportantField(x + 1, y))
                    surrounding.Add(fieldGrid[x + 1, y]);
                if (diagAllowed)
                {
                    if (y - 1 >= 0)
                        if (isImportantField(x + 1, y - 1))
                            surrounding.Add(fieldGrid[x + 1, y - 1]);
                    if (y + 1 < fieldGrid.GetLength(1))
                        if (isImportantField(x + 1, y + 1))
                            surrounding.Add(fieldGrid[x + 1, y + 1]);
                }
            }
            if (x - 1 >= 0) //left side
            {
                if (isImportantField(x - 1, y))
                    surrounding.Add(fieldGrid[x - 1, y]);
                if (diagAllowed)
                {
                    if (y - 1 >= 0)
                        if (isImportantField(x - 1, y - 1))
                            surrounding.Add(fieldGrid[x - 1, y - 1]);
                    if (y + 1 < fieldGrid.GetLength(1))
                        if (isImportantField(x - 1, y + 1))
                            surrounding.Add(fieldGrid[x - 1, y + 1]);
                }
            }
            if (y - 1 >= 0) //upper side
                if (isImportantField(x, y - 1))
                    surrounding.Add(fieldGrid[x, y - 1]);
            if (y + 1 < fieldGrid.GetLength(1)) //lower side
                if (isImportantField(x, y + 1))
                    surrounding.Add(fieldGrid[x, y + 1]);

            //getFIFromCoord(position).updateStatus(FieldStatus.Evaluated);
            return surrounding;
        }

        public void updateDetails(FieldItem centerFieldItem)
        {
            FieldItem centerFI = boundaryHelper(centerFieldItem);
            for (int i = 0; i < detailsGrid.GetLength(0); i++)
            {
                for (int j = 0; j < detailsGrid.GetLength(1); j++)
                {
                    //copyDetailElements(detailsGrid[i, j].dataSource, fieldGrid[centerFI.position.X - GridWindow.DETAIL_SIZE / 2 + i, centerFI.position.Y - GridWindow.DETAIL_SIZE / 2 + j]);
                    detailsGrid[i, j].dataSource = fieldGrid[centerFI.position.X - GridWindow.DETAIL_SIZE / 2 + i, centerFI.position.Y - GridWindow.DETAIL_SIZE / 2 + j];
                    detailsGrid[i, j].renderDetails();
                }
            }
        }
        public void redrawDetails()
        {
            for (int i = 0; i < detailsGrid.GetLength(0); i++)
            {
                for (int j = 0; j < detailsGrid.GetLength(1); j++)
                {
                    detailsGrid[i, j].renderDetails();
                }
            }
        }
        //private void copyDetailElements(FieldItem a, FieldItem b)
        //{
        //    b.detailGCost_L = a.detailGCost_L;
        //    b.detailHCost_L = a.detailHCost_L;
        //    b.detailFCost_L = a.detailFCost_L;
        //    b.detailArrow = a.detailArrow;
        //    b.detailRectangle = a.detailRectangle;
        //}
        public FieldItem boundaryHelper(FieldItem toCenter)
        {
            FieldItem currFI = toCenter;
            //correct horizontaly (x)
            while (fieldGrid.GetLength(0) <= currFI.position.X + GridWindow.DETAIL_SIZE / 2)
            {
                currFI = fieldGrid[currFI.position.X - 1, currFI.position.Y];
            }
            while (0 > currFI.position.X - GridWindow.DETAIL_SIZE / 2)
            {
                currFI = fieldGrid[currFI.position.X + 1, currFI.position.Y];
            }
            //correct vertically (y)
            while (fieldGrid.GetLength(1) <= currFI.position.Y + GridWindow.DETAIL_SIZE / 2)
            {
                currFI = fieldGrid[currFI.position.X, currFI.position.Y - 1];
            }
            while (0 > currFI.position.Y - GridWindow.DETAIL_SIZE / 2)
            {
                currFI = fieldGrid[currFI.position.X, currFI.position.Y + 1];
            }
            return currFI;
        }

        public bool isImportantField(int x, int y)
        {
            switch (fieldGrid[x, y].GetStatus())
            {
                case FieldStatus.Blocked:
                case FieldStatus.Start:
                    return false;
                default:
                    return true;
            }
        }
        public void SetMouseRegistered(bool shouldBeEnabled)
        {
            if (areMouseEventsRegisterd != shouldBeEnabled)
            {
                for (int i = 0; i < fieldGrid.GetLength(0); i++)
                {
                    for (int j = 0; j < fieldGrid.GetLength(1); j++)
                    {
                        if (shouldBeEnabled)
                        {
                            fieldGrid[i, j].RegisterMouseEvents();
                        }
                        else
                        {
                            fieldGrid[i, j].UnregisterMouseEvents();
                        }
                    }
                }
                areMouseEventsRegisterd = shouldBeEnabled;
            }
        }

        public int getDistanceBetweenTwoNeighbours(FieldItem a, FieldItem b, bool diagAllowed)
        {
            if (!a.getNeighbours(diagAllowed).Contains(b) && b.position != StartPoint)
                throw new ArgumentException("Provided FIs are not Neighbours or illegaly diagonal!");
            if (a.position.X == b.position.X || a.position.Y == b.position.Y)
                return FieldItem.NORMAL_COST;
            else
                return FieldItem.DIAGONAL_COST;
        }

        public FieldItem getStartPoint()
        {
            return fieldGrid[StartPoint.X, StartPoint.Y];
        }

        public FieldItem getTargetPoint()
        {
            return fieldGrid[TargetPoint.X, TargetPoint.Y];
        }

        public FieldItem getFIFromCoord(Coord pos)
        {
            return fieldGrid[pos.X, pos.Y];
        }


    }

}
