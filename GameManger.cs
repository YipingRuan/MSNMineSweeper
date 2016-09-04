using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MSNMineSweeper
{
    public enum MineFieldCellStatus { Covered, Opened };
    public enum CurrentPlayer { Red, Blue };
    //public enum CellImageStyle { Covered, Opened };

    static class GameManger
    {
        public static MineFiledCell[,] MineFiledCells = new MineFiledCell[16, 16]; //[Row, Column]
        public static GameWindow myGameWindow;
        static Dictionary<String, Image> CellImageRef = new Dictionary<string, Image>();
        public static Player PlayerRed = new Player(), PlayerBlue = new Player();
        public static CurrentPlayer CurrentPlayer = MSNMineSweeper.CurrentPlayer.Red;

        static GameManger()
        {
            myGameWindow = new GameWindow();

            for (int i = 0; i < 16 * 16; i++)
            {
                Int32 Row = i / 16, Column = i % 16;
                MineFiledCells[Row, Column] = new MineFiledCell()
                {
                };
            }

            //Prepare mine cell image
            foreach (String ImageName in new String[] { "0", "1", "2", "3", "4", "5", "Covered", "Flag_Blue", "Flag_Red" })
            {
                BitmapImage bi4 = new BitmapImage();
                bi4.BeginInit();
                bi4.UriSource = new Uri("img/Grid/" + ImageName + ".png", UriKind.Relative);
                bi4.EndInit();

                Image myImage3 = new Image();
                myImage3.Source = bi4;

                CellImageRef.Add(ImageName, myImage3);
            }

            NewGameSetup();

            //Add CellImage
            for (int i = 0; i < 16 * 16; i++)
            {
                myGameWindow.MineBoard.Children.Add(MineFiledCells[i / 16, i % 16].CellImage);
            }

            //Add BlankImage
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("img/Grid/Transparent.png", UriKind.Relative);
            bi3.EndInit();
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    Image myImage3 = new Image();
                    myImage3.Source = bi3;
                    myImage3.SetValue(Grid.RowProperty, i);
                    myImage3.SetValue(Grid.ColumnProperty, j);
                    myGameWindow.MouseDetection.Children.Add(myImage3);
                }
            }
        }

        public static void NewGameSetup()
        {
            //Set mine list
            Boolean[] MineList = new Boolean[16 * 16];
            for (int i = 0; i < 51; i++)
            {
                MineList[i] = true;
            }
            Shuffle<Boolean>(MineList);

            //Prepare cells based on the MineList
            for (int i = 0; i < 16 * 16; i++)
            {
                Int32 Row = i / 16, Column = i % 16;
                MineFiledCells[Row, Column].isMine = MineList[i];
                MineFiledCells[Row, Column].isOpened = false;
                MineFiledCells[Row, Column].MineAroundCount = 0;

                //Reset CellImage
                MineFiledCells[Row, Column].CellImage.Source = GetCellImageByKey("Covered").Source;
                MineFiledCells[Row, Column].SetCellImageGrid(Row, Column);
            }

            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    if (MineFiledCells[i, j].isMine)
                    {
                        IncreaseMineAroundCount(i, j);
                    }
                }
            }

            //Reset Player infor
            PlayerRed.MineFound = PlayerBlue.MineFound = 0; ;
            myGameWindow.MineGot_Red.Text = myGameWindow.MineGot_Blue.Text = "00";

            myGameWindow.RemainMineNo.Content = 51;
            myGameWindow.GameResult.Visibility = System.Windows.Visibility.Hidden;
            myGameWindow.ChangePlayerNotice.Visibility = System.Windows.Visibility.Hidden;

            CurrentPlayer = MSNMineSweeper.CurrentPlayer.Red;
            myGameWindow.Arrow_Blue.Visibility = System.Windows.Visibility.Visible;
            myGameWindow.Arrow_Blue.Visibility = System.Windows.Visibility.Hidden;

            PlaySound("NewGame");
        }

        static void PlaySound(String SoundName)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer("sound/" + SoundName + ".wav");
            player.Play();
        }

        static void IncreaseMineAroundCount(Int32 Row, Int32 Column)
        {
            for (int RowOffset = -1; RowOffset <= 1; RowOffset++)
            {
                for (int ColumnOffset = -1; ColumnOffset <= 1; ColumnOffset++)
                {
                    Int32 Row_A = Row + RowOffset, Column_A = Column + ColumnOffset;
                    if (Row_A >= 0 && Row_A <= 15 && Column_A >= 0 && Column_A <= 15 && (RowOffset != 0 || ColumnOffset != 0))
                    {
                        MineFiledCells[Row_A, Column_A].MineAroundCount++;
                    }
                }
            }
        }

        static Image GetCellImageByKey(String key)
        {
            return new Image() { Source = CellImageRef[key].Source };
        }

        static Random _random = new Random();
        static void Shuffle<T>(T[] array)
        {
            var random = _random;
            for (int i = array.Length; i > 1; i--)
            {
                // Pick random element to swap.
                int j = random.Next(i); // 0 <= j <= i-1
                // Swap.
                T tmp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = tmp;
            }
        }

        //Process evenet
        public static void CellClick(Int32 Row, Int32 Column)
        {
            MineFiledCell TargetCell = MineFiledCells[Row, Column];

            if (!TargetCell.isOpened)
            {
                if (TargetCell.MineAroundCount == 0 && !TargetCell.isMine)
                {
                    Expand(Row, Column);
                }
                else
                {
                    SetCellOpen(TargetCell);
                }

                if (TargetCell.isMine)
                {
                    if ((CurrentPlayer == MSNMineSweeper.CurrentPlayer.Red && PlayerRed.MineFound >= 20)
                        ||
                        (CurrentPlayer == MSNMineSweeper.CurrentPlayer.Blue && PlayerBlue.MineFound >= 20)
                        )
                        PlaySound("CloseToWin");
                    else
                        PlaySound("FoundMine");
                    CheckWinner();
                }
                else
                {
                    PlaySound("FoundEmpty");
                    ChangePlayer();
                }
            }
        }

        private static void CheckWinner()
        {
            if (PlayerRed.MineFound == 26)
            {
                myGameWindow.GameResultText.Content = PlayerRed.PlayerName + " is the winner!";
                PlaySound("Win");
                myGameWindow.GameResult.Visibility = System.Windows.Visibility.Visible;
            }
            else if (PlayerBlue.MineFound == 26)
            {
                myGameWindow.GameResultText.Content = PlayerBlue.PlayerName + " is the winner!";
                PlaySound("Win");
                myGameWindow.GameResult.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private static void Expand(Int32 Row, Int32 Column)
        {
            MineFiledCell TargetCell = MineFiledCells[Row, Column];
            //Open it first
            SetCellOpen(TargetCell);

            if (TargetCell.MineAroundCount == 0)
            {
                for (int RowOffset = -1; RowOffset <= 1; RowOffset++)
                {
                    for (int ColumnOffset = -1; ColumnOffset <= 1; ColumnOffset++)
                    {
                        Int32 Row_A = Row + RowOffset, Column_A = Column + ColumnOffset;
                        if (Row_A >= 0 && Row_A <= 15
                            && Column_A >= 0 && Column_A <= 15
                            && (RowOffset != 0 || ColumnOffset != 0)
                            && !MineFiledCells[Row_A, Column_A].isOpened && !MineFiledCells[Row_A, Column_A].isMine)
                        {
                            Expand(Row_A, Column_A);
                        }
                    }
                }
            }
        }

        private static void SetCellOpen(MineFiledCell TargetCell)
        {
            if (TargetCell.isMine)
            {
                if (CurrentPlayer == MSNMineSweeper.CurrentPlayer.Red)
                {
                    TargetCell.CellImage.Source = CellImageRef["Flag_Red"].Source;
                    PlayerRed.MineFound++;
                    myGameWindow.MineGot_Red.Text = PlayerRed.MineFound.ToString("D2");
                }
                else
                {
                    TargetCell.CellImage.Source = CellImageRef["Flag_Blue"].Source;
                    PlayerBlue.MineFound++;
                    myGameWindow.MineGot_Blue.Text = PlayerBlue.MineFound.ToString("D2");
                }
                myGameWindow.RemainMineNo.Content = (51 - (PlayerBlue.MineFound + PlayerRed.MineFound)).ToString();
            }
            else
            {
                TargetCell.CellImage.Source = CellImageRef[TargetCell.MineAroundCount.ToString()].Source;
            }
            TargetCell.isOpened = true;
        }

        static Timer _resetStatusTimer = new Timer(12);
        private static void ChangePlayer()
        {
            if (CurrentPlayer == MSNMineSweeper.CurrentPlayer.Red)
            {
                CurrentPlayer = MSNMineSweeper.CurrentPlayer.Blue;
                myGameWindow.Arrow_Blue.Visibility = System.Windows.Visibility.Visible;
                myGameWindow.Arrow_Red.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                CurrentPlayer = MSNMineSweeper.CurrentPlayer.Red;
                myGameWindow.Arrow_Blue.Visibility = System.Windows.Visibility.Hidden;
                myGameWindow.Arrow_Red.Visibility = System.Windows.Visibility.Visible;
            }

            Task.Factory.StartNew(() =>
            {
                _resetStatusTimer.Stop();
                _resetStatusTimer = new Timer(1500);
                myGameWindow.ChangePlayerNotice.Dispatcher.Invoke(() =>
                {
                    myGameWindow.ChangePlayerNotice.Visibility = System.Windows.Visibility.Visible;
                });

                _resetStatusTimer.Elapsed += HideChangePlayer;
                _resetStatusTimer.Start();
            });
        }

        private static void HideChangePlayer(object sender, ElapsedEventArgs e)
        {
            myGameWindow.ChangePlayerNotice.Dispatcher.Invoke(() =>
                {
                    myGameWindow.ChangePlayerNotice.Visibility = System.Windows.Visibility.Hidden;
                });
            _resetStatusTimer.Stop();
        }
    }

    class Player
    {
        public String PlayerName;
        public Int32 MineFound = 0;
    }

    class MineFiledCell
    {
        public Boolean isMine = false;
        public Boolean isOpened = false;
        public Image CellImage = new Image();
        public Int32 MineAroundCount = 0;

        public void SetCellImageGrid(Int32 Row, Int32 Column)
        {
            CellImage.SetValue(Grid.RowProperty, Row);
            CellImage.SetValue(Grid.ColumnProperty, Column);
        }

    }
}
