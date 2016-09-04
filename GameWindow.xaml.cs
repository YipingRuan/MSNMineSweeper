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

namespace MSNMineSweeper
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public GameWindow()
        {
            InitializeComponent();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            var element = (UIElement)e.Source;
            Int32 Row = Grid.GetRow(element), Column = Grid.GetColumn(element);

            MouseFocus.SetValue(Grid.ColumnProperty, Column);
            MouseFocus.SetValue(Grid.RowProperty, Row);

            //Bomb
            if (Column >= 2 && Row >= 2 && Column <= 13 && Row <= 13)
            {
                BombArea.SetValue(Grid.ColumnProperty, Column - 2);
                BombArea.SetValue(Grid.RowProperty, Row - 2);
                //BombArea.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                //BombArea.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void MouseDetection_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)e.Source;
            GameManger.CellClick(Grid.GetRow(element), Grid.GetColumn(element));
        }

        private void NewGame_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GameManger.NewGameSetup();
        }
    }
}
