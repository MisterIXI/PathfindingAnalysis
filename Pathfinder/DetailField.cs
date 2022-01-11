
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;

namespace Pathfinder
{
    internal class DetailField
    {
        readonly Label detailGCost_L;
        public Label detailHCost_L;
        public TextBlock detailFCost_L;
        public Path detailArrow;
        public Rectangle detailRectangle;
        public FieldItem dataSource;

        public DetailField(Label gCostL, Label hCostL, TextBlock fCostL, Path arrow, Rectangle rec, FieldItem field)
        {
            detailGCost_L = gCostL;
            detailHCost_L = hCostL;
            detailFCost_L = fCostL;
            detailArrow = arrow;
            detailRectangle = rec;
            dataSource = field;
        }

        public void renderDetails()
        {
            //if(this)
            detailFCost_L.Text = dataSource.getFCost() + "\n" +  dataSource.ToString();
            detailGCost_L.Content = dataSource.gCost;
            detailHCost_L.Content = dataSource.getHCost();

            //get direction
            if (dataSource.sourceDirection != null)
            {
                detailArrow.Visibility = Visibility.Visible;
                //detailArrow Angle 0 == right; Angle 45 == downright; Angle 90 == down
                switch (dataSource.position.Y - dataSource.sourceDirection.position.Y)
                {
                    case -1: //right of this
                        switch (dataSource.position.X - dataSource.sourceDirection.position.X)
                        {
                            case -1: //down of this
                                detailArrow.RenderTransform = new RotateTransform(45);
                                break;
                            case 0: //same row
                                detailArrow.RenderTransform = new RotateTransform(0);
                                break;
                            case 1: //up of this
                                detailArrow.RenderTransform = new RotateTransform(315);
                                break;
                        }
                        break;
                    case 0://same column
                        switch (dataSource.position.X - dataSource.sourceDirection.position.X)
                        {
                            case -1: //down of this
                                detailArrow.RenderTransform = new RotateTransform(90);
                                break;
                            case 0: //same row
                                throw new ShouldNotHappenException("Tried to point to self!");
                            case 1: //up of this
                                detailArrow.RenderTransform = new RotateTransform(270);
                                break;
                        }
                        break;
                    case 1://left of this
                        switch (dataSource.position.X - dataSource.sourceDirection.position.X)
                        {
                            case -1: //down of this
                                detailArrow.RenderTransform = new RotateTransform(135);
                                break;
                            case 0: //same row
                                detailArrow.RenderTransform = new RotateTransform(180);
                                break;
                            case 1: //up of this
                                detailArrow.RenderTransform = new RotateTransform(225);
                                break;
                        }
                        break;
                }
            }
            else
                detailArrow.Visibility = Visibility.Hidden;
            Application.Current.Dispatcher.Invoke(() => detailRectangle.Fill = new SolidColorBrush(dataSource.getStatusColor(dataSource.status)));
        }
    }
}
