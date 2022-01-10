using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private void findTick(SortedList<int, FieldItem> availableNodes, bool diagAllowed)
        {
            bool foundPath = false;
            FieldItem currentPoint = availableNodes.First().Value;
            availableNodes.RemoveAt(0);
            if (currentPoint == targetPoint)
                foundPath = true;
            else
            {
                foreach (FieldItem e in currentPoint.getNeighbours(diagAllowed))
                {
                    if(!availableNodes.ContainsValue(e))
                        availableNodes.Add(e.calcCost(targetPoint), e);
                    
                }
                //currentPoint.updateStatus(FieldStatus.Evaluated);
            }
            if (foundPath)
                drawFinishedPath();
            else
            {
                if (stepDelay > 0 && !stopFlag)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() => currentPoint.sourceRectangle.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.HotPink));
                    //Task.Delay(stepDelay-10).ContinueWith(_ => currentPoint.updateStatus(FieldStatus.Evaluated));
                    Task.Delay(stepDelay).ContinueWith(_ => { currentPoint.updateStatus(FieldStatus.Evaluated); findTick(availableNodes, diagAllowed); });
                }
                else
                {
                    if (availableNodes.Count > 0)
                    {
                        if (currentPoint.GetStatus() != FieldStatus.Start)
                            currentPoint.updateStatus(FieldStatus.Evaluated);
                        findTick(availableNodes, diagAllowed);
                    }
                        
                }
            }
        }

        public void drawFinishedPath()
        {
            FieldItem next = targetPoint.sourceDirection;

            while (next != startPoint)
            {
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
            SortedList<int , FieldItem> availableNodes = new SortedList<int, FieldItem>(new DuplicateComparer<int>());
            

            foreach(FieldItem e in startPoint.getNeighbours(diagAllowed))
            {
                availableNodes.Add(e.calcCost(targetPoint),e);
                
            }
            FieldItem currentPoint = null;
            //while (!foundPath && availableNodes.Count() > 0)
            findTick(availableNodes, diagAllowed);
            
        }

        public void setStepDelay(int stepDelay)
        {
            this.stepDelay = stepDelay;
        }
    }
}
