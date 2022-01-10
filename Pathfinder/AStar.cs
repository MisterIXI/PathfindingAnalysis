using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Pathfinder
{
    class AStar : GridAlgorithm
    {
        private PathField workField;
        private GridWindow parentWindow;
        private FieldItem targetPoint;
        private FieldItem startPoint;

        private bool stopFlag;
        private bool isPaused = false;
        public int stepDelay;
        public int pathLength { get; private set; }

        public AStar(PathField pathField, GridWindow gridWindow, int stepDelay)
        {
            targetPoint = null;
            startPoint = null;
            workField = pathField;
            parentWindow = gridWindow;
            pathLength = 0;
            this.stepDelay = stepDelay;
            stopFlag = false;
        }
        private void findPath(PriorityQueue<FieldItem, int> availableNodes, bool diagAllowed)
        {
            bool foundPath = false;
            while (availableNodes.Count > 0 && !foundPath)
            {
                FieldItem currentPoint = availableNodes.Dequeue();
                if (currentPoint == targetPoint)
                {//finished pathing
                    foundPath = true;
                    drawFinishedPath();
                }
                else
                {//not at target
                    //process current Neighbours
                    foreach (FieldItem e in currentPoint.getNeighbours(diagAllowed))
                    {
                        if (e.GetStatus() != FieldStatus.Evaluated)
                        {
                            e.updateCosts(targetPoint, currentPoint, diagAllowed);

                            if (!availableNodes.UnorderedItems.Contains((e, e.getHCost())))
                                availableNodes.Enqueue(e, e.getHCost());
                        }
                    }
                    if (stepDelay > 0)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() => currentPoint.sourceRectangle.Fill = Brushes.HotPink);
                        Task.Delay(stepDelay).ContinueWith(_ =>
                        {
                            if (currentPoint.GetStatus() != FieldStatus.Start)
                                currentPoint.updateStatus(FieldStatus.Evaluated);
                        });
                    }
                    else
                    {
                        if (currentPoint.GetStatus() != FieldStatus.Start)
                            currentPoint.updateStatus(FieldStatus.Evaluated);
                    }
                }
            }
        }

        public void drawFinishedPath()
        {
            if (targetPoint.sourceDirection == null)
                throw new ShouldNotHappenException("Target point didn't get an direction asigned. Can't finish what I started!");

            FieldItem? next = targetPoint.sourceDirection;

            while (next != startPoint)
            {
                if (next == null)
                    throw new ShouldNotHappenException("Current point didn't get an direction asigned. Can't finish what I started!");
                next.updateStatus(FieldStatus.IsPath);
                next = next.sourceDirection;
                pathLength++;
            }
            parentWindow.finishedSearch(pathLength);
            //System.Windows.Application.Current.Dispatcher.Invoke(() => parentWindow.LB_Stats.Content = "Length: " + pathLength);
        }
        public void drawPath(FieldItem startPoint, FieldItem targetPoint, bool diagAllowed)
        {
            stopFlag = false;
            pathLength = 0;
            this.targetPoint = targetPoint;
            this.startPoint = startPoint;
            if (startPoint == targetPoint)
                throw new ArgumentException();

            bool foundPath = false;
            //SortedList<int , FieldItem> availableNodes = new SortedList<int, FieldItem>(new DuplicateComparer<int>());
            PriorityQueue<FieldItem, int> availableNodes = new PriorityQueue<FieldItem, int>();

            foreach (FieldItem e in startPoint.getNeighbours(diagAllowed))
            {
                e.updateCosts(targetPoint, startPoint, diagAllowed);
                availableNodes.Enqueue(e, e.getHCost());

            }

            findPath(availableNodes, diagAllowed);

        }

        public void setStepDelay(int stepDelay)
        {
            this.stepDelay = stepDelay;
        }
    }
}
