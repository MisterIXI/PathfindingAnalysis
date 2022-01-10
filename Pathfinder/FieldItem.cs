using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;

namespace Pathfinder
{
    enum FieldStatus
    {
        Start,
        Target,
        Blocked,
        Free,
        ToEvaluate,
        Evaluated,
        Evaluating,
        IsPath
    }




    class FieldItem
    {
        readonly Color START_COLOR = Colors.Green;
        readonly Color TARGET_COLOR = Colors.Red;
        readonly Color BLOCKED_COLOR = Colors.Black;
        readonly Color FREE_COLOR = Colors.White;
        readonly Color TOEVALUATE_COLOR = Colors.LightGreen;
        readonly Color EVALUATED_COLOR = Colors.LightBlue;
        readonly Color EVALUATING_COLOR = Colors.HotPink;
        readonly Color ISPATH_COLOR = Colors.DarkCyan;

        readonly int DIAGONAL_COST = 14;
        readonly int NORMAL_COST = 10;

        public String name;

        private PathField parentField;
        private GridWindow parentWindow;

        public Coord position { get; private set; }


        public Rectangle sourceRectangle { get; private set; }
        private FieldStatus status;
        public int costToTarget;
        public int costToSource;
        public int totalCost;

        public FieldItem sourceDirection;

        public FieldItem(Rectangle theRectangle, PathField parent, int x, int y, GridWindow gridWindow)
        {
            sourceRectangle = theRectangle;
            status = FieldStatus.Free;
            costToTarget = 0;
            costToSource = 0;
            totalCost = 0;
            position = new Coord(x, y);
            parentField = parent;
            parentWindow = gridWindow;


        }

        public int calcCost(FieldItem target)
        {
            //"diagonal" from here http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html
            int distanceY = Math.Abs(position.Y - target.position.Y);
            int distanceX = Math.Abs(position.X - target.position.X);
            return NORMAL_COST * (distanceY + distanceX) + (DIAGONAL_COST - 2 * NORMAL_COST) * Math.Min(distanceY, distanceX);
        }

        public List<FieldItem> getNeighbours(bool diagAllowed)
        {
            return parentField.getSurrounding(position, diagAllowed);
        }

        public String toString()
        {
            return name;
        }
        public void updateStatus(FieldStatus newStatus)
        {
            if (newStatus == FieldStatus.Start)
            {
                if (parentField.StartPoint != null)
                    parentField.FieldGrid[parentField.StartPoint.X, parentField.StartPoint.Y].updateStatus(FieldStatus.Free);
                if (parentField.TargetPoint.Equals(position))
                    parentField.TargetPoint = null;
                parentField.StartPoint = position;
            }
            if (newStatus == FieldStatus.Target)
            {
                if (parentField.TargetPoint != null)
                    parentField.FieldGrid[parentField.TargetPoint.X, parentField.TargetPoint.Y].updateStatus(FieldStatus.Free);
                if (parentField.StartPoint.Equals(position))
                    parentField.StartPoint = null;
                parentField.TargetPoint = position;
            }

            status = newStatus;

            Color newColor;
            switch (status)
            {
                case FieldStatus.Start:
                    newColor = START_COLOR;
                    break;
                case FieldStatus.Target:
                    newColor = TARGET_COLOR;
                    break;
                case FieldStatus.Free:
                    newColor = FREE_COLOR;
                    break;
                case FieldStatus.Blocked:
                    newColor = BLOCKED_COLOR;
                    break;
                case FieldStatus.ToEvaluate:
                    newColor = TOEVALUATE_COLOR;
                    break;
                case FieldStatus.IsPath:
                    newColor = ISPATH_COLOR;
                    break;
                case FieldStatus.Evaluated:
                    newColor = EVALUATED_COLOR;
                    break;
                case FieldStatus.Evaluating:
                    newColor = EVALUATING_COLOR;
                    break;
                default:
                    throw new ShouldNotHappenException("FieldStatus not set");
            }
            Application.Current.Dispatcher.Invoke(() => sourceRectangle.Fill = new SolidColorBrush(newColor));
        }

        public FieldStatus GetStatus()
        {
            return status;
        }

        public void RegisterMouseEvents()
        {
            sourceRectangle.MouseEnter += Rec_OnMouse;
            sourceRectangle.MouseDown += Rec_OnMouse;
        }


        public void UnregisterMouseEvents()
        {
            sourceRectangle.MouseEnter -= Rec_OnMouse;
            sourceRectangle.MouseDown -= Rec_OnMouse;
        }

        private void specialPointCheck(FieldStatus status)
        {

        }

        private void Rec_OnMouse(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (Keyboard.IsKeyDown(Key.LeftShift) || parentField.setStartPointFlag)
                {
                    updateStatus(FieldStatus.Start);
                    parentField.setStartPointFlag = false;
                    parentWindow.BT_StartFlag.IsEnabled = true;
                }
                else if (Keyboard.IsKeyDown(Key.LeftCtrl) || parentField.setTargetPointFlag)
                {
                    updateStatus(FieldStatus.Target);
                    parentField.setTargetPointFlag = false;
                    parentWindow.BT_TargetFlag.IsEnabled = true;
                }
                else
                {
                    updateStatus(FieldStatus.Blocked);
                }
            }
            else if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                updateStatus(FieldStatus.Free);
            }
            if(sourceDirection != null)
                System.Windows.Application.Current.Dispatcher.Invoke(() => parentWindow.LB_Stats.Content = $"costToSource: {costToSource}\nPos:{position.X}|{position.Y}\nsourceDir: {sourceDirection.position.X}|{sourceDirection.position.Y}" );
        }
    }
}
