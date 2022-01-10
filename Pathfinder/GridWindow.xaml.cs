using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Pathfinder
{
    /// <summary>
    /// Interaktionslogik für GridWindow.xaml
    /// </summary>
    public partial class GridWindow : Window
    {
        private PathField mainField;
        private int gridSize;
        GridAlgorithm currentAlg;

        public GridWindow()
        {
            gridSize = 50;
            InitializeComponent();
            ResizeGrid(gridSize);
            currentAlg = new AStar(mainField, this, (int)SL_DelaySlider.Value);
        }

        public void ResizeGrid(int gridSize)
        {
            mainField = new PathField(gridSize, this);


            for (int i = 0; i < gridSize; i++)
            {
                //create grid definitions
                RowDefinition newRow = new RowDefinition();
                MainGrid.RowDefinitions.Add(newRow);
                ColumnDefinition newColumn = new ColumnDefinition();
                MainGrid.ColumnDefinitions.Add(newColumn);
                //create viewboxes and rectangles
                for (int j = 0; j < gridSize; j++)
                {
                    Viewbox newVB = new Viewbox
                    {
                        Stretch = Stretch.Fill
                    };
                    Grid.SetColumn(newVB, j);
                    Grid.SetRow(newVB, i);

                    Rectangle newRect = new Rectangle
                    {
                        Width = 50,
                        Height = 50,
                        Fill = new SolidColorBrush(Colors.White),
                        Stroke = new SolidColorBrush(Colors.DarkGray)
                    };

                    newVB.Child = newRect;
                    MainGrid.Children.Add(newVB);
                    mainField.FieldGrid[i, j] = new FieldItem(newRect, mainField, i, j, this);
                    mainField.FieldGrid[i, j].name = i + "|" + j;
                }
            }

            mainField.UpdateItem(gridSize - 1, 0, FieldStatus.Start);
            mainField.UpdateItem(0, gridSize - 1, FieldStatus.Target);
            mainField.SetMouseRegistered(true);
        }

        private void ResetGrid()
        {
            for (int i = 0; i < mainField.FieldGrid.GetLength(0); i++)
            {
                for (int j = 0; j < mainField.FieldGrid.GetLength(1); j++)
                {
                    if(mainField.FieldGrid[i, j].GetStatus() == FieldStatus.ToEvaluate || mainField.FieldGrid[i, j].GetStatus() == FieldStatus.Evaluated || mainField.FieldGrid[i, j].GetStatus() == FieldStatus.IsPath)
                        mainField.FieldGrid[i, j].updateStatus(FieldStatus.Free);
                    mainField.FieldGrid[i, j].costToSource = 0;
                    mainField.FieldGrid[i, j].costToTarget = 0;
                    mainField.FieldGrid[i, j].totalCost = 0;
                    mainField.FieldGrid[i, j].sourceDirection = null;
                }
            }
            mainField.UpdateItem(gridSize - 1, 0, FieldStatus.Start);
            mainField.UpdateItem(0, gridSize - 1, FieldStatus.Target);
        }
        private void NewGrid()
        {
            
            for (int i = 0; i < mainField.FieldGrid.GetLength(0); i++)
            {
                for (int j = 0; j < mainField.FieldGrid.GetLength(1); j++)
                {
                    mainField.FieldGrid[i, j].updateStatus(FieldStatus.Free);
                    mainField.FieldGrid[i, j].costToSource = 0;
                    mainField.FieldGrid[i, j].costToTarget = 0;
                    mainField.FieldGrid[i, j].totalCost = 0;
                    mainField.FieldGrid[i, j].sourceDirection = null;
                }
            }
            mainField.UpdateItem(gridSize - 1, 0, FieldStatus.Start);
            mainField.UpdateItem(0, gridSize - 1, FieldStatus.Target);
        }

        public void finishedSearch(int pathLength)
        {
            Application.Current.Dispatcher.Invoke(() => LB_Stats.Content = "Length: " + pathLength);
            Application.Current.Dispatcher.Invoke(() => BT_GridReset.IsEnabled = true);
            Application.Current.Dispatcher.Invoke(() => BT_GridGen.IsEnabled = true);

        }

        private void ClearGrid()
        {
            MainGrid.Children.Clear();
            MainGrid.RowDefinitions.Clear();
            MainGrid.ColumnDefinitions.Clear();
        }

        private void BT_GridReset_Click(object sender, RoutedEventArgs e)
        {
            ResetGrid();
            BT_StartSearch.IsEnabled = true;
            BT_StartFlag.IsEnabled = true;
            BT_TargetFlag.IsEnabled = true;
        }

        private void BT_GridGen_Click(object sender, RoutedEventArgs e)
        {
            NewGrid();
            BT_StartSearch.IsEnabled = true;
            BT_StartFlag.IsEnabled = true;
            BT_TargetFlag.IsEnabled = true;
        }

        private void BT_StartFlag_Click(object sender, RoutedEventArgs e)
        {
            if(mainField.setTargetPointFlag)
            {
                BT_TargetFlag.IsEnabled = true;
                mainField.setTargetPointFlag = false;
            }
            BT_StartFlag.IsEnabled = false;
            mainField.setStartPointFlag = true;
        }

        private void BT_TargetFlag_Click(object sender, RoutedEventArgs e)
        {
            if (mainField.setTargetPointFlag)
            {
                BT_StartFlag.IsEnabled = true;
                mainField.setStartPointFlag = false;
            }
            BT_TargetFlag.IsEnabled = false;
            mainField.setTargetPointFlag = true;
        }

        private void BT_StartSearch_Click(object sender, RoutedEventArgs e)
        {
            BT_StartFlag.IsEnabled = false;
            BT_TargetFlag.IsEnabled = false;
            BT_GridReset.IsEnabled = false;
            BT_GridGen.IsEnabled = false;
            BT_StartSearch.IsEnabled = false;
            currentAlg.drawPath(mainField.getStartPoint(), mainField.getTargetPoint(), CB_AllowDiag.IsChecked.Value);
            LB_Stats.Content = "Steps: ";
        }

        private void DelaySliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LB_DelayLabel.Content = "Delay(ms):\n"+ e.NewValue;
            currentAlg.setStepDelay((int)e.NewValue);
        }

        private void BT_GenMaze_Click(object sender, RoutedEventArgs e)
        {
            LabyrinthGenerator.generateMaze(mainField, (int)SL_DelaySlider.Value);
        }
    }
}
