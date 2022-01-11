
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
        //must be odd and >1
        public static readonly int DETAIL_SIZE = 5;
        private PathField mainField;
        private int gridSize;
        GridAlgorithm currentAlg;

        public GridWindow()
        {
            gridSize = 50;
            currentAlg = new AStar(mainField, this, 1);
            InitializeComponent();
            ResizeGrid(gridSize);
            createDetailWindow(DETAIL_SIZE);

            CB_Heuristic_Selection.ItemsSource = FieldItem.HEURISTICS;
            CB_Heuristic_Selection.SelectedIndex = 0;
            string? v = CB_Heuristic_Selection.SelectedItem.ToString();
            if (v == null)
                throw new ShouldNotHappenException("ComboBox not initialized correctly!");
            mainField.heuristic = v;
        }
        public void createDetailWindow(int detailSize)
        {
            FieldItem center = mainField.boundaryHelper(mainField.fieldGrid[0, 0]);
            for (int i = 0; i < detailSize; i++)
            {

                RowDefinition newRow = new RowDefinition();
                DetailsGrid.RowDefinitions.Add(newRow);
                ColumnDefinition newColumn = new ColumnDefinition();
                DetailsGrid.ColumnDefinitions.Add(newColumn);
                for (int j = 0; j < detailSize; j++)
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
                    Label labelG = new Label();
                    labelG.VerticalAlignment = VerticalAlignment.Top;
                    labelG.HorizontalAlignment = HorizontalAlignment.Left;

                    Grid.SetColumn(labelG, j);
                    Grid.SetRow(labelG, i);
                    Label labelH = new Label();
                    labelH.VerticalAlignment = VerticalAlignment.Top;
                    labelH.HorizontalAlignment = HorizontalAlignment.Right;
                    Grid.SetColumn(labelH, j);
                    Grid.SetRow(labelH, i);
                    TextBlock textB_F = new TextBlock();
                    textB_F.VerticalAlignment = VerticalAlignment.Bottom;
                    textB_F.HorizontalAlignment = HorizontalAlignment.Center;
                    textB_F.Text = i + " | " + j;
                    textB_F.TextAlignment = TextAlignment.Center;
                    textB_F.Margin = new Thickness(5);
                    Grid.SetColumn(textB_F, j);
                    Grid.SetRow(textB_F, i);
                    Path arrow = new Path();
                    arrow.VerticalAlignment = VerticalAlignment.Top;
                    arrow.HorizontalAlignment = HorizontalAlignment.Center;
                    arrow.Margin = new Thickness(9);
                    arrow.Width = 16;
                    arrow.Height = 8;
                    arrow.RenderTransformOrigin = new Point(0.5, 0.5);
                    arrow.Data = Geometry.Parse("M 10 0 L 16 4 L 10 8 M 0 4 L 16 4");
                    arrow.Stroke = Brushes.Black;
                    Grid.SetColumn(arrow, j);
                    Grid.SetRow(arrow, i);
                    newVB.Child = newRect;
                    DetailsGrid.Children.Add(newVB);
                    DetailsGrid.Children.Add(labelG);
                    DetailsGrid.Children.Add(labelH);
                    DetailsGrid.Children.Add(textB_F);
                    DetailsGrid.Children.Add(arrow);
                    mainField.detailsGrid[i, j] = new DetailField(
                        labelG,
                        labelH,
                        textB_F,
                        arrow,
                        newRect,
                        mainField.fieldGrid[center.position.X - DETAIL_SIZE / 2 + i, center.position.Y - DETAIL_SIZE / 2 + j]
                        );
                }
            }
        }
        public void ResizeGrid(int gridSize)
        {
            mainField = new PathField(gridSize, this);
            currentAlg.setWorkingField(mainField);

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
                    mainField.fieldGrid[i, j] = new FieldItem(newRect, mainField, i, j, this);
                }
            }

            mainField.UpdateItem(gridSize - 1, 0, FieldStatus.Start);
            mainField.UpdateItem(0, gridSize - 1, FieldStatus.Target);
            mainField.SetMouseRegistered(true);
        }

        private void ResetGrid()
        {
            FieldItem startFI = mainField.getStartPoint();
            FieldItem targetFI = mainField.getTargetPoint();
            for (int i = 0; i < mainField.fieldGrid.GetLength(0); i++)
            {
                for (int j = 0; j < mainField.fieldGrid.GetLength(1); j++)
                {
                    if (mainField.fieldGrid[i, j].GetStatus() == FieldStatus.ToEvaluate || mainField.fieldGrid[i, j].GetStatus() == FieldStatus.Evaluated || mainField.fieldGrid[i, j].GetStatus() == FieldStatus.IsPath)
                        mainField.fieldGrid[i, j].updateStatus(FieldStatus.Free);
                    mainField.fieldGrid[i, j].resetCosts();
                    mainField.fieldGrid[i, j].sourceDirection = null;
                }
            }
            mainField.UpdateItem(startFI.position.X, startFI.position.Y, FieldStatus.Start);
            mainField.UpdateItem(targetFI.position.X, targetFI.position.Y, FieldStatus.Target);
        }
        private void NewGrid()
        {

            for (int i = 0; i < mainField.fieldGrid.GetLength(0); i++)
            {
                for (int j = 0; j < mainField.fieldGrid.GetLength(1); j++)
                {
                    mainField.fieldGrid[i, j].updateStatus(FieldStatus.Free);
                    mainField.fieldGrid[i, j].resetCosts();
                    mainField.fieldGrid[i, j].sourceDirection = null;
                }
            }
            mainField.UpdateItem(gridSize - 1, 0, FieldStatus.Start);
            mainField.UpdateItem(0, gridSize - 1, FieldStatus.Target);
        }

        public void finishedSearch(int pathLength)
        {
            //Application.Current.Dispatcher.Invoke(() => LB_Stats.Content = "Length: " + pathLength);
            Application.Current.Dispatcher.Invoke(() => BT_GridReset.IsEnabled = true);
            Application.Current.Dispatcher.Invoke(() => BT_GridGen.IsEnabled = true);
            Application.Current.Dispatcher.Invoke(() => BT_PauseSearch.IsEnabled = false);

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

        private void BT_Pause_Click(object sender, RoutedEventArgs e)
        {
            if (BT_PauseSearch.Content.Equals("Pause"))
            {
                currentAlg.setStopFlag(true);
                BT_PauseSearch.Content = "Continue";
            }
            else
            {
                BT_PauseSearch.Content = "Pause";
                currentAlg.continueSearch();
                BT_PauseSearch.Content = "Continue";
                BT_PauseSearch.IsEnabled = false;
            }
        }
        private void BT_StartFlag_Click(object sender, RoutedEventArgs e)
        {
            if (mainField.setTargetPointFlag)
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
            BT_PauseSearch.IsEnabled = true;
            BT_PauseSearch.Content = "Pause";
            currentAlg.drawPath(mainField.getStartPoint(), mainField.getTargetPoint(), CB_AllowDiag.IsChecked.Value);

        }

        private void DelaySliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LB_DelayLabel.Content = "Delay(ms):\n" + e.NewValue;
            currentAlg.setStepDelay((int)e.NewValue);
        }

        private void CB_Heuristic_Selection_Changed(object sender, RoutedEventArgs e)
        {
            string? v = CB_Heuristic_Selection.SelectedItem.ToString();
            if (v == null)
                throw new ShouldNotHappenException("ComboBox not initialized correctly!");
            mainField.heuristic = v;
        }

        private void CB_Delay_Clicked(object sender, RoutedEventArgs e)
        {
            if (!CB_Delay.IsChecked.HasValue)
                throw new ShouldNotHappenException("How is this CheckBox Value null?");
            if (CB_Delay.IsChecked.Value)
            {
                currentAlg.setStepDelay((int)SL_DelaySlider.Value);
                SL_DelaySlider.IsEnabled = true;
                LB_DelayLabel.Content = "Delay(ms):\n" + (int)SL_DelaySlider.Value;
            }
            else
            {
                currentAlg.setStepDelay(0);
                SL_DelaySlider.IsEnabled = false;
                LB_DelayLabel.Content = "Delay(ms):\n0";
            }
        }

        private void BT_GenMaze_Click(object sender, RoutedEventArgs e)
        {
            LabyrinthGenerator.generateMaze(mainField, (int)SL_DelaySlider.Value);
        }
    }
}
