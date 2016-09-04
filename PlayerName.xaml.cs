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
    /// Interaction logic for PlayerName.xaml
    /// </summary>
    public partial class PlayerName : Window
    {
        public PlayerName()
        {
            InitializeComponent();
        }

        private void StartGame(object sender, RoutedEventArgs e)
        {
            GameManger.PlayerRed.PlayerName = PlayerRed.Text;
            GameManger.PlayerBlue.PlayerName = PlayerBlue.Text;
            GameManger.myGameWindow.PlayerName_Red.Content = GameManger.PlayerRed.PlayerName;
            GameManger.myGameWindow.PlayerName_Blue.Content = GameManger.PlayerBlue.PlayerName;
            
            GameManger.myGameWindow.Show();
            this.Close();
        }
    }
}
